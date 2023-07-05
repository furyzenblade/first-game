using UnityEngine;

// Gibt das Muster an, in dem der Enemy die Character angreift
// Braucht denke ich noch Ability Support
    // Basic Attack k�nnte auch eine Ability werden ?? 
public class DarkZombieAI : EnemyAI
{
    // Spielt jeden Frame die AI
#pragma warning disable CS0108 // Element blendet vererbte Element aus; fehlendes 'new'-Schl�sselwort
    private void Update()
#pragma warning restore CS0108 // Element blendet vererbte Element aus; fehlendes 'new'-Schl�sselwort
    {
        base.Update();

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

    // Gibt an, wie viel Cooldown f�r einen Basic Attack herrscht
    public float Cooldown;
    // Slapped den targeted Character, wenn m�glich
    private void TryBasicAttack()
    {
        // Wenn kein Cooldown & in AttackRange wird der Enemy attacked
        if (IsInRange(BasicAttackRange) && Cooldown < 0)
        {
            // Cooldown wird resettet
            Cooldown = AttackSpeed;
            // F�gt dem angegriffenen Character Schaden hinzu
            AttackedCharacter.GetComponent<CharacterController>().AddDamage(Damage);
        }
        // Cooldown wird um 1.0f pro Sekunde runter gesetzt
        Cooldown -= Time.deltaTime;
    }

    // Guckt, ob ein Gegner in Range f�r eine gewisse Ability z.B. BasicAttack ist
    private bool IsInRange(int AttackRange)
    {
        // Berechnet die Distanz zum targetet Character
        float DistanceToEnemy = Vector2.Distance(AttackedCharacter.GetComponent<Collider2D>().bounds.ClosestPoint(transform.position), new Vector2(transform.position.x, transform.position.y));

        // Wenn nach dem Movement noch au�erhalb der AttackRange ist, kann nicht angegriffen
        if (DistanceToEnemy > AttackRange / 10.0f)
            return false;
        else return true;
    }
}
