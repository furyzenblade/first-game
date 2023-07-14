using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Gibt das Muster an, in dem der Enemy die Character angreift
// Braucht denke ich noch Ability Support
    // Basic Attack könnte auch eine Ability werden ?? 
public class DarkZombieAI : EnemyAI
{
    public float BasicAttackSlowDuration;
    public int BasicAttackSlowStrength;

    private new void Start()
    {
        base.Start();

        // Erstellt AbilityCooldowns für jede Ability
        for (int i = 0; i < Abilitys.Count; i++)
        {
            AbilityCooldowns.Add(-0.0000001f);
        }
    }

    // Spielt jeden Frame die AI
    private new void Update()
    {
        base.Update();

        UseBasicAttack(SceneDB.HighestAggroCharacter);

        // Nutzt on Cooldown Abilitys
        for (int i = 0; i < AbilityCooldowns.Count; i++)
        {
            if (AbilityCooldowns[i] < 0f)
                UseAbility(AbilityCooldowns.IndexOf(AbilityCooldowns[i]));
        }
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
