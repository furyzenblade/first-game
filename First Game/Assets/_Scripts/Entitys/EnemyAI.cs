using UnityEngine;

public class EnemyAI : EntityBase
{
    public GameObject AttackedCharacter;

    // Spielt jeden Frame die AI
    public new void Update()
    {
        base.Update();

        // Bestimmt, welcher Character angegriffen wird
        AttackedCharacter = SceneDB.HighestAggroCharacter;
    }

    // Gibt dem Enemy Damage abhängig von den Stats des Angreifers und der Armor
    public new void AddDamage(float Damage, float CritChance = 0, float CritDamage = 0)
    {
        base.AddDamage(Damage, CritChance, CritDamage);
        // Zerstört das GameObject, wenn der Enemy getötet wurde
        if (HP < 0)
            Destroy(gameObject);
    }

    public void CheckIfIsAttacked()
    {
        // Wenn die Maus über diesem GameObject ist, 
        if (IsMouseOverGameObject())
        {
            // Gibt sie dem Character den Befehl, diesen Enemy anzugreifen
            SceneDB.ControlledCharacter.GetComponent<CharacterController>().UseBasicAttack(gameObject);
        }
    }

    private bool IsMouseOverGameObject()
    {
        // Perform a raycast from the mouse position
        // to check if it hits the current GameObject

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            // The mouse is currently over this GameObject
            return true;
        }

        return false;
    }

    // Nutzt eine Ability
    public void UseAbilitys(int AbilityIndex)
    {
        // Ability wird im RAM gespeichert
        GameObject Ability = Abilitys[AbilityIndex];

        // Ability Script wird gespeichert
        Ability AbilityComponent = Ability.GetComponent<Ability>();

        // Setzt den Origin der Ability
        AbilityComponent.Origin = gameObject;

        // Setzt das Target der Ability
        AbilityComponent.Target = AttackedCharacter;

        // Resettet den Cooldown der Ability
        AbilityCooldowns[AbilityIndex] = GF.CalculateCooldown(AbilityComponent.Cooldown, AbliltyHaste);

        // Erschafft die Ability
        Instantiate(Ability);
    }

    public void UseAbility(int AbilityIndex)
    {
        if (!IsStunned)
        {
            GameObject AbilityPrefab = Abilitys[AbilityIndex];

            GameObject newAbilityObject = Instantiate(AbilityPrefab);
            Ability AbilityComponent = newAbilityObject.GetComponent<Ability>();

            AbilityComponent.Origin = gameObject;
            AbilityComponent.Target = AttackedCharacter;

            AbilityCooldowns[AbilityIndex] = GF.CalculateCooldown(AbilityComponent.Cooldown, AbliltyHaste);
        }
    }

}
