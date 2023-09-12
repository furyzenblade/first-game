using UnityEngine;

// Verwaltet die Hitboxen & Spawns der FireBall Ability
public class FireBall : Ability
{
    // Movement Zeugs
    public float MovementSpeed;

    new void Update()
    {
        base.Update();

        // FireBall wird nach Vorne bewegt
        transform.Translate(Vector3.right * (MovementSpeed * Time.deltaTime));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Entity"))
            DamageEntity(collision.gameObject.GetComponent<Entity>());
    }
}
