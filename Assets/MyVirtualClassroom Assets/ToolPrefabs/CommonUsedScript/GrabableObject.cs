using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabableObject : MonoBehaviour
{
    [SerializeField]
    protected Color HandTouchOutlineColor = Color.white;
    [SerializeField]
    protected InputActionProperty LeftActive, RightActive;
    [SerializeField]
    protected InputActionProperty LeftGrab, RightGrab;

    [SerializeField, HideInInspector]
    protected Rigidbody ourRB;
    [SerializeField, HideInInspector]
    protected BoxCollider ourBC;
    [SerializeField, HideInInspector]
    protected XRGrabInteractable ourXRGrab;

    public Outline ourOutline;
    
    protected HandObject ourGrabbingHandObject;

    protected Vector3 myGrabbingHandLastPosition;

    protected bool ourIsLeftHandHolding;
    protected bool ourIsRightHandHolding;
    protected bool ourHandIsHolding = false;

    protected bool ourHasOutline = false;

    protected virtual void Awake()
    {
        XRGrabInteractabkeOnTwo.OnGrabbingObject += GrabingOject;
        XRGrabInteractabkeOnTwo.OnDroppingObject += DroppingObject;

        ourBC = GetComponent<BoxCollider>();
        ourRB = GetComponent<Rigidbody>();
        ourXRGrab = GetComponent<XRGrabInteractable>();
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
        if(ourHandIsHolding)
            return ourGrabbingHandObject.transform.position;

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

    protected bool IsActivePressed()
    {
        if (!ourHandIsHolding)
            return false;

        if (ourGrabbingHandObject.Side == Preposition.LEFT)
            return LeftActive.action.IsPressed();
        
        return RightActive.action.IsPressed();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand") && !ourHandIsHolding)
        {
            DrawOutline(1);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand"))
            DrawOutline(0);
    }

    protected virtual void GrabingOject(HandObject aGrabbingHandObject, string aGrabbedObjectName)
    {
        if (transform.name != aGrabbedObjectName)
            return;

        ourGrabbingHandObject = aGrabbingHandObject;
        ourHandIsHolding = true;
        DrawOutline(0);
    }

    protected virtual void DroppingObject()
    {
        if (ourHandIsHolding)
        {
            ourGrabbingHandObject = null;
            ourHandIsHolding = false;
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
