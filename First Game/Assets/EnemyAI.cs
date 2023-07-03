using System;
using UnityEngine;

// Gibt ein generelles Muster an, nach dem jeder Enemy die Character angreift
// Braucht denke ich noch Ability Support
    // Basic Attack könnte auch eine Ability werden ?? 
public class EnemyAI : MonoBehaviour
{
    // ID, die den Enemy identifizierbar macht
    public int ID = SceneDB.AddEnemyID();

    // Stats, die kontrollieren, wann ein Enemy destroyed wird
    public float MaxHP;
    public float HP;

    // Offensive Stats
    public float Damage;
    public float AttackSpeed; // Cooldown auf Basic Attacks

    // Defensive Stats
    public float Armor;

    // Utility Stats
    public float MovementSpeed;
    public int BasicAttackRange;

    private GameObject AttackedCharacter;

    // Spielt jeden Frame die AI
    void Update()
    {
        // Zerstört das GameObject, wenn der Enemy getötet wurde
        if (HP < 0)
            Destroy(gameObject);

        // Für die AI nötige Werte werden aktualisiert

        // Bestimmt, welcher Character angegriffen wird
        AttackedCharacter = SceneDB.HighestAggroCharacter;

        // Movement & Basic Attack vom Enemy
        MoveTowardsEnemy();
        TryBasicAttack();
    }

    // Bewegt den Enemy in Richtung seines Targets
    // Unter Bedingungen wie: Nicht in Basic Attack Range sein etc.
    private void MoveTowardsEnemy()
    {
        // Berechnet die Distanz zum targetet Character
        float DistanceToEnemy = Vector2.Distance(AttackedCharacter.GetComponent<Collider2D>().bounds.ClosestPoint(transform.position), new Vector2(transform.position.x, transform.position.y));

        // Wenn Distanz > AttackRange bewegt sich das Enemy auf den Character zu
        if (DistanceToEnemy > BasicAttackRange / 10.0f)
        {
            Vector3 direction = AttackedCharacter.transform.position - transform.position;
            transform.Translate(MovementSpeed * Time.deltaTime * direction.normalized);
        }
    }

    // Gibt an, wie viel Cooldown für einen Basic Attack herrscht
    public float Cooldown;
    // Slapped den targeted Character, wenn möglich
    private void TryBasicAttack()
    {
        // Wenn kein Cooldown & in AttackRange wird der Enemy attacked
        if (IsInRange(BasicAttackRange) && Cooldown < 0)
        {
            // Cooldown wird resettet
            Cooldown = AttackSpeed;
            // Fügt dem angegriffenen Character Schaden hinzu
            AttackedCharacter.GetComponent<CharacterController>().AddDamage(Damage);
        }
        // Cooldown wird um 1.0f pro Sekunde runter gesetzt
        Cooldown -= Time.deltaTime;
    }

    // Guckt, ob ein Gegner in Range für eine gewisse Ability z.B. BasicAttack ist
    private bool IsInRange(int AttackRange)
    {
        // Berechnet die Distanz zum targetet Character
        float DistanceToEnemy = Vector2.Distance(AttackedCharacter.GetComponent<Collider2D>().bounds.ClosestPoint(transform.position), new Vector2(transform.position.x, transform.position.y));

        // Wenn nach dem Movement noch außerhalb der AttackRange ist, kann nicht angegriffen
        if (DistanceToEnemy > AttackRange / 10.0f)
            return false;
        else return true;
    }    

    // Gibt dem Enemy Damage abhängig von den Stats des Angreifers und der Armor
    public void AddDamage(float Damage, float CritChance, float CritDamage)
    {
        // Gibt den finalen Damage an, den der Enemy bekommt
        float FinalDamage;

        // Bestimmt, ob ein Treffer ein (negativer) Crit ist
        if (UnityEngine.Random.Range(0, 100) <= Mathf.Abs(CritChance))
        {
            // Wenn negative Crit Chance wird der Crit Damage negativ hinzugefügt
            FinalDamage = Damage + (Damage * ((CritChance / Mathf.Abs(CritChance)) + (Mathf.Abs(CritDamage) / 100f)));
        }
        // Wenn kein Crit bleibt der Damage gleich
        else
            FinalDamage = Damage;

        // Setzt die Damage Reduction Value exakt in den Ursprung
        float ArmorConstant = -4605.1701859479995f;

        // Berechnet, wie viel Damage durch Armor abgezogen wird
        FinalDamage *= (float)Math.Round(Convert.ToDouble(1 - ((100f - Mathf.Exp(-((ArmorConstant + Armor) / 1000f))) / 100f)), 5);

        HP -= FinalDamage;
    }
}
