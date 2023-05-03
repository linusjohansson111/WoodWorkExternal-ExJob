using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class GlueSnapArea : MonoBehaviour
{
    [SerializeField]
    private BoxHitSide PlaceOnSide = BoxHitSide.NONE;

    [SerializeField]
    public bool GotGlueOn = false;
    // Start is called before the first frame update

    public BoxHitSide Side { get { return PlaceOnSide; } }

    private MaterialPart myParent;

    public Vector3 OffsetDirection { get; private set; }
    public float OffsetDistance { get; private set; }

    void Start()
    {
        myParent = GetComponentInParent<MaterialPart>();
        ChangeMesh();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        Gluetube.glueTubeHold += ChangeMesh;
    }

    private void OnDisable()
    {
        Gluetube.glueTubeHold -= ChangeMesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Glue"))
        {
            Gluetube glueTube = other.transform.GetComponentInParent<Gluetube>();
            GlueSnapArea otherGlueSnapArea = other.transform.GetComponent<GlueSnapArea>(); 

            if(glueTube != null)
            {
                glueTube.ActiveGlueTube(true, this);
                myParent.DrawGlueTupeOutline(true);
            }

            if(otherGlueSnapArea != null && GotGlueOn)
            {
                // This line is important in case both glueSnapAreas got glue on. 
                // Otherwise will Destroy both components at the end of if-statement
                otherGlueSnapArea.GotGlueOn = false;
                // Variables for easy access
                MaterialPart[] otherMaterialParts = otherGlueSnapArea.myParent.ParentBlock.GetComponentsInChildren<MaterialPart>();
                BuildUpBlock block = otherMaterialParts[0].ParentBlock;

                // Transfer children of otherMaterialPart parent to this materialPart parent
                block.TransferChildrenTo(myParent.ParentBlock);

                
                for (int i = 0; i < otherMaterialParts.Length; i++) {

                    MaterialPart otherMaterialPart = otherMaterialParts[i];

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

    private void ChangeMesh() {
        // kolla om vi har mesh, isf ta bort
        GetComponentInChildren<MeshRenderer>().enabled = !GetComponentInChildren<MeshRenderer>().enabled;
    }
}
