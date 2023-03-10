using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Child : GrabableObject
{
    public Transform AttachPoint; 

    private Parent myParent; 
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        myParent = GetComponentInParent<Parent>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand") && !ourIsHolding)
        {
            if (myParent != null)
                myParent.GetTouchedChildAttachTransform(AttachPoint);
        }
        

        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand") && !ourIsHolding)
        {
            if (myParent != null)
                myParent.GetTouchedChildAttachTransform(null);
        }
        
        //base.OnTriggerExit(other);
    }
}
