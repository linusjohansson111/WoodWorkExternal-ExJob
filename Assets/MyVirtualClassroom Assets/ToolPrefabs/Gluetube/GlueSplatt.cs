using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GlueSplatt : MonoBehaviour
{
    [SerializeField]
    public GameObject AssemblyParentPrefab;

    public Transform TestVerticalSnapPoint;
    public Transform TestHorizontalSnapPoint;

    private bool myIsAttachingOtherSubstance = false;

    private BoxCollider myBC;

    [HideInInspector]
    public Vector3 Snap { get { return myVerticalSnapPosition; } }
    private Vector3 myVerticalSnapPosition;
    private Vector3 myHorizontalSnapPosition;

    public BoxHitSide AtParentSide = BoxHitSide.NONE;
    private Substance myParentSubstance;

    // Start is called before the first frame update
    void Start()
    {
        myBC = GetComponent<BoxCollider>();
        myParentSubstance = GetComponentInParent<Substance>();
        SetSnapTransform(AtParentSide);
    }

    // Update is called once per frame
    void Update()
    {
        RayHitSubstance();
    }

    public void SetSnapPosition(Vector3 aTubeMuzzlePosition)
    {
        AtParentSide = ColliderTools.GetHitSide(transform.parent, aTubeMuzzlePosition);
    }

    public Vector3 GetSnapPosition(bool isVertical)
    {
        return (isVertical ? myVerticalSnapPosition : myHorizontalSnapPosition);
    }

    /// <summary>
    /// by senpais
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Sliceable")
        {
            //Collider myCollider = collision.contacts[1].thisCollider;
            //Debug.Log("LIMMMMM");
            //collision.gameObject.GetComponent<Substance>().SetWoodToKinematic(true);
            //collision.gameObject.transform.parent = gameObject.transform.parent;
            //rb.transform.parent = collision.transform;

            //pointer.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
    }

    private void RayHitSubstance()
    {
        if (myIsAttachingOtherSubstance)
            return;

        if (Physics.Raycast(transform.position, transform.up, out RaycastHit hit, .001f))
        {
            if (hit.transform.CompareTag("Sliceable"))
            {
                //transform.parent.GetComponent<Substance>().AttachingNewPart(hit.transform.GetComponent<Substance>(), transform);
                myIsAttachingOtherSubstance = true;
                Destroy(this.gameObject);
            }
        }
    }

    /// <summary>
    /// Set the snap position of this glue object on the side of it parents
    /// Depending on which side the object was put on, one of the axis will be locked
    /// with the value of the locked axis value
    /// </summary>
    /// <param name="aParentHitSide">The side this glue object hit on the parent</param>
    private void SetSnapTransform(BoxHitSide aParentHitSide)
    {
        if (aParentHitSide == BoxHitSide.RIGHT || aParentHitSide == BoxHitSide.LEFT)
        {
            transform.Rotate(new Vector3(0f,0f,(aParentHitSide == BoxHitSide.RIGHT ? -90 : 90)));
            myVerticalSnapPosition = new Vector3(myParentSubstance.GetTheFurthestPositionOn(aParentHitSide).x, transform.parent.transform.position.y, transform.position.z);
            myHorizontalSnapPosition = new Vector3(myParentSubstance.GetTheFurthestPositionOn(aParentHitSide).x, transform.position.y, transform.parent.transform.position.z);
        }
        else if (aParentHitSide == BoxHitSide.TOP || aParentHitSide == BoxHitSide.BOTTOM)
        {
            transform.Rotate(new Vector3((aParentHitSide == BoxHitSide.TOP ? 0 : 180), 0f, 0f));
            myVerticalSnapPosition = new Vector3(transform.position.x, myParentSubstance.GetTheFurthestPositionOn(aParentHitSide).y, transform.parent.transform.position.z);
            myHorizontalSnapPosition = new Vector3(transform.parent.transform.position.x, myParentSubstance.GetTheFurthestPositionOn(aParentHitSide).y, transform.position.z);
        }
        else if (aParentHitSide == BoxHitSide.FRONT || aParentHitSide == BoxHitSide.BACK)
        {
            transform.Rotate(new Vector3((aParentHitSide == BoxHitSide.FRONT ? 90 : -90), 0f, 0f));
            myVerticalSnapPosition = new Vector3(transform.position.x, transform.parent.transform.position.y, myParentSubstance.GetTheFurthestPositionOn(aParentHitSide).z);
            myHorizontalSnapPosition = new Vector3(transform.parent.transform.position.x, transform.position.y, myParentSubstance.GetTheFurthestPositionOn(aParentHitSide).z);
        }

        TestVerticalSnapPoint.position = myVerticalSnapPosition;
        TestHorizontalSnapPoint.position = myHorizontalSnapPosition;
    }

    private Vector3 SnapOrientation(bool isVertical, float anYValueOnTheHitingSide)
    {
        float xValue = (isVertical ? transform.position.x : transform.parent.transform.position.x);
        float zValue = (isVertical ? transform.parent.transform.position.z : transform.position.z);

        return new Vector3(xValue, anYValueOnTheHitingSide, zValue);
    }

    private void CreateQuad()
    {

    }
}
