using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parent : GrabableObject
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
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
            DrawOutline(TouchTag.HAND);
        }
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand") && ourIsHolding)
        {
            DrawOutline(TouchTag.NONE);
        }
        base.OnTriggerExit(other);
    }

    public void GetTouchedChildAttachTransform(Transform anAttachTransform)
    {
        UseTheGivenAttachTransform(anAttachTransform);
    }
}
