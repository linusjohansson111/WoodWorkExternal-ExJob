using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractabkeOnTwo : XRGrabInteractable
{
    public Transform LeftAttachTransform;
    public Transform RightAttachTransform;

    public bool IsRightHandGraping { get; private set; }
    public bool IsLeftHandGraping { get; private set; }

    public bool IsGrapped { get; private set; }

    public delegate void GrabbingToolWithHand(UsingHand grabbingHandIndex);
    public static GrabbingToolWithHand OnGrabbingWithHand;

    public delegate void GrabbingTool(HandObject aHandObject, string aGrabbedObjectName);
    public static GrabbingTool OnGrabbingObject;

    public delegate void DroppingTool();
    public static DroppingTool OnDroppingObject;

    private HandObject myGrappingHandObject;


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

        if(args.interactorObject.transform.CompareTag("Hand"))
        {
            myGrappingHandObject = args.interactorObject.transform.GetComponent<HandObject>();
            if(myGrappingHandObject != null)
            {
                string name = this.transform.name;

                if (myGrappingHandObject.Side == Preposition.LEFT)
                {
                    attachTransform = LeftAttachTransform;
                    IsGrapped = IsLeftHandGraping = true;
                    //OnGrabbingWithHand?.Invoke(UsingHand.LEFT_HAND);
                }
                else if(myGrappingHandObject.Side == Preposition.RIGHT)
                {
                    attachTransform = RightAttachTransform;
                    IsGrapped = IsRightHandGraping = true;
                    //OnGrabbingWithHand?.Invoke(UsingHand.RIGHT_HAND);
                }
                
                myGrappingHandObject.IsGrapingObject(IsGrapped);
                OnGrabbingObject?.Invoke(myGrappingHandObject, this.transform.name);
            }
        }
        
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (IsLeftHandGraping && args.interactorObject.transform.CompareTag("LeftHandTag"))
            IsLeftHandGraping = false;
        else if (IsRightHandGraping && args.interactorObject.transform.CompareTag("RightHandTag"))
            IsRightHandGraping = false;

        if(IsGrapped && args.interactorObject.transform.CompareTag("Hand"))
        {
            IsGrapped = IsLeftHandGraping = IsRightHandGraping = false;
            myGrappingHandObject.IsGrapingObject(IsGrapped);
        }

        OnDroppingObject?.Invoke();

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
