using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireballSpawner : MonoBehaviour
{
    public GameObject fireball;

    void Start()
    {
        SpawnFireBall();
    }

    void Update()
    {
        
    }

    public void SpawnFireBall()
    {
        #region CalculateRotation
        // Get the mouse position in screen coordinates
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = - Camera.main.transform.position.z; // Adjust the z-coordinate based on the camera's position

        // Convert the mouse position from screen coordinates to world coordinates
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Calculate the direction vector from the object's position to the mouse position
        Vector3 direction = mouseWorldPosition - transform.position;

        // Calculate the rotation angle based on the direction vector
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Convert the rotation to a Quaternion
        Quaternion Rotation = Quaternion.Euler(0f, 0f, angle);
        #endregion CalculateRotation

        Instantiate(fireball, gameObject.transform.position, Rotation);
    }
}
