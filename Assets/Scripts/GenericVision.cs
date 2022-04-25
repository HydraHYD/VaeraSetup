using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericVision : MonoBehaviour
{
    public List<GameObject> inRange;
    public List<GameObject> inSight;
    int maxCounter = 500;
    int resetCounter;
    public string vision = "sharpSight";
    

    void Start()
    {
        inRange = new List<GameObject>();
        inSight = new List<GameObject>();
        resetCounter = maxCounter;
    }

    void Update()
    {
        Invoke(vision, 0);
    }

    void FixedUpdate()
    {
        resetTargets();
    }

    void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.tag == "NPC" || hit.tag == "Destroyed")
        {
            try
            {
                inRange.Add(hit.gameObject);
            }
            catch
            {
                Debug.Log("Can't Add");
            }
        }
    }

    void OnTriggerExit2D(Collider2D hit)
    {
        if (hit.tag == "NPC" || hit.tag == "Destroyed")
        {
            try
            {
                inRange.Remove(hit.gameObject);
            }
            catch
            {
                Debug.Log("Can't Remove");
            }
        }
    }
    
    void castSight()
    {
        if (inRange.Count > 0)
        {
            foreach (GameObject target in inRange)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, (new Vector2(target.transform.position.x, target.transform.position.y) - new Vector2(transform.position.x, transform.position.y)), Vector2.Distance(new Vector2(target.transform.position.x, target.transform.position.y), new Vector2(transform.position.x, transform.position.y)));
                Debug.DrawRay(transform.position, (new Vector2(target.transform.position.x, target.transform.position.y) - new Vector2(transform.position.x, transform.position.y)), Color.green);
                if (hit.collider != null & target.tag == "NPC")
                {
                    bool found = false;
                    foreach (GameObject sight in inSight)
                    {
                        if (sight == target)
                        {
                            found = true;
                        }
                    }
                    if (found == false)
                    {
                        inSight.Add(target);
                    }
                }
            }
        }
    }

    void sharpSight()
    {
        if (inRange.Count > 0)
        {
            foreach (GameObject target in inRange)
            {
                bool visible = false;
                foreach (Vector2 point in target.GetComponent<PolygonCollider2D>().points)
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, (new Vector2(target.transform.position.x + point.x, target.transform.position.y + point.y) - new Vector2(transform.position.x, transform.position.y)), Vector2.Distance(new Vector2(target.transform.position.x + point.x, target.transform.position.y + point.y), new Vector2(transform.position.x, transform.position.y)));
                    Debug.DrawRay(transform.position, (new Vector2(target.transform.position.x + point.x, target.transform.position.y + point.y) - new Vector2(transform.position.x, transform.position.y)), Color.green);
                    if (hit.collider != null & target.tag == "NPC")
                    {
                        visible = true;
                    }
                }
                if (visible == true)
                {
                    bool found = false;
                    foreach (GameObject sight in inSight)
                    {
                        if (sight == target)
                        {
                            found = true;
                        }
                    }
                    if (found == false)
                    {
                        inSight.Add(target);
                    }
                }
            }
        }

    }

    void resetTargets() // RUN THROUGH FIXEDUPDATE, CLEARS OUT TARGETS
    {
        if (resetCounter > 0)
        {
            resetCounter -= 1;
        }
        else
        {
            List<GameObject> removeSight = new List<GameObject>();
            foreach (GameObject sight in inSight)
            {
                bool found = false;
                if (inRange.Count > 0)
                {
                    foreach (GameObject range in inRange)
                    {
                        if (sight == range & sight.tag != "Destroyed")
                        {
                            found = true;
                        }
                    }
                    if (found == false)
                    {
                        removeSight.Add(sight);
                    }
                }
                else
                {
                    removeSight.Add(sight);
                }
            }
            foreach (GameObject remove in removeSight)
            {
                inSight.Remove(remove);
            }
            resetCounter = maxCounter;
        }
        
    }
    
}
