using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundAtRandomTime : MonoBehaviour {
    public int firstPossibleSecond;
    public int lastPossibleSecond;
	// Use this for initialization
	void Start () {
        int willPlayAt = Random.Range(firstPossibleSecond, lastPossibleSecond);
        Invoke("PlaySound", willPlayAt);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void PlaySound()
    {
        int willPlayAt = Random.Range(firstPossibleSecond, lastPossibleSecond);
        Invoke("PlaySound", willPlayAt);
        GetComponent<AudioSource>().Play();
    }
}
