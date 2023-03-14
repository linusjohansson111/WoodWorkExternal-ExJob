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
