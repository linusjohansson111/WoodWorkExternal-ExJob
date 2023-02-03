using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Preposition { LEFT, RIGHT }
public class HandObject : MonoBehaviour
{
    [SerializeField]
    private Preposition HandSide;

    [HideInInspector]
    public Preposition Side { get { return HandSide; } }

    [HideInInspector]
    public bool IsGrapping { get; private set; }

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
