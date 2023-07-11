using Unity.VisualScripting;
using UnityEngine;

// Gibt das Muster an, in dem der Enemy die Character angreift
// Braucht denke ich noch Ability Support
    // Basic Attack könnte auch eine Ability werden ?? 
public class DarkZombieAI : EnemyAI
{
    public float BasicAttackSlowDuration;
    public int BasicAttackSlowStrength;

    // Spielt jeden Frame die AI
    private new void Update()
    {
        base.Update();

        UseBasicAttack(SceneDB.HighestAggroCharacter);
    }

    public void UseBasicAttack(GameObject Enemy)
    {
        if (HandleBasicAttacks(Enemy))
        {
            // Erstellt eine neue BasicAttack
            BasicAttack NewAttack = gameObject.AddComponent<BasicAttack>();

            // Gibt dem BasicAttack Werte
            NewAttack.Damage = Damage;
            NewAttack.CritChance = CritChance;

            // Utility Werte
            NewAttack.Range = BasicAttackRange;

            // Target
            NewAttack.Target = Enemy;

            // Statuseffekte
            NewAttack.SlowDuration = BasicAttackSlowDuration;
            NewAttack.SlowStrength = BasicAttackSlowStrength;
        }
    }
}
