using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    private float CurrentMovementSpeed;
    public int BasicAttackRange;

    public GameObject AttackedCharacter;

    // Negative Statuseffekte etc.
    private List<SlowAttribute> Slows = new() { };

    // Spielt jeden Frame die AI
    public void Update()
    {
        // Zerstört das GameObject, wenn der Enemy getötet wurde
        if (HP < 0)
            Destroy(gameObject);

        // Bestimmt, welcher Character angegriffen wird
        AttackedCharacter = SceneDB.HighestAggroCharacter;
    }

    private void HandleSlows()
    {
        CurrentMovementSpeed = MovementSpeed;

        // Wenn Slows existieren, dann werden sie aufgelistet
        try { Slows = gameObject.GetComponents<SlowAttribute>().ToList(); } catch { }

        // Bestimmt, wie viel MovementSpeed der Character hat, abhängig von Slows
        foreach (SlowAttribute Slow in Slows)
            CurrentMovementSpeed *= 1f - (Slow.Strength / 100f);
    }

    // Gibt dem Enemy Damage abhängig von den Stats des Angreifers und der Armor
    public void AddDamage(float Damage, float CritChance, float CritDamage)
    {
        HP -= GF.CalculateDamage(Damage, Armor, CritChance, CritDamage);
    }
}
