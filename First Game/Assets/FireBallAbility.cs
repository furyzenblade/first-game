using UnityEngine;

// Verwaltet die Hitboxen & Spawns der FireBall Ability
public class FireBallAbility : Ability
{
    // Movement Zeugs
    public float MovementSpeed;

    private void Start()
    {
        // Bestimmt die Rotation der Maus als Quaternion (ChatGPT Code)
        #region CalculateRotation

        // Maus Position wird abgerufen
        Vector3 mousePosition = Input.mousePosition;
        // Setzt die Z- Koordinate auf einen richtigen Wert oder so
        mousePosition.z = -Camera.main.transform.position.z;

        // Convert the mouse position from screen coordinates to world coordinates
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Calculate the direction vector from the object's position to the mouse position
        Vector3 direction = mouseWorldPosition - transform.position;

        // Calculate the rotation angle based on the direction vector
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Convert the rotation to a Quaternion
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        #endregion CalculateRotation
    }

    void Update()
    {
        // FireBall wird nach Vorne bewegt
        transform.Translate(Vector3.right * (MovementSpeed * Time.deltaTime));
    }
}
