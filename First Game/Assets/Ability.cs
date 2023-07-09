using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

// Prefab für alle Abilitys
public class Ability : MonoBehaviour
{
    // Tag vom GameObject, was diese Ability gezündet hat
    public string OriginTag;

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

    // Healing Operatoren
    public float Healing;
    public float HealingPower;
    
    // Nutzbarkeit
    public int Slot;

    // Bestimmt, ob mehrere Targets getroffen werden können
    public bool CanHitMultipleTargets;

    // Bestimmt, ob und mit welcher Frequenz Enemys gehittet werden können
    public float MultipleHitFrequence;

    // Listen zum Verhindern falschen Verhaltens wie multiple Hits etc. 
    public List<int> HitEntityIDs;
    public List<float> HitEntityFrequence;

    // Timer bis die Ability despawned
    public float TimerTillDeath;
    public void Update()
    {
        // Reduziert das HitDelay der Ability für jeden einzelnen Entity
        for (int i = 0; i < HitEntityFrequence.Count; i++)
        {
            HitEntityFrequence[i] -= Time.deltaTime;
        }

        // Reduziert die Zeit bis zum Despawn um 1f/Sek
        TimerTillDeath -= Time.deltaTime;
        if (TimerTillDeath < 0)
            Destroy(gameObject);
    }

    // Fügt einem Entity unter Bedingungen Schaden hinzu
    public void DamageEntity(GameObject Entity, bool HitsEnemys, bool HitsAllys)
    {
        if (HitsEnemys && Entity.CompareTag("Enemy"))
        {
            Entity.GetComponent<EnemyAI>().AddDamage(Damage, CritChance, CritDamage);
            if (!CanHitMultipleTargets)
                Destroy(gameObject);
        }
        if (HitsAllys && Entity.CompareTag("Ally"))
        {
            Entity.GetComponent<CharacterController>().AddDamage(Damage, CritChance, CritDamage);
            if (!CanHitMultipleTargets)
                Destroy(gameObject);
        }
    }
    // Healed einen Entity unter Bedingungen
    public void HealEntity(GameObject Entity, bool HitsEnemys, bool HitsAllys)
    {
        if (HitsEnemys && Entity.CompareTag("Enemy"))
        {
            Entity.GetComponent<EnemyAI>().Heal(Healing, HealingPower);
            if (!CanHitMultipleTargets)
                Destroy(gameObject);
        }
        if (HitsAllys && Entity.CompareTag("Ally"))
        {
            Entity.GetComponent<CharacterController>().Heal(Healing, HealingPower);
            if (!CanHitMultipleTargets)
                Destroy(gameObject);
        }
    }
}
