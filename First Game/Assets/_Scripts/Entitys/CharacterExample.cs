using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterExample : Entity
{
    private void Start()
    {
        // Subscribe to the OnDeath event and specify the method to execute
        OnDeath += HandleCharacterDeath;
    }

    // This method will be executed when OnDeath event is invoked
    private bool HandleCharacterDeath()
    {
        // Place your logic here for what should happen when the character dies
        Debug.Log("Character is dead!");
        // You can add more logic here as needed



        if (Faction == Faction.Ally)
            return true;

        return true;
    }

    // You can add other methods and properties specific to CharacterExample here
}

