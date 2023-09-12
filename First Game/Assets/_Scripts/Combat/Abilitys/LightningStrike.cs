using UnityEngine;

// Verwaltet die Hitboxen & Spawns der LightningStrike Ability
public class LightningStrike : Ability
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Entity"))
            DamageEntity(collision.gameObject.GetComponent<Entity>());
    }
}