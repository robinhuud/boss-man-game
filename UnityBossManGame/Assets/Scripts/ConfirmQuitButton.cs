//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//

using UnityEngine;

public class ConfirmQuitButton : MonoBehaviour, IDeviceControl {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void activate()
    {
        Debug.Log("Called activate on quit confirmation, quitting.");
        Application.Quit();
    }

    public bool isActive()
    {
        return true;
    }
}
