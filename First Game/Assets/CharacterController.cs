using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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
    void Start()
    {
        // Abilitys werden gesetzt
        Abilitys.Add(0);
        AbilityCooldowns.Add(0);
        Abilitys.Add(1);
        AbilityCooldowns.Add(1);
    }

    // Bewegt den Character jeden Frame in die global errechnete Richtung
    public void MoveCharacter(Vector3 Dir)
    {
        // Bewegt den Character abh�ngig von der Direction
        gameObject.transform.position += Dir * (Speed * Time.deltaTime);

        // Bewegt die Kamera mit dem Movement
        if (IsControlledChar)
            GameObject.FindGameObjectWithTag("MainCamera").transform.position += Dir * (Speed * Time.deltaTime);
    }

    // Nutzt die Ability mit einem bestimmten Index, die hier gespeichert wurde
    // Bestimmungsverfahren, welche Ability genutzt wird, erfordert dringend ein Rework
    public void UseAbility(int Index)
    {
        // Bestimmt & holt die Ability, die zu nutzen ist
        GameObject Ability = SceneDB.AllAbilitys[Abilitys[Index]];
        Ability AbilityComponent = Ability.GetComponent<Ability>();

        // Wenn die Ability keinen Cooldown hat, wird sie gez�ndet
        if (AbilityCooldowns[Index] < 0.0f)
        {
            // Setzt den Cooldown der Ability zur�ck
            AbilityCooldowns[Index] += GF.CalculateCooldown(AbilityComponent.Cooldown, AbliltyHaste);

            // Erstellt die Ability mit Position & Rotation
            Instantiate(Ability, gameObject.transform.position, Quaternion.identity);

            // Gibt der Ability ihre Stats
            Ability.GetComponent<Ability>().Damage = Damage;
            Ability.GetComponent<Ability>().CritChance = CritChance;
            Ability.GetComponent<Ability>().CritDamage = CritDamage;
        }
    }

    // Gibt dem Character Damage ohne M�glichkeit auf Crits
    public void AddDamage(float Damage)
    {
        HP -= GF.CalculateDamage(Damage, Armor);

        // Wenn der Character keine HP mehr hat, wird er inaktiv gesetzt
        if (HP < 0)
            gameObject.SetActive(false);
    }
}
