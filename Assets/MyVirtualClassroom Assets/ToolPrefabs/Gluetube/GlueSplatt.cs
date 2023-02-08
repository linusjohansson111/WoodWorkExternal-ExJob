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

    private AssembledProduct myNewAssembyProductParent;
    private Transform myAddingSubstanceTransform;

    private BoxCollider myBC;

    // Start is called before the first frame update
    void Start()
    {
        myBC = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        RayHitSubstance();
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
            //rb.transform.parent = collision.transform;

            //pointer.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            
            //collision.transform.rotation = Quaternion.FromToRotation(transform.up, collision.transform.up);
            //transform.parent.GetComponent<Substance>().AttachingNewPart(collision.transform.GetComponent<Substance>());
            //if(transform.parent.parent == null)
            //{
            //    transform.parent.parent = Instantiate(AssemblyParentPrefab.gameObject, transform.parent.position, transform.parent.rotation).transform;
            //}
            //collision.transform.GetComponent<Substance>().AttachToGlue(transform);


        }
    }

    private void RayHitSubstance()
    {
        if (myIsAttachingOtherSubstance)
            return;

        if (Physics.Raycast(transform.position, transform.up, out RaycastHit hit, .001f))
        {
            if (hit.transform.CompareTag("Sliceable"))
            {
                transform.parent.GetComponent<Substance>().AttachingNewPart(hit.transform.GetComponent<Substance>(), transform);
                myIsAttachingOtherSubstance = true;
            }
        }
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
