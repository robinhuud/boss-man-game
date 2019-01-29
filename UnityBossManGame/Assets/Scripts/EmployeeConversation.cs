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
    [SerializeField]
    public IDCQuestionButton[] questionButtons;
    [SerializeField]
    public Texture[] questionTextures;
    [SerializeField]
    public string[] questionCommands;
    public bool isInHall = true;

    private static System.Random random = new System.Random();
    private bool[] saidAlready;
    private bool hasFallen = false;
    private int nextQuestion = 2;

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

    // called by the SceneControl script in response to certain events
    // other events are sent to this script from the animator (see below)
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
        if(saidAlready[0] && !isInHall)
        {
            StartCoroutine(WaitThenSay(1f, 3));
        }
    }

    // Primary way the question tiles send questions to the conversation
    // is by using the IDCSendMessageToController interface.
    public void AskedQuestion(string questionKey)
    {
        if(saidAlready[2])
        {
            if(!isInHall)
            {
                switch (questionKey)
                {
                    case "whatplace":
                        if (!saidAlready[5])
                        {
                            StartCoroutine(WaitThenSay(2f, 5));
                        }
                        break;
                    case "howmake":
                        if (!saidAlready[6])
                        {
                            StartCoroutine(WaitThenSay(2f, 6));
                        }
                        break;
                }
            }
        }
        else
        {
            Debug.Assert(false, "asked question before revealing buttons, WTF??");
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
        yield return new WaitForSeconds(speechClips[clipId].length);
        doneSpeaking(clipId);
    }

    private void doneSpeaking(int clip)
    {
        if(clip > 4)
        {
            foreach (IDCQuestionButton qb in questionButtons)
            {
                if (questionCommands[clip].Equals(qb.GetCommand()))
                {
                    nextQuestion++;
                    if (nextQuestion < questionTextures.Length)
                    {
                        qb.ShowQuestion(questionTextures[nextQuestion], questionCommands[nextQuestion]);
                    }
                    else
                    {
                        if (!saidAlready[4])
                        {
                            StartCoroutine(WaitThenSay(.2f, 4));
                        }
                    }
                }
            }
        }
 
        if(clip == 2)
        {
            questionButtons[0].ShowQuestion(questionTextures[0], questionCommands[0]);
            questionButtons[1].ShowQuestion(questionTextures[1], questionCommands[1]);
        }
    }

    // Triggered by the animation triggers of the same name, (see employeeBehavior.cs)
    // Unity allows multiple scripts to respond to the same animation events if they are
    // attached to the same object. This can make the code confusing, but also allows
    // you to separate reactions to the same event by audio vs. animation vs. AI or whatever
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
            StartCoroutine(WaitThenSay(.2f, 2));
        }
    }
}
