using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

// Kontrolliert einen Character mit allen Inputs etc., 
public class CharacterController : MonoBehaviour
{
    // Character Stats
    #region Stats

    // Defensive Stats
    public int MaxHP;
    public double HP;
    public int Armor;

    // Offensive Stats    
    public int Damage;
    public float AttackSpeed;
    public int CritChance;
    public float CritDamage;

    // Utility Stats
    public float MovementSpeed;
    public int AbliltyHaste;

    public float CurrentMovementSpeed;

    #endregion Stats

    // Control Variablen
    public bool IsControlledChar;

    // Negative Statuseffekte etc.
    private List<SlowAttribute> Slows = new() { };

    // Aggro 
    public int Aggro;

    // Ability Management
    public List<int> Abilitys = new() { };
    public List<float> AbilityCooldowns = new() { };

    // Beim Start werden die Abilitys, die der Character hat, zugewiesen 
    // Sobald es Einstellungen, Ability Trees etc. gibt, muss das hier reworked werden
    // Mit "Skilltree" oder so reworken
    void Start()
    {
        // Abilitys werden gesetzt
        Abilitys.Add(0);
        AbilityCooldowns.Add(0);
        Abilitys.Add(1);
        AbilityCooldowns.Add(1);
    }

    // Jeden Frame wird geprüft, ob der Character noch lebt etc. 
    // Reduziert auch Ability Cooldowns
    // Statuseffekte, Buffs etc. zerstören sich selbst, wenn die Duration abgelaufen ist
    void Update()
    {
        // Wenn der Character keine HP mehr hat, wird er inaktiv gesetzt
        if (HP < 0)
            gameObject.SetActive(false);

        // Aktuelle Stats werden ermittelt
        CurrentMovementSpeed = MovementSpeed;

        // Wenn Slows existieren, dann werden sie aufgelistet
        try { Slows = gameObject.GetComponents<SlowAttribute>().ToList(); } catch { }

        // Bestimmt, wie viel MovementSpeed der Character hat, abhängig von Slows
        foreach (SlowAttribute Slow in Slows)
            CurrentMovementSpeed *= 1 - (Slow.Strength / 100);

        // Reduziert die Cooldowns aller abilitys slightly
        // Muss auch für Buff Durations etc. gemacht werden
        for(int i = 0; i < AbilityCooldowns.Count; i++)
        {
            // Time.deltaTime sorgt dafür, dass pro Sekunde 1.0f reduziert werden
            AbilityCooldowns[i] -= Time.deltaTime;
        }
    }

    // Bewegt den Character jeden Frame in die global errechnete Richtung
    public void MoveCharacter(Vector3 Dir)
    {
        // Bewegt den Character abhängig von der Direction
        gameObject.transform.position += Dir * (MovementSpeed * Time.deltaTime);

        // Bewegt die Kamera mit dem Movement
        if (IsControlledChar)
            GameObject.FindGameObjectWithTag("MainCamera").transform.position += Dir * (MovementSpeed * Time.deltaTime);
    }

    // Nutzt die Ability mit einem bestimmten Index, die hier gespeichert wurde
    // Bestimmungsverfahren, welche Ability genutzt wird, erfordert dringend ein Rework
    public void UseAbility(int Index)
    {
        // Bestimmt & holt die Ability, die zu nutzen ist
        GameObject Ability = SceneDB.AllAbilitys[Abilitys[Index]];
        Ability AbilityComponent = Ability.GetComponent<Ability>();

        // Wenn die Ability keinen Cooldown hat, wird sie gezündet
        if (AbilityCooldowns[Index] < 0.0f)
        {
            // Setzt den Cooldown der Ability zurück
            AbilityCooldowns[Index] = GF.CalculateCooldown(AbilityComponent.Cooldown, AbliltyHaste);

            // Erstellt die Ability mit Position & Rotation
            Instantiate(Ability, gameObject.transform.position, Quaternion.identity);

            // Gibt der Ability ihre Stats
            Ability.GetComponent<Ability>().Damage = Damage;
            Ability.GetComponent<Ability>().CritChance = CritChance;
            Ability.GetComponent<Ability>().CritDamage = CritDamage;
        }
    }

    // Gibt dem Character Damage ohne Möglichkeit auf Crits
    public void AddDamage(float Damage)
    {
        HP -= GF.CalculateDamage(Damage, Armor);
    }
}
