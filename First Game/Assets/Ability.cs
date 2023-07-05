using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

// Prefab für alle Abilitys
public class Ability : MonoBehaviour
{
    // Utility Upgrades
    public float Cooldown;
    public float Scale
    {
        get { return scale; }
        set
        {
            scale = value;
            gameObject.transform.localScale += Vector3.one * (scale / 600f);
        }
    }
    private float scale;

    // Damage Operatoren
    public float Damage;
    public float CritChance;
    public float CritDamage = 30;
    
    // Nutzbarkeit
    public int Slot;

    // Bestimmt, ob mehrere Targets getroffen werden können
    public bool CanHitMultipleTargets;

    // Bestimmt, ob und mit welcher Frequenz Enemys gehittet werden können
    public float MultipleHitFrequence;

    // Listen zum Verhindern falschen Verhaltens wie multiple Hits etc. 
    public List<int> HitEntityIDs;
    public List<float> HitEntityFrequence;
        
    void Update()
    {
        // Reduziert das HitDelay der Ability für jeden einzelnen Entity
        for (int i = 0; i < HitEntityFrequence.Count; i++)
        {
            HitEntityFrequence[i] -= Time.deltaTime;
        }
    }

    // Wenn der mit einem Objekt collidet (Auch Behaviour eigentlich)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Wenn kein EnemyEventHandler vorhanden ist, wird die Collision ignoriert
        try
        {
            if (CanHitEntity(collision))
            {
                // GameObject mit der Collision bekommt Damage
                collision.gameObject.GetComponent<EnemyAI>().AddDamage(Damage, CritChance, CritDamage);

                // Wenn CanHitMultipleTargets an ist, wird die Ability nicht zerstört
                if (!CanHitMultipleTargets)
                    Destroy(gameObject);
            }
        }
        // Getroffenes Objekt war kein Enemy (hat keine EnemyAI) also wird er ignoriert
        catch { }
    }

    // Analysiert ein Entity, ob er gehittet werden kann
    bool CanHitEntity(Collision2D collision)
    {
        // Bestimmt, ob der Entity gehittet werden kann
        bool CanHit = true;
        // Variablen zur Identifizierung vom Entity
        int EntityID = collision.gameObject.GetComponent<EnemyAI>().ID;
        int EntityIndex = HitEntityIDs.IndexOf(EntityID);

        // Wenn das Entity schon in der Liste steht & der Cooldown > 0f ist, kann nicht gehittet werden
        if (EntityIndex != -1 && HitEntityFrequence[EntityIndex] > 0f)
            CanHit = false;

        // Aktualisiert die HitEntityID- und HitEntityFrequence- Listen
        if (CanHit)
        {
            // Wenn kein Index gefunden wurde, wird ein neues erstellt
            if (EntityIndex == -1)
            {
                HitEntityIDs.Add(EntityID);
                HitEntityFrequence.Add(MultipleHitFrequence);
            }
            else
            {
                HitEntityFrequence[EntityIndex] = MultipleHitFrequence;
            }
        }

        return CanHit;
    }
}
