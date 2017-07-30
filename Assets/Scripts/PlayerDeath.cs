using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour {
    bool dying = false;
    GameObject enemyThatKilledMe;
    GameObject hand1;
    GameObject hand2;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (dying)
        {
            Vector3 direction = transform.position - enemyThatKilledMe.transform.position;
            direction.Normalize();
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-direction), Time.deltaTime * 4);
            Vector3 distanceBetweenHands = hand1.transform.position - hand2.transform.position;
            Vector3 wantedPosition = hand1.transform.position - distanceBetweenHands.normalized * distanceBetweenHands.magnitude;
            transform.position = wantedPosition;
        }
	}

    public void ActivateDeathAnimation(GameObject enemy)
    {
        if (!dying)
        {
            enemyThatKilledMe = enemy;
            hand1 = enemyThatKilledMe.GetComponentsInChildren<HandDummy>()[0].gameObject;
            hand2 = enemyThatKilledMe.GetComponentsInChildren<HandDummy>()[1].gameObject;
            dying = true;
            Invoke("GoToLostScene", 3f);
        }
    }

    void GoToLostScene()
    {
        FindObjectOfType<SceneChanger>().ChangeScene("LostScene");
    }
}
