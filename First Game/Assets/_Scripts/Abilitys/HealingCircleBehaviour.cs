using UnityEngine;

public class HealingCircleBehaviour : Ability
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Entity"))
            HealEntity(collision.gameObject.GetComponent<Entity>());
    }
}
