using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractabkeBase : XRGrabInteractable
{
    public bool IsGrapped { get { return ourIsGrabbed; } }

    protected Transform ourOptionalAttachPoint;

    protected HandObject ourGrappingHand;
    protected GrabableObject ourGrabableObject;

    protected bool ourIsGrabbed = false;
    protected bool ourHasAttachPoint = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("Hand") && !OurObjectIsHolding())
        {
            if (ourGrappingHand == null)
                ourGrappingHand = args.interactorObject.transform.GetComponent<HandObject>();

            if (ourGrappingHand != null && !ourGrappingHand.IsGrapping)
            {
                if (ourOptionalAttachPoint != null)
                    attachTransform = ourOptionalAttachPoint;

                ourIsGrabbed = true;
                ourGrappingHand.IsGrapingObject(IsGrapped);
                ourGrabableObject.GrabbObject(ourGrappingHand);
            }
        }
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("Hand") && OurObjectIsHolding())
        {
            ourIsGrabbed = false;
            ourGrappingHand.IsGrapingObject(IsGrapped);
            ourGrabableObject.DroppObject();
        }


        base.OnSelectExited(args);
    }


    public void SetGrabbingObject(GrabableObject aGrabbingObject)
    {
        ourGrabableObject = aGrabbingObject;
    }

    public void SetOptionalAttachPoint(Transform anAttachPoint)
    {
        if (ourOptionalAttachPoint != null)
            return;

        ourOptionalAttachPoint = anAttachPoint;
    }

    public void ResetOptionalAttachPoint()
    {
        ourOptionalAttachPoint = null;
    }

    private bool OurObjectIsHolding()
    {
        if (ourGrabableObject == null)
            return false;

        return ourGrabableObject.IsHolding;
    }
}
