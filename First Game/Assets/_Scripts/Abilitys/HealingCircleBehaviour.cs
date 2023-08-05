using UnityEngine;

public class HealingCircleBehaviour : Ability
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        HealEntity(collision.gameObject);
    }
}
