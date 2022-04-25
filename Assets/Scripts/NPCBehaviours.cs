using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviours : MonoBehaviour
{
    // ALL NPC BEHAVIOURS
    // NOTE: Keep in mind while writing these that all behaviours must function under the Update() function.
    // Modify pattern/time sensitive methods to fit this.

    // Simple Behaviours (single action behaviours that may combine to form complex behaviours)

    // COMPONENTS

    public void newCollider(Vector2[] points, GameObject attachCollider) // Creates new PolygonCollider2D for gameObject (use for cone of vision and hitbox collider)
    {
        attachCollider.AddComponent<PolygonCollider2D>();
        attachCollider.GetComponent<PolygonCollider2D>().points = points;
    }

    public void newChild(string childName)
    {
        GameObject newChild = new GameObject(childName);
        newChild.transform.position = transform.position;
        newChild.transform.SetParent(transform);
    }

    public void turnChild(int direction, string childName) // Turns child object based on direction
    {
        GameObject turnChild = transform.Find(childName).gameObject;
        switch (direction)
        {
            case 1:
                turnChild.transform.rotation = new Quaternion(0, 0, 0, 90);
                break;
            case 2:
                turnChild.transform.rotation = new Quaternion(0, 0, -90, 90);
                break;
            case 3:
                turnChild.transform.rotation = new Quaternion(0, 0, -90, 0);
                break;
            case 4:
                turnChild.transform.rotation = new Quaternion(0, 0, 90, 90);
                break;
            default:
                break;
        }
    }

    // SPRITE CONTROL
    


    public int animateLoop(bool condition, int[] frameOrder, Sprite[] mainSprite, int frameCount, int animateSpeed) // Continuous animation while boolean value is true
    {
        if (condition == true)
        {
            if (frameCount < frameOrder.Length * animateSpeed)
            {
                if (frameCount % animateSpeed == 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = mainSprite[frameOrder[frameCount / animateSpeed]];
                }
                frameCount++;
                return frameCount;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = mainSprite[frameOrder[0]];
                return 1;
            }
            
        }
        return 0;
    }

    public bool stopAnimate(bool condition, int[] frameOrder, Sprite[] mainSprite, int frameCount, int animateSpeed) // Stops a single loop animation
    {
        if (frameCount < frameOrder.Length * animateSpeed)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void resetFrame(Sprite[] mainSprite, GameObject spriteChild) // Reset animation frame to default state
    {
        spriteChild.GetComponent<SpriteRenderer>().sprite = mainSprite[0];
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

    // MOVEMENT

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

    // CALCULATIONS

    public int findDirection(Vector2 targetPoint, int direction) // Find facing direction based on Vector2 position
    {
        int newDirection = direction;

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

    // DETECTION

    // STATES

    public bool checkIdle(bool[] conditions)
    {
        bool isIdle = true;
        foreach (bool a in conditions)
        {
            if (a == true)
            {
                isIdle = false;
            }
        }
        return isIdle;
    }

    public bool isMoving(GameObject parentObject) // Check if object with Rigidbody2D is moving, return boolean value accordingly
    {
        if (parentObject.GetComponent<Rigidbody2D>().velocity.x == 0 & parentObject.GetComponent<Rigidbody2D>().velocity.y == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // Compound Behaviours (behaviours made up of modified simple behaviours)
    public Sprite[] rotateTowards(Vector2 targetPoint, int direction, Sprite[] upSprite, Sprite[] rightSprite, Sprite[] downSprite, Sprite[] leftSprite, Sprite[] mainSprite, string[] children, GameObject spriteChild) // Rotate whole NPC towards direction and update values accordingly
    {
        direction = findDirection(targetPoint, direction);
        Sprite[] newMainSprite = turnSprite(direction, upSprite, rightSprite, downSprite, leftSprite, mainSprite);
        if (children != null)
        {
            foreach (string a in children)
            {
                turnChild(direction, a);
            }
        }
        resetFrame(newMainSprite, spriteChild);
        return newMainSprite;
    }

    public void newVision(float distance, float width, float sizeMult, GameObject attachCollider) // Simplified newCollider for creating cone of vision
    {
        Vector2 origin = new Vector2(0, 0);
        Vector2 point1 = new Vector2(distance * sizeMult, (width/2) * sizeMult);
        Vector2 point2 = new Vector2(distance * sizeMult, -(width / 2) * sizeMult);
        Vector2[] points = new Vector2[] {origin, point1, point2};
        newCollider(points, attachCollider);
    }

    public Vector2 idleWander(bool isIdle, int walkChance, Vector2 targetPoint, Sprite[] mainSprite, GameObject spriteChild)
    {
        Vector2 newPoint = targetPoint;

        if (isIdle == true)
        {
            resetFrame(mainSprite, spriteChild);
            int newPointChance = Random.Range(0, walkChance);
            if (newPointChance == 0)
            {
                float newPointX = Random.Range(-0.5f, 0.5f);
                float newPointY = Random.Range(-0.5f, 0.5f);
                newPoint = new Vector2(transform.position.x + newPointX, transform.position.y + newPointY);
            }

        }
        return newPoint;
    }



}
