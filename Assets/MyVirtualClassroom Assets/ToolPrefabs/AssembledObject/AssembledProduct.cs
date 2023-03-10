using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssembledProduct : GrabableObject
{
    // Start is called before the first frame update
    protected override void Start()
    {
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("WorkStation"))
            ourRB.isKinematic = true;
    }

    /// <summary>
    /// Add in a new substance object among the assembled child to this object
    /// </summary>
    /// <param name="aNewPart">The substance object to be added</param>
    public void AddNewPart(Substance aNewPart)
    {
        aNewPart.PutIntoAssamblyParent(this);
    }
}
