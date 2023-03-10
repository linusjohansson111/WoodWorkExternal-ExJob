using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hammer : GrabbingTool
{
    // by Senpai
    private AudioSource audioSource; //Local audiosource
    [SerializeField]
    private AudioClip hammerOnWood; //Audioclip when hitting something of wood

    private bool hasPlayedAudio = false; //To stop it from playing sfx twice
    //

    //protected bool ourToolIsHolded = false;
    //private Outline myOutline;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        /* Get the audiosource attached to this gameobject */
        audioSource = gameObject.GetComponent<AudioSource>();
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
