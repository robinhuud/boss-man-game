//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//

using UnityEngine;

public class LightButton : MonoBehaviour, IDeviceControl {

    private bool isOn; // am I currently illuminated?
    private Material myMat; // the instance of the material that I am using
    private SceneControl parentScript;
    public Color glowColor = new Color(1f, .05f, .05f);
	// Use this for runtime initialization
	void Start () {
        isOn = false;
        myMat = GetComponent<Renderer>().material;
        // this little bit of insanity is lazy coding, just grabs a object of type SceneControl out of my parent, no check
        // to see if it's valid, nothing. (bad programer)
        parentScript = GetComponentInParent<SceneControl>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void activate()
    {
        isOn = !isOn;
        if(isOn)
        {
            myMat.SetColor("_Emission", glowColor);
            parentScript.react(this);
        }
        else
        {
            myMat.SetColor("_Emission", new Color(0f, 0f, 0f));
            parentScript.react(this);
        }
    }

    public bool isActive()
    {
        return isOn;
    }
}
