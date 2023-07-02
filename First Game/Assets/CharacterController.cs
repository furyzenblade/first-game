using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using Unity.VisualScripting;
using System.Linq;
using System;

public class CharacterController : MonoBehaviour
{
    // Character Stats
    #region Stats

    // Defensive Stats
    public int MaxHP;
    public double HP;
    public int Armor;

    // Offensive Stats    
    public int Damage;
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

    // Ability Management
    public List<int> Abilitys = new() { };
    public List<float> AbilityCooldowns = new() { };

    void Start()
    {
        GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        // Abilitys werden gesetzt
        Abilitys.Add(0);
        AbilityCooldowns.Add(0);
    }

    void Update()
    {
        if (HP < 0)
            gameObject.SetActive(false);

        // Aktuelle Stats werden ermittelt
        CurrentMovementSpeed = MovementSpeed;

        // Wenn Slows existieren, dann werden sie aufgelistet
        try { Slows = gameObject.GetComponents<SlowAttribute>().ToList(); } catch { }

        foreach (SlowAttribute Slow in Slows)
            CurrentMovementSpeed *= 1 - (Slow.Strength / 100);

        // Reduziert die Cooldowns aller abilitys slightly
        for(int i = 0; i < AbilityCooldowns.Count; i++)
        {
            AbilityCooldowns[i] -= Time.deltaTime;
        }
    }

    public void MoveCharacter(Vector3 Dir)
    {
        // Bewegt den Character
        gameObject.transform.position += Dir * (MovementSpeed * Time.deltaTime);

        // Bewegt die Kamera
        if (IsControlledChar)
            GameObject.FindGameObjectWithTag("MainCamera").transform.position += Dir * (MovementSpeed * Time.deltaTime);
    }

    public void UseAbility(int Index)
    {
        GameObject Ability = SceneDB.AllAbilitys[Abilitys[Index]];
        AbilityManager AbilityManager = Ability.GetComponent<AbilityManager>();

        if (AbilityCooldowns[Index] < 0.0f)
        {
            AbilityCooldowns[Index] = AbilityManager.Cooldown;

            #region CalculateRotation
            // Maus Position wird abgerufen
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z; // Adjust the z-coordinate based on the camera's position

            // Convert the mouse position from screen coordinates to world coordinates
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // Calculate the direction vector from the object's position to the mouse position
            Vector3 direction = mouseWorldPosition - transform.position;

            // Calculate the rotation angle based on the direction vector
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Convert the rotation to a Quaternion
            Quaternion Rotation = Quaternion.Euler(0f, 0f, angle);
            #endregion CalculateRotation

            Instantiate(Ability, gameObject.transform.position, Rotation);

            Ability.GetComponent<AbilityManager>().Damage = Damage;
            Ability.GetComponent<AbilityManager>().CritChance = CritChance;
            Ability.GetComponent<AbilityManager>().CritDamage = CritDamage;
        }
    }

    // Gibt dem Character Damage
    public void AddDamage(float Damage)
    {
        // Setzt die Damage Reduction Value exakt in den Ursprung
        float ArmorConstant = -4605.1701859479995f;

        // Berechnet, wie viel Damage durch Armor abgezogen wird
        Damage *= (float)Math.Round(Convert.ToDouble(1 - ((100f - Mathf.Exp(-((ArmorConstant + Armor) / 1000f))) / 100f)), 5);

        HP -= Damage;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided with: " + collision.gameObject);
    }

}
