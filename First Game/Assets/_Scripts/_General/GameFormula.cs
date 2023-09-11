using System.Collections.Generic;
using UnityEngine;

// Speichert alle Formeln zur Berechnung von Stats im Game
public class GF
{
    // Formeln zum Berechnen, wie viel Damage eine Unit bekommt

    // Berechnet den direkten Damage
    public static float CalculateDamage(float Damage, float Armor, float CritChance = 0, float CritDamage = 0, float ArmorPen = 0f, int FlatArmorPen = 0)
    {
        // Wenn Crit eingetroffen hat, wird der Damage verändert
        if (Random.Range(1, 101) <= Mathf.Abs(CritChance))
        {
            // Wenn negative Crit Chance wird der Crit Damage negativ hinzugefügt
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
        // Wenn die Armor < 0 ist, wird die Unit härter bestraft
        else if (Armor < 0)
            return 1f + (Mathf.Pow(Mathf.Abs(Armor), 1.15f) / 100f);
        // Wenn die Armor = 0 ist, wird der default wert zurück gegeben
        else
            return 1;
    }


    // Formeln zum Berechnen von Cooldowns
    public static float CalculateCooldown(float Cooldown, int CDR)
    {
        return Cooldown * (100f / (100 + CDR));
    }

    // Formeln zum Berechnen von Healing
    public static float CalculateHealing(float Healing, float HealingScaling = 1, float HealingPower = 0, float HealingModifier = 1)
    {
        return HealingModifier * (Healing + (HealingScaling * HealingPower));
    }
}
