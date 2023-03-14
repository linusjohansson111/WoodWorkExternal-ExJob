using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public enum TouchTag { HAND, FASTERNER, SUBSTANCE, SLICER, MEASUREMENT, OTHER, NONE = -1 }
public class GrabableObject : MonoBehaviour
{
    //[SerializeField]
    //protected Transform LeftAttachPoint, RightAttachPoint;
    //[SerializeField]
    //protected Color HandTouchOutlineColor = Color.white;
    [SerializeField]
    protected Color[] TouchOutlineColor = new Color[1] { Color.white };
    [SerializeField]
    protected TouchTag[] OutlineTouchTags = new TouchTag[1] { TouchTag.HAND };

    protected Rigidbody ourRB;
    protected BoxCollider ourBC;
    protected XRGrabInteractabkeBase ourXRGrab;

    public Outline ourOutline;

    public bool IsHolding{ get { return ourIsHolding; } }
    
    protected HandObject ourGrabbingHand;

    protected bool ourIsHolding = false;

    protected bool ourHasOutline = false;

    protected virtual void Awake()
    {
        //XRGrabInteractabkeOnTwo.OnGrabbingObject += GrabingOject;
        //XRGrabInteractabkeOnTwo.OnDroppingObject += DroppingObject;

        ourBC = GetComponent<BoxCollider>();
        ourRB = GetComponent<Rigidbody>();
        if (GetComponent<XRGrabInteractabkeBase>() != null)
        {
            ourXRGrab = GetComponent<XRGrabInteractabkeBase>();

            ourXRGrab.SetGrabbingObject(this);
        }
    }

    protected virtual void OnDestroy()
    {
        //XRGrabInteractabkeOnTwo.OnGrabbingObject -= GrabingOject;
        //XRGrabInteractabkeOnTwo.OnDroppingObject -= DroppingObject;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        DrawOutline(TouchTag.NONE);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected void SetOutlineAppearence(Outline.Mode aMode, Color aColor)
    {
        ourOutline.OutlineMode = aMode;
        ourOutline.OutlineColor = aColor;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Hand") && !ourIsHolding)
        //{
        //    Preposition hand = other.transform.GetComponent<HandObject>().Side;
        //    if (hand == Preposition.LEFT)
        //        ourXRGrab.SetOptionalAttachPoint(LeftAttachPoint);
        //    else 
        //    if(hand == Preposition.RIGHT)
        //        ourXRGrab.SetOptionalAttachPoint(RightAttachPoint);

        //    DrawOutline(1);
        //}
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Hand"))
        //{
        //    ourXRGrab.SetOptionalAttachPoint(null);
        //}
        DrawOutline(TouchTag.NONE);
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

    protected virtual void DrawOutline(TouchTag aTag)
    {
        if (ourOutline != null)
            ourHasOutline = true;
        else
            return;

        if (aTag == TouchTag.NONE)
            ourOutline.enabled = false;
        else
        {
            ourOutline.enabled = true;
            if (aTag == TouchTag.HAND)
                SetOutlineAppearence(Outline.Mode.OutlineVisible, GetColorFor(TouchTag.HAND));
        }
    }

    protected void UseTheGivenAttachTransform(Transform anAttachTransform)
    {
        ourXRGrab.SetOptionalAttachPoint(anAttachTransform);
    }

    protected void NullifyGivenAttachPoint()
    {
        ourXRGrab.SetGrabbingObject(null);
    }

    protected Color GetColorFor(int anIndex)
    {
        return TouchOutlineColor[anIndex];
    }

    protected Color GetColorFor(TouchTag anIndexTag)
    {
        int index = Array.FindIndex(OutlineTouchTags, tags => tags == anIndexTag);
        return TouchOutlineColor[index];
    }

    public void GrabbObject(HandObject aGrabbingHand)
    {
        if(ourIsHolding)
            return;

        ourGrabbingHand = aGrabbingHand;
        ourIsHolding = true;
        DrawOutline(0);
    }

    public void DroppObject()
    {
        if (!ourIsHolding)
            return;

        ourGrabbingHand = null;
        ourIsHolding = false;
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
