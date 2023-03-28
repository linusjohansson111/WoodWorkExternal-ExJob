using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Substance : GrabableObject, MaterialInterface
{
    public delegate void DisplayInfo(BoxHitSide hitSide);
    public static DisplayInfo OnDisplayTransformInfo;

    public Color HandTouchColor = new(96, 203, 21, 255), SlicerTouchColor = new(203, 21, 40, 255), FastenerTouchColor = Color.magenta;
    public Color SubstanceTouchColor, RightAngleTouchColor;

    [SerializeField]
    public MaterialType MaterialType;

    [SerializeField]
    public GameObject AssemblyParentPrefab;

    [HideInInspector]
    public float Lenght { get => mySize.x; }
    
    [HideInInspector] 
    public float Height { get => mySize.y; }
    
    [HideInInspector] 
    public float Width { get => mySize.z; }

    [HideInInspector]
    public float HalfWidth { get => Width * .5f; }

    [HideInInspector]
    public float HalfHeight { get => Height * .5f; }

    [HideInInspector]
    public float HalfLenght { get => Lenght * .5f; }

    [HideInInspector]
    public Vector3 TopPos { get => GetTheFurthestPositionOn(BoxHitSide.TOP); }

    [HideInInspector]
    public Vector3 BottomPos { get => GetTheFurthestPositionOn(BoxHitSide.BOTTOM); }

    [HideInInspector]
    public Vector3 RightPos { get => GetTheFurthestPositionOn(BoxHitSide.RIGHT); }

    [HideInInspector]
    public Vector3 LeftPos { get => GetTheFurthestPositionOn(BoxHitSide.LEFT); }

    [HideInInspector]
    public Vector3 FrontPos { get => GetTheFurthestPositionOn(BoxHitSide.FRONT); }

    [HideInInspector]
    public Vector3 BackPos { get => GetTheFurthestPositionOn(BoxHitSide.REAR); }

    [SerializeField, HideInInspector] 
    private Sliceable Sliceable;

    internal BoxHitSide HitSide = BoxHitSide.NONE;

    [SerializeField, HideInInspector] 
    public int JumpDir = 0;

    [SerializeField, HideInInspector] 
    private SubstanceInfo mySubstanceInfo;

    [SerializeField, HideInInspector] 
    private Outline myOutline;

    public BoxHitSide GlueHitSide = BoxHitSide.NONE;

    private bool mySlicerOnTouch = false;

    private HandObject myGrappingHand;

    private AssembledProduct myProductParent;

    private MeshCollider myMC;

    private Vector3 myAttachOnGluePosition;
    private Vector3 mySize;

    private Vector3 myAttachGlueHitPoint;
    private Vector3 myVerticalGlueSnapPoint;
    private Vector3 myHorizontalGlueSnapPoint;

    private Vector3 myDirectionToGlueHitPoint;
    private float myDistanceToGlueHitPoint;

    private bool myIsVertical = false;

    private float myHorizontalDistanceBetweenGlueHitPoint = 0;
    private Vector3 myHorizontalDirectionToGlueHitPoint = Vector3.zero;

    private float myVerticalDistanceBetweenGlueHitPoint = 0;
    private Vector3 myVerticalDirectionToGlueHitPoint = Vector3.zero;


    private enum TouchMode { NONE = -1, HAND = 0, SLICER = 1, FASTENER = 2, SUBSTANCE = 3, RIGHTANGLED = 4, RAY = 5 }

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

        WhenSliced();

        DrawOutline(TouchTag.NONE);

        mySubstanceInfo = new SubstanceInfo(
            GetComponent<BoxCollider>().bounds.max,
            GetComponent<BoxCollider>().bounds.min);


        mySize = GetComponent<Renderer>().bounds.size;

        mySlicerOnTouch = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //OnDisplayTransformInfo?.Invoke(transform);
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

    public void SetAttachGlueHitPoint(Vector3 aHitPoint)
    {
        if(transform.parent != null)
            myAttachGlueHitPoint = aHitPoint;
    }

    public void SetSnapPosOnGlueHitPoint(Transform aGlueTransform)
    {
        myVerticalGlueSnapPoint = new Vector3(aGlueTransform.position.x, transform.position.y, transform.position.z);
        myHorizontalGlueSnapPoint = new Vector3(transform.position.x, transform.position.y, aGlueTransform.position.z);

        Vector3 vertical = (transform.position - myVerticalGlueSnapPoint);
        myVerticalDirectionToGlueHitPoint = vertical.normalized;
        myVerticalDistanceBetweenGlueHitPoint = vertical.magnitude;

        Vector3 horizontal = (transform.position - myHorizontalGlueSnapPoint);
        myHorizontalDirectionToGlueHitPoint = horizontal.normalized;
        myHorizontalDistanceBetweenGlueHitPoint = horizontal.magnitude;

        myAttachOnGluePosition = aGlueTransform.position;
    }

    private float GetAngleBetweenSubstances(Vector3 anObjectAxis1, Vector3 anObjectAxis2)
    {
        float angle = Vector3.Angle(anObjectAxis1, anObjectAxis2);
        Debug.Log(angle);
        return angle;
    }

    private void SetDistAndDirBetweenGluePoint(bool lookForVertical, Vector3 aSidePoint, float x, float y, float z)
    {
        Vector3 vec = (aSidePoint - new Vector3(x, y, z));
        if(lookForVertical)
        {
            myVerticalDistanceBetweenGlueHitPoint = vec.magnitude;
            myVerticalDirectionToGlueHitPoint = vec.normalized;
        }
        else
        {
            myHorizontalDistanceBetweenGlueHitPoint = vec.magnitude;
            myHorizontalDirectionToGlueHitPoint = vec.normalized;
        }
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
            DrawOutline(TouchTag.OTHER);
        else
            DrawOutline(TouchTag.NONE);
    }

    /// <summary>
    /// Assign this object as child to the given parent transform
    /// </summary>
    /// <param name="aParentTransform">Parent transform to be assign to</param>
    public void PutIntoAssamblyParent(AssembledProduct aParentProduct)
    {
        transform.parent = aParentProduct.transform;
        myProductParent = aParentProduct;

    }

    public Vector3 GetTheFurthestPositionOn(BoxHitSide aHitSide)
    {
        if (aHitSide == BoxHitSide.TOP)
            return transform.position + (transform.up * HalfHeight);
        else if(aHitSide == BoxHitSide.BOTTOM)
            return transform.position - (transform.up * HalfHeight);
        else if (aHitSide == BoxHitSide.RIGHT)
            return transform.position + (transform.right * HalfLenght);
        else if (aHitSide == BoxHitSide.LEFT)
            return transform.position - (transform.right * HalfLenght);
        else if (aHitSide == BoxHitSide.FRONT)
            return transform.position + (transform.forward * HalfWidth);
        else if (aHitSide == BoxHitSide.REAR)
            return transform.position - (transform.forward * HalfWidth);

        return transform.position;
    }

    private void CreateNewProduct()
    {
        RemoveGrabAndRigidbody();
        Instantiate(AssemblyParentPrefab, transform.position, transform.rotation).GetComponent<AssembledProduct>().AddNewPart(this);
        ourBC.isTrigger = true;
    }

    public void AttachToGlue(Transform aGlueTransform)
    {
        RemoveGrabAndRigidbody();

        SnapOnGlue(GlueHitSide = ColliderTools.GetHitSide(transform, aGlueTransform.position), aGlueTransform.GetComponent<GlueSplattQuad>());
        

        ourIsHolding = false;
        ourBC.isTrigger = true;
    }

    public void AttachToGlue(RaycastHit aRayCast, Transform aGlueTransform)
    {
        RemoveGrabAndRigidbody();

        SnapOnGlue(GlueHitSide = ColliderTools.GetSideOnRectangle(GetComponent<MeshFilter>().mesh, aRayCast), aGlueTransform.GetComponent<GlueSplattQuad>());

        ourIsHolding = false;
        ourBC.isTrigger = true;
    }

    /// <summary>
    /// Fasterning a substance object with this substance object
    /// If this object is not assign to an AssembledProduct as parent
    /// an AssembledProduct object will instantiate into the scene and
    /// the sended substance object will be added into the same AssembledProduct
    /// </summary>
    /// <param name="anAttachingSubstance">The attaching substance object to be attach to this substance</param>
    public void AttachingNewPart(Substance anAttachingSubstance, Transform aGlueTransform)
    {
        if (transform.parent == null)
        {
            CreateNewProduct();
        }

        anAttachingSubstance.AttachToGlue(aGlueTransform);
        myProductParent.AddNewPart(anAttachingSubstance);
    }

    public void AttachNewNail(Nail aNail, Vector3 aSurfacePoint)
    {
        BoxHitSide nailHitOn = ColliderTools.GetHitSide(transform, aSurfacePoint);
        aNail.transform.parent = transform;

        
        aNail.transform.rotation = Quaternion.identity;
        aNail.transform.position = aSurfacePoint + aNail.transform.up * aNail.HalfLenght;

    }

    // this will be removed since the MeshCollider is no more
    public void AttachingNewPart(RaycastHit aRaycast, Transform aGlueTransform)
    {
        if (transform.parent == null)
        {
            CreateNewProduct();
        }

        aRaycast.transform.GetComponent<Substance>().AttachToGlue(aRaycast, aGlueTransform);
        myProductParent.AddNewPart(aRaycast.transform.GetComponent<Substance>());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(transform.parent != null)
            return;

        if(collision.transform.CompareTag("WorkStation"))
            SetWoodToKinematic(true);

        if(collision.transform.CompareTag("Glue"))
        {
            
        }

        //if(collision.transform.CompareTag("Fastener"))
        //{
        //    DrawOutline((int)TouchMode.FASTENER);
        //}
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
            DrawOutline(TouchTag.SUBSTANCE);

        if(other.CompareTag("Slicer"))
            DrawOutline(TouchTag.SLICER);

        if (other.CompareTag("Fastener"))
            DrawOutline(TouchTag.FASTERNER);
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        DrawOutline(TouchTag.NONE);
    }

    protected override void DrawOutline(TouchTag aTag)
    {
        base.DrawOutline(aTag);

        if (aTag == TouchTag.SLICER)
            SetOutlineAppearence(Outline.Mode.OutlineVisible, GetColorFor(TouchTag.SLICER)/*SlicerTouchColor*/);
        if (aTag == TouchTag.SUBSTANCE)
            SetOutlineAppearence(Outline.Mode.OutlineAndSilhouette, GetColorFor(TouchTag.SUBSTANCE)/*SubstanceTouchColor*/);
        if (aTag == TouchTag.OTHER)
            SetOutlineAppearence(Outline.Mode.OutlineVisible, Color.grey);
        if(aTag == TouchTag.FASTERNER)
            SetOutlineAppearence(Outline.Mode.OutlineVisible, GetColorFor(TouchTag.FASTERNER)/*FastenerTouchColor*/);


        //        case TouchMode.RIGHTANGLED:
        //            myOutline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
        //            myOutline.OutlineColor = RightAngleTouchColor;
        //            break;

    }

    protected override void GrabingOject(HandObject aGrabbingHandObject, string aGrabbedObjectName)
    {
        base.GrabingOject(aGrabbingHandObject, aGrabbedObjectName);

    }
    protected override void DroppingObject()
    {
        base.DroppingObject();
        SetWoodToKinematic(false);
    }

    public void HitByGlueRay()
    {
        DrawOutline(TouchTag.OTHER);
    }

    private Vector3 GetTouchMinMaxPosition(BoxHitSide aTouchSide)
    {
        mySubstanceInfo.TouchSide = aTouchSide;

        if(HitSide == BoxHitSide.TOP || HitSide == BoxHitSide.RIGHT || HitSide == BoxHitSide.FRONT)
            return mySubstanceInfo.MaxPos;

        return mySubstanceInfo.MinPos;
    }

    private void GetBoundaryPoint(BoxHitSide aTouchSide)
    {
        mySubstanceInfo.TouchSide = aTouchSide;

        if (aTouchSide == BoxHitSide.TOP || aTouchSide == BoxHitSide.RIGHT || aTouchSide == BoxHitSide.FRONT)
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
        DrawOutline(TouchTag.SLICER);
        mySlicerOnTouch = true;
    }

    private Vector3 Vector3Abs(Vector3 v1, Vector3 v2)
    {
        return new Vector3(Mathf.Abs(v2.x - v1.x), Mathf.Abs(v2.y - v1.y), Mathf.Abs(v2.y - v1.y));
    }

    private void SnapOnGlue(BoxHitSide aHitSide, GlueSplattQuad aSplatt)
    {
        SetSnapPosOnGlueHitPoint(aSplatt.transform);

        if(aHitSide == BoxHitSide.RIGHT || aHitSide == BoxHitSide.LEFT)
        {
            SnapPosition(aHitSide, aSplatt, HalfLenght);
            transform.Rotate(new Vector3(0f, (myIsVertical ? 0 : 90), (aHitSide == BoxHitSide.RIGHT ? -90f : 90f)));
        }
        else if(aHitSide == BoxHitSide.TOP || aHitSide == BoxHitSide.BOTTOM)
        {
            SnapPosition(aHitSide, aSplatt, HalfHeight);
            transform.Rotate(new Vector3(0f, (myIsVertical ? 0 : 90), (aHitSide == BoxHitSide.TOP ? 180f : 0f)));
        }
        else if (aHitSide == BoxHitSide.FRONT || aHitSide == BoxHitSide.REAR)
        {
            SnapPosition(aHitSide, aSplatt, HalfWidth);
            transform.Rotate(new Vector3((aHitSide == BoxHitSide.FRONT ? 90f : -90f), (myIsVertical ? 90f : 0f), 0f));
        }
    }

    private void SnapPosition(BoxHitSide hitSide, GlueSplattQuad aSplatt, float aMoveUpValue)
    {
        if(hitSide == BoxHitSide.FRONT || hitSide == BoxHitSide.REAR)
            myIsVertical = IsVerticalSnap(aSplatt.transform.forward, transform.up);
        else
            myIsVertical = IsVerticalSnap(aSplatt.transform.forward, transform.forward);

        transform.position = MoveFromGluePosition(aSplatt.GetSnapPosition(myIsVertical));
        transform.rotation = Quaternion.identity;

        transform.position += aSplatt.transform.up * aMoveUpValue;
    }

    private bool IsVerticalSnap(Vector3 aGlueTransformDir, Vector3 aSubstandeDir)
    {
        float angle = Vector3.Angle(aGlueTransformDir, aSubstandeDir);


        return ((angle <= 45 || angle >= 135) ? true : false);
    }

    private Vector3 MoveFromGluePosition(Vector3 aGlueSnapPosition)
    {
        float moveDistance = (myIsVertical) ? myVerticalDistanceBetweenGlueHitPoint : myHorizontalDistanceBetweenGlueHitPoint;
        Vector3 moveDirection = (myIsVertical) ? myVerticalDirectionToGlueHitPoint : myHorizontalDirectionToGlueHitPoint;

        return aGlueSnapPosition + (moveDirection * moveDistance);
    }

    private Vector3 SnapRotation(bool isVertical, float aDegree)
    {
        return new Vector3((isVertical ? 0 : aDegree), 0f, (isVertical ? aDegree : 0));
    }

    private Vector3 GetAxisDistances(Vector3 hitSideCenterPoint, Vector3 aGluePoint)
    {
        return new Vector3(Mathf.Abs(hitSideCenterPoint.x - aGluePoint.x), Mathf.Abs(hitSideCenterPoint.y - aGluePoint.y), Mathf.Abs(hitSideCenterPoint.z - aGluePoint.z));
    }

    //private Vector3 GetCenterPointOnSide(BoxHitSide aHitSide)
    //{
    //    if (aHitSide == BoxHitSide.LEFT)
    //        return LeftPos;
    //    else if (aHitSide == BoxHitSide.RIGHT)
    //        return RightPos;
    //    else if (aHitSide == BoxHitSide.TOP)
    //        return TopPos;
    //    else if (aHitSide == BoxHitSide.BOTTOM)
    //        return BottomPos;
    //    else if (aHitSide == BoxHitSide.FRONT)
    //        return FrontPos;
    //    else if(aHitSide == BoxHitSide.BACK)
    //        return BackPos;

    //    return transform.position;
    //}
}