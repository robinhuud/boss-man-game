using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class humanoidAnimatorInterface : MonoBehaviour {

    private Animator myAnimator;
	// Use this for initialization
	void Start () {
        myAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SteppedBack()
    {
        Debug.Log("it worked");
        transform.position += new Vector3(-.375f, 0, 0);
    }

    void SitLoop()
    {
        switch((int)(Random.value * 5f))
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
        switch((int)(Random.value * 5f))
        {
            case 0:
                myAnimator.SetTrigger("uncross");
                break;
        }
    }

    void TapLoop()
    {
        switch((int)(Random.value * 3f))
        {
            case 0:
                myAnimator.SetTrigger("stop_tapping");
                break;
        }
    }
}
