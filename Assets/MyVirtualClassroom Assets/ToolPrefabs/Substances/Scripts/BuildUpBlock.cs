using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class BuildUpBlock : GrabableObject
{
    [SerializeField] private float Width = 0;
    [SerializeField] private float Height = 0;
    [SerializeField] private float Lenght = 0;
    [SerializeField] private Vector3 Center = Vector3.zero;

    private List<MaterialPart> myParts = new List<MaterialPart>();

    private BoxHitSide myHitSide = BoxHitSide.NONE;

    private float myActiveDeactiveCooldown = 0;

    public bool IsGrabable { get; private set; }
    private bool myTouchByHand = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        foreach(Transform child in transform)
        {
            if (child.GetComponent<MaterialPart>() != null)
                myParts.Add(child.GetComponent<MaterialPart>());
        }

        BakeBlockSize();

        IsGrabable = ourXRGrab.enabled;
    }

    // Update is called once per frame
    protected override void Update()
    {
        

        base.Update();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }

    protected override void OnCollisionExit(Collision collision)
    {
        base.OnCollisionExit(collision);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        /**
        if (other.gameObject.layer == LayerMask.NameToLayer("Hand"))
        {
            if (myActiveDeactiveCooldown <= 0)
            {
                if (other.GetComponent<HandObject>().IsActivePressed)
                {
                    IsGrabable = ourXRGrab.enabled = !ourXRGrab.enabled;
                    Debug.Log("NOW");
                    myActiveDeactiveCooldown = 5f;
                }
            }
            else
                myActiveDeactiveCooldown -= Time.deltaTime;
        }*/

        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    public void AttachToGlue(GlueSplattQuad aSplatt, Vector3 hitPoint)
    {
        BoxHitSide side = ColliderTools.GetHitSide(transform, hitPoint);
    }

    public void TransferChildrenTo(BuildUpBlock aParentBlock)
    {
        for(int i = transform.childCount - 1; i >= 0; i--)
        {
            aParentBlock.AddPart(transform.GetChild(i).GetComponent<MaterialPart>());
        }
        
        
    }

    public void GetChildAttachPointFor(Preposition aHand, Transform anAttachPoint)
    {
        if (aHand == Preposition.LEFT)
            ourXRGrab.SetLeftAttachPoint(anAttachPoint);
        else if (aHand == Preposition.RIGHT)
            ourXRGrab.SetRightAttachPoint(anAttachPoint);
    }

    public void NullifyAttachPointFor(Preposition aHand)
    {
        if (aHand == Preposition.LEFT)
            ourXRGrab.SetLeftAttachPoint(null);
        else if (aHand == Preposition.RIGHT)
            ourXRGrab.SetRightAttachPoint(null);
    }

    public void SetHitSideOnBuildUp(Vector3 hitPoint)
    {
        
    }

    public void NullifyLeftAttachPoint()
    {
        ourXRGrab.ResetLeftAttachPoint();
    }

    public void NullifRightAttachPoint()
    {
        ourXRGrab.ResetRightAttachPoint();
    }


    public void SelfDestroy()
    {
        if (transform.childCount == 0)
            Destroy(gameObject);
    }

    public Color GetOutlineColorFor(int anOutlineIndex)
    {
        return GetColorFor(anOutlineIndex);
    }

    public Color GetOutlineColorFor(TouchTag aTouchTag)
    {
        return GetColorFor(aTouchTag);
    }

    private void AddPart(MaterialPart aNewPart)
    {
        //aNewPart.transform.parent = transform;
        aNewPart.SetParent(this);
        myParts.Add(aNewPart);

        if (ourXRGrab.enabled == false && myParts.Count == 4)
            ourXRGrab.enabled = true;

        // BakeBlockSize();

        //center = getCenter(transform);
    }

    private void BakeBlockSize()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; ++i)
        {
            bounds.Encapsulate(renderers[i].bounds.min);
            bounds.Encapsulate(renderers[i].bounds.max);
        }

        foreach (Transform subObj in transform)
        {
            Center += getCenter(subObj);
        }

        Width = bounds.size.x;
        Height = bounds.size.y;
        Lenght = bounds.size.z;

        
    }

    Vector3 getCenter(Transform obj)
    {
        Vector3 center = new Vector3();
        if (obj.GetComponent<Renderer>() != null)
        {
            center = obj.GetComponent<Renderer>().bounds.center;
        }
        else
        {
            foreach (Transform subObj in obj)
            {
                center += getCenter(subObj);
            }
            center /= obj.childCount;
        }
        return center;
    }
}
