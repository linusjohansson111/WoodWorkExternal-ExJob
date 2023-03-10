using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FastenerSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform Lid;

    [SerializeField]
    private GameObject Fastener;

    [SerializeField]
    private float LidOpenSpeed = 1;

    [SerializeField]
    private float myLidAngle = 0f;
    private float myLidDirection  = -1f;

    private bool myLidIsMoving = false;

    private const float OPENED_ANGLE = 100f;

    private HandObject myGrapHand;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(myLidIsMoving)
        {
            myLidAngle += myLidDirection * LidOpenSpeed * Time.deltaTime;
            if (myLidAngle > OPENED_ANGLE || myLidAngle < 0f)
            {
                myLidIsMoving = false;
                if(myLidDirection == 1f)
                    myLidAngle = OPENED_ANGLE;
                else
                    myLidAngle = 0f;
            }
            Lid.eulerAngles = new Vector3(myLidAngle, 0, 0);
        }

        //if(myHandIsTouching && !myHandIsHolding)
        //{
        //    if (myGrapHand.IsGrabPressed)
        //    {
        //        Instantiate(Fastener, transform.position, Quaternion.identity);
        //    }
        //}
        //if(Input.GetKey(KeyCode.Space)/* && !myLidIsMoving*/)
        //{
        //    myLidIsMoving = true;
        //    myLidDirection *= -1;
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.transform.CompareTag("WorkStation"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Hand"))
        {
            OpenLid();

            if (myGrapHand == null)
                myGrapHand = other.transform.GetComponent<HandObject>();

            if (myGrapHand.IsGrabPressed && !myGrapHand.IsGrapping)
            {
                Instantiate(Fastener, transform.position, Quaternion.identity);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Hand"))
        {
            CloseLid();

            if(myGrapHand != null)
               myGrapHand = null;
        }
    }


    private void OpenLid()
    {
        myLidIsMoving = true;
        myLidDirection = 1f;
    }

    private void CloseLid()
    {
        myLidIsMoving = true;
        myLidDirection = -1f;
    }

    private bool IsHandHoldingFastener()
    {
        if (myGrapHand == null)
            return true;

        return myGrapHand.IsGrapping;
    }
}
