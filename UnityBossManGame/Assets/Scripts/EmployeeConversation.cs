using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeConversation : MonoBehaviour {

    public AudioSource voiceSource; // source for voice clips, attached to same game object as the employee's head
    [SerializeField]
    public AudioClip[] speechClips;
    public AudioSource screamSource; // separate source for the screams, dont really need it but it makes the mixing easier
    [SerializeField]
    public AudioClip[] fallingClips; // set of clips for falling sounds

    private static System.Random random = new System.Random();
    private bool[] saidAlready;
    private bool hasFallen = false;
    // Called form the editor Cog menu, useful for setting defaults, which can then be overridden
    void Reset()
    {
        // Set the 2 audio Sources as the same by default.
        voiceSource = GetComponentInChildren<AudioSource>();
        screamSource = GetComponentInChildren<AudioSource>();
    }

    // Use this for initialization
    void Start () {
        saidAlready = new bool[speechClips.Length];
        for(int i =0; i < saidAlready.Length; i++)
        {
            saidAlready[i] = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DoorOpened()
    {
        if(!saidAlready[0])
        {
            StartCoroutine(WaitThenSay(6f, 0));
        }
        if(hasFallen && !saidAlready[1])
        {
            StartCoroutine(WaitThenSay(6f, 1));
        }
    }

    IEnumerator WaitThenSay(float waitTime, int clipId)
    {
        float speakAtTime = Time.time + waitTime;
        while (Time.time < speakAtTime)
        {
            yield return null;
        }
        // After the timer runs out, the AudioSource plays the clip as a one-shot
        // does this mean we can't interrupt it?
        voiceSource.PlayOneShot(speechClips[clipId]);
        saidAlready[clipId] = true;
    }

    // Triggered by the animation trigger of the same name, (see employeeBehavior.cs)
    void NoticedFalling()
    {
        AudioClip clip = fallingClips[random.Next(0, fallingClips.Length - 1)];
        screamSource.PlayOneShot(clip);
        hasFallen = true;
    }

    void SatDown()
    {
        if(saidAlready[0] && !saidAlready[2])
        {
            StartCoroutine(WaitThenSay(1f, 2));
        }
    }
}
