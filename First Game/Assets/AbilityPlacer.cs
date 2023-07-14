using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPlacer
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

    public static Quaternion GetSpawnRotation(Transform Origin, Transform Target = null)
    {
        Vector3 direction;
        // Wenn Target null ist, wird die Mausposition verwendet
        if (Target == null)
        {
            direction = GetMousePosition(Origin) - Origin.position;
        }
        else
        {
            direction = Target.position - Origin.position;
        }

        // Richtung zum Target wird ermittelt
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

    // Berechnet die kleinstmögliche Distanz zum Target GameObject
    public static Vector3 GetClosestPositionToTarget(Transform Origin, Transform Target, float radius, bool OnlyReturnIfHits = false)
    {
        Vector3 direction = Target.transform.position - Origin.transform.position;
        float distance = direction.magnitude;
        float clampedDistance = Mathf.Clamp(distance, 0f, radius);

        Vector3 closestPosition = Origin.transform.position + direction.normalized * clampedDistance;

        if (OnlyReturnIfHits && closestPosition != Target.transform.position)
            return Vector3.negativeInfinity;
        return closestPosition;
    }
}
