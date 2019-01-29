using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Class for managing question buttons, Start out hidden, have the ability to change questions and send messages to the scene controller
public class IDCQuestionButton : MonoBehaviour, IDeviceControl {

    [Tooltip("link to the scenecontroller in the scene")]
    public SceneControl sceneController;
    [Tooltip("Set to true if this is a toggling button (only send command if activating, not deactivating")]
    public bool toggle = false;

    private bool active = false;
    private string commandString = "";

	// Use this for initialization
	void Start () {
        Hide();
    }
	
    public void ShowQuestion(Texture tex, string command)
    {
        Show();
        GetComponent<Renderer>().material.SetTexture("_MainTex", tex);
        commandString = command;
    }

    public void Hide()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        active = false;
    }

    public void Show()
    {
        GetComponent<Collider>().enabled = true;
        GetComponent<Renderer>().enabled = true;
        active = true;
    }

    public string GetCommand()
    {
        return commandString;
    }

    public void Activate()
    {
        active = !active;
        if (active || !toggle)
        {
            sceneController.react(this, commandString);
        }
    }

    public bool IsActive()
    {
        return active;
    }
}
