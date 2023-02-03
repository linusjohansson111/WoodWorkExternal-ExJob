using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class Substance : GrabableObject, MaterialInterface
{
    public Color HandTouchColor = new(96, 203, 21, 255), SlicerTouchColor = new(203, 21, 40, 255);
    public Color SubstanceTouchColor, RightAngleTouchColor;

    [SerializeField]
    public MaterialType MaterialType;

    [SerializeField]
    public AssembledProduct AssemblyParentPrefab;

    [SerializeField, HideInInspector] 
    private Sliceable Sliceable;

    internal BoxHitSide HitSide = BoxHitSide.NONE;

    [SerializeField, HideInInspector] 
    public int JumpDir = 0;

    [SerializeField, HideInInspector] 
    private SubstanceInfo mySubstanceInfo;

    [SerializeField, HideInInspector] 
    private Outline myOutline;

    private bool mySlicerOnTouch = false;

    private bool myHoldingByHand = false;

    private HandObject myGrappingHand;

    private bool tempKeypressed = false;

    private enum TouchMode { HAND = 0, SLICER = 2, SUBSTANCE = 3, RIGHTANGLED = 5, RAY = 4, NONE = 0 }

    protected override void Awake()
    {
        base.Awake();

        SlicingTool.OnSlicedThrough += OnSlicedThrough;
        SlicingTool.OnCutThrough += OnCutThrough;
        SlicingTool.OnSlicerTouching += OnSlicerTouching;

        Gluetube.OnGluetubeRayHitObject += BeingHitByGluetubeRay;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        SlicingTool.OnSlicedThrough -= OnSlicedThrough;
        SlicingTool.OnCutThrough -= OnCutThrough;
        SlicingTool.OnSlicerTouching -= OnSlicerTouching;

        Gluetube.OnGluetubeRayHitObject -= BeingHitByGluetubeRay;
    }

    protected override void Start()
    {
        base.Start();

        if (!gameObject.CompareTag("Sliceable"))
            gameObject.tag = "Sliceable";

        Sliceable = GetComponent<Sliceable>();

        if (GetComponent<BoxCollider>() == null)
            transform.AddComponent<BoxCollider>();

        //if (GetComponent<Outline>() != null)
        //    myOutline = GetComponent<Outline>();
        //else if (GetComponentInChildren<Outline>() != null)
        //    myOutline = GetComponentInChildren<Outline>();
        //else
        //    myOutline = transform.AddComponent<Outline>();

        WhenSliced();

        //myOutline.OutlineWidth = 10;

        DrawOutline((int)TouchMode.NONE);

        mySubstanceInfo = new SubstanceInfo(
            GetComponent<BoxCollider>().bounds.max,
            GetComponent<BoxCollider>().bounds.min);

        mySlicerOnTouch = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// This is called when a new substance object with this class is created
    /// through slicing, it'll force this object to jumb toward the direction
    /// opposite to the other sliced object
    /// </summary>
    public void WhenSliced()
    {
        ourRB.AddForce(1 * JumpDir * transform.right, ForceMode.Impulse);
        ourXRGrab.enabled = true;
    }

    /// <summary>
    /// Active or deactive the rigidbody's IsKinematic boolian to make the object
    /// depending by force or by code structure
    /// By Senpai
    /// </summary>
    /// <param name="isKinematic"></param>
    //public void SetWoodToKinematic(bool isKinematic)
    //{
    //    ourRB.isKinematic = isKinematic;
    //    ourRB.useGravity = !isKinematic;
    //}

    /// <summary>
    /// Return what type of material this object is assigned with
    /// By Senpai
    /// </summary>
    /// <returns></returns>
    public MaterialType ReturnMaterialType()
    {
        return MaterialType;
    }

    /// <summary>
    /// This is called by the delegate to the gluetube when it muzzle ray hit this object
    /// When being hit, it'll compare with the name of the object the gluetube ray hit with
    /// this object name.
    /// If the compare is true, this object will active the outline colour of when being hit
    /// by gluetube ray or deactive it when the ray is off.
    /// </summary>
    /// <param name="isHitByRay">Boolian of if the ray is hitting the object</param>
    /// <param name="hitObjectName">The name string to the object the gluetube ray currently hit</param>
    public void BeingHitByGluetubeRay(bool isHitByRay, string hitObjectName)
    {
        if (hitObjectName != transform.name)
            return;

        if(isHitByRay)
            DrawOutline((int)TouchMode.RAY);
        else
            DrawOutline((int)TouchMode.NONE);
    }

    /// <summary>
    /// Assign this object as child to the given parent transform
    /// </summary>
    /// <param name="aParentTransform">Parent transform to be assign to</param>
    public void PutIntoAssamblyParent(Transform aParentTransform)
    {
        transform.parent = aParentTransform;
        DeactiveGrabAndRigidBody();
    }

    public void AttachToGlue(Transform aGlueTransform)
    {
        transform.parent = aGlueTransform.parent;
        DeactiveGrabAndRigidBody();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(transform.parent != null)
            return;

        if(collision.transform.CompareTag("WorkStation"))
            SetWoodToKinematic(true);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if(mySlicerOnTouch)
            return;

        if (other.CompareTag("Drawing"))
        {
            other.GetComponent<MarkingTool>().StartDraw(
                this.transform, 
                mySubstanceInfo.SetTouchedSide(
                    ColliderTools.GetHitSide(this.gameObject, other.gameObject),
                    other.transform.position));
        }

        if(other.CompareTag("Sliceable"))
            DrawOutline((int)TouchMode.SUBSTANCE);

        if(other.CompareTag("Slicer"))
            DrawOutline((int)TouchMode.SLICER);
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        DrawOutline((int)TouchMode.NONE);
    }

    protected override void DrawOutline(int aModeIndex)
    {
        base.DrawOutline(aModeIndex);

        if (aModeIndex == (int)TouchMode.SLICER)
            SetOutlineAppearence(Outline.Mode.OutlineVisible, SlicerTouchColor);
        if (aModeIndex == (int)TouchMode.SUBSTANCE)
            SetOutlineAppearence(Outline.Mode.OutlineAndSilhouette, SubstanceTouchColor);
        if (aModeIndex == (int)TouchMode.RAY)
            SetOutlineAppearence(Outline.Mode.OutlineVisible, Color.grey);

        
        //        case TouchMode.RIGHTANGLED:
        //            myOutline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
        //            myOutline.OutlineColor = RightAngleTouchColor;
        //            break;
        
    }

    protected override void DroppingObject()
    {
        base.DroppingObject();
        SetWoodToKinematic(false);
    }

    public void HitByGlueRay()
    {
        DrawOutline((int)TouchMode.RAY);
    }

    /// <summary>
    /// Fasterning a substance object with this substance object
    /// If this object is not assign to an AssembledProduct as parent
    /// an AssembledProduct object will instantiate into the scene and
    /// the sended substance object will be added into the same AssembledProduct
    /// </summary>
    /// <param name="anAttachingSubstance">The attaching substance object to be attach to this substance</param>
    public void AttachingNewPart(Transform anAttachingSubstance)
    {
        //if (transform.parent == null)
        //{
        //    AssembledProduct formingParent = Instantiate(AssemblyParentPrefab.gameObject, transform.position, Quaternion.identity).GetComponent<AssembledProduct>();
        //    formingParent.AddNewPart(this);

        //    //SetWoodToKinematic(true);
        //    //transform.parent = formingParent.transform;
        //}

            //transform.parent.GetComponent<AssembledProduct>().AddNewPart(anAttachingSubstance.GetComponent<Substance>());
            //anAttachingSubstance.parent = transform.parent;
            //anAttachingSubstance.GetComponent<Substance>().SetWoodToKinematic(true);
    }

    private Vector3 GetTouchMinMaxPosition(BoxHitSide aTouchSide)
    {
        mySubstanceInfo.TouchSide = aTouchSide;

        if(HitSide == BoxHitSide.TOP || HitSide == BoxHitSide.RIGHT || HitSide == BoxHitSide.FORWARD)
            return mySubstanceInfo.MaxPos;

        return mySubstanceInfo.MinPos;
    }

    private void GetBoundaryPoint(BoxHitSide aTouchSide)
    {
        mySubstanceInfo.TouchSide = aTouchSide;

        if (aTouchSide == BoxHitSide.TOP || aTouchSide == BoxHitSide.RIGHT || aTouchSide == BoxHitSide.FORWARD)
            mySubstanceInfo.TouchPoint = mySubstanceInfo.MaxPos;
        else
            mySubstanceInfo.TouchPoint = mySubstanceInfo.MinPos;
    }

    private void OnSlicedThrough(Transform aSlicingTool, Vector3 exitPos, Vector3 enterPos1, Vector3 enterPos2)
    {
        Vector3 rigthPartSize = Vector3Abs(aSlicingTool.position, mySubstanceInfo.MaxPos);//mySubstanceInfo.MaxPos - aSlicingTool.position;
        Vector3 leftPartSize = Vector3Abs(aSlicingTool.position, mySubstanceInfo.MinPos);//mySubstanceInfo.MinPos - aSlicingTool.position;
        SlicerSupportTools.SliceTheObject(this.gameObject, exitPos, enterPos1, enterPos2);
        //GameObject[] slicedParts = SlicerSupportTools.GetSliceParts(this.gameObject, exitPos, enterPos1, enterPos2);
        Destroy(this.gameObject);
    }

    private void OnCutThrough(Transform aSlicingTool)
    {
        SlicerSupportTools.CutObject(this.transform, aSlicingTool);
    }

    private void OnSlicerTouching(string aTouchedSubstanceName)
    {
        if(aTouchedSubstanceName != transform.name)
            return;
        ourXRGrab.enabled = false;
        DrawOutline((int)TouchMode.SLICER);
        mySlicerOnTouch = true;
    }

    private Vector3 Vector3Abs(Vector3 v1, Vector3 v2)
    {
        return new Vector3(Mathf.Abs(v2.x - v1.x), Mathf.Abs(v2.y - v1.y), Mathf.Abs(v2.y - v1.y));
    }
}