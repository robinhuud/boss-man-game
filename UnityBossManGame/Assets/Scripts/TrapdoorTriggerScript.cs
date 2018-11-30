using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapdoorTriggerScript : MonoBehaviour {

    Animator targetAnimator;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider source)
    {
        if(targetAnimator == null)
        {
            targetAnimator = source.GetComponentInParent<Animator>();
        }
        targetAnimator.SetBool("can_fall", true);
    }

    void OnTriggerExit(Collider source)
    {
        targetAnimator.SetBool("can_fall", false);
    }
}
