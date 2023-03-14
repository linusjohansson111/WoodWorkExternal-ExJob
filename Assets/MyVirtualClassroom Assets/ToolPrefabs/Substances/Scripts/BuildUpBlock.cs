using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildUpBlock : GrabableObject
{
    private List<MaterialPart> myParts = new List<MaterialPart>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        foreach(Transform child in transform)
        {
            if (child.GetComponent<MaterialPart>() != null)
                myParts.Add(child.GetComponent<MaterialPart>());
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void AddPart(MaterialPart aNewPart)
    {
        myParts.Add(aNewPart);
    }

    public void GetChildAttachPoint(Transform anAttachPoint)
    {
        ourXRGrab.SetOptionalAttachPoint(anAttachPoint);
    }

    public void NullifyChildAttachPoint()
    {
        ourXRGrab.ResetOptionalAttachPoint();
    }

    public Color GetOutlineColorFor(int anOutlineIndex)
    {
        return GetColorFor(anOutlineIndex);
    }

    public Color GetOutlineColorFor(TouchTag aTouchTag)
    {
        return GetColorFor(aTouchTag);
    }
}
