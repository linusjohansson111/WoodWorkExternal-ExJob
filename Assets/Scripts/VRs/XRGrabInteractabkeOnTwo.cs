using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractabkeOnTwo : XRGrabInteractabkeBase
{
    public Transform LeftAttachTransform;
    public Transform RightAttachTransform;

    public bool IsRightHandGraping { get; private set; }
    public bool IsLeftHandGraping { get; private set; }

    

    public delegate void GrabbingToolWithHand(UsingHand grabbingHandIndex);
    public static GrabbingToolWithHand OnGrabbingWithHand;

    public delegate void GrabbingTool(HandObject aHandObject, string aGrabbedObjectName);
    public static GrabbingTool OnGrabbingObject;

    public delegate void DroppingTool();
    public static DroppingTool OnDroppingObject;


    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("LeftHandTag"))
        {
            attachTransform = LeftAttachTransform;
            IsLeftHandGraping = true;
            OnGrabbingWithHand?.Invoke(UsingHand.LEFT_HAND);
        }
        else if (args.interactorObject.transform.CompareTag("RightHandTag"))
        {
            attachTransform = RightAttachTransform;
            IsRightHandGraping = true;
            OnGrabbingWithHand?.Invoke(UsingHand.RIGHT_HAND);
        }


        //if (args.interactorObject.transform.CompareTag("Hand"))
        //{
        //    ourGrappingHand = args.interactorObject.transform.GetComponent<HandObject>();
        //    GrabableObject grabbedObject = transform.GetComponent<GrabableObject>();

        //    if (ourGrappingHand != null && !ourGrappingHand.IsGrapping &&
        //        grabbedObject != null && !grabbedObject.IsHolding)
        //    {
        //        if (ourGrappingHand.Side == Preposition.LEFT)
        //        {
        //            attachTransform = LeftAttachTransform;
        //            ourIsGrabbed = IsLeftHandGraping = true;
        //            //OnGrabbingWithHand?.Invoke(UsingHand.LEFT_HAND);
        //        }
        //        else if (ourGrappingHand.Side == Preposition.RIGHT)
        //        {
        //            attachTransform = RightAttachTransform;
        //            ourIsGrabbed = IsRightHandGraping = true;
        //            //OnGrabbingWithHand?.Invoke(UsingHand.RIGHT_HAND);
        //        }

        //        ourGrappingHand.IsGrapingObject(IsGrapped);
        //        OnGrabbingObject?.Invoke(ourGrappingHand, this.transform.name);
        //    }
        //}

        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (IsLeftHandGraping && args.interactorObject.transform.CompareTag("LeftHandTag"))
            IsLeftHandGraping = false;
        else if (IsRightHandGraping && args.interactorObject.transform.CompareTag("RightHandTag"))
            IsRightHandGraping = false;


        //if (IsGrapped && args.interactorObject.transform.CompareTag("Hand"))
        //{
        //    ourIsGrabbed = IsLeftHandGraping = IsRightHandGraping = false;
        //    ourGrappingHand.IsGrapingObject(IsGrapped);
        //}

        //OnDroppingObject?.Invoke();

        base.OnSelectExited(args);
    }


    public void GetAttachTransform(Transform aLeftAttach, Transform aRightAttach)
    {
        LeftAttachTransform = aLeftAttach;
        RightAttachTransform = aRightAttach;


        interactionLayers = InteractionLayerMask.GetMask("DirectInteraction");
        movementType = MovementType.Kinematic;
    }
}
