using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueSnapArea : MonoBehaviour
{
    [SerializeField]
    private BoxHitSide PlaceOnSide = BoxHitSide.NONE;

    [SerializeField]
    private bool GotGlueOn = false;
    // Start is called before the first frame update

    public BoxHitSide Side { get { return PlaceOnSide; } }

    private MaterialPart myParent;

    public Vector3 OffsetDirection { get; private set; }
    public float OffsetDistance { get; private set; }

    void Start()
    {
        myParent = GetComponentInParent<MaterialPart>();
        
        OffsetDistance = (transform.parent.position - transform.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (GotGlueOn)
        {
            if (Physics.Raycast(transform.position, transform.up, out RaycastHit hit, 0.05f, LayerMask.NameToLayer("Glue")))
            {

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Glue") && GotGlueOn)
        {
            

            if(other.transform.GetComponent<GlueSnapArea>() != null)
            {
                GlueSnapArea snapArea = other.transform.GetComponent<GlueSnapArea>();
                BoxHitSide hitSide = snapArea.Side;
                BuildUpBlock block = other.transform.GetComponentInParent<MaterialPart>().ParentBlock;

                //other.transform.GetComponentInParent<MaterialPart>().TempAttachToGlueArea(this, hitSide);
                other.transform.GetComponentInParent<MaterialPart>().TempAttachToGlueArea2(snapArea, this.transform.position, hitSide);
                block.TransferChildrenTo(myParent.ParentBlock);
                GotGlueOn = false;

            }
        }
    }

    public Vector3 GetOffsetDirection()
    {
        return OffsetDirection = (transform.parent.position - transform.position).normalized;
    }
}