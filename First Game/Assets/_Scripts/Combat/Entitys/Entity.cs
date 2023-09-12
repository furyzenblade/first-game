// Base Klasse zum Erstellen eines Entitys
// Behaviour Mode: NPC / Hosted Character / Online Character

using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Identifier

    public int ID { get; set; }
    public ControlMode ControlMode;
    public Faction Faction;

    public static float DefaultZCoordinate = -5.0f;

    #endregion Identifier

    #region TopAlgorithm

    public void Start()
    {
        // Setzt für jedes Entity die Z Koordinate auf DefaultZLayer
        AdjustZCoordinate();

        // Holt eine EntityID ab
        ID = SceneDB.AddEntityID();

        // Für jede Ability wird der Cooldown am Anfang auf 0 gesetzt
        for (int i = 0; i < Abilitys.Count; i++)
        {
            AbilityCooldowns.Add(-0.0001f);
        }
    }

    public void Update()
    {
        HandleAttributes();

        BasicAttackAlgorithm();

        UpdateAbilityCooldowns();

        AbilityAlgorithm();
    }

    #endregion Algorithm

    #region Stats

    // Defensive Stats
    public float MaxHP;
    public float HP;

    public float Armor;
    public float CurrentArmor { get; set; }

    // Offensive Stats
    public float Damage;
    public float CurrentDamage { get; set; }

    public int CritChance;
    public float CritDamage;

    // Utility Stats
    public float Speed;
    public float CurrentSpeed { get; set; }

    public float HealingPower;
    public float HealModifier { get; set; }

    public int AbilityHaste;    

    // Debug Values
    public bool CanBeRevived;

    public float GetStat(EntityStat Stat)
    {
        if (Stat == EntityStat.MaxHP)
            return MaxHP;
        else if (Stat == EntityStat.CurrentHP)
            return HP;
        else if (Stat == EntityStat.MissingHP)
            return 100 * (1 - (HP / MaxHP));
        else if (Stat == EntityStat.Armor)
            return CurrentArmor;
        else if (Stat == EntityStat.Damage)
            return CurrentDamage;
        else if (Stat == EntityStat.CritChance)
            return CritChance;
        else if (Stat == EntityStat.CritDamage)
            return CritDamage;
        else if (Stat == EntityStat.Speed)
            return CurrentSpeed;
        else if (Stat == EntityStat.HealingPower)
            return HealingPower;
        else if (Stat == EntityStat.AbilityHaste)
            return AbilityHaste;
        else if (Stat == EntityStat.AttackSpeed)
            return AttackSpeed;

        // Kein Stat gefunden => 0
        else return 0;
    }

    #endregion Stats

    #region Attributes

    bool IsStunned;
    public List<Attribute> Attributes = new() { };
    public List<Attribute> SavedAttributes = new() { };
    public float Tenacity;

    public bool CustomAttributeHandling = true;

    private void HandleAttributes()
    {
        #region StatReset

        CurrentSpeed = Speed;
        IsStunned = false;
        CurrentDamage = Damage;
        HealModifier = 1;
        CurrentAttackSpeed = AttackSpeed;
        CurrentArmor = Armor;

        #endregion StatReset

        foreach (Attribute Attribute in Attributes)
        {
            AttributeIdentifier Identifier = Attribute.Identifier;

            // Container werden hier geskipped
            if (Identifier == AttributeIdentifier.SpeedChange)
            {
                CurrentSpeed *= Attribute.Strength;
            }
            else if (Identifier == AttributeIdentifier.Root)
            {
                CurrentSpeed = 0;
            }
            else if (Identifier == AttributeIdentifier.Stun)
            {
                IsStunned = true;
                CurrentSpeed = 0;
            }
            else if (Identifier == AttributeIdentifier.DamageChange)
            {
                CurrentDamage *= Attribute.Strength;
            }
            else if (Identifier == AttributeIdentifier.HealChange)
            {
                HealModifier *= Attribute.Strength;
            }
            else if (Identifier == AttributeIdentifier.AttackSpeedChange)
            {
                CurrentAttackSpeed *= Attribute.Strength;
            }
            else if (Identifier == AttributeIdentifier.ArmorChange)
            {
                CurrentArmor *= Attribute.Strength;
            }

            Attribute.ReduceDuration();
        }
    }

    #endregion Attributes

    #region Abilitys

    public List<GameObject> Abilitys;
    public List<float> AbilityCooldowns = new() { };

    // Reduziert die Cooldowns aller Abilitys um -1.0f/s
    private void UpdateAbilityCooldowns()
    {
        // Effekte wie Buffs machen das bei sich selbst
        for (int i = 0; i < AbilityCooldowns.Count; i++)
        {
            Ability CurrentAbility = Abilitys[i].GetComponent<Ability>();

            // Wenn die Ability mehr als einen Charge hat, wird der Cooldown so lange runter gesetzt bis sie X-Mal gedrückt werden kann
            if (AbilityCooldowns[i] > (-Time.deltaTime - (GF.CalculateCooldown(CurrentAbility.Cooldown, AbilityHaste) * (CurrentAbility.MaxCharges - 1))))
                AbilityCooldowns[i] -= Time.deltaTime;
        }
    }

    // Algorithmus zum Nutzen von Abilitys
    private void AbilityAlgorithm()
    {
        if (!IsStunned)
        {
            // Algorithmus für NPCs
            if (ControlMode == ControlMode.NPC)
            {
                for (int i = 0; i < AbilityCooldowns.Count; i++)
                {
                    // Nutzt eine Ability, wenn kein Cooldown
                    if (AbilityCooldowns[i] < 0)
                        UseAbility(i);
                }
            }
            // Algorithmus für den Host
            else if (ControlMode == ControlMode.HostControl)
            {
                // Nutzt eine Ability, wenn kein Cooldown und der entsprechende Button gedrückt ist
                for (int i = 0; i < AbilityCooldowns.Count; i++)
                {
                    if (SceneDB.Settings.KeyBindings[i + 1].IsActive() && AbilityCooldowns[i] < 0)
                        UseAbility(i);
                }
            }
            // Algorithmus für Mitspieler
            else if (ControlMode == ControlMode.Receiver)
            {

            }
            else
                Debug.LogError("Entity: " + name + " doesnt't have a ControlMode");
        }
    }

    // Nutzt eine Ability

    public delegate void EntityAbilityUseEvent();
    public event EntityAbilityUseEvent OnAbilityUse;
    private void UseAbility(int index)
    {
        OnAbilityUse?.Invoke();

        // Erstellt eine Ability und setzt die nötigen Properties
        Ability NewAbility = Instantiate(Abilitys[index], transform.position, transform.rotation).GetComponent<Ability>();
        NewAbility.Origin = this;

        // Setzt das Target der Ability
        if (Target != null)
            NewAbility.Target = Target;

        // Resettet den Cooldown
        AbilityCooldowns[index] = NewAbility.Cooldown;
    }

    #endregion Abilitys

    #region Events

    // Event, wenn der Entity gehealed wird
    public delegate void EntityHealEvent();
    public event EntityHealEvent OnHeal;

    // Healt ein Entity um eine gewisse Value
    public void Heal(float Healing)
    {
        OnHeal?.Invoke();

        // Healing wird berechnet mit Healing, HealPower & AntiHeal
        HP += Healing;

        // HP werden auf MaxHP gedeckelt
        if (HP > MaxHP)
            HP = MaxHP;
    }

    // Event, wenn der Entity Damage kassiert
    public delegate void EntityDamageEvent(int OriginEntityID);
    public event EntityDamageEvent OnDamage;

    // Event beim Tod des Entitys
    public delegate bool EntityDeathEvent();
    public event EntityDeathEvent OnDeath;

    // Gibt dem Enemy Damage abhängig von den Stats des Angreifers und der Armor
    public void AddDamage(int OriginEntityID, float Damage, float CritChance = 0, float CritDamage = 0)
    {
        OnDamage?.Invoke(OriginEntityID);

        HP -= GF.CalculateDamage(Damage, CurrentArmor, CritChance, CritDamage);

        // Wenn Entity getötet wird das GameObject zerstört. Es kann aber noch eine Custom Methode ausführen
        if (HP < 0)
        {
            try
            {
                // Wenn OnDeath Invoke true returned, wird der Entity deleted
                if ((bool)(OnDeath?.Invoke()))
                    Destroy(gameObject);
                // Wenn false returned wird der Entity Death anders handled
                else
                {
                    // Entity hier in reviveable status bringen oder so


                }
            }
            // OnDeath?.Invoke() wird nicht behandelt also default: Entity wird deleted
            catch
            {
                Debug.Log("Unhandled OnDeath Event at" + gameObject.name);
                Destroy(gameObject);
            }
        }
    }

    // Methode, die auf dieser Klasse basierende Objekte fürs Death Handling nutzen können

    #endregion Events

    #region BasicAttack / Movement

    // Required Values
    public Entity Target;                           // Vom BasicAttack anvisiertes Target
    public Vector3 TargetPosition = Vector3.zero;   // Position, auf die sich zubewegt wird

    // BasicAttack Stats
    public float AttackRange;                       // Range, in der attackiert werden kann
    public float AttackSpeed;                       // 1.0 = 1x pro Sekunde angreifen
    private float CurrentAttackSpeed;               // Attackspeed nach Berechnung von Attributes
    private float AttackCooldown = -0.0001f;        // Cooldown bis der nächste Attack folgen kann

    // Fasst alle BasicAttack / Movement Algorithmen zusammen
    private void BasicAttackAlgorithm()
    {
        // Updatet Inputs / Events
        UpdateTarget();
        GetTargetPosition();

        if (!IsStunned)
        {
            // Lässt den Entity etwas ausführen
            MoveEntity();
            TryBasicAttack();
        }

        // Reduziert den AttackCooldown
        if (AttackCooldown > -Time.deltaTime)
            AttackCooldown -= Time.deltaTime;
    }

    // Überprüft, ob gewisse Ereignisse eingetroffen sind, die Movement / BasicAttacks verändern
    public void UpdateTarget()
    {
        if (ControlMode == ControlMode.NPC)
        {

        }
        else if (ControlMode == ControlMode.HostControl)
        {
            // Wenn Key gedrückt wurde
            if (SceneDB.Settings.KeyBindings[0].IsActive())
            {
                // Versucht, ein Target zu holen
                GetTarget();
            }
        }
        else if (ControlMode == ControlMode.Receiver)
        {

        }
    }

    // Versucht, ein Target zu holen oder holt eine TargetPosition
    private void GetTarget()
    {
        try
        {
            // Ray wird erstellt
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Hit Object wird geholt
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            // Wenn die Faction None oder eine andere als die 
            if (hit.collider.gameObject.GetComponent<Entity>().Faction != Faction || hit.collider.gameObject.GetComponent<Entity>().Faction == Faction.None)
            {
                Target = hit.collider.gameObject.GetComponent<Entity>();
            }

            // Kein Entity also wird Target Location verändert
            else
            {
                Target = null;
                TargetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        // Kein Entity also wird Target Location verändert
        catch
        { 
            Target = null;
            TargetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    // Holt eine TargetPosition, wenn ein Target gesetzt ist
    private void GetTargetPosition()
    {
        if (Target != null)
        {
            // Calculate the direction from the current GameObject to the target.
            Vector3 directionToTarget = Target.transform.position - transform.position;

            // Calculate the distance to the target.
            float distanceToTarget = directionToTarget.magnitude;

            // Wenn Distance > AttackRange wird die Target Position nicht verändert (Entity bleibt stehen)
            if (distanceToTarget > AttackRange)
                TargetPosition = transform.position + directionToTarget.normalized * distanceToTarget;
            else
                TargetPosition = transform.position;
        }
    }

    // Event was beim Movement eines Entitys auslöst
    public delegate void EntityMoveEvent(float Distance, Vector3 Direction);
    public event EntityMoveEvent OnMove;

    // Moved den Entity in Richtung TargetPosition
    private void MoveEntity()
    {
        // Holt die Richtung zur TargetPosition
        Vector3 directionToTarget = (Vector2)TargetPosition - (Vector2)transform.position;

        // Wenn die Distance größer als der Movement Speed ist, wird in Richtung des Targets bewegt
        if (directionToTarget.magnitude > CurrentSpeed * Time.deltaTime)
        {
            // Move
            transform.position += CurrentSpeed * Time.deltaTime * directionToTarget.normalized;

            // Movement Event Trigger mit Distance & Direction
            OnMove?.Invoke((CurrentSpeed * Time.deltaTime * directionToTarget.normalized).magnitude, directionToTarget.normalized);
        }

        // Sonst wird direkt aufs Target gewarped
        else
        {
            // Move
            transform.position = TargetPosition;

            // Movement Event Trigger mit Distance & Direction
            OnMove?.Invoke((transform.position - TargetPosition).magnitude, directionToTarget);
        }

        if (ControlMode == ControlMode.HostControl)
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        // Korrigiert die Z-Koordinate
        AdjustZCoordinate();
    }

    // Event beim Ausführen eines BasicAttacks
    public delegate void EntityBasicAttackEvent(int TargetID);
    public event EntityBasicAttackEvent OnBasicAttack;

    // Versucht, einen BasicAttack auszuführen (Wenn AttackRange > Distance zum Zentrum vom Target)
    private void TryBasicAttack()
    {
        if (Target != null && AttackCooldown < 0)
        {
            if ((Target.transform.position - transform.position).magnitude < AttackRange)
            {
                OnBasicAttack?.Invoke(Target.ID);

                // Überträgt alle Attribute wenn kein custom handling
                if (!CustomAttributeHandling)
                    foreach (Attribute Attribute in SavedAttributes)
                        Target.Attributes.Add(Attribute);

                // Damaged das Target
                Target.AddDamage(ID, Damage, CritChance, CritDamage);

                AttackCooldown += 1.0f / CurrentAttackSpeed;
            }
        }
    }

    public void AttatchAllAttributes(Entity Target)
    {
        foreach (Attribute Attribute in SavedAttributes)
        {
            AttatchAttribute(Attribute, Target);
        }
    }
    public void AttatchAttribute(Attribute Attribute, Entity Target)
    {
        Target.Attributes.Add(Attribute);
    }

    #endregion BasicAttack / Movement

    private void AdjustZCoordinate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, DefaultZCoordinate);        
    }
}