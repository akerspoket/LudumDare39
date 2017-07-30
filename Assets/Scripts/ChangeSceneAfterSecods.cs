using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneAfterSecods : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //Invoke("ChangeScene", 6);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown )
        {
            CancelInvoke();
            ChangeScene();
        }
	}

    void ChangeScene()
    {
        FindObjectOfType<SceneChanger>().ChangeScene("MainMenu");
    }
}
