using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlueSplatt : MonoBehaviour
{
    [SerializeField]
    public GameObject AssemblyParentPrefab;

    private bool myIsAttachingOtherSubstance = false;
    private bool myCreateNewProduct = false;

    private bool tempBool = false;
    private AssembledProduct myNewAssembyProductParent;
    private Transform myAddingSubstanceTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //RayHitSubstance();

        //if(myCreateNewProduct && !tempBool)
        //{
        //    myNewAssembyProductParent.AddNewPart(transform.parent.transform.GetComponent<Substance>());
        //    //myCreateNewProduct = false;
        //    tempBool = true;
        //}

        //if(Input.GetKey(KeyCode.Space) && !myCreateNewProduct)
        //{
        //    myNewAssembyProductParent = Instantiate(AssemblyParentPrefab.gameObject, transform.parent.transform.position, transform.parent.transform.rotation).GetComponent<AssembledProduct>();
        //    //Instantiate(transform.parent.gameObject, transform.parent.position, transform.parent.rotation, myNewAssembyProductParent.transform);
        //    myCreateNewProduct = true;
        //}
    }

    private void LateUpdate()
    {
        //CreateNewAssamblyProduct();
    }

    /// <summary>
    /// by senpais
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Sliceable")
        {
            //Collider myCollider = collision.contacts[1].thisCollider;
            //Debug.Log("LIMMMMM");
            //collision.gameObject.GetComponent<Substance>().SetWoodToKinematic(true);
            //collision.gameObject.transform.parent = gameObject.transform.parent;
            collision.transform.GetComponent<Substance>().AttachToGlue(transform);
            //rb.transform.parent = collision.transform;
        }
    }

    private void RayHitSubstance()
    {
        if (myIsAttachingOtherSubstance)
            return;

        //if(Physics.Raycast(transform.position, transform.up, out RaycastHit hit, .001f))
        //{
        //    if(hit.transform.CompareTag("Sliceable"))
        //    {
        //        if (transform.parent.parent == null)
        //        {
        //            //transform.parent.parent = Instantiate(AssemblyParentPrefab.gameObject, transform.position, Quaternion.identity).transform;
        //            //myNewAssembyProductParent = Instantiate(AssemblyParentPrefab.gameObject, transform.parent.position, transform.parent.rotation).GetComponent<AssembledProduct>();
        //            //myAddingSubstanceTransform = hit.transform;
        //            //myCreateNewProduct = true;
        //        }
        //        //else
        //        //    AttachObject(hit.transform);
        //    }
        //}
    }

    private void CreateNewAssamblyProduct()
    {
        if (!myCreateNewProduct)
            return;

        //Instantiate(transform.parent.gameObject, transform.parent.transform.position, transform.parent.rotation, myNewAssembyProductParent.transform);
        //myNewAssembyProductParent.AddNewPart(transform.parent.gameObject.GetComponent<Substance>());
        //Destroy(transform.parent.gameObject);

        //AttachObject(myAddingSubstanceTransform);

        //myCreateNewProduct = false;
    }

    private void AttachObject(Transform anAttachingObject)
    {
        Instantiate(anAttachingObject.gameObject, transform.position + (Vector3.up * 0.5f), Quaternion.identity, myNewAssembyProductParent.transform);
        Destroy(anAttachingObject.gameObject);
        //transform.parent.GetComponent<Substance>().AttachingNewPart(anAttachingObject);
    }

    private Substance CreateCloneSubstanceToProduct(Transform aSubstanceTransform, Transform aProductParentTransform)
    {
        return Instantiate(aSubstanceTransform.gameObject, aSubstanceTransform.position, aSubstanceTransform.rotation, aProductParentTransform).GetComponent<Substance>();
    }
}
