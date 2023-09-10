// Base Klasse zum Erstellen eines Entitys
// Behaviour Mode: NPC / Hosted Character / Online Character

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

    public int AbliltyHaste;

    // Debug Values
    public bool CanBeRevived;

    #endregion Stats

    #region Attributes

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
                CurrentSpeed = 0;

            IsStunned = false;
            HandleDamageReductions();
            HandleArmorReductions();
            HandleAttackspeedSlows();
        }
        // Wenn Stuns existieren, können keine Abilitys gecastet werden & Speed ist 0
        else
        {
            CurrentSpeed = 0;
            IsStunned = true;
        }
    }

    private void HandleSlows()
    {
        CurrentSpeed = Speed;

        // Bestimmt, wie viel MovementSpeed der Character hat, abhängig von Slows
        foreach (SlowAttribute Slow in gameObject.GetComponents<SlowAttribute>())
            CurrentSpeed *= 1f - (Slow.Strength / 100f);
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
            if (AbilityCooldowns[i] > (-Time.deltaTime - (GF.CalculateCooldown(CurrentAbility.Cooldown, AbliltyHaste) * (CurrentAbility.MaxCharges - 1))))
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

                Target.AddDamage(ID, Damage, CritChance, CritDamage);

                AttackCooldown += 1.0f / CurrentAttackSpeed;
            }
        }
    }

    #endregion BasicAttack / Movement

    private void AdjustZCoordinate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, DefaultZCoordinate);
    }
}