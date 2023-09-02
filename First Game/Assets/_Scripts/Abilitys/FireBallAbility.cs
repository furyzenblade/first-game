using UnityEngine;

// Verwaltet die Hitboxen & Spawns der FireBall Ability
public class FireBallAbility : Ability
{
    // Movement Zeugs
    public float MovementSpeed;

    new void Update()
    {
        base.Update();

        // FireBall wird nach Vorne bewegt
        transform.Translate(Vector3.right * (MovementSpeed * Time.deltaTime));
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DamageEntity(collision.gameObject.GetComponent<Entity>());
    }
}
