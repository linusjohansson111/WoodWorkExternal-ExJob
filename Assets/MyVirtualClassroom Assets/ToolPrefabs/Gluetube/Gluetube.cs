using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Gluetube;

public class Gluetube : GrabbingTool
{
    [SerializeField]
    private Transform Muzzle;

    [SerializeField]
    private GameObject Splatter;

    public delegate void GlueTubeHold();
    public static GlueTubeHold glueTubeHold;

    //Transform transform;
    //Rigidbody rigidbody;
    //Vector3 localScale;

    public delegate void GluetubeRayHitObject(bool isHitByRay, string hitObjectName);
    public static GluetubeRayHitObject OnGluetubeRayHitObject;
    public float glueSize = 10f;

    private GameObject myRayHitObject;
    private string myRayHitObjectName = "";

    private bool myGlueWasClicked = false;

    private int myLayerMask = 0;

    private bool myIsHitMaterialPart = false;
    private MaterialPart myHitMaterial;
    private GlueSnapArea mySnapArea;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //transform = GetComponent<Transform>();
        //rigidbody = GetComponent<Rigidbody>();
        //localScale = transform.localScale;
        myLayerMask = LayerMask.NameToLayer("Glue");
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        

        if (ourIsHolding)
        {
            //HasRayHitTarget();
            //HasSphereCastHitTarget();

            //if (myGlueWasClicked)
            //    myGlueWasClicked = ourGrabbingHand.IsActivePressed;
            if (myIsHitMaterialPart && Physics.Raycast(Muzzle.position, Muzzle.up, out RaycastHit hit, .05f))
            {
                if (ourGrabbingHand.IsActivePressed)
                {
                    CreateGlueDot(hit.point, mySnapArea.transform);
                    mySnapArea.ThisAreaIsGlued();
                }
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.gameObject.layer == LayerMask.NameToLayer("Glue"))
        {
            InfoCanvas.Ins.DisplayAboveObjectInfo(other.name);
            DrawOutline(TouchTag.OTHER);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (other.gameObject.layer == LayerMask.NameToLayer("Glue"))
        {
            InfoCanvas.Ins.DisplayAboveObjectInfo("");
        }
    }

    public void ActiveGlueTube(bool isActive, GlueSnapArea aGlueArea)
    {
        myIsHitMaterialPart = isActive;
        mySnapArea = aGlueArea;
    }

    private void HasRayHitTarget()
    {
        if (Physics.Raycast(Muzzle.position, Muzzle.up, out RaycastHit hit, .05f))
        {
            DrawRaycastingObjectOutline(true, hit.collider.transform.name);
            
            if (hit.collider.transform.CompareTag("Sliceable") && ClickOutGlue())
            {
                CreateGlueDot(hit.point, hit.collider.transform);
            }
            else if (hit.collider.transform.CompareTag("Glue"))
            {
            }
        }
        else
        {
            DrawRaycastingObjectOutline(false, myRayHitObjectName);
        }
    }

    private void HasSphereCastHitTarget()
    {
        if(Physics.Raycast(Muzzle.position/*, .01f*/, Muzzle.up, out RaycastHit hit, .05f))
        {
            if(hit.collider.gameObject.layer == 8)
            { 
                myHitMaterial = hit.collider.transform.GetComponent<MaterialPart>();
                myIsHitMaterialPart = true; 

                if(ClickOutGlue())
                    CreateGlueDot(hit.point, hit.collider.transform);
            }
        }
        else
            myIsHitMaterialPart = false;

        if(myIsHitMaterialPart)
            myHitMaterial.DrawGlueTupeOutline(true);
        else if(!myIsHitMaterialPart && myHitMaterial != null)
        {
            myHitMaterial.DrawGlueTupeOutline(false);
            myHitMaterial = null;
        }
    }

    /// <summary>
    /// 
    /// OBS this shall be removed later
    /// 
    /// Calling the object that had subscribe to the delegate function OnGluetubeRayHitObject
    /// to draw their outline of being hit by the gluetube's ray.
    /// If the subscribe object is hit, it'll send the name of the hitting object
    /// and when the subscribe object recieve the call and confirm the sending name
    /// it's its name, the object will active the outline and deactive when the ray is
    /// not hitting it
    /// </summary>
    /// <param name="isHitting">Tell if the ray is hitting object</param>
    /// <param name="hitObjectName">The name of the object the ray hit</param>
    private void DrawRaycastingObjectOutline(bool isHitting, string hitObjectName)
    {
        if(isHitting)
        {
            if (myRayHitObjectName != hitObjectName)
            {
                myRayHitObjectName = hitObjectName;
                OnGluetubeRayHitObject?.Invoke(true, myRayHitObjectName);
            }
        }
        else
        {
            if (myRayHitObjectName != "")
            {
                OnGluetubeRayHitObject?.Invoke(false, myRayHitObjectName);
                myRayHitObjectName = "";
            }
        }

    }

    /// <summary>
    /// This function is use to create a glue splatter child object at the object 
    /// the the ray cast by the gluetube's muzzle hit on.
    /// </summary>
    /// <param name="aSurfacePoint">The surface position on the ray hitting object</param>
    /// <param name="aParentTransform">The object the splatter will be child to</param>
    private void CreateGlueDot(Vector3 aSurfacePoint, Transform aParentTransform)
    {
        //Instantiate(Splatter, aSurfacePoint, aParentTransform.rotation, aParentTransform).GetComponent<GlueSplattQuad>().SetSnapPosition(aSurfacePoint);
        //GameObject newGlue = Instantiate(Splatter, aParentTransform, true);
        //newGlue.GetComponent<GlueSplattQuad>().SetSnapPosition(aSurfacePoint);
        //newGlue.transform.position = aSurfacePoint;
        GameObject newGlue = Instantiate(Splatter, aSurfacePoint, aParentTransform.rotation);
        BoxHitSide side = ColliderTools.GetHitSide(aParentTransform, aSurfacePoint);
        newGlue.GetComponent<GlueSplattQuad>().RotateGlueOnParentFace(side);
        newGlue.transform.parent = aParentTransform;

    }

    private bool ClickOutGlue()
    {
        if (myGlueWasClicked)
            return false;

        return myGlueWasClicked = ourGrabbingHand.IsActivePressed;
    }

    public override void GrabbObject(HandObject aGrabbingHandObject)
    {
        glueTubeHold?.Invoke();
        base.GrabbObject(aGrabbingHandObject);
    }

    public override void DroppObject()
    {
        glueTubeHold?.Invoke();
        base.DroppingObject();
    }
}
