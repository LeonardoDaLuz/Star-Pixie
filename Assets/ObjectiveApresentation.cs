using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveApresentation : MonoBehaviour {
	public float DelayToNext = 2f;
	public float DelayToEnableControlls = 1f;
	float TimeToExpireDelay;
	Animator animator;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		MatchManager.i.disableControls = true;
		TimeToExpireDelay = Time.time + DelayToNext;

	}
	
	// Update is called once per frame
	void Update () {

        if (Time.time>TimeToExpireDelay && Input.anyKeyDown)
        {
			animator.SetTrigger("next");
			Invoke("EnableControls", DelayToEnableControlls);
        }
	}

	void EnableControls()
    {
		MatchManager.i.disableControls = false;
		gameObject.SetActive(false);
	}
}
