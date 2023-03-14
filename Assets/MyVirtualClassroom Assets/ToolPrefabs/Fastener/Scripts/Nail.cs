using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Nail : GrabbingTool, ICFasteners
{
    //[SerializeField]
    //private Transform LeftAttach, RightAttach;
    public Transform Tip , Head;

    [HideInInspector]
    public Vector3 TipPosition { get { return Tip.position; } }

    public float HalfLenght { get; private set; }

    //Transform transform;
    //Rigidbody rigidbody;

    //Variables
    bool isBeingHeld = false;
    //bool isKinematic = false;
    bool isOnWood = false;
    bool isAttached = false;
    Vector3 localScale;
    int nrOfWoodsHammered = 0;

    private BoxHitSide myLastHitSubstanceFade = BoxHitSide.NONE;
    private Vector3 myHitSubstancePoint;

    private HandObject myHoldingHand;
    /// <summary>
    /// The parent object the fastern will have as parent
    /// </summary>
    private GameObject myAttachToGO;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //transform = GetComponent<Transform>();
        //rigidbody = GetComponent<Rigidbody>();
        localScale = transform.localScale;
        //Debug.Log(localScale);


        HalfLenght = (transform.position - TipPosition).magnitude;

    }

    protected override void Update()
    {
        if (ourIsHolding)
        {
            HasRayHitTarget();
        }
    }

    public void AddForceToObject(float aVelocity)
    {
        aVelocity = -1f * 0.3f * Time.deltaTime;
        transform.position += Vector3.up * aVelocity;
        nrOfWoodsHammered++;
    }

    public void SetIsBeingHeld(bool isHeld)
    {
        isBeingHeld = isHeld;
        Debug.Log("SetIsBeingHeld got " + isHeld);
        if (isBeingHeld == false && isOnWood)
        {
            GetComponent<Rigidbody>().isKinematic = true;

            transform.parent = myAttachToGO.transform;
            transform.localScale = localScale;

            /*GameObject newGameObject = new GameObject();
            newGameObject.transform.parent = myAttachToGO.transform;
            transform.parent = newGameObject.transform;*/
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Sliceable")
        {
            isOnWood = true;
            //nrOfWoodsHammered += 1;
            myAttachToGO = collision.gameObject;
            //myAttachToGO.GetComponent<Substance>().SetWoodToKinematic(true);
            //if (nrOfWoodsHammered >= 2)
            //{
            //    myAttachToGO.transform.parent = gameObject.transform.parent;
            //}
        }

        if (collision.gameObject.tag == "WeightHead" && isOnWood == true)
        {
            Debug.Log("Hammer hit");
            //Destroy(GetComponent<XRGrabInteractabkeOnTwo>());
            //AddForceToObject(2f);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Sliceable")
        {
            isOnWood = false;
            //Debug.Log("isOnWood is " + isOnWood);
            myAttachToGO = null;

            
        }

        if (collision.gameObject.tag == "WeightHead" && isAttached == false)
        {
            //AddForceToObject(2f);
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.gameObject.CompareTag("Sliceable"))
        {
            isOnWood = true;
            myAttachToGO = other.gameObject;
            //myLastHitSubstanceFade = ColliderTools.GetHitSide(other.transform, transform.position);
            //isAttached = true;
            //GetComponent<XRGrabInteractable>().interactionLayerMask = 0;
        }

        if (other.gameObject.CompareTag("WeightHead") && isOnWood)
        {
            if (transform.parent == null)
            {
                RemoveGrabAndRigidbody();
                BoxHitSide hitFace = myLastHitSubstanceFade;
                myAttachToGO.GetComponent<Substance>().AttachNewNail(this, myHitSubstancePoint);
            }

            AddForceToObject(2f);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (other.gameObject.CompareTag("Sliceable"))
        {
            isOnWood = false;
            //Debug.Log("Left Wood");
            //isAttached = true;
            //GetComponent<XRGrabInteractable>().interactionLayerMask = 0;
        }
    }

    private void HasRayHitTarget()
    {
        if (Physics.Raycast(TipPosition, -Tip.up, out RaycastHit hit, .001f))
        {
            if (hit.collider.transform.CompareTag("Sliceable"))
            {
                myHitSubstancePoint = hit.point;                
            }
        }
    }
}
