using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealGameSettings : MonoBehaviour {
    public float masterVolume;
    public int mapWidth;
    public int mapHeight;
    public int numberOfEnemies;
    public int numberOfBatteries;
    public int numberOfLightballs;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetMasterVolume(Single v)
    {
        masterVolume = v;
    }
    public void SetMapWidth(Single v)
    {
        mapWidth = (int)v;
    }
    public void SetMapHeight(Single v)
    {
        mapHeight = (int)v;
    }
    public void SetNumberOfEnemies(Single v)
    {
        numberOfEnemies = (int)v;
    }
    public void SetNumberOfBatteries(Single v)
    {
        numberOfBatteries = (int)v;
    }
    public void SetNumberOfLightballs(Single v)
    {
        numberOfLightballs = (int)v;
    }
}
