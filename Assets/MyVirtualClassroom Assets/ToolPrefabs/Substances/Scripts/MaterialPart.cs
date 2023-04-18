using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class MaterialPart : MonoBehaviour, MaterialInterface
{
    [SerializeField]
    public MaterialType MaterialType;

    [SerializeField]
    private Transform[] HandAttachPoints = new Transform[2];

    [SerializeField]
    private List<Transform> AttachPoints;

    [SerializeField]
    private List<GlueSnapArea> GlueSnapSpots = new List<GlueSnapArea>();

    public float Width { get => mySize.x; }

    public float Height { get => mySize.y; }

    public float Lenght { get => mySize.z; }

    public Vector3 HalfSize { get => mySize * .5f; }

    public BuildUpBlock ParentBlock { get { return myParentBlock; } }
    private BuildUpBlock myParentBlock;

    private Outline myOutline;

    private Vector3 mySize = Vector3.zero;

    private enum TouchMode { HAND, GLUE, FASTERNER,  NONE = -1 };

    // Start is called before the first frame update
    void Start()
    {
        mySize = GetComponent<Renderer>().bounds.size;
        

        myParentBlock = GetComponentInParent<BuildUpBlock>();
        myOutline = GetComponentInParent<Outline>();
        
        for(int i = 0; i < (int)BoxHitSide.NONE; i++)
        {
            AttachPoints[i].position = GetOutmostPosFor((BoxHitSide)i);
        }

        DrawOutline(TouchMode.NONE);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKey(KeyCode.D))
        //    MoveAttachPointTo(BoxHitSide.RIGHT);
        //if (Input.GetKey(KeyCode.A))
        //    MoveAttachPointTo(BoxHitSide.LEFT);
        //if (Input.GetKey(KeyCode.W))
        //    MoveAttachPointTo(BoxHitSide.FRONT);
        //if (Input.GetKey(KeyCode.S))
        //    MoveAttachPointTo(BoxHitSide.BACK);
        //if (Input.GetKey(KeyCode.E))
        //    MoveAttachPointTo(BoxHitSide.TOP);
        //if (Input.GetKey(KeyCode.Q))
        //    MoveAttachPointTo(BoxHitSide.BOTTOM);
    }

    public MaterialType ReturnMaterialType()
    {
        return MaterialType;
    }

    public void BeingHitByGluetubeRay(bool isHitByRay, string hitObjectName)
    {
        if (hitObjectName != transform.name)
            return;

        //if (isHitByRay)
        //    DrawOutline((int)TouchMode.RAY);
        //else
        //    DrawOutline((int)TouchMode.NONE);
    }

    public void DrawGlueTupeOutline(bool tubeMuzzleInRange)
    {
        DrawOutline((tubeMuzzleInRange ? TouchMode.GLUE : TouchMode.NONE));
    }

    public void SetParent(BuildUpBlock aParentBlock)
    {
        myParentBlock = aParentBlock;
        transform.parent = myParentBlock.transform;
    }
    
    public void AttachToGlue(GlueSplattQuad aSplatt, Vector3 hitPoint)
    {
        Debug.Log("glue script yayaaa");
        BoxHitSide side = ColliderTools.GetHitSide(transform, hitPoint);
        

        Vector3 newRotation = Vector3.zero;
        float upPos = 0;
        float yRot;
        bool isVertical;

        if (side == BoxHitSide.FRONT || side == BoxHitSide.REAR)
        {
            isVertical = IsVerticalSnap(aSplatt.transform.forward, transform.up);
            yRot = (isVertical ? 90f : 0f);

            upPos = HalfSize.z;
            newRotation = new Vector3((side == BoxHitSide.FRONT ? 90f : -90f), yRot, z: 0f);
        }
        else
        {
            isVertical = IsVerticalSnap(aSplatt.transform.forward, transform.forward);
            yRot = (isVertical ? 0f : 90f);

            if (side == BoxHitSide.LEFT || side == BoxHitSide.RIGHT)
            {
                upPos = HalfSize.x;
                newRotation = new Vector3(x: 0f, yRot, (side == BoxHitSide.RIGHT ? -90f : 90f));
            }
            else
            if (side == BoxHitSide.TOP || side == BoxHitSide.BOTTOM)
            {
                upPos = HalfSize.y;
                newRotation = new Vector3(x: 0f, yRot, (side == BoxHitSide.TOP ? 180f : 0f));
            }
        }


        // Vector3 glueSnapPoint = new Vector3((isVertical ? hitPoint.x : transform.position.x), transform.position.y, (isVertical ? transform.position.z : hitPoint.z));

        // Vector3 vecBetCentAndHit = (transform.position - glueSnapPoint);
        // Vector3 dir = vecBetCentAndHit.normalized;
        // float dist = vecBetCentAndHit.magnitude;

        // Vector3 newPos = aSplatt.transform.position + (dir * dist);
        // transform.rotation = Quaternion.identity;
        // //transform.position = aSplatt.transform.position + (aSplatt.transform.up * upPos);
        // transform.position = newPos + (aSplatt.transform.up * upPos);
        // transform.Rotate(newRotation);

        // Add a hinge joint to connect splattObject to the glue object


    }


    // public void SnapToOtherMaterial(aGlueAreaTransformBoxHitSide hitSide, )
    // {

    // }

    // public void TempAttachToGlueArea(GlueSnapArea aGlueAreaTransform, BoxHitSide hitSide)
    // {
    //     BoxHitSide side = hitSide;

    //     float offsetPos = 0;
    //     Vector3 offsetDir = Vector3.zero;

    //     Vector3 newRotation = Vector3.zero;

    //     if (side == BoxHitSide.FRONT || side == BoxHitSide.REAR)
    //     {
    //         offsetPos = HalfSize.z;
    //         newRotation = new Vector3((side == BoxHitSide.FRONT ? 90f : -90f), y: 0, z: 0f);
    //     }
    //     else
    //     {
    //         if (side == BoxHitSide.LEFT || side == BoxHitSide.RIGHT)
    //         {
    //             offsetPos = HalfSize.x;
    //             newRotation = new Vector3(x: 0f, y: 0, (side == BoxHitSide.RIGHT ? -90f : 90f));
    //         }
    //         else
    //         if (side == BoxHitSide.TOP || side == BoxHitSide.BOTTOM)
    //         {
    //             offsetPos = HalfSize.y;
    //             newRotation = new Vector3(x: 0f, y: 0, (side == BoxHitSide.TOP ? 180f : 0f));
    //         }
    //     }
        
    // }

    //     transform.rotation = Quaternion.identity;
    //     transform.position = aGlueAreaTransform.transform.position + (aGlueAreaTransform.transform.up * offsetPos);
    //     transform.Rotate(newRotation);
    // }

    // public void TempAttachToGlueArea2(GlueSnapArea aGlueAreaTransform, Vector3 aGlueAreaPosition, BoxHitSide hitSide)
    // {
    //     BoxHitSide side = hitSide;

    //     Vector3 offsetDir = Vector3.zero;

    //     Vector3 newRotation = Vector3.zero;
    //     transform.rotation = Quaternion.identity;
    //     //if (side == BoxHitSide.FRONT || side == BoxHitSide.REAR)
    //     //{
    //     //    offsetPos = HalfSize.z;
    //     //    newRotation = new Vector3((side == BoxHitSide.FRONT ? 90f : -90f), y: 0, z: 0f);
    //     //}
    //     //else
    //     //{
    //     if (side == BoxHitSide.LEFT || side == BoxHitSide.RIGHT)
    //         {
    //             transform.Rotate(new Vector3(x: 0f, y: 0, (side == BoxHitSide.RIGHT ? -90f : 90f)));
    //             offsetDir = aGlueAreaTransform.GetOffsetDirection();//aGlueAreaTransform.OffsetDirection;
    //         }
    //         else
    //         if (side == BoxHitSide.TOP || side == BoxHitSide.BOTTOM)
    //         {
    //             transform.Rotate(new Vector3(x: 0f, y: 0, (side == BoxHitSide.TOP ? 180f : 0f)));
    //             offsetDir = aGlueAreaTransform.GetOffsetDirection();//aGlueAreaTransform.OffsetDirection;
    //     }
    //     //}

        
    //     transform.position = aGlueAreaPosition + (offsetDir * aGlueAreaTransform.OffsetDistance);
    // }

    // public void TempAttachToGlueArea(Transform aGlueAreaTransform, Vector3 hitPoint)
    // {
    //     BoxHitSide side = ColliderTools.GetHitSide(transform, hitPoint);

    //     float upPos = 0;
    //     Vector3 newRotation = Vector3.zero;

    //     if (side == BoxHitSide.FRONT || side == BoxHitSide.REAR)
    //     {
    //         upPos = HalfSize.z;
    //         newRotation = new Vector3((side == BoxHitSide.FRONT ? 90f : -90f), y: 0, z: 0f);
    //     }
    //     else
    //     {
    //         if (side == BoxHitSide.LEFT || side == BoxHitSide.RIGHT)
    //         {
    //             upPos = HalfSize.x;
    //             newRotation = new Vector3(x: 0f, y: 0, (side == BoxHitSide.RIGHT ? -90f : 90f));
    //         }
    //         else
    //         if (side == BoxHitSide.TOP || side == BoxHitSide.BOTTOM)
    //         {
    //             upPos = HalfSize.y;
    //             newRotation = new Vector3(x: 0f, y: 0, (side == BoxHitSide.TOP ? 180f : 0f));
    //         }
    //     }

    //     transform.rotation = Quaternion.identity;
    //     //transform.position = aSplatt.transform.position + (aSplatt.transform.up * upPos);
    //     transform.position = aGlueAreaTransform.position + (aGlueAreaTransform.up * upPos);
    //     transform.Rotate(newRotation);
    // }

    public void AttachNewNail(Nail aNail, Vector3 aSurfacePoint)
    {
        BoxHitSide nailHitOn = ColliderTools.GetHitSide(transform, aSurfacePoint);
        aNail.transform.parent = transform;


        aNail.transform.rotation = Quaternion.identity;
        aNail.transform.position = aSurfacePoint + aNail.transform.up * aNail.HalfLenght;

    }

    private bool IsVerticalSnap(Vector3 aGlueTransformDir, Vector3 aSubstandeDir)
    {
        float angle = Vector3.Angle(aGlueTransformDir, aSubstandeDir);

        return (angle <= 45 || angle >= 135);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Hand"))
        {
            //if (other.transform.GetComponent<HandObject>().Side == Preposition.LEFT)
            //    myParentBlock.GetChildAttachPointFor(Preposition.LEFT, HandAttachPoints[(int)Preposition.LEFT]);
            //else if(other.transform.GetComponent<HandObject>().Side == Preposition.RIGHT)
            //    myParentBlock.GetChildAttachPointFor(Preposition.RIGHT, HandAttachPoints[(int)Preposition.RIGHT]);
            //myParentBlock.GetChildAttachPoint(AttachPoint);
            DrawOutline(TouchMode.HAND);
            myParentBlock.SetWoodToKinematic(true);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Fastener")/*other.transform.CompareTag("Fastener")*/)
        {
            DrawOutline(TouchMode.FASTERNER);
            myParentBlock.SetWoodToKinematic(true);
        }

        //InfoCanvas.Ins.DisplayAboveObjectInfo(other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Hand"))
        {
            myParentBlock.NullifyAttachPointFor(other.transform.GetComponent<HandObject>().Side);
            //if (other.transform.GetComponent<HandObject>().Side == Preposition.LEFT)
            //    myParentBlock.NullifyAttachPointFor(Preposition.LEFT);
            //else if (other.transform.GetComponent<HandObject>().Side == Preposition.RIGHT)
            //    myParentBlock.NullifyAttachPointFor(Preposition.RIGHT);
            //myParentBlock.NullifyLeftAttachPoint();
            myParentBlock.SetWoodToKinematic(false);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Fastener")/*other.transform.CompareTag("Fastener")*/)
        {
            myParentBlock.SetWoodToKinematic(false);
        }
        DrawOutline(TouchMode.NONE);
        //InfoCanvas.Ins.DisplayAboveObjectInfo("");
    }

    private void DrawOutline(TouchMode aSelectMode)
    {
        if (aSelectMode == TouchMode.NONE)
            myOutline.enabled = false;
        else
        {
            myOutline.enabled = true;
            if (aSelectMode == TouchMode.HAND)
                SetOutlineAppearence(Outline.Mode.OutlineVisible, (myParentBlock.IsGrabable ? myParentBlock.GetOutlineColorFor(TouchTag.HAND) : Color.black));
            else if(aSelectMode == TouchMode.GLUE)
                SetOutlineAppearence(Outline.Mode.OutlineVisible, myParentBlock.GetOutlineColorFor(TouchTag.OTHER));
            else if(aSelectMode == TouchMode.FASTERNER)
                SetOutlineAppearence(Outline.Mode.OutlineVisible, myParentBlock.GetOutlineColorFor(TouchTag.FASTERNER));
        }
    }

    private void SetOutlineAppearence(Outline.Mode aMode, Color aColor)
    {
        myOutline.OutlineMode = aMode;
        myOutline.OutlineColor = aColor;
    }

    public void MoveAttachPointTo(Preposition hand, Vector3 aHitPoint)
    {
        BoxHitSide side = ColliderTools.GetHitSide(transform, aHitPoint);
        if (side == BoxHitSide.NONE)
            return;

        //HandAttachPoints[(int)hand].position = GetOutmostPosFor(side);
        //myParentBlock.GetChildAttachPointFor(hand, HandAttachPoints[(int)hand]);

        // myParentBlock.GetChildAttachPointFor(hand, AttachPoints[(int)side]);

        

        //myParentBlock.SetHitSideOnBuildUp(aHitPoint);
        //AttachPoint.position = GetOutmostPosFor(side);
        //if (side < BoxHitSide.NONE)
        //    myParentBlock.GetChildAttachPointFor(hand, AttachPoints[(int)side]);

    }

    private Vector3 GetOutmostPosFor(BoxHitSide aSearchSide)
    {
        
        Vector3 result = transform.position;
        if (aSearchSide == BoxHitSide.RIGHT)
            result = new Vector3(GetSidePeak(transform.right, HalfSize.x).x, transform.position.y, transform.position.z);
        else if (aSearchSide == BoxHitSide.LEFT)
            result = new Vector3(GetSidePeak(-transform.right, HalfSize.x).x, transform.position.y, transform.position.z);
        
        else if (aSearchSide == BoxHitSide.TOP)
            result = new Vector3(transform.position.x, GetSidePeak(transform.up, HalfSize.y).y, transform.position.z);
        else if (aSearchSide == BoxHitSide.BOTTOM)
            result = new Vector3(transform.position.x, GetSidePeak(-transform.up, HalfSize.y).y, transform.position.z);
        
        else if (aSearchSide == BoxHitSide.FRONT)
            result = new Vector3(transform.position.x, transform.position.y, GetSidePeak(transform.forward, HalfSize.z).z);
        else if (aSearchSide == BoxHitSide.REAR)
            result = new Vector3(transform.position.x, transform.position.y, GetSidePeak(-transform.forward, HalfSize.z).z);
        
        return result;
    }

    private Vector3 GetSidePeak(Vector3 aDir, float aDist)
    {
        return (transform.position + (aDir * aDist));
    }
}
