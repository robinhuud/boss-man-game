//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//

using UnityEngine;
using System.Collections;

public class QuitButton : MonoBehaviour, IDeviceControl {

    private bool confirming = false;
    private float confirmDelay = 3f;
    public GameObject confirmObject;

    private IEnumerator confirmationTimer()
    {
        confirming = true;
        confirmObject.SetActive(true);
        yield return new WaitForSeconds(confirmDelay);
        confirmObject.SetActive(false);
        confirming = false;
    }

	// Use this for initialization
	void Start () {
        Debug.Assert(confirmObject != null, "confirmObject is null");
        confirmObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void activate()
    {
        //Debug.Log("QUITTING!!");
        if(!confirming)
        {
            //Debug.Log("Turning 'confirming' on");
            StartCoroutine(confirmationTimer());
        }
    }

    public bool isActive()
    {
        return confirming;
    }
}
