using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAttack : MonoBehaviour
{
    public Sprite[] allAttackSprites;
    public Sprite[] attackSprite;
    Vector2 direction;
    public float Damage;
    public string[] Effects;
    public Vector2 Size;
    public Vector2 offset;
    public float flySpeed;
    public GameObject parent;
    public Vector2 targetPoint;
    public Vector2 velocity;
    public float sizeMult;
    public int lifeTime; // if set to 0, play animation once and die (not controlled/destroyed by ontrigger)

    // Animation
    int framePos = 0;
    int delay = 10;


    void attackAnimateLoop(int speed)
    {
        if (framePos < attackSprite.Length * speed)
        {
            if (framePos % speed == 0)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = attackSprite[framePos / speed];
            }

            framePos++;
        }
        else if (lifeTime == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            framePos = 0;
        }
    }

    float calcSpeed(Vector2 targetPoint)
    {
        float newSpeed = Mathf.Sqrt(Mathf.Pow(targetPoint.x, 2) + Mathf.Pow(targetPoint.y, 2));
        return newSpeed;
    }

    Vector2 limitSpeed(Vector2 targetPoint, float limitSpeed)
    {
        float currSpeed = calcSpeed(targetPoint);
        float ratio = limitSpeed / currSpeed;

        return new Vector2(targetPoint.x * ratio, targetPoint.y * ratio);
    }

    void attackComponents(Vector2 attackSize)
    {
        //lifeTime = 1;
        Size = new Vector2(0.2f, 0.06f);
        offset = new Vector2(0, -0.06f);
        //targetPoint = new Vector2(0,0);
        //flySpeed = 0;
        gameObject.AddComponent<PolygonCollider2D>();
        GetComponent<PolygonCollider2D>().points = new Vector2[] { new Vector2(-Size.x / 2 + offset.x, Size.y / 2 + offset.y), new Vector2(-Size.x / 2 + offset.x, -Size.y / 2 + offset.y), new Vector2(Size.x / 2 + offset.x, -Size.y / 2 + offset.y), new Vector2(Size.x / 2 + offset.x, Size.y / 2 + offset.y) };
        GetComponent<PolygonCollider2D>().isTrigger = true;

        if (lifeTime == 0 || lifeTime == 1)
        {
            transform.SetParent(parent.transform);
        }
        else
        {
            gameObject.AddComponent<Rigidbody2D>();
            GetComponent<Rigidbody2D>().isKinematic = true;
            //gameObject.GetComponent<Rigidbody2D>().velocity = limitSpeed(targetPoint, flySpeed);
        }
    }

    void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.gameObject.tag == "NPC" & hit.gameObject != parent.transform.parent.gameObject)
        {
            hit.gameObject.tag = "Destroyed";
            hit.gameObject.GetComponent<GenericControl>().destroyed = true;
        }
    }

    void Start()
    {
        attackComponents(Size);
        
    }

    void FixedUpdate()
    {
        attackAnimateLoop(delay);
    }


}

