using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericControl : MonoBehaviour
{
    // VARIABLES
    public int switchState = 0; // 0 - IDLE, 1 - AGGRESSIVE, 2 - DEFENSIVE, ADD MORE IF NECESSARY
    public GameObject lastHit;
    Vector2 velocity = new Vector2(0, 0);
    public bool destroyed = false;

    // SCRIPTS
    public GenericSetup setup; // NPC Component setup
    public GenericBehaviours behaviours; // NPC behaviours
    public GenericData data; // Save and load
    public GenericVision vision;

    // SETUP
    public Vector2[] visionPoints;
    public string[] subChildren;
    public string[] children;

    public Vector2 hitbox; // (Width, Height)
    public GameObject activeTarget;
    public Vector2 targetPoint;

    public GameObject globalAttackObject;

    // ANIMATION
    public Sprite[] mainSprite;
    public Sprite[] upSprite;
    public Sprite[] rightSprite;
    public Sprite[] downSprite;
    public Sprite[] leftSprite;
    public Sprite[] miscSprites;
    public int[] idleFrames;
    public int[] loopFrames;
    public int[] actionFrames;
    public int[] currentFrames;
    public int framePos;
    public bool activeAction;

    // MOVEMENT
    public int speed;

    // DATA
    public string species;
    public List<string> enemySpecies;

    public Camera cam;

    void tempSetup() // things that will be handled by load script in the future
    {
        species = "human";
        enemySpecies = new List<string>() {"human"};
    }

    void Setup() // CALL AT CREATE NPC COMPONENTS, Run through Start
    {
        // Scripts
        setup = GetComponent<GenericSetup>();
        data = GetComponent<GenericData>();
        behaviours = GetComponent<GenericBehaviours>();

        // Variable Constants
        lastHit = gameObject;
        idleFrames = new int[] { 0, 1 };
        loopFrames = idleFrames;
        actionFrames = new int[] { 0, 4, 5 };
        visionPoints = new Vector2[] { new Vector2(0, 0), new Vector2(-0.3f, -1.5f), new Vector2(0.3f, -1.5f) };
        children = new string[] { "attack", "rotate" };
        subChildren = new string[] { "vision", "overlay" };
        hitbox = new Vector2(0.1f, 0.2f);
        gameObject.tag = "NPC";
        mainSprite = upSprite;
        targetPoint = transform.position;

        // Create Components
        setup.newBody(gameObject);
        setup.newCollider(new Vector2[] { new Vector2(-hitbox.x / 2, hitbox.y / 2), new Vector2(-hitbox.x / 2, -hitbox.y / 2), new Vector2(hitbox.x / 2, -hitbox.y / 2), new Vector2(hitbox.x / 2, hitbox.y / 2) }, gameObject);

        foreach (string a in children)
        {
            setup.newChild(a, gameObject);
        }
        foreach (string a in subChildren)
        {
            setup.newChild(a, transform.Find("rotate").gameObject);
        }
        setup.newBody(transform.Find("rotate").Find("vision").gameObject);
        transform.Find("rotate").Find("vision").gameObject.AddComponent<GenericVision>();
        vision = transform.Find("rotate").Find("vision").gameObject.GetComponent<GenericVision>();
        setup.newCollider(visionPoints, transform.Find("rotate").Find("vision").gameObject);
        setup.attachVision(transform.Find("rotate").Find("vision").gameObject);
    }

    void newRandomData() // CALL IF NOT LOADING DATA, Run through Start
    {
        // STATS
        speed = 10;
    }

    void animSwitch() // DETECT ACTIONS, Run through Update, CONSTANT
    {
        if (velocity.x != 0 || velocity.y != 0)
        {
            speed = 10;
            loopFrames = new int[] { 2, 0, 3, 0 };
        }
        else
        {
            speed = 30;
            loopFrames = idleFrames;
        }

        if (activeAction == true)
        {
            speed = 10;
            currentFrames = actionFrames;
        }
        else
        {
            currentFrames = loopFrames;
        }
        

        if (Input.GetAxis("Fire1") != 0 & activeAction == false) // ACTION CONDITION, CHANGE LATER
        {
            Vector2 tempVel = new Vector2(targetPoint.x - transform.position.x, targetPoint.y - transform.position.y);
            behaviours.newAttack(tempVel, globalAttackObject, new int[] { 0, 1 }, 10, new string[0], 0, transform.Find("attack").gameObject, 1, 0.1f);
            framePos = 0;
            activeAction = true;
        }
    }

    void animUpdate() // RUN ANIMATION, Run through FixedUpdate, CONSTANT
    {
        // ANIMATE THROUGH CURRENT FRAMES AT SPEED INTERVAL
        if (framePos < currentFrames.Length * speed)
        {
            if (framePos % speed == 0)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = mainSprite[currentFrames[framePos / speed]];
            }

            framePos++;
        }
        else
        {
            activeAction = false;
            framePos = 0;
        }
    }

    void constantBehaviours() // MISC ACTIONS, Run through Update, CONSTANT
    {
        activeTarget = behaviours.genericLockTarget(behaviours.sortEnemies(vision.inSight, enemySpecies), lastHit, activeTarget);
        mainSprite = behaviours.genericTurn(targetPoint, new string[] { "rotate" }, hitbox, false, upSprite, rightSprite, downSprite, leftSprite, mainSprite);
        velocity = behaviours.moveVelocity(targetPoint, 0.2f);
        behaviours.moveTowards(targetPoint, velocity, 0.5f, 0, gameObject);
    }

    void idleUpdate()
    { 
    
    }

    void idleFixed()
    {
        if (velocity == new Vector2(0, 0))
        {
            targetPoint = behaviours.randomTarget(100, new Vector2(transform.position.x, transform.position.y), 2);
        }
    }

    void aggressiveUpdate()
    {
        targetPoint = activeTarget.transform.position;
    }

    void aggressiveFixed()
    {
        if (Vector2.Distance(transform.position, targetPoint) < 0.5f)
        {
            if (Random.Range(0, 30) == 0)
            {
                Vector2 tempVel = new Vector2(targetPoint.x - transform.position.x, targetPoint.y - transform.position.y);
                behaviours.newAttack(tempVel, globalAttackObject, new int[] { 0, 1 }, 10, new string[0], 0, transform.Find("attack").gameObject, 1, 0.1f);
                framePos = 0;
                activeAction = true;
            }
        }
    }


    void Start() // DECLARE DEFAULT VARIABLE VALUES
    {
        newRandomData();
        Setup();
        tempSetup();
    }

    void Update()
    {
        if (gameObject.tag != "Destroyed")
        {
            // CONSTANTS
            animSwitch();
            constantBehaviours();

            if (activeTarget != gameObject & activeTarget.tag != "Destroyed")
            {
                switchState = 1;
            }
            else
            {
                switchState = 0;
            }

            switch (switchState)
            {
                case 0: // IDLE
                    idleUpdate();
                    break;
                case 1: // AGGRESSIVE
                    aggressiveUpdate();
                    break;
                case 2: // DEFENSIVE
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (destroyed == true)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                foreach (string a in children)
                {
                    Destroy(transform.Find(a).gameObject);
                    GetComponent<SpriteRenderer>().sprite = miscSprites[1];
                    if (Random.Range(0, 2) == 0)
                    {
                        GetComponent<SpriteRenderer>().flipX = true;
                    }

                }
                destroyed = false;
            }
        }
        
    }

    void FixedUpdate()
    {
        if (gameObject.tag != "Destroyed")
        {
            // CONSTANTS
            animUpdate();

            switch (switchState)
            {
                case 0: // IDLE
                    idleFixed();
                    break;
                case 1: // AGGRESSIVE
                    aggressiveFixed();
                    break;
                case 2: // DEFENSIVE
                    break;
                default:
                    break;
            }
        } 
    }
}
