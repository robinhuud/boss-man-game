//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//

using UnityEngine;

public class SceneControl : MonoBehaviour {

    public GameObject[] buttons;
    public Transform baseLayer;
    private LightButton[] scripts;
    private ParticleSystem[] spots;
    private bool spinX = false;
    private bool spinY = false;
    private bool spinZ = false;
    private bool scaleX = false;
    private bool scaleY = false;
    private bool scaleZ = false;
    private bool moveX = false;
    private bool moveY = false;
    private bool moveZ = false;

    // Use this for initialization
    void Start () {
        scripts = new LightButton[buttons.Length];
        int index = 0;
		foreach(GameObject go in buttons)
        {
            if(go.GetComponent<LightButton>() != null)
            {
                scripts[index++] = go.GetComponent<LightButton>();
            }
        }
        spots = baseLayer.GetComponentsInChildren<ParticleSystem>();
        //Debug.Log("Length of scrtipts aray is " + scripts.Length);
	}
	
	// Update is called once per frame
	void Update () {
		if (spinX)
        {
            baseLayer.Rotate(new Vector3(1, 0, 0), 5f);
        }
        if (spinY)
        {
            baseLayer.GetChild(0).Rotate(new Vector3(0, 1, 0), 7f);
        }
        if (spinZ)
        {
            baseLayer.GetChild(0).GetChild(0).Rotate(new Vector3(0, 0, 1), 9f);
        }
        if (scaleX)
        {
            baseLayer.localScale = new Vector3(Mathf.Cos(Time.time), 1f, 1f);
        }
        if(scaleY)
        {
            baseLayer.GetChild(0).localScale = new Vector3(1f, Mathf.Cos(Time.time * 1.37f), 1f);
        }
        if(scaleZ)
        {
            baseLayer.GetChild(0).GetChild(0).localScale = new Vector3(1f, 1f, Mathf.Cos(Time.time * 1.73f));
        }
        if (moveX)
        {
            baseLayer.Translate(new Vector3(.01f * Mathf.Cos(Time.time * 1.37f), 0f, 0f));
        }
        if (moveY)
        {
            baseLayer.Translate(new Vector3(0f, .01f * Mathf.Cos(Time.time), 0f));
        }
        if (moveZ)
        {
            baseLayer.Translate(new Vector3(0f, 0f, .01f * Mathf.Cos(Time.time * 1.73f)));
        }
    }

    public void react(LightButton caller)
    {
        foreach(LightButton idc in scripts)
        {
            if(idc == caller) // this button just got pressed
            {
                if(idc.isActive()) // just became active?
                {
                    /// ACTIVATIONS
                    switch(idc.name)
                    {
                        case "CB01":
                            spots[0].Play();
                            break;
                        case "CB02":
                            spots[1].Play();
                            break;
                        case "CB03":
                            spots[2].Play();
                            break;
                        case "CB04":
                            spots[3].Play();
                            break;
                        case "CB05":
                            spots[4].Play();
                            break;
                        case "CB06":
                            spots[5].Play();
                            break;
                        case "CB07":
                            spinX = true;
                            break;
                        case "CB08":
                            spinY = true;
                            break;
                        case "CB09":
                            spinZ = true;
                            break;
                        case "CB10":
                            scaleX = true;
                            break;
                        case "CB11":
                            scaleY = true;
                            break;
                        case "CB12":
                            scaleZ = true;
                            break;
                        case "CB13":
                            moveX = true;
                            break;
                        case "CB14":
                            moveY = true;
                            break;
                        case "CB15":
                            moveZ = true;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    /// DEACTIVATIONS
                    switch (idc.name)
                    {
                        case "CB01":
                            spots[0].Stop();
                            break;
                        case "CB02":
                            spots[1].Stop();
                            break;
                        case "CB03":
                            spots[2].Stop();
                            break;
                        case "CB04":
                            spots[3].Stop();
                            break;
                        case "CB05":
                            spots[4].Stop();
                            break;
                        case "CB06":
                            spots[5].Stop();
                            break;
                        case "CB07":
                            spinX = false;
                            break;
                        case "CB08":
                            spinY = false;
                            break;
                        case "CB09":
                            spinZ = false;
                            break;
                        case "CB10":
                            scaleX = false;
                            break;
                        case "CB11":
                            scaleY = false;
                            break;
                        case "CB12":
                            scaleZ = false;
                            break;
                        case "CB13":
                            moveX = false;
                            break;
                        case "CB14":
                            moveY = false;
                            break;
                        case "CB15":
                            moveZ = false;
                            break;
                        default:
                            break;
                    }
                }
            }
            else // not the current button
            {

            }
        }
    }
}
