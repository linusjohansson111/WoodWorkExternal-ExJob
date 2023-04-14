using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    }

    // Update is called once per frame
    void Update()
    {
        // if (GotGlueOn)
        // {
        //     if (Physics.Raycast(transform.position, transform.up, out RaycastHit hit, 0.05f, LayerMask.NameToLayer("Glue")))
        //     {

        //     }
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter GlueSnapArea");
        if(other.gameObject.layer == LayerMask.NameToLayer("Glue"))
        {
            if(other.transform.GetComponentInParent<Gluetube>() != null)
            {
                other.transform.GetComponentInParent<Gluetube>().ActiveGlueTube(true, this);
                myParent.DrawGlueTupeOutline(true);
            }

            if(other.transform.GetComponent<GlueSnapArea>() != null && GotGlueOn)
            {
                 // Variables for easy access
                MaterialPart otherMaterialPart = other.transform.GetComponentInParent<MaterialPart>();
                BuildUpBlock block = otherMaterialPart.ParentBlock;

                // Transfer children of otherMaterialPart parent to this materialPart parent
                block.TransferChildrenTo(myParent.ParentBlock);

                // get angle of materialPart, round up or down in order to have a working snapping effect
                Vector3 eulerAng = otherMaterialPart.transform.localRotation.eulerAngles;
                eulerAng.x = (Mathf.Round(eulerAng.x / 90f)*90f);
                eulerAng.y = (Mathf.Round(eulerAng.y / 90f)*90f);
                eulerAng.z = (Mathf.Round(eulerAng.z / 90f)*90f);
                // Apply rounded numbers to otherMaterialPart
                otherMaterialPart.transform.localRotation = Quaternion.Euler(eulerAng);
                
                Vector3 halfGlueBox = GetComponent<BoxCollider>().size;
                Vector3 difference = this.transform.position-other.transform.position;
                Debug.Log(halfGlueBox);

                // Move other materialPart closer to this material Part. 
                // (In our opinion this logic should not yield a correct result. Instead the other materialPart should hover above this materialPart.)
                otherMaterialPart.transform.position = otherMaterialPart.transform.position + (this.transform.position-other.transform.position);
                // if (difference.y < 0){
                //     Vector3 newPos = new Vector3(
                //         otherMaterialPart.transform.position.x, 
                //         otherMaterialPart.transform.position.y - halfGlueBox.y/4, 
                //         otherMaterialPart.transform.position.z);
                //     otherMaterialPart.transform.position = newPos;
                // } else {
                //     Vector3 newPos = new Vector3(
                //         otherMaterialPart.transform.position.x, 
                //         otherMaterialPart.transform.position.y + halfGlueBox.y/4, 
                //         otherMaterialPart.transform.position.z);
                //     otherMaterialPart.transform.position = newPos;
                // }

                // Remove glue & delete parent of other materialPart (children have been transfered to this materialPart).
                GotGlueOn = false;
                Destroy(block.gameObject);

            }
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Glue"))
        {
            if (other.transform.GetComponentInParent<Gluetube>() != null)
            {
                other.transform.GetComponentInParent<Gluetube>().ActiveGlueTube(false, null);
                myParent.DrawGlueTupeOutline(false);
            }
        }
    }

    public Vector3 GetOffsetDirection()
    {
        return OffsetDirection = (transform.parent.position - transform.position).normalized;
    }

    public void ThisAreaIsGlued()
    {
        GotGlueOn = true;
    }
}
