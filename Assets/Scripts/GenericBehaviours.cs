using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericBehaviours : MonoBehaviour
{
    // BASIC METHODS

    public int findDirection(Vector2 targetPoint) // Find facing direction based on Vector2 position
    {
        int newDirection = 0;

        if (Mathf.Abs(targetPoint.x - transform.position.x) > Mathf.Abs(targetPoint.y - transform.position.y))
        {
            if (targetPoint.x - transform.position.x > 0)
            {
                newDirection = 2;
            }
            else if (targetPoint.x - transform.position.x < 0)
            {
                newDirection = 4;
            }
        }
        else
        {
            if (targetPoint.y - transform.position.y > 0)
            {
                newDirection = 1;
            }
            else if (targetPoint.y - transform.position.y < 0)
            {
                newDirection = 3;
            }
        }
        return newDirection;
    }


    public void turnObject(int direction, GameObject turnObject) // Turns object based on direction (1-4)
    {
        switch (direction)
        {
            case 1:
                turnObject.transform.rotation = new Quaternion(0, 0, -90, 0);
                break;
            case 2:
                turnObject.transform.rotation = new Quaternion(0, 0, 90, 90);
                break;
            case 3:
                turnObject.transform.rotation = new Quaternion(0, 0, 0, 90);
                break;
            case 4:
                turnObject.transform.rotation = new Quaternion(0, 0, -90, 90);
                break;
            default:
                break;
        }
    }

    public void turnCollider(int direction, GameObject turnObject, Vector2 hitbox, bool crawler)
    {
        if (crawler == true)
        {
            switch (direction)
            {
                case 1:
                    turnObject.GetComponent<PolygonCollider2D>().points = new Vector2[] { new Vector2(-hitbox.x / 2, hitbox.y / 2), new Vector2(-hitbox.x / 2, -hitbox.y / 2), new Vector2(hitbox.x / 2, -hitbox.y / 2), new Vector2(hitbox.x / 2, hitbox.y / 2) };
                    break;
                case 2:
                    turnObject.GetComponent<PolygonCollider2D>().points = new Vector2[] { new Vector2(-hitbox.y / 2, hitbox.x / 2), new Vector2(-hitbox.y / 2, -hitbox.x / 2), new Vector2(hitbox.y / 2, -hitbox.x / 2), new Vector2(hitbox.y / 2, hitbox.x / 2) };
                    break;
                case 3:
                    turnObject.GetComponent<PolygonCollider2D>().points = new Vector2[] { new Vector2(-hitbox.x / 2, hitbox.y / 2), new Vector2(-hitbox.x / 2, -hitbox.y / 2), new Vector2(hitbox.x / 2, -hitbox.y / 2), new Vector2(hitbox.x / 2, hitbox.y / 2) };
                    break;
                case 4:
                    turnObject.GetComponent<PolygonCollider2D>().points = new Vector2[] { new Vector2(-hitbox.y / 2, hitbox.x / 2), new Vector2(-hitbox.y / 2, -hitbox.x / 2), new Vector2(hitbox.y / 2, -hitbox.x / 2), new Vector2(hitbox.y / 2, hitbox.x / 2) };
                    break;
                default:
                    break;
            }
        }
    }

    public Sprite[] turnSprite(int direction, Sprite[] upSprite, Sprite[] rightSprite, Sprite[] downSprite, Sprite[] leftSprite, Sprite[] mainSprite) // Change in-use sprite sheet based on direction
    {
        switch (direction)
        {
            case 1:
                return downSprite;
            case 2:
                return rightSprite;
            case 3:
                return upSprite;
            case 4:
                return leftSprite;
            default:
                return mainSprite;
        }
    }

    public Sprite[] genericTurn(Vector2 targetPoint, string[] children, Vector2 hitbox, bool crawler, Sprite[] upSprite, Sprite[] rightSprite, Sprite[] downSprite, Sprite[] leftSprite, Sprite[] mainSprite) // Turn whole NPC
    {
        if (targetPoint != new Vector2(transform.position.x, transform.position.y))
        {
            int newDirection = findDirection(targetPoint);
            turnCollider(newDirection, gameObject, hitbox, crawler);

            foreach (string a in children)
            {
                turnObject(newDirection, transform.Find(a).gameObject);
            }
            return turnSprite(newDirection, upSprite, rightSprite, downSprite, leftSprite, mainSprite);
        }
        else
        {
            return mainSprite;
        }
    }

    public Vector2 randomTarget(int targetChance, Vector2 currentPos, float randomRange) // Return random point near gameObject, use for idle wandering (completely random), larger targetChance = lower chance of activating
    {
        Vector2 newPoint = currentPos;
        if (Random.Range(0, targetChance) == 0)
        {
            float newPointX = Random.Range(-randomRange, randomRange);
            float newPointY = Random.Range(-randomRange, randomRange);
            newPoint = new Vector2(transform.position.x + newPointX, transform.position.y + newPointY);
        }
        return newPoint;
    }

    public Vector2 moveVelocity(Vector2 targetPoint, float sensitivity) // Return velocity multiplier based on x y distance from targetPoint
    {
        Vector2 velocity = new Vector2(0, 0);

        float distX = targetPoint.x - transform.position.x;

        float distY = targetPoint.y - transform.position.y;

        if (Mathf.Abs(distX) > sensitivity)
        {
            velocity.x = distX / Mathf.Abs(distX);
        }
        if (Mathf.Abs(distY) > sensitivity)
        {
            velocity.y = distY / Mathf.Abs(distY);
        }
        return velocity;
    }

    public void moveTowards(Vector2 targetPoint, Vector2 velocity, float speedMult, float stopDist, GameObject parentObject) // Move towards target Vector2 position at set velocity (apply speed multiplier here) until close enough (stopDist)
    {
        if (Vector2.Distance(transform.position, targetPoint) > stopDist)
        {
            Vector2 moveSpeed = velocity * speedMult;
            parentObject.GetComponent<Rigidbody2D>().velocity = moveSpeed;
        }
    }

    public GameObject genericLockTarget(List<GameObject> targetChoices, GameObject lastHit, GameObject currentTarget)
    {
        GameObject newTarget;
        if (targetChoices.Count > 1)
        {
            newTarget = targetChoices[Random.Range(1, targetChoices.Count)];
        }
        else
        {
            newTarget = gameObject;
        }

        foreach (GameObject target in targetChoices)
        {
            if (target == currentTarget & currentTarget != gameObject)
            {
                newTarget = currentTarget;
            }
        }
        foreach (GameObject target in targetChoices)
        {
            if (target == lastHit & lastHit != gameObject)
            {
                newTarget = target;
            }
        }
        return newTarget;
    }

    public List<GameObject> sortEnemies(List<GameObject> targetChoices, List<string> Enemies)
    {
        List<GameObject> enemyList = new List<GameObject>();
        foreach (GameObject target in targetChoices)
        {
            bool isEnemy = false;
            foreach (string enemy in Enemies)
            {
                if (target.GetComponent<GenericControl>().species == enemy)
                {
                    isEnemy = true;
                }
            }
            if (isEnemy == true)
            {
                enemyList.Add(target);
            }
        }
        return enemyList;
    }


    public void newAttack(GameObject attackObject, int[] spriteValue, float damage, string[] effects, float speed, GameObject parent, float sizeMult, Vector3 offset)
    {
        GameObject newAttack = Instantiate(attackObject, parent.transform.position + offset, Quaternion.identity);
        GenericAttack attackData = newAttack.GetComponent<GenericAttack>();
        attackData.attackSprite = new Sprite[spriteValue.Length];
        attackData.parent = parent;
        for (int a = 0; a < spriteValue.Length; a++)
        {
            attackData.attackSprite[a] = attackData.allAttackSprites[spriteValue[a]];
        }
    }

}
