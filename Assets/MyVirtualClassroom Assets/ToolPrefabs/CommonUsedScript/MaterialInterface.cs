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

    public void BeingHitByGluetubeRay(bool isHitByRay, string hitObjectName);
}
