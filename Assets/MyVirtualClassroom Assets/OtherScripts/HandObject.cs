using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public enum Preposition { LEFT, RIGHT }
public class HandObject : MonoBehaviour
{
    [SerializeField]
    private Preposition HandSide;

    [SerializeField]
    protected InputActionProperty ActiveProperty;
    [SerializeField]
    protected InputActionProperty GrabProperty;

    [SerializeField]
    protected InputActionProperty ButtonPrimary;
    [SerializeField]
    protected InputActionProperty ButtonSecondary;


    [SerializeField]
    private float SphereCastRadius = .5f;

    [HideInInspector]
    public Preposition Side { get { return HandSide; } }

    [HideInInspector]
    public bool IsGrapping { get; private set; }

    public bool IsActivePressed { get { return ActiveProperty.action.IsPressed(); } }
    public bool IsGrabPressed { get { return GrabProperty.action.IsPressed(); } }

    public bool IsButtonPrimaryPressed { get { return ButtonPrimary.action.IsPressed(); } }
    public bool IsButtonSecondaryPressed { get { return ButtonSecondary.action.IsPressed(); } }
    
    // public Vector2 JoyStickValue { get { return JoyStickProperty.action?.ReadValue<Vector2>() ?? Vector2.zero; } }



    private SphereCollider mySC;

    private int mySubstanceLayer = 0;

    // Start is called before the first frame update
    void Start()
    {
        mySC = GetComponent<SphereCollider>();
        mySubstanceLayer = LayerMask.GetMask("Substance");

    }

    // Update is called once per frame
    void Update()
    {

        // Debug.Log(IsAPressed);
        // Debug.Log(IsBPressed);
        if (Physics.SphereCast(transform.position, SphereCastRadius, (HandSide == Preposition.LEFT ? transform.right : -transform.right), out RaycastHit hit, .02f, mySubstanceLayer))
        {
            hit.collider.transform.GetComponent<MaterialPart>().MoveAttachPointTo(HandSide, hit.point);
        }
        //if (Physics.Raycast(transform.position, (HandSide == Preposition.LEFT ? transform.right : -transform.right), out RaycastHit hit, .1f, mySubstanceLayer))
        //{
        //    hit.collider.transform.GetComponent<MaterialPart>().MoveAttachPointTo(HandSide, hit.point);
        //}
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SphereCastRadius);
        //Gizmos.DrawLine(transform.position, transform.position + ((HandSide == Preposition.LEFT ? transform.right : -transform.right) * .1f));
    }

    public void IsGrapingObject(bool isGraping)
    {
        IsGrapping = isGraping;
        mySC.enabled = !isGraping;
    }
}
