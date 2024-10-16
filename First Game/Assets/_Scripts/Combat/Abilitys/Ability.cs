using System.Collections.Generic;
using UnityEngine;

// Prefab f�r alle Abilitys
public class Ability : MonoBehaviour
{
    #region Properties

    // GameObject, das die Ability abgefeuert hat
    public Entity Origin { get; set; }
    private ControlMode ControlMode;
    public Entity Target { get; set; }

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
    public List<Scaling> DamageScalings = new() { };

    public float CritChance { get; set; }
    public float CritDamage { get; set; }

    // Healing Operatoren
    public float Healing;
    public float HealingScaling;

    // Bestimmt, ob und mit welcher Frequenz Enemys gehittet werden k�nnen
    public float MultipleHitFrequence;

    // Listen zum Verhindern falschen Verhaltens wie multiple Hits etc. 
    public List<int> HitEntityIDs = new() { };
    public List<float> HitEntityFrequence = new() { };

    // Timer bis die Ability despawned
    public float TimerTillDeath;

    // Bestimmt, ob mehrere Targets getroffen werden k�nnen
    public bool CanHitMultipleTargets;

    public float Range;

    public bool GetsSpawnRotation;
    public bool GetsSpawnPosition;

    public ushort MaxCharges;

    public List<Attribute> SavedAttributes = new() { };

    #endregion Properties

    #region TopAlgorithm

    public void Start()
    {
        // Setzt den ControlMode
        ControlMode = Origin.ControlMode;

        // Property Nullwerte 

        // Spawn Position & Rotation werden gesetzt
        if (GetsSpawnRotation)
            transform.rotation = GetSpawnRotation();
        else if (GetsSpawnPosition)
            transform.position = GetSpawnPosition();

        // Z Position wird gesetzt
        transform.position = new Vector3(transform.position.x, transform.position.y, Entity.DefaultZCoordinate);

        // Stats der Ability werden berechnet / abgeholt
        foreach (Scaling Scaling in DamageScalings)
            Damage += Scaling.GetScale();
        CritChance = Origin.CritChance;
        CritDamage = Origin.CritDamage;
        Healing = GF.CalculateHealing(Healing, HealingScaling, Origin.HealingPower, Origin.HealModifier);
    }

    public void Update()
    {
        // Reduziert das HitDelay der Ability f�r jeden einzelnen Entity
        for (int i = 0; i < HitEntityFrequence.Count; i++)
        {
            HitEntityFrequence[i] -= Time.deltaTime;
        }

        // Reduziert die Zeit bis zum Despawn um 1f/Sek und zerst�rt die Ability
        TimerTillDeath -= Time.deltaTime;
        if (TimerTillDeath < 0)
            Destroy(gameObject);
    }

    #endregion TopAlgorithm

    #region Events

    // Event bei Damagen eines Entitys
    public delegate void DamageEvent (Ability Ability, Entity HitEntity);
    public event DamageEvent OnDamage;

    // F�gt einem Entity unter Bedingungen Schaden hinzu
    public void DamageEntity(Entity Entity)
    {
        // Wenn die Faction None ist oder ungleich des Origins ist, kann Entity getroffen werden
        if (CanHitEntity(Entity) && Entity.Faction != Origin.Faction || Origin.Faction == Faction.None)
        {
            // Entity wird in die Liste hinzugef�gt
            HitEntityIDs.Add(Entity.ID);
            HitEntityFrequence.Add(MultipleHitFrequence);

            // OnAbilityDamage Event wird aufgerufen
            OnDamage?.Invoke(this, Entity);

            // Damaged den Entity
            Entity.AddDamage(Origin.ID, Damage, CritChance, CritDamage);

            // Resettet den Cooldown f�r den getroffenen Entity
            try   { HitEntityFrequence[HitEntityIDs.IndexOf(Entity.ID)] = MultipleHitFrequence; }
            catch { HitEntityFrequence.Add(MultipleHitFrequence); }

            // �bertr�gt alle Attribute wenn kein custom handling
            if (!CustomAttributeHandling)
                foreach (Attribute Attribute in SavedAttributes)
                    Target.Attributes.Add(Attribute);

            // Zerst�rt die Ability, wenn sie nicht mehrere targets hitten darf
            if (!CanHitMultipleTargets)
                Destroy(gameObject);

            // F�gt den Entity in die Liste hinzu
            AddEntitysToList(Entity.ID);
        }
    }

    // Event bei Healen eines Entitys
    public delegate void HealEvent(Ability Ability, Entity HitEntity);
    public event HealEvent OnHeal;


    // Healed einen Entity unter Bedingungen
    public void HealEntity(Entity Entity)
    {
        // Wenn die Faction gleich der Origin Faction ist, wird gehealed es sei denn Faction ist None
        if (CanHitEntity(Entity) && Entity.Faction == Origin.Faction && Entity.Faction != Faction.None)
        {
            // Entity wird in die Liste hinzugef�gt
            HitEntityIDs.Add(Entity.ID);
            HitEntityFrequence.Add(MultipleHitFrequence);

            // EntityHealEvent wird ausgel�st
            OnHeal?.Invoke(this, Entity);

            // Healt den Entity
            Entity.Heal(Healing);

            // Resettet den Cooldown f�r den getroffenen Entity
            try   { HitEntityFrequence[HitEntityIDs.IndexOf(Entity.ID)] = MultipleHitFrequence; }
            catch { HitEntityFrequence.Add(MultipleHitFrequence); }
            // F�gt, falls gew�nscht, Statuseffekte zum Entity hinzu
            if (!CustomAttributeHandling)
                AttatchAttributes(Entity);

            // Zerst�rt die Ability, wenn sie nicht mehrere targets hitten darf
            if (!CanHitMultipleTargets)
                Destroy(gameObject);

            // F�gt den Entity in die Liste hinzu
            AddEntitysToList(Entity.ID);
        }
    }

    // pr�ft, ob ein Entity gehittet werden kann & verwaltet die Liste f�r ihn
    private bool CanHitEntity(Entity Entity)
    {
        // Wenn ein Entity in der Liste von EntityIDs ist, wird er genauer inspiziert
        if (HitEntityIDs.Contains(Entity.ID))
        {
            // Wenn die Frequence > 0 ist, kann der Entity nicht gehittet werden
            if (HitEntityFrequence[HitEntityIDs.IndexOf(Entity.ID)] > 0)
                return false;
            // Entity kann gehittet werden weil Frequence < 0
            return true;
        }
        // Entity ist nicht in der Liste also wird true returnt
        else
            return true;
    }

    private void AddEntitysToList(int EntityID)
    {
        try
        {
            // Wenn der Entity in der Liste ist, wird sein Cooldown resettet
            HitEntityIDs.IndexOf(EntityID);
            HitEntityFrequence[HitEntityIDs.IndexOf(EntityID)] = MultipleHitFrequence;
        }
        catch
        {
            // Entity ist nicht in der Liste also wird die Frequence und ID neu hinzugef�gt
            HitEntityIDs.Add(EntityID);
            HitEntityFrequence.Add(MultipleHitFrequence);
        }
    }

    #endregion Events

    #region Attributes

    // Bool zum Bestimmen, ob immer alle Attributes �bergeben werden sollen
    public bool CustomAttributeHandling;

    // F�gt dem Target alle gespeicherten Attribute hinzu
    public void AttatchAttributes(Entity Entity)
    {
        foreach (Attribute Attribute in SavedAttributes)
        {
            Entity.Attributes.Add(Attribute);
        }
    }

    #endregion Attributes

    #region Placement

    private Quaternion GetSpawnRotation()
    {
        // Wenn NPC dann wird die Target Position genommen
        if (ControlMode == ControlMode.NPC)
            return GetRotation(Target.transform.position);

        // Wenn HostControl dann wird Rotation von der MousePosition berechnet
        else if (ControlMode == ControlMode.HostControl)
            return GetRotation(GetMousePositionOnScreen());

        // Hier Receiver Mode erstellen
        else if (ControlMode == ControlMode.Receiver) 
        {
            return new Quaternion(45, 0, 0, 0);
        }
        else
        {
            Debug.LogError("Entity: " + Origin.name + " doesn't have a ControlMode");
            return new Quaternion(0, 45, 0, 0);
        }
    }
    private Vector3 GetSpawnPosition()
    {
        if (Origin.ControlMode == ControlMode.NPC)
        {
            Vector3 directionToTarget = Target.transform.position - transform.position;
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget < Range)
                return Target.transform.position;

            return transform.position + directionToTarget.normalized * Range;
        }
        else if (Origin.ControlMode == ControlMode.HostControl)
        {
            Vector3 directionToTarget = GetMousePositionOnScreen() - transform.position;
            float distanceToTarget = directionToTarget.magnitude;

            // Wenn die Target Position weiter entfernt ist als Range wird m�glichst nah ran gesetzt
            if (distanceToTarget > Range)
                return transform.position + directionToTarget.normalized * Range;

            // Sonst wird direkt die Target Position genommen
            return transform.position + directionToTarget;
        }
        else if (Origin.ControlMode == ControlMode.Receiver)
        {
            // Hier Receiver Mode einf�gen
            return Vector3.zero;
        }
        else
        {
            Debug.LogError("Entity: " + Origin.name + " doesn't have a ControlMode");
            return Vector3.zero;
        }
    }

    private Vector3 GetMousePositionOnScreen()
    {
        Vector3 mousePosition = Input.mousePosition;

        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        mousePosition = new Vector3(mousePosition.x, mousePosition.y, -5.0f);

        return mousePosition;
    }

    private Quaternion GetRotation(Vector3 TargetPosition)
    {
        Vector3 directionToTarget = TargetPosition - transform.position;
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        Quaternion Rotation = Quaternion.Euler(0, 0, angle);
        return Rotation;
    }

    #endregion Placement
}