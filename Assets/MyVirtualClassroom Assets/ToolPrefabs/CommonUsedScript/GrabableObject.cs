using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabableObject : MonoBehaviour
{
    [SerializeField]
    protected Transform LeftAttachPoint, RightAttachPoint;
    [SerializeField]
    protected Color HandTouchOutlineColor = Color.white;
    [SerializeField]
    protected InputActionProperty LeftActive, RightActive;
    [SerializeField]
    protected InputActionProperty LeftGrab, RightGrab;
    

    protected Rigidbody ourRB;
    protected BoxCollider ourBC;
    protected XRGrabInteractabkeBase ourXRGrab;

    public Outline ourOutline;

    public bool IsHolding{ get { return ourIsHolding; } }
    
    protected HandObject ourGrabbingHand;

    protected Vector3 myGrabbingHandLastPosition;

    protected bool ourIsLeftHandHolding;
    protected bool ourIsRightHandHolding;
    protected bool ourIsHolding = false;

    protected bool ourHasOutline = false;

    protected virtual void Awake()
    {
        XRGrabInteractabkeOnTwo.OnGrabbingObject += GrabingOject;
        XRGrabInteractabkeOnTwo.OnDroppingObject += DroppingObject;

        ourBC = GetComponent<BoxCollider>();
        ourRB = GetComponent<Rigidbody>();
        ourXRGrab = GetComponent<XRGrabInteractabkeBase>();

        ourXRGrab.SetGrabbingObject(this);

    }

    protected virtual void OnDestroy()
    {
        XRGrabInteractabkeOnTwo.OnGrabbingObject -= GrabingOject;
        XRGrabInteractabkeOnTwo.OnDroppingObject -= DroppingObject;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        DrawOutline(0);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void LateUpdate()
    {
        myGrabbingHandLastPosition = GrabbingHandPosition();
    }

    protected Vector3 GrabbingHandPosition()
    {
        if(ourIsHolding)
            return ourGrabbingHand.transform.position;

        return Vector3.zero;
    }

    protected Vector3 PosDifference()
    {
        return GrabbingHandPosition() - myGrabbingHandLastPosition;
    }

    protected void SetOutlineAppearence(Outline.Mode aMode, Color aColor)
    {
        ourOutline.OutlineMode = aMode;
        ourOutline.OutlineColor = aColor;
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

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand") && !ourIsHolding)
        {
            Preposition hand = other.transform.GetComponent<HandObject>().Side;
            if (hand == Preposition.LEFT)
                ourXRGrab.SetOptionalAttachPoint(LeftAttachPoint);
            else 
            if(hand == Preposition.RIGHT)
                ourXRGrab.SetOptionalAttachPoint(RightAttachPoint);

            DrawOutline(1);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            ourXRGrab.SetOptionalAttachPoint(null);
            DrawOutline(0);
        }
    }

    protected virtual void GrabingOject(HandObject aGrabbingHandObject, string aGrabbedObjectName)
    {
        if (transform.name != aGrabbedObjectName)
            return;

        ourGrabbingHand = aGrabbingHandObject;
        ourIsHolding = true;
        DrawOutline(0);
    }

    protected virtual void DroppingObject()
    {
        if (ourIsHolding)
        {
            ourGrabbingHand = null;
            ourIsHolding = false;
        }
    }

    protected virtual void DrawOutline(int aModeIndex)
    {
        if(ourOutline != null)
            ourHasOutline = true;

        if (aModeIndex == 0)
            ourOutline.enabled = false;
        else
        {
            ourOutline.enabled = true;
            if (aModeIndex == 1)
                SetOutlineAppearence(Outline.Mode.OutlineVisible, HandTouchOutlineColor);
        }
    }

    public void GrabbObject(HandObject aGrabbingHand)
    {
        ourGrabbingHand = aGrabbingHand;
        ourIsHolding = true;
        DrawOutline(0);
    }

    public void DroppObject()
    {
        if(ourIsHolding)
        {
            ourGrabbingHand = null;
            ourIsHolding = false;
        }
    }

    public void RemoveGrabAndRigidbody()
    {
        Destroy(ourXRGrab);
        Destroy(ourRB);
    }

    /// <summary>
    /// Active or deactive the rigidbody's IsKinematic boolian to make the object
    /// depending by force or by code structure
    /// By Senpai
    /// </summary>
    /// <param name="isKinematic">Boolian to active kinematic and deactive gravity usage</param>
    public void SetWoodToKinematic(bool activeKinematic)
    {
        if (ourRB == null)
            return;
        
        ourRB.isKinematic = activeKinematic;
        ourRB.useGravity = !activeKinematic;
    }
}
