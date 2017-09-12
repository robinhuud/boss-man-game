using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMessageToController : MonoBehaviour , IDeviceControl {

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

    public void activate()
    {
        active = !active;
        if(active || !toggle)
        {
            sceneController.react(this, commandString);
        }
    }

    public bool isActive()
    {
        return active;
    }
}
