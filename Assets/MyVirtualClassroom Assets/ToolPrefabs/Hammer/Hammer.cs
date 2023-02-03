using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hammer : GrabableObject
{
    // by Senpai
    private AudioSource audioSource; //Local audiosource
    [SerializeField]
    private AudioClip hammerOnWood; //Audioclip when hitting something of wood

    private bool hasPlayedAudio = false; //To stop it from playing sfx twice
    //

    protected bool ourToolIsHolded = false;
    private Outline myOutline;

    private enum TouchMode { HAND, NONE }

    /*
        Vector3(0.647000015,1.06099999,0.621999979)
        Vector3(-1.49,11.991,0.436)
    */
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        /* Get the audiosource attached to this gameobject */
        audioSource = gameObject.GetComponent<AudioSource>();

        if (GetComponent<Outline>() != null)
            myOutline = GetComponent<Outline>();
        else if (GetComponentInChildren<Outline>() != null)
        {
            myOutline = GetComponentInChildren<Outline>();
        }
        //DrawOutline(TouchMode.HAND);

        //Vector3 pos = new Vector3(0.5728601f, 1.132392f, 0.4270033f) - new Vector3(0.5737f, 1.1085f, 0.3936f);
        //Vector3 rot = new Vector3(-9.012f, -9.992f, 4.286f) - new Vector3(14.663f, -21.328f, 1.11f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<MaterialInterface>() != null)
        {
            switch (collision.gameObject.GetComponent<MaterialInterface>().ReturnMaterialType())
            {
                case MaterialType.Wood:
                    audioSource.PlayOneShot(hammerOnWood);
                    hasPlayedAudio = true;
                    break;
                default:
                    break;

            }


        }

    }

    private void OnCollisionExit(Collision collision)
    {
        hasPlayedAudio = false;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    protected override void DrawOutline(int aModeIndex)
    {
        base.DrawOutline(aModeIndex);
    }
}
