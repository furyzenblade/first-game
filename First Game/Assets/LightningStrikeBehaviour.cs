using UnityEngine;

// Verwaltet die Hitboxen & Spawns der LightningStrike Ability
public class LightningStrikeBehaviour : Ability
{
    // Range, in der der Blitz platziert werden kann
    public float Range;

    // Setzt das GameObject an die richtige Stelle
    new void Start()
    {
        base.Start();

        transform.position = AbilityPlacer.GetSpawnPosition(Range, gameObject.AddComponent<CircleCollider2D>(), transform);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DamageEntity(collision.gameObject);
    }
}
