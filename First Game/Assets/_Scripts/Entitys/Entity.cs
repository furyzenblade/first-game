// Base Klasse zum Erstellen eines Entitys
// Behaviour Mode: NPC / Hosted Character / Online Character

using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Identifier

    public int ID { get; set; }
    public ControlMode ControlMode;
    public Faction Faction;

    public static float DefaultZLayer;

    #endregion Identifier

    #region TopAlgorithm

    public void Start()
    {
        // Setzt für jedes Entity die Z Koordinate auf DefaultZLayer
        transform.position = new Vector3(transform.position.x, transform.position.y, DefaultZLayer);

        // Holt eine EntityID ab
        ID = SceneDB.AddEntityID();

        // Entfernt nullwerte aus AbilityCooldowns
        AbilityCooldowns = new List<float>() { };
    }

    public void Update()
    {
        HandleAttributes();

        BasicAttackAlgorithm();

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

    #endregion Abilitys

    #region Events

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

    #endregion Events

    #region BasicAttack / Movement

    // Properties
    public float AttackRange;                   // Range, in der attackiert werden kann
    public float AttackSpeed;                   // 1.0 = 1x pro Sekunde angreifen
    private float CurrentAttackSpeed;           // Attackspeed nach Berechnung von Attributes
    private float AttackCooldown = -0.0001f;    // Cooldown bis der nächste Attack folgen kann

    private Entity Target;

    // Algorithmus vom BasicAttack
    private void BasicAttackAlgorithm()
    {
        // Wenn Entity nicht gestunnt ist, wird der Basic Attack ausgeführt
        if (!IsStunned)
        {
            // Left Click wurde gedrückt => Movement / BasicAttack Event
            if (Input.GetKeyDown(KeyCode.Mouse0))
                HandleMovementEvent();

            // Target Position wird analysiert & drauf zu bewegt
            MoveTowards(AnalyseTargetPosition());

            // BasicAttack wird versucht
            TryBasicAttack();
        }

        // Cooldown wird runter gesetzt
        if (AttackCooldown > Time.deltaTime)
            AttackCooldown -= Time.deltaTime;
    }

    public void HandleMovementEvent()
    {
        try
        {
            // Ray wird erstellt
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Hit Object wird geholt
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            // Wenn die Faction None oder eine andere als die 
            if (hit.collider.gameObject.GetComponent<Entity>().Faction != Faction || hit.collider.gameObject.GetComponent<Entity>().Faction == Faction.None)
                Target = hit.collider.gameObject.GetComponent<Entity>();

            // Kein Entity also wird Target Location verändert
            else
                Target = null;
        }
        // Kein Entity also wird Target Location verändert
        catch
        { Target = null; }
    }

    // Analysiert die Target Position
    private Vector3 AnalyseTargetPosition()
    {
        Vector3 TargetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Bewegt sich in Richtung der TargetPosition bzw. in AttackRange wenn Target != null
        if (Target != null)
        {
            TargetPosition = Target.transform.position;

            float DistanceToEnemy = Vector2.Distance(TargetPosition, transform.position);
            if (DistanceToEnemy > AttackRange / 10.0f)
                return TargetPosition;
        }
        else
            return TargetPosition;

        // null wird returned
        return Vector3.zero;
    }

    // Bewegt das aktuelle GameObject möglichst nah an einen Punkt ran
    private void MoveTowards(Vector3 Position)
    {
        // Direction wird ermittelt
        Vector3 direction = Vector3.Normalize(Position - transform.position);

        // Bewegung richtung Target Position
        transform.Translate(CurrentSpeed * Time.deltaTime * direction);        
    }

    private void TryBasicAttack()
    {
        if (Target != null && AttackCooldown < 0f)
        {
            // Distance calculation
            float DistanceToEnemy = Vector2.Distance(Target.transform.position, new Vector2(transform.position.x, transform.position.y));

            // Prüft, ob der Gegner in Range ist
            if (DistanceToEnemy < AttackRange / 10.0f)
            {
                // Damaged den Gegner
                Target.AddDamage(Damage, CritChance, CritDamage);

                // Resettet den Cooldown
                AttackCooldown = CurrentAttackSpeed;
            }
        }
    }

    #endregion BasicAttack / Movement
}