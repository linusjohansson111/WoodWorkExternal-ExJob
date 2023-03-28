using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class GrabbingTool : GrabableObject
{
    [SerializeField]
    protected Transform[] AttachPoint;

    protected Vector3 ourStartPosition;
    protected Quaternion ourStartRotation;

    protected Vector3 myGrabbingHandLastPosition;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ourStartPosition = transform.position;
        ourStartRotation = transform.rotation;
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
    protected virtual void LateUpdate()
    {
        myGrabbingHandLastPosition = GrabbingHandPosition();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

            
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand") && !ourIsHolding)
        {
            Preposition hand = other.transform.GetComponent<HandObject>().Side;
            //if (hand == Preposition.LEFT)
            //    ourXRGrab.SetOptionalAttachPoint(LeftAttachPoint);
            //else
            //if (hand == Preposition.RIGHT)
            //    ourXRGrab.SetOptionalAttachPoint(RightAttachPoint);

            SetToolAttachPoint(AttachPoint[(int)Preposition.LEFT], AttachPoint[(int)Preposition.RIGHT]);
            DrawOutline(TouchTag.HAND);
        }
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            NullifyToolAttachPoint();
            
        }

        base.OnTriggerExit(other);
    }

    public override void DroppObject()
    {
        base.DroppObject();
        transform.SetPositionAndRotation(ourStartPosition, ourStartRotation);
    }

    protected Vector3 GrabbingHandPosition()
    {
        if (ourIsHolding)
            return ourGrabbingHand.transform.position;

        return Vector3.zero;
    }

    protected float DotProductForAxis(Vector3 aTransformAxis)
    {
        Vector3 dir = PosDifference().normalized;
        float dot = Vector3.Dot(dir, aTransformAxis);

        if (dot < 0)
            return -1;
        else if (dot > 0)
            return 1f;

        return 0;
    }

    protected Vector3 PosDifference()
    {
        return GrabbingHandPosition() - myGrabbingHandLastPosition;
    }
}
