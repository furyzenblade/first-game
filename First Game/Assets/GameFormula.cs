using System.Collections.Generic;
using UnityEngine;

// Speichert alle Formeln zur Berechnung von Stats im Game
public class GF
{
    // Formeln zum Berechnen, wie viel Damage eine Unit bekommt

    // Berechnet den direkten Damage
    public static float CalculateDamage(float Damage, float Armor, float CritChance = 0, float CritDamage = 0, float ArmorPen = 0f, int FlatArmorPen = 0)
    {
        // Wenn Crit eingetroffen hat, wird der Damage ver�ndert
        if (Random.Range(0, 100) <= Mathf.Abs(CritChance))
        {
            // Wenn negative Crit Chance wird der Crit Damage negativ hinzugef�gt
            Damage += Damage * ((CritChance / Mathf.Abs(CritChance)) + (Mathf.Abs(CritDamage) / 100f));
        }

        // Damage Multiplier wird geholt
        return Damage * CalculateDamageMultiplier(Armor, ArmorPen, FlatArmorPen);
    }

    // Berechnet, mit welchem Wert der Damage multipliziert werden muss
    public static float CalculateDamageMultiplier(float Armor, float ArmorPen = 0f, int FlatArmorPen = 0)
    {
        // Erst wird die Armor um die ArmorPen reduziert
        Armor *= 1 - ArmorPen;
        // Dann wird sie mit FlatArmorPen subtrahiert
        Armor -= FlatArmorPen;

        // Wenn die Armor > 0 ist, wird die Unit leicht belohnt
        if (Armor > 0)
            return 100f / (100f + Armor);
        // Wenn die Armor < 0 ist, wird die Unit h�rter bestraft
        else if (Armor < 0)
            return 1f + (Mathf.Pow(Mathf.Abs(Armor), 1.15f) / 100f);
        // Wenn die Armor = 0 ist, wird der default wert zur�ck gegeben
        else
            return 1;
    }


    // Formeln zum Berechnen von Cooldowns
    public static float CalculateCooldown(float Cooldown, float CDR)
    {
        return Cooldown * (100f / (100 + CDR));
    }

    // Formeln zum Berechnen von Healing
    public static float CalculateHealing(float Healing, float HealingScaling = 1, float HealingPower = 0, float AntiHeal = 0)
    {
        return (Healing + (HealingScaling * HealingPower)) * AntiHeal;
    }
    public static float CalculateAntiHealing(List<AntiHealAttribute> AntiHeals)
    {
        // Healing ist anfangs auf 1 also 100%
        float Healing = 1;

        // Healing wird f�r jede AntiHeal Quelle reduziert
        foreach (AntiHealAttribute AntiHeal in AntiHeals)
            Healing -= Healing * AntiHeal.Strength;

        // Healing wird als effektive Value zum direkten Multiplizieren �bergeben
        return Healing;
    }
}
