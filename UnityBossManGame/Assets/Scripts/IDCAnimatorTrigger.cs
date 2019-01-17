//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//


// This is an extension of IDeviceControl, (The base class that enables
// the gaze-selection methods of "activate()" and "isActive()"
// This version is a oneshot version that takes an animator, and calls a "trigger"

using UnityEngine;

public class IDCAnimatorTrigger : MonoBehaviour, IDeviceControl
{
    public Animator controlObject;
    public string trigger_name;
    private int trigger_hash;

    // Use this for initialization
    void Start () {
        if (controlObject == null || string.IsNullOrEmpty(trigger_name))
        {
            Debug.Log("WTF! No controlObject found, or trigger name is empty.");
        }
        else
        {
            trigger_hash = Animator.StringToHash(trigger_name);
            //Debug.Log("isOpen is " + isOpen);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Activate()
    {
        controlObject.SetTrigger(trigger_hash);
    }

    public bool IsActive()
    {
        return false;
    }
}
