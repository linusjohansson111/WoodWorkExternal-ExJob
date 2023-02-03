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

    public void BeingHitByGluetubeRay(bool isHitByRay, string hitObjectName);
}
