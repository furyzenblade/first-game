using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Grundlage für alle GameObjects, die getroffen werden können sollen
public class EntityBase : MonoBehaviour
{
    // Properties
    #region Stats

    public int ID { get; set; }

    // Defensive Stats
    public int MaxHP;
    public double HP;

    public float Armor;
    public float CurrentArmor { get; set; }

    // Offensive Stats
    public float Damage;
    public float CurrentDamage { get; set; }

    public float AttackSpeed;
    public float CurrentAttackSpeed { get; set; }

    public int CritChance;
    public float CritDamage;

    // Utility Stats
    public float MaxSpeed;
    public float Speed { get; set; }

    public float HealingPower;

    public int BasicAttackRange;

    public int AbliltyHaste;


    // Ability Management
    public bool IsStunned { get; set; }

    public List<GameObject> Abilitys;
    public List<float> AbilityCooldowns { get; set; }

    #endregion Stats

    public void Start()
    {
        ID = SceneDB.AddEntityID();

        AbilityCooldowns = new List<float>() { };
    }

    // Updated Character Stats & 
    public void Update()
    {
        HandleAttributes();
        UpdateAbilityCooldowns();
    }

    // Attribute Handling
    #region Attributes

    // Handled effizient alle Attributes, die es gibt (Updaten bei neuen Attributes)
    private void HandleAttributes()
    {
        // Wenn keine Stuns existieren, werden Roots alle Attributes gehandled
        if (gameObject.GetComponents<StunAttribute>().Length == 0)
        {
            // Wenn keine Roots existieren, wird speed normal berechnet
            if (gameObject.GetComponents<RootAttribute>().Length == 0)
                HandleSlows();
            // Wenn Roots existieren, wird speed = 0 gesetzt
            else
                Speed = 0;

            IsStunned = false;
            HandleDamageReductions();
            HandleArmorReductions();
            HandleAttackspeedSlows();
        }
        // Wenn Stuns existieren, können keine Abilitys gecastet werden & Speed ist 0
        else
        {
            Speed = 0;
            IsStunned = true;
        }
    }

    private void HandleSlows()
    {
        Speed = MaxSpeed;

        // Bestimmt, wie viel MovementSpeed der Character hat, abhängig von Slows
        foreach (SlowAttribute Slow in gameObject.GetComponents<SlowAttribute>())
            Speed *= 1f - (Slow.Strength / 100f);
    }
    private void HandleDamageReductions()
    {
        CurrentDamage = Damage;

        // Bestimmt, wie viel MovementSpeed der Character hat, abhängig von Slows
        foreach (DamageReductionAttribute DamageReduction in gameObject.GetComponents<DamageReductionAttribute>())
            CurrentDamage *= 1f - (DamageReduction.Strength / 100f);
    }
    private void HandleArmorReductions()
    {
        CurrentArmor = Armor;

        // Bestimmt, wie viel MovementSpeed der Character hat, abhängig von Slows
        foreach (ArmorReductionAttribute ArmorReduction in gameObject.GetComponents<ArmorReductionAttribute>())
            CurrentArmor *= 1f - (ArmorReduction.Strength / 100f);
    }
    private void HandleAttackspeedSlows()
    {
        CurrentAttackSpeed = AttackSpeed;

        // Bestimmt, wie viel MovementSpeed der Character hat, abhängig von Slows
        foreach (AttackSpeedChange AttackSpeedSlow in gameObject.GetComponents<AttackSpeedChange>())
            CurrentAttackSpeed *= 1f - (AttackSpeedSlow.Strength / 100f);
    }
    public float GetAntiHealing()
    {
        return GF.CalculateAntiHealing(gameObject.GetComponents<AntiHealAttribute>().ToList());
    }

    #endregion Attributes

    private void UpdateAbilityCooldowns()
    {
        // Reduziert die Cooldowns aller abilitys slightly
        // Effekte wie Buffs machen das bei sich selbst
        for (int i = 0; i < AbilityCooldowns.Count; i++)
        {
            // Time.deltaTime sorgt dafür, dass pro Sekunde 1.0f reduziert werden
            if (AbilityCooldowns[i] > -Time.deltaTime)
                AbilityCooldowns[i] -= Time.deltaTime;
        }
    }


    // Healt ein Entity um eine gewisse Value
    public void Heal(float Healing)
    {
        // Healing wird berechnet mit Healing, HealPower & AntiHeal
        HP += Healing;

        // HP werden auf MaxHP gedeckelt
        if (HP > MaxHP)
            HP = MaxHP;
    }

    // Gibt dem Enemy Damage abhängig von den Stats des Angreifers und der Armor
    public void AddDamage(float Damage, float CritChance = 0, float CritDamage = 0)
    {        
        HP -= GF.CalculateDamage(Damage, CurrentArmor, CritChance, CritDamage);

        // Wenn Entity getötet wird das GameObject zerstört. Es kann aber noch eine Custom Methode ausführen
        if (HP < 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
    // Methode, die auf dieser Klasse basierende Objekte fürs Death Handling nutzen können
    public delegate void EntityDeathEvent();
    public event EntityDeathEvent OnDeath;


    public bool HandleBasicAttacks(GameObject Enemy)
    {
        if (Enemy.CompareTag("Enemy") || Enemy.CompareTag("Ally"))
        {
            BasicAttack CurrentAttack = null;
            try
            {
                CurrentAttack = gameObject.GetComponent<BasicAttack>();
            }
            catch { }

            if (CurrentAttack == null)
                return true;
            else if (CurrentAttack.Target.name != Enemy.name)
            {
                Destroy(CurrentAttack);
                return true;
            }
        }
        return false;
    }
}
