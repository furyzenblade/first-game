using UnityEngine;

public class StunStone : Ability
{
    public float MovementSpeed;

    new void Start()
    {
        base.Start();

        // Stun Attribut wird hinzugefügt
        SavedAttributes.Add(new Attribute(AttributeIdentifier.Stun, 1, 1.5f));
    }

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
