//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DrawerControl : MonoBehaviour, IDeviceControl {
    public Animator controlObject;
    public string trigger_name;
    private bool isOpen;
    public AudioClip openSound;
    public AudioClip closeSound;
    AudioSource au;

    // Use this for initialization
    void Start () {
        if (controlObject == null || string.IsNullOrEmpty(trigger_name))
        {
            Debug.Log("WTF! No controlObject found, or trigger name is empty.");
        }
        else
        {
            isOpen = controlObject.GetBool(trigger_name);
            //Debug.Log("isOpen is " + isOpen);
        }
        au = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void activate()
    {
        if (isOpen)
        {
            //Debug.Log("was open, setting open to false");
            controlObject.SetBool(trigger_name, false);
            au.PlayOneShot(closeSound, 1f);
        }
        else
        {
            //Debug.Log("was closed, setting open to true");
            controlObject.SetBool(trigger_name, true);
            au.PlayOneShot(openSound, 1f);
        }
        isOpen = controlObject.GetBool(trigger_name);
    }

    public bool isActive()
    {
        return isOpen;
    }
}
