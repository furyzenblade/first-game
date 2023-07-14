using UnityEngine;

public class HealingCircleBehaviour : Ability
{
    public float Range;

    // Setzt das GameObject an die richtige Stelle
    new void Start()
    {
        base.Start();
        if (!Origin.CompareTag("Enemy"))
            transform.position = AbilityPlacer.GetSpawnPosition(Range, gameObject.AddComponent<CircleCollider2D>(), transform);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        HealEntity(collision.gameObject);
    }
}
