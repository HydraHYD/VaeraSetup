using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCControl : MonoBehaviour
{
    // Standard NPC behaviour loop

    // VARIABLES

    // ASSET VARIABLES (assets associated with this NPC (character sprites, gui, etc.)) [NOT INDIVIDUALLY SAVED]
    public Sprite[] upSprites;
    public Sprite[] rightSprites;
    public Sprite[] downSprites;
    public Sprite[] leftSprites;

    // GAMEPLAY VARIABLES (effects methods during runtime) [NOT INDIVIDUALLY SAVED]

    // ANIMATION VARIABLES
    public int frameCount = 0; // Defines the current step in an animation sequence
    public int[] currentFrames; // Current animation to iterate through
    public bool animationLock = false; // Locks new actions while current animation is playing



    public Camera mainCam;
    public GameObject target;
    public bool walking = false;
    
    public bool idleState = true;
    public bool targetState = false;
    public bool interactState = false;
    public NPCBehaviours behaviourScript;
    public int direction = 1;

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

    void Start() // Use for initializing variables
    {
    }

    void Update() // Use for all detection/calculations/misc methods
    { }

    void FixedUpdate() // Use for all animations 
    { }

}
