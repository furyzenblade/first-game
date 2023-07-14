using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// Prefab für alle Abilitys
public class Ability : MonoBehaviour
{
    // GameObject, das die Ability abgefeuert hat
    public GameObject Origin { get; set; }
    public EntityBase OriginBase { get; set; }

    public bool HitsEnemys { get; set; }
    public bool HitsAllys { get; set; }
    public bool HealsEnemys { get; set; }
    public bool HealsAllys { get; set; }

    // Utility Values
    public float Cooldown;

    // Größe der Ability
    public float Scale
    {
        get { return scale; }
        set
        {
            scale = value;
            gameObject.transform.localScale += Vector3.one * (scale / 600f);
        }
    }
    private float scale;

    // Damage Operatoren
    public float Damage;
    public float DamageScaling;

    public float CritChance { get; set; }
    public float CritDamage { get; set; }

    // Healing Operatoren
    public float Healing;
    public float HealingScaling;

    // Bestimmt, ob und mit welcher Frequenz Enemys gehittet werden können
    public float MultipleHitFrequence;

    // Listen zum Verhindern falschen Verhaltens wie multiple Hits etc. 
    public List<int> HitEntityIDs { get; set; }
    public List<float> HitEntityFrequence { get; set; }

    // Timer bis die Ability despawned
    public float TimerTillDeath;

    // Bestimmt, ob mehrere Targets getroffen werden können
    public bool CanHitMultipleTargets;

    public Transform Target { get; set; }

    // Color für die Sprite, wenn sie von einem Enemy gespawned wurde
    public Color EnemyAbilityColor;

    public void Start()
    {
        transform.position = Origin.transform.position;

        if (Origin.CompareTag("Enemy"))
            GetComponent<SpriteRenderer>().color = EnemyAbilityColor;

        HitEntityIDs = new List<int>() { };
        HitEntityFrequence = new List<float>() { };

        // Setzt das Tag der Ability, falls nicht default gesetzt (kann evtl. trotzdem manche bugs nicht verhindern) 
        tag = "Ability";

        // Setzt die Origin Base für Performance Einsparungen
        OriginBase = Origin.GetComponent<EntityBase>();

        // Wenn die bools nicht belegt wurden, werden sie automatisch zugewiesen
        if (HitsEnemys == false && HitsAllys == false && HealsEnemys == false && HealsAllys == false)
        {
            // Wenn der Ersteller der Ability ein Enemy ist, hittet er Allys & healed Enemys
            if (Origin.CompareTag("Enemy"))
            {
                HitsAllys = true;
                HealsEnemys = true;
            }
            // Wenn der Ersteller der Ability ein Ally ist, hittet er Enemys & healed Allys
            else if (Origin.CompareTag("Ally"))
            {
                HitsEnemys = true;
                HealsAllys = true;
            }
            // GameObject wird zerstört, wenn es ungültig erschaffen wurde (von keinem Entity)
            else
                Destroy(gameObject);
        }

        // Stats der Ability werden berechnet
        Damage += DamageScaling / 100 * OriginBase.Damage;
        CritChance = OriginBase.CritChance;
        CritDamage = OriginBase.CritDamage;
        Healing = GF.CalculateHealing(Healing, HealingScaling, OriginBase.HealingPower, OriginBase.GetAntiHealing());
    }
    public void Update()
    {
        // Reduziert das HitDelay der Ability für jeden einzelnen Entity
        for (int i = 0; i < HitEntityFrequence.Count; i++)
        {
            HitEntityFrequence[i] -= Time.deltaTime;
        }

        // Reduziert die Zeit bis zum Despawn um 1f/Sek
        TimerTillDeath -= Time.deltaTime;
        if (TimerTillDeath < 0)
            Destroy(gameObject);
    }

    // Fügt einem Entity unter Bedingungen Schaden hinzu
    public void DamageEntity(GameObject Entity)
    {
        if (CanHitEntity(Entity) && Entity.CompareTag("Enemy"))
        {
            // Damaged den Entity
            Entity.GetComponent<EnemyAI>().AddDamage(Damage, CritChance, CritDamage);
            // Fügt, falls gewünscht, Statuseffekte hinzu
            if (!CustomAttributeHandling)
                AttatchAllAttributes(Entity);
            // Fügt den Entity in die Liste hinzu
            AddEntitysToList(Entity.GetComponent<EntityBase>().ID);
            // Zerstört die Ability, wenn sie nicht mehrere targets hitten darf
            if (!CanHitMultipleTargets)
                Destroy(gameObject);
        }
        else if (CanHitEntity(Entity) && Entity.CompareTag("Ally"))
        {
            // Damaged den Entity
            Entity.GetComponent<CharacterController>().AddDamage(Damage, CritChance, CritDamage);
            // Fügt, falls gewünscht, Statuseffekte hinzu
            if (!CustomAttributeHandling)
                AttatchAllAttributes(Entity);
            // Fügt den Entity in die Liste hinzu
            AddEntitysToList(Entity.GetComponent<EntityBase>().ID);
            // Zerstört die Ability, wenn sie nicht mehrere targets hitten darf
            if (!CanHitMultipleTargets)
                Destroy(gameObject);
        }
    }
    // Healed einen Entity unter Bedingungen
    public void HealEntity(GameObject Entity)
    {
        if (CanHitEntity(Entity, true) && Entity.CompareTag("Enemy"))
        {
            // Healed den Entity
            Entity.GetComponent<EnemyAI>().Heal(Healing);
            // Fügt, falls gewünscht, Statuseffekte hinzu
            if (!CustomAttributeHandling)
                AttatchAllAttributes(Entity);
            // Fügt den Entity in die Liste hinzu
            AddEntitysToList(Entity.GetComponent<EntityBase>().ID);
            // Zerstört die Ability, wenn sie nicht mehrere targets hitten darf
            if (!CanHitMultipleTargets)
                Destroy(gameObject);
        }
        else if(CanHitEntity(Entity, true) && Entity.CompareTag("Ally"))
        {
            // Healed den Entity
            Entity.GetComponent<CharacterController>().Heal(Healing);
            // Fügt, falls gewünscht, Statuseffekte hinzu
            if (!CustomAttributeHandling)
                AttatchAllAttributes(Entity);
            // Fügt den Entity in die Liste hinzu
            AddEntitysToList(Entity.GetComponent<EntityBase>().ID);
            // Zerstört die Ability, wenn sie nicht mehrere targets hitten darf
            if (!CanHitMultipleTargets)
                Destroy(gameObject);
        }
    }

    // prüft, ob ein Entity gehittet werden kann & verwaltet die Liste für ihn
    private bool CanHitEntity(GameObject Entity, bool heal = false)
    {
        // EntityBase wird gespeichert
#pragma warning disable UNT0026
        EntityBase EntityBase = Entity.GetComponent<EntityBase>();
#pragma warning restore UNT0026

        // Wenn keine EntityBase gefunden wurde, kann das Entity nicht getroffen werden
        if (EntityBase == null) 
            return false;

        // bool zum speichern, ob ein Entity schonmal getroffen wurde
        EntityWasHitBefore = false;

        // Wenn das Entity schonmal getroffen wurde & der Cooldown größer 0 ist, kann es nicht getroffen werden
        if (HitEntityIDs.Contains(EntityBase.ID))
        {
            // Wenn der Entity bereits in der Liste ist, wurde er schonmal gehittet
            EntityWasHitBefore = true;

            if (HitEntityFrequence[HitEntityIDs.IndexOf(EntityBase.ID)] > 0)
                return false;
        }

        if (heal)
        {
            if (HealsEnemys && Entity.CompareTag("Enemy"))
                return true;
            else if (HealsAllys && Entity.CompareTag("Ally"))
                return true;
        }
        else
        {
            if (HitsEnemys && Entity.CompareTag("Enemy"))
                return true;
            else if (HitsAllys && Entity.CompareTag("Ally"))
                return true;
        }

        return false;
    }

    private bool EntityWasHitBefore = false;
    private void AddEntitysToList(int EntityID)
    {
        if (!EntityWasHitBefore)
        {
            HitEntityIDs.Add(EntityID);
            HitEntityFrequence.Add(MultipleHitFrequence);
        }
        else
        {
            HitEntityFrequence[HitEntityIDs.IndexOf(EntityID)] = MultipleHitFrequence;
        }
    }

    // Speichert alle übergebenen Statuseffekte 
    #region SavedAttributes

    public float SlowDuration;
    public int SlowStrength;

    public float DamageReductionDuration;
    public int DamageReductionStrength;

    public float ArmorReductionDuration;
    public int ArmorReductionStrength;

    public float AttackSpeedSlowDuration;
    public int AttackSpeedSlowStrength;

    public float AntiHealDuration;
    public int AntiHealStrength;

    public float StunDuration;

    #endregion SavedAttributes

    // Bool zum Bestimmen, ob immer alle Attributes übergeben werden sollen
    public bool CustomAttributeHandling;

    // Fügt dem Target alle gespeicherten Attribute hinzu
    public void AttatchAllAttributes(GameObject Entity)
    {
        AttatchSlow(Entity);
        AttatchDamageReduction(Entity);
        AttatchArmorReduction(Entity);
        AttatchAttackSpeedSlow(Entity);
        AttatchAntiHeal(Entity);
        AttatchStun(Entity);
    }

    public void AttatchSlow(GameObject Entity)
    {
        SlowAttribute Slow = Entity.AddComponent<SlowAttribute>();
        Slow.Duration = SlowDuration;
        Slow.Strength = SlowStrength;
    }
    public void AttatchDamageReduction(GameObject Entity)
    {
        DamageReductionAttribute DamageReduction = Entity.AddComponent<DamageReductionAttribute>();
        DamageReduction.Duration = DamageReductionDuration;
        DamageReduction.Strength = DamageReductionStrength;
    }
    public void AttatchArmorReduction(GameObject Entity)
    {
        ArmorReductionAttribute ArmorReduction = Entity.AddComponent<ArmorReductionAttribute>();
        ArmorReduction.Duration = ArmorReductionDuration;
        ArmorReduction.Strength = ArmorReductionStrength;
    }
    public void AttatchAttackSpeedSlow(GameObject Entity)
    {
        AttackSpeedChange AttackSpeedSlow = Entity.AddComponent<AttackSpeedChange>();
        AttackSpeedSlow.Duration = AttackSpeedSlowDuration;
        AttackSpeedSlow.Strength = AttackSpeedSlowStrength;
    }
    public void AttatchAntiHeal(GameObject Entity)
    {
        AntiHealAttribute AntiHeal = Entity.AddComponent<AntiHealAttribute>();
        AntiHeal.Duration = AntiHealDuration;
        AntiHeal.Strength = AntiHealStrength;
    }
    public void AttatchStun(GameObject Entity)
    {
        StunAttribute Stun = Entity.AddComponent<StunAttribute>();
        Stun.Duration = StunDuration;
    }
}
