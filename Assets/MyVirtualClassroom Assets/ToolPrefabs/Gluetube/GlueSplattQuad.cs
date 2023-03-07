using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueSplattQuad : MonoBehaviour
{
    [SerializeField]
    public Material SplattMaterial;

    [SerializeField]
    public GameObject AssemblyParentPrefab;

    [SerializeField]
    public Transform VerticalSnapPoint, HorizontalSnapPoint;

    public BoxHitSide AtParentSide = BoxHitSide.NONE;

    private Vector3 myVerticalSnapPosition;
    private Vector3 myHorizontalSnapPosition;

    private Substance myParentSubstance;
    private Transform myParentTransform;

    // Start is called before the first frame update
    void Start()
    {
        CreateQuad();
        if(transform.parent != null)
        {
            myParentTransform = transform.parent.transform;
            myParentSubstance = GetComponentInParent<Substance>();
            SetSnapTransformOn(AtParentSide);
        }
    }

    // Update is called once per frame
    void Update()
    {
        RayHitSubstance();
    }

    public void SetSnapPosition(Vector3 aTubeMuzzlePos)
    {
        AtParentSide = ColliderTools.GetHitSide(transform.parent, aTubeMuzzlePos);
    }

    public Vector3 GetSnapPosition(bool isVertical)
    {
        return (isVertical ? myVerticalSnapPosition : myHorizontalSnapPosition);
    }

    private void CreateQuad()
    {
        float width = 1;
        float lenght = 1;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = SplattMaterial;

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(width * -.5f, 0, lenght * -.5f),
            new Vector3(width * .5f, 0,lenght * -.5f),
            new Vector3(width * -.5f, 0, lenght * .5f),
            new Vector3(width * .5f, 0, lenght * .5f)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            Vector3.up,
            Vector3.up,
            Vector3.up,
            Vector3.up
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        meshFilter.mesh = mesh;
    }

    private void RayHitSubstance()
    {
        if (Physics.Raycast(transform.position, transform.up, out RaycastHit hit, .001f))
        {
            if (hit.transform.CompareTag("Sliceable"))
            {
                if (hit.transform.parent == null)
                {
                    myParentSubstance.AttachingNewPart(hit.transform.GetComponent<Substance>(), transform);
                    //myParentSubstance.AttachingNewPart(hit, transform);
                    Destroy(this.gameObject);
                }
            }
        }
    }

    private void SetSnapTransformOn(BoxHitSide aParentHitSide)
    {
        if (aParentHitSide == BoxHitSide.RIGHT || aParentHitSide == BoxHitSide.LEFT)
        {
            transform.Rotate(new Vector3(0f, 0f, (aParentHitSide == BoxHitSide.RIGHT ? -90 : 90)));
            
            myVerticalSnapPosition = new Vector3(myParentTransform.position.x, transform.position.y, transform.position.z);
            myHorizontalSnapPosition = new Vector3(transform.position.x, transform.position.y, myParentTransform.position.z);
        }
        else if (aParentHitSide == BoxHitSide.TOP || aParentHitSide == BoxHitSide.BOTTOM)
        {
            transform.Rotate(new Vector3((aParentHitSide == BoxHitSide.TOP ? 0 : 180), 0f, 0f));

            myVerticalSnapPosition = new Vector3(transform.position.x, transform.position.y, myParentTransform.position.z);
            myHorizontalSnapPosition = new Vector3(myParentTransform.position.x, transform.position.y, transform.position.z);
        }
        else if (aParentHitSide == BoxHitSide.FRONT || aParentHitSide == BoxHitSide.BACK)
        {
            transform.Rotate(new Vector3((aParentHitSide == BoxHitSide.FRONT ? 90 : -90), 0f, 0f));

            myVerticalSnapPosition = new Vector3(transform.position.x, transform.position.y, myParentTransform.position.z);
            myHorizontalSnapPosition = new Vector3(myParentTransform.position.x, transform.position.y, transform.position.z);
        }

        VerticalSnapPoint.position = myVerticalSnapPosition;
        HorizontalSnapPoint.position = myHorizontalSnapPosition;
    }
}
