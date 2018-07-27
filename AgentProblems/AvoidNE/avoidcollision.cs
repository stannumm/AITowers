using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class avoidcollision : MonoBehaviour {

    // Use this for initialization
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "final")
            transform.parent.SendMessage("finishedtrack", (object)0);
        
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.tag == "obstacle")
        {
            transform.parent.SendMessage("crashed", (object)0);
        }
        else if (coll.collider.tag == "edge")
        {
            transform.parent.SendMessage("crashed", (object)0);

        }
    }
        
 }
