using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEventHandler : MonoBehaviour
{
    // Listen zum Verhindern falschen Verhaltens wie multiple Hits etc. 
    public int ID = SceneDB.AddEnemyID();

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void Damage(float Damage, float CritChance, float CritDamage)
    {
        // Gibt den finalen Damage an, den der Enemy bekommt
        float FinalDamage;

        // Bestimmt, ob ein Treffer ein (negativer) Crit ist
        if (Random.Range(0, 100) <= Mathf.Abs(CritChance))
        {
            // Wenn negative Crit Chance wird der Crit Damage negativ hinzugefügt
            FinalDamage = Damage + (Damage * ((CritChance / Mathf.Abs(CritChance)) + (Mathf.Abs(CritDamage) / 100f)));
        }
        else
            FinalDamage = Damage;

        // Setzt die Damage Reduction Value exakt in den Ursprung
        float ArmorConstant = -4605.1701859479995f;

        // Berechnet, wie viel Damage durch Armor abgezogen wird
        FinalDamage *= 1 - ((100 - Mathf.Exp((ArmorConstant + gameObject.GetComponent<EnemyStats>().Armor) / 1000)) / 100);

        gameObject.GetComponent<EnemyStats>().HP -= FinalDamage;
    }
}
