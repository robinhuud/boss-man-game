using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class humanoidAnimatorInterface : MonoBehaviour {

    private Animator myAnimator;
	// Use this for initialization
	void Start () {
        myAnimator = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SteppedBack()
    {
        //Debug.Log("SteppedBack");
        //Debug.Log("it worked");
        transform.position += new Vector3(-.375f, 0, 0);
    }

    void SitLoop()
    {
        //Debug.Log("SitLoop");
        switch((int)(Random.value * 5f)) // 1 in 5 chance of each tap foot or cross legs
        {
            case 0:
            case 1:
            case 2:
            case 3:
                myAnimator.SetTrigger("tap_foot");
                break;
            case 4:
                myAnimator.SetTrigger("cross_legs");
                break;
        }
    }

    void CrossedLoop()
    {
        //Debug.Log("CrossedLoop");
        switch ((int)(Random.value * 5f)) // 1 in 5 chance of uncrossing legs every cycle
        {
            case 0:
                myAnimator.SetTrigger("uncross");
                break;
        }
    }

    void TapLoop()
    {
        //Debug.Log("TapLoop");
        switch ((int)(Random.value * 3f)) // 1 in 3 chance to stop tapping foot every cycle
        {
            case 0:
                myAnimator.SetTrigger("stop_tapping");
                break;
        }
    }

    void SwingLoop()
    {
        //Debug.Log("SwingLoop");
        switch((int)(Random.value * 3f)) // 1 in 3 chance of leaving foot-swing animation, uncrossing legs.
        {
            case 0:
                myAnimator.SetTrigger("uncross");
                break;
        }
    }
}
