﻿using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class employeeBehavior : MonoBehaviour {

    public Animator animator;
    public NavMeshAgent agent;
    public Transform target;
    public Transform lookTarget;
    public Transform chairTarget;

    private bool stopped = true;
    private Animator myAnimator;
    // Use this for initialization
    void Start ()
    {
        agent.isStopped = true;
        myAnimator = GetComponentInChildren<Animator>();
        //animator.SetBool("Walking", false);
    }

    // Called from the editor from the cog icon, useful for hooking up dependant scene attributes
    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponentInChildren<NavMeshAgent>();
        Debug.Assert(animator != null,"Can't find an Animator attached to object");
        Debug.Assert(agent != null, "Can't find a NavMeshAgent attached to object");
    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    private void arrived()
    {
        animator.SetBool("Walking", false);
        agent.isStopped = true;
        find_chair();
        Debug.Log("Arrived at destination, stopping walk cycle");    
    }

    public void DoorOpened()
    {
        //Debug.Log("Door has opened");
        animator.SetBool("Sitting", false);
        animator.SetBool("Walking", true);
        stopped = false;
    }

    void find_chair()
    {
        agent.updateRotation = false;
        transform.LookAt(lookTarget);
        animator.SetTrigger("step_back");
    }

    void StandLoop()
    {
        Debug.Log("StandLoop");
        if (!stopped)
        {
            agent.updatePosition = true;
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            //arrived();
        }
    }

    void WalkLoop()
    {
        if (!stopped)
        {
            stopped = true;
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                arrived();
            }
        }
    }

    void SteppedBack()
    {
        Debug.Log("SteppedBack");
        animator.SetBool("Sitting", true);
        agent.updatePosition = false;
        transform.position += new Vector3(-.375f, 0, 0);

    }

    void SitLoop()
    {
        //Debug.Log("SitLoop");
        switch ((int)(Random.value * 5f)) // 1 in 5 chance of each tap foot or cross legs
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
        switch ((int)(Random.value * 3f)) // 1 in 3 chance of leaving foot-swing animation, uncrossing legs.
        {
            case 0:
                myAnimator.SetTrigger("uncross");
                break;
        }
    }

}
