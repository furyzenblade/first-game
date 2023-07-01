using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Gibt ein generelles Muster an, nach dem jeder Enemy die Character angreift
public class EnemyAI : MonoBehaviour
{
    // ID, die den Enemy identifizierbar macht
    public int ID = SceneDB.AddEnemyID();

    // Stats, die kontrollieren, wann ein Enemy destroyed wird
    public float HP;
    public float MaxHP;

    // Stats, die den Damage vom Enemy angeben
    public float Damage;
    public float AttackSpeed;

    // Stats, die kontrollieren, wie viel Damage man bekommt
    public float Armor;

    // Utility Stats
    public float MovementSpeed;
    public int BasicAttackRange;

    private GameObject AttackedCharacter;

    void Update()
    {
        if (HP < 0)
            Destroy(gameObject);

        // Für die AI nötige Werte werden aktualisiert
        AttackedCharacter = SceneDB.HighestAggroCharacter;

        MoveTowardsEnemy();
        TryBasicAttack();
    }

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
    float Cooldown;
    // Wenn kein Cooldown & in AttackRange wird der Enemy attacked
    private void TryBasicAttack()
    {
        if (IsInRange(BasicAttackRange) && Cooldown < 0)
        {
            Cooldown = AttackSpeed;
            AttackedCharacter.GetComponent<CharacterController>().AddDamage(Damage);
        }
        // Cooldown wird um 1 frame runter gesetzt, wenn nicht angegriffen werden konnte
        else { Cooldown -= Time.deltaTime; }
    }

    // Guckt, ob ein Gegner in AttackRange für eine gewisse Ability ist
    private bool IsInRange(int AttackRange)
    {
        // Berechnet die Distanz zum targetet Character
        float DistanceToEnemy = Vector2.Distance(AttackedCharacter.GetComponent<Collider2D>().bounds.ClosestPoint(transform.position), new Vector2(transform.position.x, transform.position.y));

        // Wenn nach dem Movement noch außerhalb der AttackRange ist, kann nicht angegriffen
        if (DistanceToEnemy > AttackRange / 10.0f)
            return false;
        else return true;
    }    

    // Gibt dem Enemy Damage abhängig von den Stats des Gegners und der eigenen Armor
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
        else
            FinalDamage = Damage;

        // Setzt die Damage Reduction Value exakt in den Ursprung
        float ArmorConstant = -4605.1701859479995f;

        // Berechnet, wie viel Damage durch Armor abgezogen wird
        FinalDamage *= (float)Math.Round(Convert.ToDouble(1 - ((100f - Mathf.Exp(-((ArmorConstant + Armor) / 1000f))) / 100f)), 5);

        HP -= FinalDamage;
    }
}
