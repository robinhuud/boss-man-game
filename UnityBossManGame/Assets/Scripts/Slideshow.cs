using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slideshow : MonoBehaviour, IDeviceControl {

    [SerializeField]
    public Texture[] slides;
    public float delay;
    bool isRunning = true;
    int slideId = 0;
    private static IEnumerator CoRoutine;
	// Use this for initialization
	void Start () {
        Debug.Assert(slides.Length > 0, "Unable to find slides to show.");
        CoRoutine = WaitAndAdvance(delay);
        AdvanceFrame();
        StartCoroutine(CoRoutine);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate()
    {
        isRunning = !isRunning;
        if(isRunning)
        {
            StartCoroutine(CoRoutine);
        }
        else
        {
            StopCoroutine(CoRoutine);
        }
    }

    public bool IsActive()
    {
        return isRunning;
    }

    private void AdvanceFrame()
    {
        if (++slideId >= slides.Length)
        {
            slideId = 0;
        }
        GetComponent<Renderer>().material.SetTexture("_MainTex", slides[slideId]);
    }

    private IEnumerator WaitAndAdvance(float waitTime)
    {
        while(true)
        {
            yield return new WaitForSeconds(waitTime);
            AdvanceFrame();
        }
    }
}
