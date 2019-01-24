using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to send a generic message from IDC gaze-activated object to
/// the scene controller used in this project, in this case, it can send a string
/// I could have defined another interface, but I'm not super-stoked about 
/// the whole string-based command interface anyway, so I won't bother.
/// </summary>

public class IDCSendMessageToController : MonoBehaviour , IDeviceControl {

    [Tooltip("Pointer to the SceneControl object that processes these messages with it's react(IDeviceControl,string) method")]
    public SceneControl sceneController;
    [Tooltip("string to send to the controller's react() method")]
    public string commandString = "";
    [Tooltip("Set to true if this is a toggling button (only send command if activating, not deactivating")]
    public bool toggle = false;

    private bool active = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate()
    {
        active = !active;
        if(active || !toggle)
        {
            sceneController.react(this, commandString);
        }
    }

    public bool IsActive()
    {
        return active;
    }
}
