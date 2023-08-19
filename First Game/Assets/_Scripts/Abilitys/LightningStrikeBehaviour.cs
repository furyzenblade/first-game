using UnityEngine;

// Verwaltet die Hitboxen & Spawns der LightningStrike Ability
public class LightningStrikeBehaviour : Ability
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        DamageEntity(collision.gameObject);
    }
}