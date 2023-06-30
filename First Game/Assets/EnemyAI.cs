using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Gibt ein generelles Muster an, nach dem jeder Enemy die Character angreift
public class EnemyAI : MonoBehaviour
{
    private GameObject AttackedCharacter;
    private EnemyStats EnemyStats;

    void Start()
    {

    }

    void Update()
    {
        // Für die AI nötige Werte werden aktualisiert
        AttackedCharacter = SceneDB.HighestAggroCharacter;
        EnemyStats = gameObject.GetComponent<EnemyStats>();

        MoveTowardsEnemy();
    }

    private void MoveTowardsEnemy()
    {
        // Berechnet die Distanz zum targetet Character
        float DistanceToEnemy = Vector2.Distance(AttackedCharacter.GetComponent<Collider2D>().bounds.ClosestPoint(transform.position), new Vector2(transform.position.x, transform.position.y));

        // Wenn Distanz > AttackRange bewegt sich das Enemy auf den Character zu
        if (DistanceToEnemy > EnemyStats.BasicAttackRange)
        {
            Vector3 direction = AttackedCharacter.transform.position - transform.position;
            transform.Translate(EnemyStats.MovementSpeed * Time.deltaTime * direction.normalized);
        }
    }
}
