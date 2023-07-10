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
}
