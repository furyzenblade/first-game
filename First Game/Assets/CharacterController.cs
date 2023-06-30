using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using Unity.VisualScripting;
using System.Linq;

public class CharacterController : MonoBehaviour
{
    // Character Stats
    #region Stats

    // Defensive Stats
    public int MaxHP;
    public double HP;
    public int Armor;

    // Offensive Stats    
    public int Attack;
    public float AttackSpeed;
    public int CritChance;
    public float CritDamage;

    // Utility Stats
    public float MovementSpeed;
    public int AbliltyHaste;

    public float CurrentMovementSpeed;

    #endregion Stats

    // Control Variablen
    public bool IsControlledChar;

    // Negative Statuseffekte etc.
    private List<SlowAttribute> Slows = new() { };

    // Aggro 
    public int Aggro;

    void Start()
    {
        
    }
    void Update()
    {
        // Aktuelle Stats werden ermittelt
        CurrentMovementSpeed = MovementSpeed;

        // Wenn Slows existieren, dann werden sie aufgelistet
        try { Slows = gameObject.GetComponents<SlowAttribute>().ToList(); } catch { }

        foreach (SlowAttribute Slow in Slows)
            CurrentMovementSpeed *= 1 - (Slow.Strength / 100);
    }

    public void MoveCharacter(Vector3 Dir)
    {
        gameObject.transform.position += Dir * (MovementSpeed * Time.deltaTime);
    }
}
