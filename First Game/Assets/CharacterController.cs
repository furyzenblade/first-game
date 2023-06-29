using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using Unity.VisualScripting;

public class CharacterController : MonoBehaviour
{
    // Character Stats
    #region Stats

    // Defensive Stats
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

    #endregion Stats

    // Control Variablen
    public bool IsControlledChar;

    void Start()
    {
        
    }
    void Update()
    {

    }

    public void MoveCharacter(Vector3 Dir)
    {
        gameObject.transform.position += Dir * (MovementSpeed * Time.deltaTime);
    }
}
