using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedAutoToggle : MonoBehaviour, IDeviceControl {
    public float delay;
    private bool timerRunning = false;
    private IDeviceControl[] idc;
    private IEnumerator timer;
    // Use this for initialization
    void Start () {
        idc = GetComponents<IDeviceControl>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate()
    {
        if(!IsActive())
        {
            timer = activateAfter(delay);
            StartCoroutine(timer);
        }
        else
        {
            StopCoroutine(timer);
            timerRunning = false;
        }
    }

    public bool IsActive()
    {
        return timerRunning;
    }

    IEnumerator activateAfter(float time)
    {
        timerRunning = true;
        yield return new WaitForSeconds(time);
        foreach (IDeviceControl id in idc)
        {
            id.Activate();
        }
        timerRunning = false;
    }
}
