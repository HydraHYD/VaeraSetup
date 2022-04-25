using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSetup : MonoBehaviour
{
    public void newCollider(Vector2[] points, GameObject attachCollider) // Creates new PolygonCollider2D for gameObject (use for cone of vision and hitbox collider)
    {
        attachCollider.AddComponent<PolygonCollider2D>();
        attachCollider.GetComponent<PolygonCollider2D>().points = points;
        attachCollider.GetComponent<PolygonCollider2D>().isTrigger = true;
    }

    public void newBody(GameObject attachBody) // Creates new PolygonCollider2D for gameObject (use for cone of vision and hitbox collider)
    {
        attachBody.AddComponent<Rigidbody2D>();
        attachBody.GetComponent<Rigidbody2D>().isKinematic = true;
    }


    public void newChild(string childName, GameObject setParent)
    {
        GameObject newChild = new GameObject(childName);
        newChild.transform.position = transform.position;
        newChild.transform.SetParent(setParent.transform);
    }

    public void attachVision(GameObject attachVision)
    {
        attachVision.AddComponent<GenericVision>();
    }



}
