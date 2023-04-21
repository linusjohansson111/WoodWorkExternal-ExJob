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
                // Plocka ut alla barn som har materialPart
                // GÃ¶r en for loop med samma kod.


                 // Variables for easy access
                MaterialPart[] otherMaterialParts = other.GetComponent<GlueSnapArea>().myParent.ParentBlock.GetComponentsInChildren<MaterialPart>();
                Debug.Log(otherMaterialParts);

                // Transfer children of otherMaterialPart parent to this materialPart parent
                BuildUpBlock block = otherMaterialParts[0].ParentBlock;
                block.TransferChildrenTo(myParent.ParentBlock);
                for (int i = 0; i < otherMaterialParts.Length; i++) {
                    Debug.Log("Ran " + i + " times");
                    // Object other = otherMaterialParts[i]; 
                    // MaterialPart otherMaterialPart = other.transform.GetComponentInParent<MaterialPart>();
                    MaterialPart otherMaterialPart = otherMaterialParts[i];
                    //BuildUpBlock block = otherMaterialPart.ParentBlock;

                    // get angle of materialPart, round up or down in order to have a working snapping effect
                    
                    Vector3 eulerAng = otherMaterialPart.transform.localRotation.eulerAngles;
                    eulerAng.x = (Mathf.Round(eulerAng.x / 90f)*90f);
                    eulerAng.y = (Mathf.Round(eulerAng.y / 90f)*90f);
                    eulerAng.z = (Mathf.Round(eulerAng.z / 90f)*90f);
                    // Apply rounded numbers to otherMaterialPart
                    
                    otherMaterialPart.transform.localRotation = Quaternion.Euler(eulerAng);
                    
                    // Move other materialPart closer to this material Part. 
                    
                    otherMaterialPart.transform.position = otherMaterialPart.transform.position + (this.transform.position-other.transform.position);
                }

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
