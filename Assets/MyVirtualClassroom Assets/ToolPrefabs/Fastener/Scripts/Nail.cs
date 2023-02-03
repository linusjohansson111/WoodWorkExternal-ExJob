using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Nail : MonoBehaviour, ICFasteners
{
    Transform transform;
    Rigidbody rigidbody;

    //Variables
    bool isBeingHeld = false;
    bool isKinematic = false;
    bool isOnWood = false;
    bool isAttached = false;
    Vector3 localScale;
    int nrOfWoodsHammered = 0;

    /// <summary>
    /// The parent object the fastern will have as parent
    /// </summary>
    private GameObject myAttachToGO;

    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
        localScale = transform.localScale;
        Debug.Log(localScale);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddForceToObject(float aVelocity)
    {
        aVelocity = -1f * 0.3f * Time.deltaTime;
        transform.position += Vector3.up * aVelocity;
    }

    public void SetIsBeingHeld(bool isHeld)
    {
        isBeingHeld = isHeld;
        Debug.Log("SetIsBeingHeld got " + isHeld);
        if (isBeingHeld == false && isOnWood)
        {
            rigidbody.isKinematic = true;

            transform.parent = myAttachToGO.transform;
            transform.localScale = localScale;

            /*GameObject newGameObject = new GameObject();
            newGameObject.transform.parent = myAttachToGO.transform;
            transform.parent = newGameObject.transform;*/
        }
        else
        {
            rigidbody.isKinematic = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Sliceable")
        {
            isOnWood = true;
            nrOfWoodsHammered += 1;
            myAttachToGO = collision.gameObject;
            myAttachToGO.GetComponent<Substance>().SetWoodToKinematic(true);
            if (nrOfWoodsHammered >= 2)
            {
                myAttachToGO.transform.parent = gameObject.transform.parent;
            }
        }

        if (collision.gameObject.tag == "Hammer" && isOnWood == true)
        {
            AddForceToObject(2f);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Sliceable")
        {
            isOnWood = false;
            Debug.Log("isOnWood is " + isOnWood);
            myAttachToGO = null;
        }

        if (collision.gameObject.tag == "Hammer" && isAttached == false)
        {
            AddForceToObject(2f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Sliceable")
        {
            isAttached = true;
            GetComponent<XRGrabInteractable>().interactionLayerMask = 0;
        }
    }
}
