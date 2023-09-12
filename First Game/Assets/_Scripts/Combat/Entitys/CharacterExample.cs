using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterExample : Entity
{
    private new void Start()
    {
        base.Start();

        #region ActivateEvents

        OnDeath += HandleCharacterDeath;
        OnBasicAttack += HandleBasicAttack;
        OnAbilityUse += HandleAbilityUse;
        OnDamage += HandleDamage;
        OnHeal += HandleHeal;
        OnMove += HandleMove;

        #endregion ActivateEvents
    }

    private bool HandleCharacterDeath()
    {
        // Place your logic here for what should happen when the character dies
        Debug.Log("Character is dead!");
        // You can add more logic here as needed

        if (Faction == Faction.Ally)
            return false;

        return true;
    }

    private void HandleBasicAttack(int TargetID)
    {
        Debug.Log("Entity: " + name + " performed a BasicAttack");
    }
    private void HandleAbilityUse()
    {
        Debug.Log("Entity: " + name + " used an Ability");
    }
    private void HandleDamage(int TargetID)
    {
        Debug.Log("Entity: " + name + " received damage");
    }
    private void HandleHeal()
    {
        Debug.Log("Entity: " + name + " received heal");
    }
    private void HandleMove(float Distance, Vector3 Direction)
    {
        Debug.Log("Entity: " + name + " moved");
    }
}

