using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireBallBehaviour : MonoBehaviour
{
    // Generelle Objects, die wir brauchen für alles
    public CircleCollider2D Hitbox;

    // Movement Zeugs
    public float MovementSpeed;

    // Utility Upgrades / Values etc.
    public bool CanHitMultipleTargets;
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

    // Listen zum Verhindern falschen Verhaltens wie multiple Hits etc. 
    private readonly List<int> HitEntityIDs = new() { };

    void Start()
    {
        
    }

    void Update()
    {
        // Move the object forward based on its rotation
        transform.Translate(Vector3.right * (MovementSpeed / 60));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Wenn kein EnemyEventHandler vorhanden ist, wird die Collision ignoriert
        try
        {
            // Prüft, ob der Entity schonmal vom FireBall getroffen wurde & verhindert mehrfache Treffer
            bool CanHitEntity = true;
            foreach (int ID in HitEntityIDs)
            {
                if (collision.gameObject.GetComponent<EnemyEventHandler>().ID == ID)
                    CanHitEntity = false;
            }

            if (CanHitEntity)
            {
                // GameObject mit der Collision bekommt Damage
                collision.gameObject.GetComponent<EnemyEventHandler>().Damage(Damage, CritChance, CritDamage);

                // Getroffene GameObject ID wird gespeichert
                HitEntityIDs.Add(collision.gameObject.GetComponent<EnemyEventHandler>().ID);

                // Wenn CanHitMultipleTargets an ist, wird der Feuerball nicht zerstört
                if (!CanHitMultipleTargets)
                    Destroy(gameObject);
            }
        }
        catch { }
    }
}
