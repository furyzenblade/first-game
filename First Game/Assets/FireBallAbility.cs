using System.Collections.Generic;
using UnityEngine;

// Speichert bzw. ist das Behaviour der FireBall Ability
public class FireBallAbility : Ability
{
    // Individuelle Upgrades
    public bool CanHitMultipleTargets;

    // Listen zum Verhindern falschen Verhaltens wie multiple Hits etc. 
    private readonly List<int> HitEntityIDs = new() { };

    void Update()
    {
        // Objekt wird nach Vorne bewegt
        // Noch in Behaviour- Klasse rein packen
        transform.Translate(Vector3.right * (MovementSpeed * Time.deltaTime));
    }

    // Wenn der mit einem Objekt collidet (Auch Behaviour eigentlich)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Wenn kein EnemyEventHandler vorhanden ist, wird die Collision ignoriert
        try
        {
            // Prüft, ob der Entity schonmal vom FireBall getroffen wurde & verhindert mehrfache Treffer
            bool CanHitEntity = true;
            foreach (int ID in HitEntityIDs)
            {
                // Wenn der Getroffene die gleiche ID hat, wie eine gespeicherte, wird er ignoriert und nicht mehrfach getroffen
                if (collision.gameObject.GetComponent<EnemyAI>().ID == ID)
                    CanHitEntity = false;
            }

            // Wenn das erste Mal gehittet: 
            if (CanHitEntity)
            {
                // GameObject mit der Collision bekommt Damage
                collision.gameObject.GetComponent<EnemyAI>().AddDamage(Damage, CritChance, CritDamage);

                // Getroffene GameObject ID wird gespeichert
                HitEntityIDs.Add(collision.gameObject.GetComponent<EnemyAI>().ID);

                // Wenn CanHitMultipleTargets an ist, wird der Feuerball nicht zerstört
                if (!CanHitMultipleTargets)
                    Destroy(gameObject);
            }
        }
        // Getroffenes Objekt war kein Enemy (hat keine EnemyAI) also wird er ignoriert
        catch { }
    }
}
