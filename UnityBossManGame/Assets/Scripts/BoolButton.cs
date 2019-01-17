//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//
// This version of IDeviceControl is for buttons that toggle a boolean on an animator rather than triggering an event
// shouldnt need to trigger the lightbutton part because the caller shoudl do that.
// to trigger an event instead use IDC_AnimatorTrigger

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BoolButton : MonoBehaviour, IDeviceControl {

    public Animator controlObject;
    public string boolName;
    private int boolHash;
    private bool isOpen;
    public AudioClip clickSound;
    AudioSource au;

	// Use this for initialization
	void Start () {
	    if(controlObject == null || string.IsNullOrEmpty(boolName))
        {
            Debug.Log("WTF! No controlObject found, or trigger name is empty.");
        }
        else
        {
            boolHash = Animator.StringToHash(boolName);
            isOpen = controlObject.GetBool(boolHash);
            //Debug.Log("isOpen is " + isOpen);
        }
        au = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Activate()
    {
        controlObject.SetBool(boolHash, !controlObject.GetBool(boolHash));
        au.PlayOneShot(clickSound,1f);
    }

    public bool IsActive()
    {
        return isOpen;
    }
}
