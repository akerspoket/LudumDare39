using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighBallDrop : MonoBehaviour {
    public GameObject m_ballPrefab;
    [Tooltip("The number this is set to at start will be how many balls the player starts with")]
    public int m_ballsLeft;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Mouse1) && m_ballsLeft > 0)
        {
            GameObject newLight = Instantiate(m_ballPrefab, transform.position, Quaternion.identity);
            Light lightComp = newLight.GetComponent<Light>();
            newLight.GetComponent<MeshRenderer>().material.color = lightComp.color;
            m_ballsLeft--;
        }	
	}
}
