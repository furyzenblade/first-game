using Unity.VisualScripting;
using UnityEngine;

// Gibt das Muster an, in dem der Enemy die Character angreift
// Braucht denke ich noch Ability Support
    // Basic Attack könnte auch eine Ability werden ?? 
public class DarkZombieAI : EnemyAI
{
    public int SlowStrength;

    // Spielt jeden Frame die AI
    private new void Update()
    {
        base.Update();

        MoveTowardsEnemy();
        if (TryBasicAttack())
        {
            SlowAttribute Slow = AttackedCharacter.AddComponent<SlowAttribute>();
            Slow.Strength = SlowStrength;
        }
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
    private bool TryBasicAttack()
    {
        // Wenn kein Cooldown & in AttackRange wird der Enemy attacked
        bool CanAttack = IsInRange(BasicAttackRange) && (Cooldown < 0);

        if (CanAttack)
        {
            // Cooldown wird resettet
            Cooldown += AttackSpeed;
            // Fügt dem angegriffenen Character Schaden hinzu
            AttackedCharacter.GetComponent<CharacterController>().AddDamage(Damage);
        }
        // Cooldown wird um 1.0f pro Sekunde runter gesetzt
        if (Cooldown > -Time.deltaTime)
            Cooldown -= Time.deltaTime;

        return CanAttack;
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
}
