using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SlowAttribute : MonoBehaviour
{
    void Update()
    {
        // Slow Duration verringert sich
        Duration -= Time.deltaTime;
        // Wenn die Slow Duration um ist, zerst�rt sich das Component
        if (Duration < 0)
            Destroy(this);
    }

    // Wenn Strength 1 => 1% slow ...
    public int Strength;
    // Sekunden, die der Slow anh�lt
    public float Duration = 1.0f;
}
