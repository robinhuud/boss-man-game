using UnityEngine.AI;
using UnityEngine;
using System.Collections;

/// <summary>
/// Coordinates and controls the NavMeshAgent and the Animator
/// 
/// </summary>

public class employeeBehavior : MonoBehaviour {

    public Animator animator; // The Animator which controls the animation of the employee
    public NavMeshAgent agent; // The NavMeshAgent which controls where the employee can move in the scene
    public Transform spawnPoint; // world coordinates of the spawn point (and reset point) of the employee
    public Transform target; // world coordinates to move toward for the NavMeshAgent
    public Transform lookTarget; // world coordinates to look toward for the animator (player's face)
    public Transform benchTarget; // world coordinates of benchfor hallway navigation.
    public AudioSource voiceSource; // source for voice clips, attached to same game object as the employee's head

    private bool stopped = true; // Is the NavMeshAgent currently stopped?
    private float brakingDistance = .35f; // fudge factor to trigger the stop walking animation
    private bool isInHall = true;
    private bool playedWelcome = false;

    // Called from the editor from the cog icon, useful for hooking up dependant scene attributes
    // in this case, that's the required public variables of Animator, and NavMeshAgent
    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponentInChildren<NavMeshAgent>();
        voiceSource = GetComponentInChildren<AudioSource>();
        Debug.Assert(animator != null, "Can't find an Animator attached to object, please assign it in the inspector");
        Debug.Assert(agent != null, "Can't find a NavMeshAgent attached to object, please assign it in the inspector");
    }

    // Use this for initialization
    void Start ()
    {
        // make sure we have our scene references
        Debug.Assert(animator != null, "No Animator assigned, cannot continue.");
        Debug.Assert(agent != null, "No NavMeshAgent assigned, cannot continue.");
        // prepare the NavMeshAgent
        Debug.Assert(agent.isOnNavMesh, "NavMeshAgent is not attached to a navmesh");
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(benchTarget.position, path);
        if(path.status == NavMeshPathStatus.PathComplete)
        {
            //Debug.Log("Complete Path found.");
            agent.Warp(benchTarget.position);
        }
        else
        {
            Debug.Log("No complete path found");
        }
        arrived();
    }

	
	// Update is called once per frame
	void Update ()
    {

	}

    void FixedUpdate()
    {

    }

    private void LateUpdate()
    {

    }

    private void arrived()
    {
        //Debug.Log("Arrived");
        agent.isStopped = true;
        find_chair();
        //Debug.Log("Arrived at destination, stopping walk cycle");    
    }

    public void DoorOpened()
    {
        // this is called when the door has opened, 
        //Debug.Log("Door has opened at " + Time.time);
        animator.SetBool("Sitting", false);
        stopped = false;
        if(!playedWelcome)
        {
            StartCoroutine(WaitToTalk(7.0f, voiceSource.clip));
        }
    }


    void find_chair()
    {
        agent.updateRotation = false;
        //transform.LookAt(lookTarget);
        StartCoroutine(LerpTurn(transform.rotation, lookTarget, 0.5f));
    }

    IEnumerator LerpTurn(Quaternion startRotation, Transform target, float duration)
    {
        float startTime = Time.time;
        float thisTime = startTime;
        float fraction = 0;
        Quaternion destinationRotation = Quaternion.LookRotation(new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z));
        while ( thisTime < startTime + duration + Time.deltaTime)
        {
            thisTime += Time.deltaTime;
            fraction = (thisTime - startTime) / duration;
            transform.localRotation = Quaternion.Slerp(startRotation, destinationRotation, fraction);
            yield return null;
        }
        ready_to_sit();
    }

    IEnumerator WaitToTalk(float duration, AudioClip clip)
    {
        float startTime = Time.time;
        while(Time.time < startTime + duration)
        {
            yield return null;
        }
        voiceSource.PlayOneShot(clip);
        playedWelcome = true;
    }

    void ready_to_sit()
    {
        animator.SetTrigger("step_back");
        agent.updateRotation = false;
        agent.updatePosition = false;
        //animator.SetBool("Sitting", true);
    }

    // All of these functions are called by the animator based on events in the various timelines of the animation clips
    // these are used to string animations together, and trigger other events
    void StandLoop()
    {
        // This is the moment when we decide whether we should start walking or just continue to stand
        //Debug.Log("StandLoop");
        if (!stopped) // This is the "Start Walking" case
        {
            //Debug.Log("Setting destination to new walk target at " + Time.time);
            animator.SetBool("Walking", true); // should trigger state change, then StartedWalking() callback
            //agent.updatePosition = true;

            if (isInHall)
            {
                agent.SetDestination(target.position);
                isInHall = false;
            }
            else
            {
                agent.SetDestination(benchTarget.position);
                isInHall = true;
            }
            
            //agent.updateRotation = true;
            //agent.updatePosition = true;
 
            //breakNow = true;
        }
    }

    void StartedWalking()
    {
        //Debug.Break();
        // Turns on the Nav mesh agent which takes over the root motion.
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;
    }

    void WalkLoop()
    {
        // after each step we check to see if we are close enough to the destinatiopn to trigger the stop walking animation transition.
        if (!stopped && agent.remainingDistance <= agent.stoppingDistance + brakingDistance)
        {
            //Debug.Log("Walk Loop at destination, stop walking at " + Time.time);
            animator.SetBool("Walking", false); // should trigger state transition, then StoppedWalking() callback
            stopped = true;
            arrived();
        }
    }

    void StoppedWalking()
    {
        // turn off navigation because step back animation moves the gameobject and nav would try to move it back
        agent.isStopped = true; 
        //Debug.Log("StoppedWalking");
    }

    void SteppedBack()
    {
        // After we step back toward the chair, we must trigger the sitting animation.
        //Debug.Log("SteppedBack");
        transform.position += new Vector3(-.41f, 0, 0.01f);
        animator.SetBool("Sitting", true);
    }

    void SitLoop()
    {
        // every time we finish a loop animation, we pick what to do next randomly
        switch ((int)(Random.value * 3f)) // 1 in 5 chance of each tap foot or cross legs
        {
            case 0:
                animator.SetTrigger("tap_foot");
                break;
            case 1:
                animator.SetTrigger("cross_legs");
                break;
        }
    }

    void CrossedLoop()
    {
        // every time we finish a loop animation, we pick what to do next randomly
        switch ((int)(Random.value * 6f)) // 1 in 5 chance of uncrossing legs every cycle
        {
            case 0:
                animator.SetTrigger("uncross");
                break;
        }
    }

    void TapLoop()
    {
        // every time we finish a loop animation, we pick what to do next randomly
        switch ((int)(Random.value * 4f)) // 1 in 3 chance to stop tapping foot every cycle
        {
            case 0:
                animator.SetTrigger("stop_tapping");
                break;
        }
    }

    void SwingLoop()
    {
        // every time we finish a loop animation, we pick what to do next randomly
        switch ((int)(Random.value * 4f)) // 1 in 3 chance of leaving foot-swing animation, uncrossing legs.
        {
            case 0:
                animator.SetTrigger("uncross");
                break;
        }
    }

    void FinishedFalling()
    {
        agent.updateRotation = true;
        agent.updatePosition = true;
        agent.Warp(spawnPoint.position);
        agent.SetDestination(benchTarget.position);
        isInHall = true;
    }
}
