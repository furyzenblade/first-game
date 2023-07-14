using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunKickBehaviour : Ability
{
    public float MovementSpeed;

    private new void Start()
    {
        base.Start();

        transform.rotation = AbilityPlacer.GetSpawnRotation(transform, Target);
    }

    new void Update()
    {
        base.Update();

        // Stein wird nach Vorne bewegt
        transform.Translate(Vector3.right * (MovementSpeed * Time.deltaTime));
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DamageEntity(collision.gameObject);
    }
}
