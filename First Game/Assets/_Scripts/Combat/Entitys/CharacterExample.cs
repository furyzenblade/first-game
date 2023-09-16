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

    #region EventManagement

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
    private void HandleAbilityUse(Ability Ability)
    {
        Debug.Log("Entity: " + name + " used an Ability");

        Ability.OnDamage += HandleAbilityDamage;
        Ability.OnHeal += HandleAbilityHeal;
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

    private void HandleAbilityDamage(Ability Ability, Entity HitEntity)
    {
        Debug.Log("Ability: " + Ability.name + " hit Entity: " + HitEntity.name);
    }
    private void HandleAbilityHeal(Ability Ability, Entity HitEntity)
    {
        Debug.Log("Ability: " + Ability.name + " healed Entity: " + HitEntity.name);
    }

    #endregion EventManagement
}

