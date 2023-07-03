using System.Collections.Generic;
using UnityEngine;

// Eine Klasse, die grundlegend alle Abilitys haben soll
// Speichert außerdem das custom behaviour jeder Ability
// Was die Ability machen soll, wird mit dem GameObject- Tag bestimmt
// Oder generisch programmieren wie FindComponent<T> oder so
public class AbilityManager : MonoBehaviour
{
    // Upgrades der Ability
    // Besser noch in eine eigene Klasse stopfen oder so
    public bool CanHitMultipleTargets;

    // Movement Zeugs
    public float MovementSpeed;

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

    // Listen zum Verhindern falschen Verhaltens wie multiple Hits etc. 
    private readonly List<int> HitEntityIDs = new() { };
    public int Slot;

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
