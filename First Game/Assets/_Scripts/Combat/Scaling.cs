using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct Scaling
{
    public Scaling(float Value, float Scale)
    {
        this.Value = Value;
        this.Scale = Scale;
    }
    public Scaling(Entity Origin, EntityStat Stat, float Scale)
    {
        Value = Origin.GetStat(Stat);
        this.Scale = Scale;
    }

    public float Value;
    public float Scale;

    public readonly float GetScale()
    {
        return Value * Scale;
    }
}
