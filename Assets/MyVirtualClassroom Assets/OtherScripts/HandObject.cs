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

    [HideInInspector]
    public Preposition Side { get { return HandSide; } }

    [HideInInspector]
    public bool IsGrapping { get; private set; }

    public bool IsActivePressed { get { return ActiveProperty.action.IsPressed(); } }
    public bool IsGrabPressed { get { return GrabProperty.action.IsPressed(); } }

    private SphereCollider mySC;

    // Start is called before the first frame update
    void Start()
    {
        mySC = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IsGrapingObject(bool isGraping)
    {
        IsGrapping = isGraping;
        mySC.enabled = !isGraping;
    }
}
