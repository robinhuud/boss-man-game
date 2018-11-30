//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LightControl : MonoBehaviour, IDeviceControl {

    public GameObject bulb_light;
    //public Material bulb_material;
    private bool isOn = false;
    public AudioClip clickSound;
    AudioSource au;

    // Use this for initialization
    void Start () {
        Debug.Assert(bulb_light != null, "bulb_light is null");
        //Debug.Assert(bulb_material != null, "bulb_material is null");
        au = GetComponent<AudioSource>();
    }
	
    public void Activate()
    {
        if(isOn)
        {
            turn_off();
        }
        else
        {
            turn_on();
        }
        au.PlayOneShot(clickSound, 1f);
    }

    public bool IsActive()
    {
        return isOn;
    }

    private void turn_on()
    {
        //Debug.Log("On");
        Material[] mats = GetComponent<Renderer>().materials;
        foreach(Material mat in mats)
        {
            //Debug.Log(mat.name);
            if(mat.name.StartsWith("Lightbulb"))
            {
                mat.SetColor("_Emission", new Color(1f, 1f, 1f, 1f));
            }
            if(mat.name.StartsWith("lampshade"))
            {
                mat.SetColor("_Emission", new Color(.3f, .3f, .7f, 1f));
            }
        }
        GetComponent<Renderer>().materials = mats;
        bulb_light.SetActive(true);
        isOn = true;
    }

    private void turn_off()
    {
        //Debug.Log("Off");
        Material[] mats = GetComponent<Renderer>().materials;
        foreach (Material mat in mats)
        {
            if (mat.name.StartsWith("Lightbulb"))
            {
                mat.SetColor("_Emission", new Color(0f, 0f, 0f, 0f));
            }
            if (mat.name.StartsWith("lampshade"))
            {
                mat.SetColor("_Emission", new Color(0f, 0f, 0f, 0f));
            }
        }
        GetComponent<Renderer>().materials = mats;
        bulb_light.SetActive(false);
        isOn = false;
    }
}
