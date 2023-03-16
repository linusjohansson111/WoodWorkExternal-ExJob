using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialPart : MonoBehaviour, MaterialInterface
{
    [SerializeField]
    public MaterialType MaterialType;

    public Transform AttachPoint;

    public float Width { get => mySize.x; }

    public float Height { get => mySize.y; }

    public float Lenght { get => mySize.z; }

    public Vector3 HalfSize { get => mySize * .5f; }

    public BuildUpBlock ParentBlock { get { return myParentBlock; } }
    private BuildUpBlock myParentBlock;

    private Outline myOutline;

    private Vector3 mySize = Vector3.zero;

    private enum TouchMode { HAND, GLUE, NONE = -1 };

    // Start is called before the first frame update
    void Start()
    {
        mySize = GetComponent<Renderer>().bounds.size;

        myParentBlock = GetComponentInParent<BuildUpBlock>();
        myOutline = GetComponentInParent<Outline>();

        DrawOutline(TouchMode.NONE);
    }

    // Update is called once per frame
    void Update()
    {
        
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
        BoxHitSide side = ColliderTools.GetHitSide(transform, hitPoint);
        

        float upPos = 0;
        Vector3 newRotation = Vector3.zero;
        bool isVertical = false;
        float yRot = 0;
        if (side == BoxHitSide.FRONT || side == BoxHitSide.BACK)
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

        transform.rotation = Quaternion.identity;
        //transform.position = aSplatt.transform.position + (aSplatt.transform.up * upPos);
        transform.position = (isVertical ? aSplatt.VerticalSnapPoint.position : aSplatt.HorizontalSnapPoint.position) + (aSplatt.transform.up * upPos);
        transform.Rotate(newRotation);
    }

    private bool IsVerticalSnap(Vector3 aGlueTransformDir, Vector3 aSubstandeDir)
    {
        float angle = Vector3.Angle(aGlueTransformDir, aSubstandeDir);

        return ((angle <= 45 || angle >= 135) ? true : false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Hand"))
        {
            myParentBlock.GetChildAttachPoint(AttachPoint);
            DrawOutline(TouchMode.HAND);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Hand"))
        {
            myParentBlock.NullifyChildAttachPoint();
            DrawOutline(TouchMode.NONE);
        }
    }

    private void DrawOutline(TouchMode aSelectMode)
    {
        if (aSelectMode == TouchMode.NONE)
            myOutline.enabled = false;
        else
        {
            myOutline.enabled = true;
            if (aSelectMode == TouchMode.HAND)
                SetOutlineAppearence(Outline.Mode.OutlineVisible, myParentBlock.GetOutlineColorFor(TouchTag.HAND)/*Color.white*/);
            else if(aSelectMode == TouchMode.GLUE)
                SetOutlineAppearence(Outline.Mode.OutlineVisible, myParentBlock.GetOutlineColorFor(TouchTag.OTHER)/*Color.white*/);
        }
    }

    private void SetOutlineAppearence(Outline.Mode aMode, Color aColor)
    {
        myOutline.OutlineMode = aMode;
        myOutline.OutlineColor = aColor;
    }
}
