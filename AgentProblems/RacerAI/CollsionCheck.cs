using UnityEngine;
using System.Collections;

public class CollsionCheck : MonoBehaviour
{

    void OnCollisionEnter2D(Collision2D coll)
    {
        
        if (coll.collider.name.Contains("G"))
            transform.parent.SendMessage("OtherActivity", (object)0);
       

    }

    void OnCollisionExit2D(Collision2D coll)
    {
        //if (coll.collider.name.Contains("B"))
        //transform.parent.SendMessage("OtherActivity", (object)2);
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        //if (coll.collider.name.Contains("B"))
        //transform.parent.SendMessage("OtherActivity", (object)3);
        //transform.parent.SendMessage("OtherActivity", (object)0);
    }



}
