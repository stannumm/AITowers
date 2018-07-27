using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionboomerang : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D coll)
    {
        
        if (coll.collider.name == "Cube")
            transform.parent.SendMessage("OnFinished");


    }
}
