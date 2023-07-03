using UnityEngine;

// Enthält einen Slow mit Cooldown
public class SlowAttribute : MonoBehaviour
{
    // Reduziert den Slow Cooldown & zerstört sich selbst, wenn der Cooldown vorbei ist
    void Update()
    {
        // Slow Duration verringert sich
        Duration -= Time.deltaTime;
        // Wenn die Slow Duration um ist, zerstört sich das Component
        if (Duration < 0)
            Destroy(this);
    }

    // Wenn Strength 1 => 1% slow ...
    public int Strength;
    // Sekunden, die der Slow anhält
    public float Duration = 1.0f;
}
