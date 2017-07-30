using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadMapFromTexture : MonoBehaviour {
    public GameObject debugDisplay;
    public GameObject wallPrefab;
    public GameObject player;
    public GameObject goalTilePrefab;
    public GameObject startTilePrefab;
    public GameObject batteryPickupPrefab;
    public GameObject lightBallPickupPrefab;
    public GameObject enemyPrefab;
    //public Terrain ter;
	// Use this for initialization
	void Start () {
        PCGMapCreation.wallDebug = wallPrefab;
        Texture2D test = GetComponent< PCGMapCreation >().M_GeneratePCGMap();// M_LoadImage(50, 50, Path.GetFullPath("Assets/MapTextures/testmap.png"));
        //debugDisplay.GetComponent<RawImage>().texture = test;
        M_MakeMapFromTexture(test);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private Texture2D M_LoadImage(int widht, int height, string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(widht, height, TextureFormat.RGBA32, false);
        texture.LoadImage(bytes);
        return texture;
    }

    private void M_MakeMapFromTexture(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        int width = texture.width;
        int height = texture.height;
        int length = pixels.Length;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = PCGMapCreation.singleton.ConvertGridPositionToVector2(new PCGMapCreation.IntPoint(x, y));
                if (pixels[y * width + x] == Color.black)
                {
                    // Calc position
                    Instantiate(wallPrefab, new Vector3(pos.x - 1, 0.5f, pos.y - 1), Quaternion.identity);
                }
                else if (pixels[y * width + x] == Color.green)
                {
                    // Calc position
                    Instantiate(goalTilePrefab, new Vector3(pos.x - 1, 0.5f, pos.y - 1), Quaternion.identity);
                }
                else if (pixels[y * width + x] == Color.blue)
                {
                    // start position
                    player.transform.position = new Vector3(pos.x - 1, 0f, pos.y - 1);
                    Instantiate(startTilePrefab, new Vector3(pos.x - 1, 0.5f, pos.y - 1), Quaternion.identity);
                }
                else if (pixels[y * width + x] == Color.cyan)
                {
                    // Calc position
                    Instantiate(batteryPickupPrefab, new Vector3(pos.x - 1, 0.5f, pos.y - 1), Quaternion.identity);
                }
                else if (pixels[y * width + x] == Color.yellow)
                {
                    // Calc position
                    Instantiate(lightBallPickupPrefab, new Vector3(pos.x - 1, 0.5f, pos.y - 1), Quaternion.identity);
                }
                else if (pixels[y * width + x] == Color.magenta)
                {
                    // Calc position
                    Instantiate(enemyPrefab, new Vector3(pos.x - 1, 0.5f, pos.y - 1), Quaternion.identity);
                }

            }
        }
    }

    private void M_PropagateHeight(ref float[,] array, int propagationSize, int x, int y, float startHeight)
    {
        int i = x - propagationSize;
        int j = y - propagationSize;
        Vector2 maxDistanceToCenter = new Vector2(Mathf.Abs(i - x), Mathf.Abs(j - y));
        for (; i < x + propagationSize; i++)
        {
            for (; j < y + propagationSize; j++)
            {
                Vector2 distanceToCenter = new Vector2( Mathf.Abs(i - x), Mathf.Abs(j - y));
                float percentageOfStart = 1 - distanceToCenter.magnitude / maxDistanceToCenter.magnitude;
                if (i < array.GetLength(0) && i>=0 &&
                    j >= 0 && j < array.GetLength(1))
                {
                    array[i, j] = Mathf.Max(array[i, j], percentageOfStart * startHeight);
                }
                
            }
        }
    }
}
