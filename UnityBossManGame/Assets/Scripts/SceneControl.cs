//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//

// this script does only one thing, it forwards the message that the door 
// opened to the employeeBehavior instance referenced in the public variable

using UnityEngine;
using UnityEngine.AI;

public class SceneControl : MonoBehaviour {

    public employeeBehavior employeeScript;
    public EmployeeConversation conversationScript;
    public bool isInHall = true;

    // This method is called from the editor cog menu, useful for attaching
    // serializable fileds with default values.
    void Reset()
    {
    }

    // Use this for initialization
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
    }

    // Need to define an interface for this?? WTF??
    public void react(IDeviceControl caller, string command)
    {
        switch(command)
        {
            case "door":
                employeeScript.DoorOpened();
                conversationScript.DoorOpened();
                break;
            case "trapdoor":
                break;
            case "howmake":
            case "whatplace":
            default:
                conversationScript.AskedQuestion(command);
                break;
        }
    }
}
