using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Attribute
{
    // Erstellt ein Attribut
    public Attribute(AttributeIdentifier Identifier, float Strength, float Duration, List<Scaling> StrengthScalings = null, List<Scaling> DurationScalings = null, float TargetTenacity = 0)
    {
        this.Identifier = Identifier;
        this.Strength = Strength;
        this.Duration = Duration;

        if (StrengthScalings != null)
        {
            foreach (Scaling Scaling in StrengthScalings)
            {
                Strength += Scaling.GetScale();
            }
        }
        if (DurationScalings != null)
        {
            foreach (Scaling Scaling in DurationScalings)
            {
                Duration += Scaling.GetScale();
            }
        }

        this.TargetTenacity = TargetTenacity;
    }

    public AttributeIdentifier Identifier;
    public float TargetTenacity;

    public float Strength;
    public float Duration;

    public float ReduceDuration()
    {
        Duration -= Time.deltaTime;
        return Duration;
    }
}
