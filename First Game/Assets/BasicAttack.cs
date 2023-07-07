using UnityEngine;

// Das BasicAttack im Game
public class BasicAttack : MonoBehaviour
{
    // Attackiertes GameObject
    public GameObject Target;

    // Damage Operatoren
    public float Damage;
    public float CritChance = 0;
    public float CritDamage = 0;

    // Utility
    public float AttackSpeed;
    public int Range;

    // Statuseffekte, die von BasicAttack übergeben werden: 
    public int SlowStrength = 0;

    void Start()
    {
        //DestroyOtherComponents();

        Debug.Log("BasicAttack was generated successfully on: " + gameObject.name + " targeting: " + Target.name);
    }

    void Update()
    {
        Damage = gameObject.GetComponent<EntityBase>().CurrentDamage;
        AttackSpeed = gameObject.GetComponent<EntityBase>().CurrentAttackSpeed;

        if (Target == null || !Target.activeSelf)
            Destroy(this);

        MoveTowardsEnemy();
        TryBasicAttack();

        // Cooldown wird um 1.0f pro Sekunde runter gesetzt
        if (Cooldown > -Time.deltaTime)
            Cooldown -= Time.deltaTime;
    }

    // Bewegt den Enemy in Richtung seines Targets
    // Unter Bedingungen wie: Nicht in Basic Attack Range sein etc.
    private void MoveTowardsEnemy()
    {
        // Berechnet die Distanz zum targetet Character
        float DistanceToEnemy = Vector2.Distance(Target.GetComponent<Collider2D>().bounds.ClosestPoint(transform.position), new Vector2(transform.position.x, transform.position.y));

        // Wenn Distanz > AttackRange bewegt sich das Enemy auf den Character zu
        if (DistanceToEnemy > Range / 10.0f)
        {
            Vector3 direction = Target.transform.position - transform.position;

            if (CompareTag("Hostile"))
                transform.Translate(gameObject.GetComponent<EnemyAI>().Speed * Time.deltaTime * direction.normalized);
            else
                gameObject.GetComponent<CharacterController>().MoveCharacter(direction);
        }
    }

    // Gibt an, wie viel Cooldown für einen Basic Attack herrscht
    public float Cooldown;
    // Slapped den targeted Character, wenn möglich
    private bool TryBasicAttack()
    {
        // Wenn kein Cooldown & in AttackRange wird der Enemy attacked
        bool CanAttack = IsInRange(Range) && (Cooldown < 0);

        if (CanAttack)
        {
            try
            {
                // Cooldown wird resettet
                Cooldown += AttackSpeed;

                // Fügt dem angegriffenen Target Schaden hinzu, wenn möglich
                HitIfEnemy();

                // Fügt, falls vorhanden, Statuseffekte hinzu
                if (SlowStrength != 0)
                {
                    SlowAttribute CurrSlow = Target.AddComponent<SlowAttribute>();
                    CurrSlow.Strength = SlowStrength;
                }
            }
            catch { Debug.Log("GameObject: " + Target.name + "is not a targetable gameObject"); }
        }
        return CanAttack;
    }

    private void HitIfEnemy()
    {
        if (CompareTag("Hostile") && Target.CompareTag("Character"))
            Target.GetComponent<CharacterController>().AddDamage(Damage, CritChance, CritDamage);
        else if (CompareTag("Character") && Target.CompareTag("Hostile"))
            Target.GetComponent<EnemyAI>().AddDamage(Damage, CritChance, CritDamage);
        else 
            Debug.Log("Target: " + Target.name + " is not attackable with: " + name);
    }

    // Guckt, ob ein Gegner in Range für eine gewisse Ability z.B. BasicAttack ist
    private bool IsInRange(int AttackRange)
    {
        // Berechnet die Distanz zum targetet Character
        float DistanceToEnemy = Vector2.Distance(Target.GetComponent<Collider2D>().bounds.ClosestPoint(transform.position), new Vector2(transform.position.x, transform.position.y));

        // Wenn nach dem Movement noch außerhalb der AttackRange ist, kann nicht angegriffen
        if (DistanceToEnemy > AttackRange / 10.0f)
            return false;
        else return true;
    }
}
