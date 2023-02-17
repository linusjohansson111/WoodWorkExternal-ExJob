using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaterialType
{
    Wood,
    Nail,
    Glue
}

interface MaterialInterface
{
    MaterialType ReturnMaterialType();

    public float Width { get; }

    public float Height { get; }

    public float Lenght { get; }

    public Vector3 TopPos { get; }

    public Vector3 BottomPos { get; }

    public Vector3 RightPos { get; }

    public Vector3 LeftPos { get; }

    public Vector3 FrontPos { get; }

    public Vector3 BackPos { get; }

    public void BeingHitByGluetubeRay(bool isHitByRay, string hitObjectName);
}
