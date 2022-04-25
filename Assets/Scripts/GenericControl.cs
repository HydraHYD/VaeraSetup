﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericControl : MonoBehaviour
{
    // VARIABLES
    public int switchState = 0; // 0 - IDLE, 1 - AGGRESSIVE, 2 - DEFENSIVE, ADD MORE IF NECESSARY
    public GameObject lastHit;
    Vector2 velocity = new Vector2(0, 0);

    // SCRIPTS
    public GenericSetup setup; // NPC Component setup
    public GenericBehaviours behaviours; // NPC behaviours
    public GenericData data; // Save and load
    public GenericVision vision;

    // SETUP
    public Vector2[] visionPoints;
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
        idleFrames = new int[] { 0, 1, 0, 2 };
        loopFrames = idleFrames;
        actionFrames = new int[] { 3, 0, 4, 0 };
        visionPoints = new Vector2[] { new Vector2(0, 0), new Vector2(-0.3f, -1.5f), new Vector2(0.3f, -1.5f) };
        children = new string[] { "vision", "attack", "overlay" };
        hitbox = new Vector2(0.1f, 0.2f);
        gameObject.tag = "NPC";
        mainSprite = upSprite;
        targetPoint = transform.position;

        // Create Components
        setup.newBody(gameObject);
        setup.newCollider(new Vector2[] { new Vector2(-hitbox.x / 2, hitbox.y / 2), new Vector2(-hitbox.x / 2, -hitbox.y / 2), new Vector2(hitbox.x / 2, -hitbox.y / 2), new Vector2(hitbox.x / 2, hitbox.y / 2) }, gameObject);

        setup.newChild("rotate", gameObject);
        foreach (string a in children)
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
        if (activeAction == true)
        {
            currentFrames = actionFrames;
        }
        else
        {
            currentFrames = loopFrames;
        }
        if (Input.GetAxis("Fire1") != 0 & activeAction == false) // ACTION CONDITION, CHANGE LATER
        {
            behaviours.newAttack(globalAttackObject, new int[] { 0, 1 }, 10, new string[0], 0, gameObject, 1, new Vector3 (0, -0.1f, 0));
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

            if (activeTarget != gameObject)
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