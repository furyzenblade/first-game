using UnityEngine;

// Verwaltet die Hitboxen & Spawns der FireBall Ability
public class FireBallAbility : Ability
{
    // Movement Zeugs
    public float MovementSpeed;

    private void Start()
    {
        transform.rotation = AbilityPlacer.GetSpawnRotation(transform);
    }

    new void Update()
    {
        base.Update();

        // FireBall wird nach Vorne bewegt
        transform.Translate(Vector3.right * (MovementSpeed * Time.deltaTime));
    }
}
