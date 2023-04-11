using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoxHitSide { RIGHT, LEFT, TOP, BOTTOM, FRONT, REAR, NONE }

public class ColliderTools
{
    /// <summary>
    /// Thanks to Erik
    /// </summary>
    /// <param name="raycastHit"></param>
    /// <returns></returns>
    public static BoxHitSide GetSideOnRectangle(Mesh aMeshToCheck, RaycastHit aRayFromCaster)
    {
        for (var i = 0; i < aMeshToCheck.triangles.Length; i++)
        {
            if (aRayFromCaster.triangleIndex == i)
            {
                if (i == 0 || i == 1)
                    return BoxHitSide.FRONT;
                else if (i == 4 || i == 5)
                    return BoxHitSide.REAR;
                else if (i == 2 || i == 3)
                    return BoxHitSide.TOP;
                else if (i == 6 || i == 7)
                    return BoxHitSide.BOTTOM;
                else if (i == 8 || i == 9)
                    return BoxHitSide.LEFT;
                else if (i == 10 || i == 11)
                    return BoxHitSide.RIGHT;
                else
                    return BoxHitSide.NONE;
            }
        }

        return BoxHitSide.NONE;
    }

    public static BoxHitSide GetHitSide(Transform aObjectToBeHit, Vector3 aContactPoint)
    {
        Vector3 localPoint = aObjectToBeHit.InverseTransformPoint(aContactPoint) * 5;

        float boxHitDistance = 1f;
        
        if (Mathf.RoundToInt(localPoint.x) == boxHitDistance)
            return BoxHitSide.RIGHT;
        else
        if (Mathf.RoundToInt(localPoint.x) == -boxHitDistance)
            return BoxHitSide.LEFT;
        else
        if (Mathf.RoundToInt(localPoint.y) == boxHitDistance)
            return BoxHitSide.TOP;
        else
        if (Mathf.RoundToInt(localPoint.y) == -boxHitDistance)
            return BoxHitSide.BOTTOM;
        else
        if (Mathf.RoundToInt(localPoint.z) == boxHitDistance)
            return BoxHitSide.FRONT;
        else
        if (Mathf.RoundToInt(localPoint.z) == -boxHitDistance)
            return BoxHitSide.REAR;

        return BoxHitSide.NONE;
    }

    /// <summary>
    /// ref: https://answers.unity.com/questions/339532/how-can-i-detect-which-side-of-a-box-i-collided-wi.html
    /// </summary>
    /// <param name="anObj1"></param>
    /// <param name="anObj2"></param>
    /// <returns></returns>
    public static BoxHitSide GetHitSide(GameObject anObj1, GameObject anObj2)
    {
        //BoxHitSide hitSide = BoxHitSide.NONE;

        BoxHitSide hitDirection = BoxHitSide.NONE;
        RaycastHit MyRayHit;
        Vector3 direction = (anObj1.transform.position - anObj2.transform.position).normalized;
        Ray MyRay = new Ray(anObj2.transform.position, direction);

        if (Physics.Raycast(MyRay, out MyRayHit))
        {

            if (MyRayHit.collider != null)
            {

                Vector3 MyNormal = MyRayHit.normal;
                MyNormal = MyRayHit.transform.TransformDirection(MyNormal);

                if (MyNormal == MyRayHit.transform.up) { hitDirection = BoxHitSide.TOP; }
                if (MyNormal == -MyRayHit.transform.up) { hitDirection = BoxHitSide.BOTTOM; }
                if (MyNormal == MyRayHit.transform.forward) { hitDirection = BoxHitSide.FRONT; }
                if (MyNormal == -MyRayHit.transform.forward) { hitDirection = BoxHitSide.REAR; }
                if (MyNormal == MyRayHit.transform.right) { hitDirection = BoxHitSide.RIGHT; }
                if (MyNormal == -MyRayHit.transform.right) { hitDirection = BoxHitSide.LEFT; }
            }
        }
        return hitDirection;
    }

    /// <summary>
    /// Calculation of the bounds of the inserted object with childs
    /// It will return a new bounds data based the number of childs and their size in the
    /// parentObject
    /// </summary>
    /// <param name="parentObject">The parent object to be get a bounds data</param>
    /// <returns>The calculated bound data of the parentObject</returns>
    public static Bounds GetChildRendererBounds(GameObject parentObject)
    {
        Renderer[] childRenderes = parentObject.GetComponentsInChildren<Renderer>();
        if (childRenderes.Length > 0)
        {
            Bounds bounds = childRenderes[0].bounds;
            for (int i = 1, ni = childRenderes.Length; i < ni; i++)
                bounds.Encapsulate(childRenderes[i].bounds);
            return bounds;
        }
        else
            return new Bounds();
    }
}
