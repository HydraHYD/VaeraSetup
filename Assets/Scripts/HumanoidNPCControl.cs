using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidNPCControl : MonoBehaviour
{
    // VARIABLES

    // ASSET VARIABLES (assets associated with this NPC (character sprites, gui, etc.)) [NOT INDIVIDUALLY SAVED]
    public Sprite[] upSprite;
    public Sprite[] rightSprite;
    public Sprite[] downSprite;
    public Sprite[] leftSprite;
    public Sprite[] mainSprite;

    // GAMEPLAY VARIABLES (effects methods during runtime) [NOT INDIVIDUALLY SAVED]
    public int frameCount = 0;
    public Camera mainCam;
    public GameObject target;
    public Vector2 targetPoint;
    public Vector2 targetPointLog;
    public bool walking = false;
    public int animate = 0;
    public bool idleState = true;
    public bool targetState = false;
    public bool interactState = false;
    public NPCBehaviours behaviourScript;
    public int direction = 1;

    public GameObject box;

    // STAT VARIABLES (all number values that effect in-game effectiveness) [INDIVIDUALLY SAVED]
    public float health;
    public float vitality;
    public float speed;
    public float strength;
    public float defense;

    // CLASSIFICATION VARIABLES (titles and other factors that effect how this NPC interacts with others) [INDIVIDUALLY SAVED]
    public string nature; //simple natures, expand upon this later (i.e. passive, aggressive, neutral)

    // MEMORY VARIABLES (variables that determine behaviour and state of certain actions) [INDIVIDUALLY SAVED]
    public string[,] speciesOpinion;

    // Start is called before the first frame update
    void Start()
    {
        behaviourScript = gameObject.GetComponent<NPCBehaviours>();
        targetPoint = transform.position;
        mainSprite = upSprite;
    }

    // Update is called once per frame
    void Update()
    {
        targetPoint = behaviourScript.idleWander(behaviourScript.checkIdle(new bool[] { walking }), 500, targetPoint, mainSprite, gameObject);
        Vector2 velocity = behaviourScript.moveVelocity(targetPoint, 0.05f);
        behaviourScript.moveTowards(targetPoint, velocity, 0.5f, 0.005f, gameObject);

        walking = behaviourScript.isMoving(gameObject);

        if (targetPointLog != targetPoint)
        {
            mainSprite = behaviourScript.rotateTowards(targetPoint, direction, upSprite, rightSprite, downSprite, leftSprite, mainSprite, new string[] { "child1" }, gameObject); // Rotate whole NPC towards direction and update values accordingly
            targetPointLog = targetPoint;
        }
    }

    void FixedUpdate()
    {
        frameCount = behaviourScript.animateLoop(walking, new int[] { 1, 0, 2, 0 }, mainSprite, frameCount, 5); // Continuous animation while boolean value is true
    }
}
