using UnityEngine;

public class HealingCircleBehaviour : Ability
{
    public float Range;

    // Setzt das GameObject an die richtige Stelle
    new void Start()
    {
        base.Start();

        transform.position = AbilityPlacer.GetSpawnPosition(Range, gameObject.AddComponent<CircleCollider2D>(), transform);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HealEntity(collision.gameObject);
    }
}
