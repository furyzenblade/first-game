using System.Collections.Generic;
using UnityEngine;

// Prefab f�r alle Abilitys
public class Ability : MonoBehaviour
{
    // Wird ben�tigt, um OnCollisionEnter2D auszul�sen
    public Collider2D Hitbox;
    public Rigidbody2D CharacterPhysics;

    // Movement Zeugs
    public float MovementSpeed;

    // Utility Upgrades
    public float Cooldown;
    public float Scale
    {
        get { return scale; }
        set
        {
            scale = value;
            gameObject.transform.localScale += Vector3.one * (scale / 600f);
        }
    }
    private float scale;

    // Damage Operatoren
    public float Damage;
    public float CritChance;
    public float CritDamage = 30;
    
    // Nutzbarkeit
    public int Slot;
}
