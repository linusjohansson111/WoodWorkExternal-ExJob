using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractabkeBase : XRGrabInteractable
{
    public bool IsGrapped { get { return ourIsGrabbed; } }

    protected Transform ourLeftAttachPoint, ourRightAttachPoint;

    [SerializeField]
    public Transform plankAttachPoint;

    protected HandObject ourGrappingHand;
    protected GrabableObject ourGrabableObject;

    protected bool ourIsGrabbed = false;
    protected bool ourHasAttachPoint = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void TestChangeLayer()
    {
        int layer = interactionLayers.value;
        this.interactionLayers = InteractionLayerMask.GetMask("DirectInteraction");
        layer = interactionLayers.value;
    }

    // Update is called once per frame
    void Update()
    {
        // ourGrabableObject.transform.rotation *= Quaternion.Euler(0f, 30f * Time.deltaTime, 0f);
        // if (ourGrappingHand != null && ourGrappingHand.Side == Preposition.LEFT && ourGrappingHand.IsGrapping){
        //     Debug.Log("GRABBING LEFT");
        // }
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
        // Debug.Log(args.ToString());
        // args.interactorObject.transform.Rotate(0, 45, 0);

        if (args.interactorObject.transform.CompareTag("Hand") && ourGrappingHand == null/*!OurObjectIsHolding()*/)
        {
            HandObject selectingHand = args.interactorObject.transform.GetComponent<HandObject>();

            if (selectingHand == null || ourGrappingHand != selectingHand)
                ourGrappingHand = selectingHand;

            if (ourGrappingHand != null && !ourGrappingHand.IsGrapping)
            {
                //if (ourOptionalAttachPoint != null)
                //    attachTransform = ourOptionalAttachPoint;

                if (ourGrappingHand.Side == Preposition.LEFT && ourLeftAttachPoint != null)
                {
                    attachTransform = ourLeftAttachPoint;                    
                }
                else if (ourGrappingHand.Side == Preposition.RIGHT && ourRightAttachPoint != null)
                {

                    // hämta objektet och spara rotation
                    // rotera ourRightAttachPoint ^
                    // sätt attachTransform = ^
                    
                    // ourRightAttachPoint.Rotate(0, 45, 0);
                    attachTransform = ourRightAttachPoint;                    
                }
                else if (plankAttachPoint != null) {

                    Quaternion localRotationPlank = this.transform.rotation;


                    attachTransform = plankAttachPoint;

                    attachTransform.transform.localRotation = Quaternion.Inverse(localRotationPlank) * args.interactorObject.transform.rotation;
                }
                


                ourIsGrabbed = true;
                ourGrappingHand.IsGrapingObject(IsGrapped);
                ourGrabableObject.GrabbObject(ourGrappingHand);
            }

        }
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("Hand") && ourGrappingHand != null/*OurObjectIsHolding()*/)
        {
            ourIsGrabbed = false;
            ourGrappingHand.IsGrapingObject(IsGrapped);
            ourGrabableObject.DroppObject();
            ourGrappingHand = null;
        }


        base.OnSelectExited(args);
    }


    public void SetGrabbingObject(GrabableObject aGrabbingObject)
    {
        ourGrabableObject = aGrabbingObject;
    }

    public void SetLeftAttachPoint(Transform anAttachPoint)
    {
        if (ourLeftAttachPoint != null)
            return;

        ourLeftAttachPoint = anAttachPoint;
    }

    public void SetRightAttachPoint(Transform anAttachPoint)
    {
        if (ourRightAttachPoint != null)
            return;

        ourRightAttachPoint = anAttachPoint;
    }

    public void ResetLeftAttachPoint()
    {
        ourLeftAttachPoint = null;
    }

    public void ResetRightAttachPoint()
    {
        ourRightAttachPoint = null;
    }

    private bool OurObjectIsHolding()
    {
        if (ourGrabableObject == null)
            return false;

        return ourGrabableObject.IsHolding;
    }
}
