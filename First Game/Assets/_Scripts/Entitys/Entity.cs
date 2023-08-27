// Base Klasse zum Erstellen eines Entitys
// Behaviour Mode: NPC / Hosted Character / Online Character

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Identifier

    public int ID { get; set; }
    public ControlMode ControlMode;
    public Faction Faction;

    #endregion Identifier

    #region Algorithm

    public void Start()
    {
        ID = SceneDB.AddEntityID();

        AbilityCooldowns = new List<float>() { };
    }

    public void Update()
    {
        HandleAttributes();
        UpdateAbilityCooldowns();
    }

    #endregion Algorithm

    #region Stats

    // Defensive Stats
    public double MaxHP;
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
    public float Speed;
    public float CurrentSpeed { get; set; }

    public float HealingPower;

    // Basic Attack
    public int AttackRange;

    public int AbliltyHaste;

    // Debug Values
    public bool CanBeRevived;

    public float AttackCooldown;

    #endregion Stats

    #region AttributeManagement

    bool IsStunned;

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
        CurrentSpeed = Speed;

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

    #endregion AttributeManagement

    #region AbilityManagement

    public List<GameObject> Abilitys;
    public List<float> AbilityCooldowns { get; set; }

    // Reduziert die Cooldowns aller Abilitys um -1.0f/s
    private void UpdateAbilityCooldowns()
    {
        // Effekte wie Buffs machen das bei sich selbst
        for (int i = 0; i < AbilityCooldowns.Count; i++)
        {
            Ability CurrentAbility = Abilitys[i].GetComponent<Ability>();

            // Wenn die Ability mehr als einen Charge hat, wird der Cooldown so lange runter gesetzt bis sie X-Mal gedrückt werden kann
            if (AbilityCooldowns[i] > (-Time.deltaTime - (GF.CalculateCooldown(CurrentAbility.Cooldown, AbliltyHaste) * (CurrentAbility.MaxCharges - 1))))
                AbilityCooldowns[i] -= Time.deltaTime;
        }

        for (int i = 0; i < AbilityCooldowns.Count; i++)
        {

        }
    }

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

    #endregion AbilityManagement

    #region EventManagement

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
    public delegate void EntityDeathEvent();
    public event EntityDeathEvent OnDeath;
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

    #endregion EventManagement
}