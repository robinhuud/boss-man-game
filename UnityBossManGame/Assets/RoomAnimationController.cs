using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomAnimationController : MonoBehaviour {

    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    public Animator animator; // The Animator which controls the animation of the employee
    public AudioSource audioSource;

    // Use this for initialization
    void Start () {
        animator = GetComponentInChildren<Animator>();
        audioSource = animator.gameObject.GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // Animation Events
    void Contact () // called when the door open animation is at contact point
    {
        //Debug.Log("Contact");
        if(animator.GetBool("open_door")) // if the animation is playing backwards, we play the close sound instead of the open sound.
        {
            audioSource.PlayOneShot(doorOpenSound);
        }
        else
        {
            audioSource.PlayOneShot(doorCloseSound);
        }
    }
}
