using UnityEngine;

// Verwaltet die Hitboxen & Spawns der LightningStrike Ability
public class LightningStrikeBehaviour : Ability
{
    // Range, in der der Blitz platziert werden kann
    public float Range;

    // Collider mit dem Radius vom Blitz
    private CircleCollider2D SpawnRadius;

    // Setzt das GameObject an die richtige Stelle
    void Start()
    {
        SpawnRadius = gameObject.AddComponent<CircleCollider2D>();
        SpawnRadius.isTrigger = true;
        SpawnRadius.radius = Range;

        // für die Korrektur der Z-Koordinate
        float zCoordinate = transform.position.z;

        if (Vector3.Distance(transform.position, GetMousePosition()) > Range)
        {
            transform.position = SpawnRadius.ClosestPoint(GetMousePosition());
        }
        else
        {
            Vector3 mousePosition = GetMousePosition();
            mousePosition.z = transform.position.z; // Set the same z-coordinate as the GameObject
            transform.position = mousePosition;
        }

        // Z-Koordinate wird korrigiert
        transform.position = new Vector3(transform.position.x, transform.position.y, zCoordinate);

        Destroy(SpawnRadius);
    }

    // Berechnet die Mouse Position auf der Kamera
    Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = transform.position.z; // Set the same z-coordinate as the GameObject
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        return mousePosition;
    }
}
