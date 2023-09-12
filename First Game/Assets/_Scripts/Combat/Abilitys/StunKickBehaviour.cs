using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunKickBehaviour : Ability
{
    public float MovementSpeed;

    new void Update()
    {
        base.Update();

        // Stein wird nach Vorne bewegt
        transform.Translate(Vector3.right * (MovementSpeed * Time.deltaTime));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Entity"))
            DamageEntity(collision.gameObject.GetComponent<Entity>());
    }
}
