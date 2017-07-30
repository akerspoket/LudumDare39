using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour {
    Animator animator;
    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        animator.Play("Move",0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayGivenAnimation(string clip)
    {
        animator.Play(clip, 0);
    }
}
