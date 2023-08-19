using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

// Kontrolliert einen Character mit allen Inputs etc., 
public class CharacterController : EntityBase
{
    // Control Variablen
    public bool IsControlledChar;

    // Aggro 
    public int Aggro;

    // Beim Start werden die Abilitys, die der Character hat, zugewiesen 
    // Sobald es Einstellungen, Ability Trees etc. gibt, muss das hier reworked werden
    // Mit "Skilltree" oder so reworken
    new void Start()
    {
        base.Start();

        for (int i = 0; i < Abilitys.Count; i++)
        {
            AbilityCooldowns.Add(-0.000001f);
        }
        // Abilitys werden gesetzt (temporär disabled)
        /*Abilitys.Add(0);
        AbilityCooldowns.Add(0);
        Abilitys.Add(1);
        AbilityCooldowns.Add(1);
        Abilitys.Add(2);
        AbilityCooldowns.Add(2); 
        Abilitys.Add(3);
        AbilityCooldowns.Add(3);*/
    }

    private new void Update()
    {
        base.Update();
        AddDamage(1);
    }

    // Bewegt den Character jeden Frame in die global errechnete Richtung
    public void MoveCharacter(Vector3 Dir)
    {
        // Bewegt den Character abhängig von der Direction
        transform.position += Dir * (Speed * Time.deltaTime);

        // Bewegt die Main Camera parallel zum Character
        Camera.main.transform.position += Dir * (Speed * Time.deltaTime);
    }

    // Nutzt die Ability mit einem bestimmten Index, die hier gespeichert wurde
    // Bestimmungsverfahren, welche Ability genutzt wird, erfordert dringend ein Rework
    public void UseAbility(int Index)
    {
        // Bestimmt & holt die Ability, die zu nutzen ist
        GameObject Ability = Abilitys[Index];
        Ability AbilityComponent = Ability.GetComponent<Ability>();

        // Wenn die Ability keinen Cooldown hat, wird sie gezündet
        if (AbilityCooldowns[Index] < 0.0f)
        {
            // Setzt den Cooldown der Ability zurück
            AbilityCooldowns[Index] += GF.CalculateCooldown(AbilityComponent.Cooldown, AbliltyHaste);

            // Erstellt die Ability mit Position & Rotation
            GameObject SpawnedAbility = Instantiate(Ability);
            SpawnedAbility.GetComponent<Ability>().Origin = gameObject;
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
            NewAttack.Range = BasicAttackRange;
            NewAttack.Target = Enemy;
        }
    }

    // Gibt dem Character Damage ohne Möglichkeit auf Crits
    public new void AddDamage(float Damage, float CritChance = 0, float CritDamage = 0)
    {
        base.AddDamage(Damage, CritChance, CritDamage);
        OnDeath += DeathHandler;
    }

    bool EntityIsDead = false;
    private void DeathHandler()
    {
        if (!EntityIsDead)
            Debug.Log($"{gameObject.name} has died.");
        else
            EntityIsDead = true;
    }
}
