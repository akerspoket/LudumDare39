using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour {
    public float batteryLeft = 1;
    public float fadeThreshold = 0.01f;
    public float batteryDecay = 0.01f;
    float startingIntensity;
    Light m_spotLight;
    // Use this for initialization
    void Start () {
        m_spotLight = GetComponentInChildren<Light>();
        m_spotLight.enabled = false;
        startingIntensity = m_spotLight.intensity;
    }
	
	// Update is called once per frame
	void Update () {
        if (m_spotLight.enabled)
        {
            batteryLeft -= batteryDecay * Time.deltaTime;
        }
        if (batteryLeft > 0 && Input.GetKeyDown(KeyCode.Mouse0))
        {
            m_spotLight.enabled = !m_spotLight.enabled;
        }
        if (batteryLeft <= 0)
        {
            m_spotLight.enabled = false;
        }

        m_spotLight.intensity = startingIntensity * Mathf.Min(batteryLeft / fadeThreshold,1);
        
    }
}
