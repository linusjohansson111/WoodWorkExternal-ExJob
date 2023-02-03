using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkippingBox : MonoBehaviour
{
    [SerializeField]
    private Transform Lid;
    [SerializeField]
    private float SkippingSpeed = 1000f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        JumpingBoxPrank();
    }

    private void JumpingBoxPrank()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Lid.Rotate(new Vector3(1 * SkippingSpeed, 0f, 0f) * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Lid.Rotate(new Vector3(-1 * SkippingSpeed, 0f, 0f) * Time.deltaTime);
        }
    }
}
