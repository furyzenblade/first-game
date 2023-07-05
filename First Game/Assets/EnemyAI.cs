using UnityEngine;

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

    public GameObject AttackedCharacter;

    // Spielt jeden Frame die AI
    public void Update()
    {
        // Zerstört das GameObject, wenn der Enemy getötet wurde
        if (HP < 0)
            Destroy(gameObject);

        // Bestimmt, welcher Character angegriffen wird
        AttackedCharacter = SceneDB.HighestAggroCharacter;
    }

    // Gibt dem Enemy Damage abhängig von den Stats des Angreifers und der Armor
    public void AddDamage(float Damage, float CritChance, float CritDamage)
    {
        HP -= GF.CalculateDamage(Damage, Armor, CritChance, CritDamage);
    }
}
