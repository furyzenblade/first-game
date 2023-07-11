using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Prefab f�r alle Abilitys
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

    // Gr��e der Ability
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

    // Bestimmt, ob und mit welcher Frequenz Enemys gehittet werden k�nnen
    public float MultipleHitFrequence;

    // Listen zum Verhindern falschen Verhaltens wie multiple Hits etc. 
    public List<int> HitEntityIDs { get; set; }
    public List<float> HitEntityFrequence { get; set; }

    // Timer bis die Ability despawned
    public float TimerTillDeath;

    // Bestimmt, ob mehrere Targets getroffen werden k�nnen
    public bool CanHitMultipleTargets;

    public void Start()
    {
        HitEntityIDs = new List<int>() { };
        HitEntityFrequence = new List<float>() { };

        // Setzt das Tag der Ability, falls nicht default gesetzt (kann evtl. trotzdem manche bugs nicht verhindern) 
        tag = "Ability";

        // Setzt die Origin Base f�r Performance Einsparungen
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
            // GameObject wird zerst�rt, wenn es ung�ltig erschaffen wurde (von keinem Entity)
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
        // Reduziert das HitDelay der Ability f�r jeden einzelnen Entity
        for (int i = 0; i < HitEntityFrequence.Count; i++)
        {
            HitEntityFrequence[i] -= Time.deltaTime;
        }

        // Reduziert die Zeit bis zum Despawn um 1f/Sek
        TimerTillDeath -= Time.deltaTime;
        if (TimerTillDeath < 0)
            Destroy(gameObject);
    }

    // F�gt einem Entity unter Bedingungen Schaden hinzu
    public void DamageEntity(GameObject Entity)
    {
        if (CanHitEntity(Entity) && Entity.CompareTag("Enemy"))
        {
            // Damaged den Entity
            Entity.GetComponent<EnemyAI>().AddDamage(Damage, CritChance, CritDamage);
            // F�gt den Entity in die Liste hinzu
            AddEntitysToList(Entity.GetComponent<EntityBase>().ID);
            // Zerst�rt die Ability, wenn sie nicht mehrere targets hitten darf
            if (!CanHitMultipleTargets)
                Destroy(gameObject);
        }
        else if (CanHitEntity(Entity) && Entity.CompareTag("Ally"))
        {
            // Damaged den Entity
            Entity.GetComponent<CharacterController>().AddDamage(Damage, CritChance, CritDamage);
            // F�gt den Entity in die Liste hinzu
            AddEntitysToList(Entity.GetComponent<EntityBase>().ID);
            // Zerst�rt die Ability, wenn sie nicht mehrere targets hitten darf
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
            // F�gt den Entity in die Liste hinzu
            AddEntitysToList(Entity.GetComponent<EntityBase>().ID);
            // Zerst�rt die Ability, wenn sie nicht mehrere targets hitten darf
            if (!CanHitMultipleTargets)
                Destroy(gameObject);
        }
        else if(CanHitEntity(Entity, true) && Entity.CompareTag("Ally"))
        {
            // Healed den Entity
            Entity.GetComponent<CharacterController>().Heal(Healing);
            // F�gt den Entity in die Liste hinzu
            AddEntitysToList(Entity.GetComponent<EntityBase>().ID);
            // Zerst�rt die Ability, wenn sie nicht mehrere targets hitten darf
            if (!CanHitMultipleTargets)
                Destroy(gameObject);
        }
    }

    // pr�ft, ob ein Entity gehittet werden kann & verwaltet die Liste f�r ihn
    private bool CanHitEntity(GameObject Entity, bool heal = false)
    {
        // EntityBase wird gespeichert
        EntityBase EntityBase = Entity.GetComponent<EntityBase>();

        // Wenn keine EntityBase gefunden wurde, kann das Entity nicht getroffen werden
        if (EntityBase == null) 
            return false;

        // bool zum speichern, ob ein Entity schonmal getroffen wurde
        EntityWasHitBefore = false;

        // Wenn das Entity schonmal getroffen wurde & der Cooldown gr��er 0 ist, kann es nicht getroffen werden
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
}
