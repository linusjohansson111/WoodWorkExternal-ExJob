using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Gluetube;

public class Gluetube : GrabableObject
{
    [SerializeField]
    private Transform Muzzle;

    [SerializeField]
    private GameObject Splatter;

    Transform transform;
    Rigidbody rigidbody;
    Vector3 localScale;

    public delegate void GluetubeRayHitObject(bool isHitByRay, string hitObjectName);
    public static GluetubeRayHitObject OnGluetubeRayHitObject;
    public float glueSize = 10f;

    private GameObject myRayHitObject;
    private string myRayHitObjectName = "";

    private bool myGlueWasClicked = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
        localScale = transform.localScale;

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (ourHandIsHolding)
        {
            HasRayHitTarget();

            if(myGlueWasClicked)
                myGlueWasClicked = IsActivePressed();
        }
    }

    protected override void LateUpdate()
    {
        //base.LateUpdate();
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

    /// <summary>
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
        Instantiate(Splatter, aSurfacePoint, aParentTransform.rotation, aParentTransform).GetComponent<GlueSplattQuad>().SetSnapPosition(Muzzle.position);
        
    }

    protected override void DrawOutline(int aModeIndex)
    {
        base.DrawOutline(aModeIndex);

        if(aModeIndex == 2)
        {
            SetOutlineAppearence(Outline.Mode.OutlineAll, Color.yellow);
        }

        if(aModeIndex == 3)
            SetOutlineAppearence(Outline.Mode.OutlineAll, Color.red);
        if (aModeIndex == 4)
            SetOutlineAppearence(Outline.Mode.OutlineAll, Color.green);
    }

    private bool ClickOutGlue()
    {
        if (myGlueWasClicked)
            return false;

        return myGlueWasClicked = IsActivePressed();
    }
}
