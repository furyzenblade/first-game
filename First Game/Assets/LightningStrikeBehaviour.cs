using UnityEngine;

// Verwaltet die Hitboxen & Spawns der LightningStrike Ability
public class LightningStrikeBehaviour : Ability
{
    // Range, in der der Blitz platziert werden kann
    public float Range;

    // Setzt das GameObject an die richtige Stelle
    void Start()
    {
        AbilityPlacer.GetSpawnPosition(Range, gameObject.AddComponent<CircleCollider2D>(), transform);
    }
}
