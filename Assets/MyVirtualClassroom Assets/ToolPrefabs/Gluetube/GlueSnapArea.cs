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
                GlueSnapArea snapArea = other.transform.GetComponent<GlueSnapArea>();
                //Ger oss sida av hit på material
                BoxHitSide hitSide = snapArea.Side;
                BuildUpBlock block = other.transform.GetComponentInParent<MaterialPart>().ParentBlock;


                // Vector3 relativePos = other.transform.position - this.transform.position;
                // other.transform.GetComponentInParent<MaterialPart>().transform.position = other.transform.GetComponentInParent<MaterialPart>().transform.position + (this.transform.position-other.transform.position);
                
                // other.transform.GetComponentInParent<MaterialPart>().transform.position = other.transform.GetComponentInParent<MaterialPart>().transform.position + this.transform.position;

                // Vector3 eulerAng = other.transform.rotation.eulerAngles;
                // eulerAng.x = (Mathf.Round(eulerAng.x / 90f)*90f);
                // eulerAng.y = (Mathf.Round(eulerAng.y / 90f)*90f);
                // eulerAng.z = (Mathf.Round(eulerAng.z / 90f)*90f);
                // other.transform.GetComponentInParent<MaterialPart>().transform.rotation = other.transform.GetComponentInParent<MaterialPart>().transform.rotation * Quaternion.Euler(eulerAng);
                
                // eulerAng.x = (Mathf.Round(eulerAng.x / 90f)*90f);
                // eulerAng.y = (Mathf.Round(eulerAng.y / 90f)*90f);
                // eulerAng.z = (Mathf.Round(eulerAng.z / 90f));
                // eulerAng.z *= 90f;

                // Rotation verkar fungera, men positionen blir lite off av den här koden
                block.TransferChildrenTo(myParent.ParentBlock);
                Vector3 eulerAng = other.transform.GetComponentInParent<MaterialPart>().transform.localRotation.eulerAngles;
                eulerAng.x = (Mathf.Round(eulerAng.x / 90f)*90f);
                eulerAng.y = (Mathf.Round(eulerAng.y / 90f)*90f);
                eulerAng.z = (Mathf.Round(eulerAng.z / 90f)*90f);
                other.transform.GetComponentInParent<MaterialPart>().transform.localRotation = Quaternion.Euler(eulerAng);

                Vector3 relativePos = other.transform.position - this.transform.position;

                other.transform.GetComponentInParent<MaterialPart>().transform.position = other.transform.GetComponentInParent<MaterialPart>().transform.position + (this.transform.position-other.transform.position);

                // other.transform.GetComponentInParent<MaterialPart>().transform.rotation = this.transform.GetComponentInParent<MaterialPart>().transform.rotation;
                // other.transform.GetComponentInParent<MaterialPart>().transform.Rotate(0, 0, 270f);
                
                // other.transform.GetComponentInParent<MaterialPart>().transform.rotation = this.transform.GetComponentInParent<MaterialPart>().transform.rotation;
                // Debug.Log(other.transform.GetComponentInParent<MaterialPart>().transform.rotation);
                // new WaitForSeconds(4);
                // other.transform.GetComponentInParent<MaterialPart>().transform.Rotate(eulerAng.x, eulerAng.y, eulerAng.z);
                // Debug.Log(other.transform.GetComponentInParent<MaterialPart>().transform.rotation);

                // other.transform.GetComponentInParent<MaterialPart>().transform.rotation = Quaternion.Euler(eulerAng);
                

                // block.TransferChildrenTo(myParent.ParentBlock);
                GotGlueOn = false;
                //Destroy(this.gameObject);
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
