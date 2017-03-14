﻿//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ButtonControl : MonoBehaviour, IDeviceControl {

    public Animator controlObject;
    public string trigger_name;
    private int trigger_hash;
    private bool isOpen;
    public AudioClip clickSound;
    AudioSource au;

	// Use this for initialization
	void Start () {
	    if(controlObject == null || string.IsNullOrEmpty(trigger_name))
        {
            Debug.Log("WTF! No controlObject found, or trigger name is empty.");
        }
        else
        {
            trigger_hash = Animator.StringToHash(trigger_name);
            isOpen = controlObject.GetBool(trigger_hash);
            //Debug.Log("isOpen is " + isOpen);
        }
        au = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void activate()
    {
        if(isOpen)
        {
            //Debug.Log("was open, setting open to false");
            controlObject.SetBool(trigger_hash, false);
        }
        else
        {
            //Debug.Log("was closed, setting open to true");
            controlObject.SetBool(trigger_hash, true);
        }
        isOpen = controlObject.GetBool(trigger_hash);
        au.PlayOneShot(clickSound,1f);
    }

    public bool isActive()
    {
        return isOpen;
    }
}
