﻿//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//

////
// Simple prototype activation makes the material glow when active
// color is specified in glow color attribute
////

using UnityEngine;

public class IDCLightButton : MonoBehaviour, IDeviceControl {

    private bool isOn; // am I currently illuminated?
    private Material myMat; // the instance of the material that I am using
    public Color glowColor = new Color(1f, .05f, .05f);
	// Use this for runtime initialization
	void Start () {
        isOn = false;
        myMat = GetComponent<Renderer>().material;
        // this little bit of insanity is lazy coding, just grabs a object of type SceneControl out of my parent, no check
        // to see if it's valid, nothing. (bad programer)
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Activate()
    {
        isOn = !isOn;
        if(isOn)
        {
            myMat.SetColor("_Emission", glowColor);
        }
        else
        {
            myMat.SetColor("_Emission", new Color(0f, 0f, 0f));
        }
    }

    public bool IsActive()
    {
        return isOn;
    }
}
