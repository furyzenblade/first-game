using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attribute : MonoBehaviour
{
    public float Strength;
    public AttributeIdentifier Identifier;
    public float Duration;

    // Wenn IsContainer ist das Attribut ein Mögliches zum applien auf Entitys
    public bool IsContainer = false;
    public float ContainerDuration = float.MaxValue;

    public void Update()
    {
        // Wenn das Attribut ein Container ist, wird die "Buff" Duration verringert
        if (IsContainer)
            ContainerDuration -= Time.deltaTime;
        // Wenn es kein Container ist, wird die generelle Duration verringert
        else
            Duration -= Time.deltaTime;

        if (Duration < 0 || ContainerDuration < 0)
            Destroy(this);
    }
}
