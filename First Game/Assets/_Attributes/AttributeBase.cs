using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeBase : MonoBehaviour
{
    // Reduziert den Attribute Cooldown & zerstört sich selbst, wenn der Cooldown vorbei ist
    void Update()
    {
        // Slow Duration verringert sich
        Duration -= Time.deltaTime;
        // Wenn die Attribute Duration um ist, zerstört sich das Component
        if (Duration < 0)
            Destroy(this);
    }
    // Sekunden, die das Attribute anhält
    public float Duration = 1.0f;
}
