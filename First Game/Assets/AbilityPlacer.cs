using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPlacer : MonoBehaviour
{
    // Returned einen Vector3 mit den Koordinaten, an denen ein Objekt erstellt werden soll
    public static Vector3 GetSpawnPosition(float Range, CircleCollider2D SpawnRadius, Transform transform)
    {
        SpawnRadius.isTrigger = true;
        SpawnRadius.radius = Range;

        // für die Korrektur der Z-Koordinate
        float zCoordinate = transform.position.z;

        if (Vector3.Distance(transform.position, GetMousePosition(transform)) > Range)
        {
            transform.position = SpawnRadius.ClosestPoint(GetMousePosition(transform));
        }
        else
        {
            Vector3 mousePosition = GetMousePosition(transform);
            mousePosition.z = transform.position.z; // Set the same z-coordinate as the GameObject
            transform.position = mousePosition;
        }

        // Z-Koordinate wird korrigiert
        return new Vector3(transform.position.x, transform.position.y, zCoordinate);
    }

    public static Quaternion GetSpawnRotation(Transform transform)
    {
        Vector3 direction = GetMousePosition(transform) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, angle);
    }

    // Berechnet die Mouse Position auf der Kamera
    static Vector3 GetMousePosition(Transform transform)
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = transform.position.z; // Set the same z-coordinate as the GameObject
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        return mousePosition;
    }
}
