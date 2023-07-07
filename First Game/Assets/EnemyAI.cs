using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : EntityBase
{
    // ID, die den Enemy identifizierbar macht
    public int ID = SceneDB.AddEnemyID();

    public int BasicAttackRange;

    public GameObject AttackedCharacter;

    // Spielt jeden Frame die AI
    public new void Update()
    {
        base.Update();

        // Bestimmt, welcher Character angegriffen wird
        AttackedCharacter = SceneDB.HighestAggroCharacter;
    }

    // Gibt dem Enemy Damage abh�ngig von den Stats des Angreifers und der Armor
    public void AddDamage(float Damage, float CritChance, float CritDamage)
    {
        HP -= GF.CalculateDamage(Damage, Armor, CritChance, CritDamage);

        // Zerst�rt das GameObject, wenn der Enemy get�tet wurde
        if (HP < 0)
            Destroy(gameObject);
    }
}
