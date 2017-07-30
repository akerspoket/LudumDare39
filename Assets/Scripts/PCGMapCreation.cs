using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCGMapCreation: MonoBehaviour{
    // 0 unconsidered
    // 1 considered
    // 2 wall
    // 3 empty space
    // 4 spawn space
    // 5 goal space
    // 6 battery pickup
    // 7 ball pickup
    // 8 enemy
    public static PCGMapCreation singleton = null;
    public static int[,] grid;

    public static GameObject wallDebug;
    public int numberOfBatteryPickups;
    public int numberOfLightBallPickups;
    public int numberOfEnemies = 1;
    [Range(-10, 10)]
    public float branchingFactor;
    public int minPathSize = 4;
    public int width = 50;
    public int height = 50;

    private void Awake()
    {
        singleton = this;
    }

    public class IntPoint
    {
        public int x;
        public int y;
        public IntPoint(int px, int py)
        {
            x = px;
            y = py;
        }

        public static bool operator ==(IntPoint a, IntPoint b)
        {
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(IntPoint a, IntPoint b)
        {
            return a.x != b.x || a.y != b.y;
        }
    }

    public class PointDistance
    {
        public IntPoint point;
        public float distance = -1;
    }

    public Texture2D M_GeneratePCGMap()
    {
        grid = new int[width-2,height-2];
        
        int VertX = UnityEngine.Random.Range(minPathSize, width- minPathSize);
        int vertStartY = 0;
        int vertEndY = grid.GetLength(1);
        int horiY = UnityEngine.Random.Range(minPathSize, height- minPathSize);
        int horiStartX = 0;
        int horiEndX = grid.GetLength(0);

        RecursiveSplit(VertX, vertStartY, vertEndY, horiY, horiStartX, horiEndX, minPathSize);

        List<IntPoint> availableLocations = new List<IntPoint>();
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i,j] == 0 || grid[i,j] == 3)
                {
                    availableLocations.Add(new IntPoint(i, j));
                }
            }
        }
        // This is unnecesary it seems...
        //IntPoint[] goalAndStart = FindPointFurthestFromEachother();
        //grid[goalAndStart[0].x, goalAndStart[0].y] = 4;
        //grid[goalAndStart[1].x, goalAndStart[1].y] = 5;

        SetStartPosition(ref availableLocations);
        SetGoalPosition(ref availableLocations);
        RandomizeEnemyLocation(ref availableLocations);
        RandomizeBatteryPickups(ref availableLocations);
        RandomizeLightballPickups(ref availableLocations);

        Texture2D returnTexture = new Texture2D(width, height,TextureFormat.ARGB32, false);
        ColorTextureFromGrid(ref returnTexture);

        return returnTexture;
    }

    private void SetGoalPosition(ref List<IntPoint> availableLocations)
    {
        IntPoint point = availableLocations[availableLocations.Count-1];
        grid[point.x, point.y] = 5;
        availableLocations.RemoveAt(availableLocations.Count - 1);
    }

    private void SetStartPosition(ref List<IntPoint> availableLocations)
    {
        IntPoint point = availableLocations[0];
        grid[point.x, point.y] = 4;
        availableLocations.RemoveAt(0);
    }

    public IntPoint ConvertVector3ToGridPosition(Vector3 position)
    {
        // -2 to account for border
        IntPoint returnValue = new IntPoint(Mathf.RoundToInt(position.x - 2 + 0.5f), Mathf.RoundToInt(position.z - 2 + 0.5f));
        return returnValue;
    }

    public Vector2 ConvertGridPositionToVector2(IntPoint point)
    {
        // 2 to account for border and 0.5 to account for cell sizes
        Vector2 returnValue = new Vector2(point.x + 1 + 0.5f, point.y + 1 + 0.5f);//new IntPoint(Mathf.RoundToInt(position.x - 2 + 0.5f), Mathf.RoundToInt(position.z - 2 + 0.5f));
        return returnValue;
    }

    private void RandomizeLightballPickups(ref List<IntPoint> availableLocations)
    {
        for (int i = 0; i < numberOfLightBallPickups; i++)
        {
            if (availableLocations.Count == 0)
            {
                return;
            }
            int index = UnityEngine.Random.Range(0, availableLocations.Count);
            IntPoint point = availableLocations[index];
            grid[point.x, point.y] = 7;
            availableLocations.RemoveAt(index);
        }
    }

    private void RandomizeBatteryPickups(ref List<IntPoint> availableLocations)
    {
        for (int i = 0; i < numberOfBatteryPickups; i++)
        {
            if (availableLocations.Count == 0)
            {
                return;
            }
            int index = UnityEngine.Random.Range(0, availableLocations.Count);
            IntPoint point = availableLocations[index];
            grid[point.x, point.y] = 6;
            availableLocations.RemoveAt(index);
        }
    }

    private void RandomizeEnemyLocation(ref List<IntPoint> availableLocations)
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            if (availableLocations.Count == 0)
            {
                return;
            }
            int index = UnityEngine.Random.Range(0, availableLocations.Count);
            IntPoint point = availableLocations[index];
            grid[point.x, point.y] = 8;
            availableLocations.RemoveAt(index);
        }
    }

    private void RecursiveSplit(int vertX, int vertStartY ,int vertEndY, int horiY, int horiStartX, int horiEndX, int minPathSize)
    {
        for (int i = vertStartY; i < vertEndY; i++)
        {
            if (i >= 0 && i < grid.GetLength(1) && grid[vertX, i] == 0)
            {
                grid[vertX, i] = 2;
            }
        }
        for (int i = horiStartX; i < horiEndX; i++)
        {
            if (i >= 0 && i < grid.GetLength(0) && grid[i, horiY] == 0)
            {
                grid[i, horiY] = 2;
            }   
        }
        // remove four section to connect rooms
        int randomVert = UnityEngine.Random.Range(Mathf.CeilToInt(vertStartY + minPathSize / 2), Mathf.FloorToInt( horiY - minPathSize / 2));
        for (int i = randomVert - minPathSize / 2; i < randomVert + minPathSize / 2; i++)
        {
            if (IsPointInsideGrid(new IntPoint(vertX,i)))
            {
                grid[vertX, i] = 3;
            }
        }

        randomVert = UnityEngine.Random.Range(Mathf.CeilToInt(horiY + minPathSize / 2), Mathf.FloorToInt(vertEndY - minPathSize / 2));
        for (int i = randomVert - minPathSize / 2; i < randomVert + minPathSize / 2; i++)
        {
            if (IsPointInsideGrid(new IntPoint(vertX, i)))
            {
                grid[vertX, i] = 3;
            }
        }

        int randomHori = UnityEngine.Random.Range(Mathf.CeilToInt(horiStartX + minPathSize / 2), Mathf.FloorToInt(vertX - minPathSize / 2));
        for (int i = randomHori - minPathSize / 2; i < randomHori + minPathSize / 2; i++)
        {
            if (IsPointInsideGrid(new IntPoint(i,horiY)))
            {
                grid[i, horiY] = 3;
            }
        }

        randomHori = UnityEngine.Random.Range(Mathf.CeilToInt(vertX + minPathSize / 2), Mathf.FloorToInt(horiEndX - minPathSize / 2));
        for (int i = randomHori - minPathSize / 2; i < randomHori + minPathSize / 2; i++)
        {
            if (IsPointInsideGrid(new IntPoint(i, horiY)))
            {
                grid[i, horiY] = 3;
            }
        }
        // and now we start spitting up the newly created room
        // Top left room
        if (vertX - minPathSize > horiStartX + minPathSize && horiY - minPathSize > minPathSize)
        {
            int newVertX = UnityEngine.Random.Range(horiStartX + minPathSize, vertX - minPathSize);
            int newHoriY = UnityEngine.Random.Range(vertStartY + minPathSize, horiY - minPathSize);
            RecursiveSplit(newVertX, vertStartY, horiY, newHoriY, horiStartX, vertX, minPathSize);
        }

        // Top right room
        if (horiEndX - minPathSize > vertX + minPathSize && horiY - minPathSize > vertStartY + minPathSize)
        {
            int newVertX = UnityEngine.Random.Range(vertX + minPathSize, horiEndX - minPathSize);
            int newHoriY = UnityEngine.Random.Range(vertStartY + minPathSize, horiY - minPathSize);
            RecursiveSplit(newVertX, vertStartY, horiY, newHoriY, vertX, horiEndX, minPathSize);
        }
        // bottom left room
        if (vertX - minPathSize > horiStartX + minPathSize && vertEndY - minPathSize > horiY + minPathSize)
        {
            int newVertX = UnityEngine.Random.Range(horiStartX + minPathSize, vertX - minPathSize);
            int newHoriY = UnityEngine.Random.Range(horiY + minPathSize, vertEndY - minPathSize);
            RecursiveSplit(newVertX, horiY, vertEndY, newHoriY, horiStartX, vertX, minPathSize);
        }

        // bottom right room
        if (horiEndX - minPathSize > vertX + minPathSize && vertEndY - minPathSize > horiY + minPathSize)
        {
            int newVertX = UnityEngine.Random.Range(vertX + minPathSize, horiEndX - minPathSize);
            int newHoriY = UnityEngine.Random.Range(horiY + minPathSize, vertEndY - minPathSize);
            RecursiveSplit(newVertX, horiY, vertEndY, newHoriY, vertX, horiEndX, minPathSize);
        }
    }

    private IntPoint[] FindPointFurthestFromEachother()
    {
        PointDistance distance = new PointDistance();
        IntPoint[] returnValue = new IntPoint[2];
        IntPoint startPoint = new IntPoint(0,0);
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                IntPoint newPoint = new IntPoint(i, j);
                int[,] replacementGrid = (int[,])grid.Clone();
                PointDistance distanceReturned = FloodFill(newPoint, 2, 1, 0, ref replacementGrid);
                if (distanceReturned.distance > distance.distance)
                {
                    distance = distanceReturned;
                    startPoint = newPoint;
                }
            }
        }
        returnValue[0] = startPoint;
        returnValue[1] = distance.point;
        return returnValue;
    }

    private PointDistance FloodFill(IntPoint node, int wallTargetColor, int replacementColor, float distance, ref int [,] grid)
    {
        PointDistance returnValue = new PointDistance();
        
        grid[node.x, node.y] = replacementColor;

        IntPoint newNode = node;
        newNode.x++;
        if (IsPointInsideGrid(newNode) && grid[newNode.x, newNode.y] != wallTargetColor && grid[newNode.x, newNode.y] != replacementColor)
        {
            PointDistance pointDistance = FloodFill(newNode, wallTargetColor, replacementColor, distance + 1, ref grid);
            if (pointDistance.distance > returnValue.distance)
            {
                returnValue = pointDistance;
            }
        }

        newNode = node;
        newNode.x--;
        if (IsPointInsideGrid(newNode) && grid[newNode.x, newNode.y] != wallTargetColor && grid[newNode.x, newNode.y] != replacementColor)
        {
            PointDistance pointDistance = FloodFill(newNode, wallTargetColor, replacementColor, distance + 1, ref grid);
            if (pointDistance.distance > returnValue.distance)
            {
                returnValue = pointDistance;
            }
        }

        newNode = node;
        newNode.y++;
        if (IsPointInsideGrid(newNode) && grid[newNode.x, newNode.y] != wallTargetColor && grid[newNode.x, newNode.y] != replacementColor)
        {
            PointDistance pointDistance = FloodFill(newNode, wallTargetColor, replacementColor, distance + 1, ref grid);
            if (pointDistance.distance > returnValue.distance)
            {
                returnValue = pointDistance;
            }
        }

        newNode = node;
        newNode.y--;
        if (IsPointInsideGrid(newNode) && grid[newNode.x, newNode.y] != wallTargetColor && grid[newNode.x, newNode.y] != replacementColor)
        {
            PointDistance pointDistance = FloodFill(newNode, wallTargetColor, replacementColor, distance + 1, ref grid);
            if (pointDistance.distance > returnValue.distance)
            {
                returnValue = pointDistance;
            }
        }

        // This will probably only happen in the case where all other nodes are walls or visisted
        if (distance > returnValue.distance)
        {
            returnValue.distance = distance;
            returnValue.point = new IntPoint(node.x, node.y);
        }
        return returnValue;
    }

    private static IntPoint[] GetUnexaminedPoints(IntPoint point)
    {
        List<IntPoint> unexaminedPoints = new List<IntPoint>();
        if (IsPointUnexamined(new IntPoint(point.x+1, point.y)))
        {
            unexaminedPoints.Add(new IntPoint(point.x + 1, point.y));
        }
        if (IsPointUnexamined(new IntPoint(point.x - 1, point.y)))
        {
            unexaminedPoints.Add(new IntPoint(point.x - 1, point.y));
        }
        if (IsPointUnexamined(new IntPoint(point.x, point.y + 1)))
        {
            unexaminedPoints.Add(new IntPoint(point.x, point.y + 1));
        }
        if (IsPointUnexamined(new IntPoint(point.x, point.y - 1)))
        {
            unexaminedPoints.Add(new IntPoint(point.x, point.y - 1));
        }
        return unexaminedPoints.ToArray();
    }

    private static bool SetConsideredPointToValue(IntPoint point)
    {
        int numberOfExposed = 0;

        if (IsPointInsideGrid(new IntPoint(point.x + 1, point.y)) &&
            grid[point.x + 1, point.y] == 3)
        {
            numberOfExposed++;
        }
        if (IsPointInsideGrid(new IntPoint(point.x - 1, point.y)) &&
            grid[point.x - 1, point.y] == 3)
        {
            numberOfExposed++;
        }
        if (IsPointInsideGrid(new IntPoint(point.x, point.y + 1)) &&
            grid[point.x, point.y + 1] == 3)
        {
            numberOfExposed++;
        }
        if (IsPointInsideGrid(new IntPoint(point.x, point.y - 1)) &&
            grid[point.x, point.y - 1] == 3)
        {
            numberOfExposed++;
        }

        if (numberOfExposed > 1)
        {
            grid[point.x, point.y] = 2;
            return false;
        }
        else
        {
            grid[point.x, point.y] = 3;
            return true;
        }
    }

    public static bool IsPointInsideGrid(IntPoint point)
    {
        if (point.x < 0 || point.x >= grid.GetLength(0) ||
        point.y < 0 || point.y >= grid.GetLength(1))
        {
            return false;
        }
        return true;
    }

    private static bool IsPointUnexamined(IntPoint point)
    {
        if (!IsPointInsideGrid(point))
        {
            return false;
        }
        return grid[point.x, point.y] == 0;
    }

    private static void ColorTextureFromGrid(ref Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        MakeBorder(ref pixels, texture.width, texture.height);
        for (int i = 1; i < grid.GetLength(0) + 1; i++)
        {
            for (int j = 1; j < grid.GetLength(1) + 1; j++)
            {
                int index = ConvertCoordinatesToIndex(i, j, texture.width);
                pixels[index] = GetColorFromGrid(i - 1, j - 1);
            }
        }
        texture.SetPixels(pixels);
    }

    private static Color GetColorFromGrid(int x, int y)
    {
        if (grid[x,y] == 2)
        {
            return Color.black;
        }
        if (grid[x, y] == 4)
        {
            return Color.blue;
        }
        if (grid[x, y] == 5)
        {
            return Color.green;
        }
        if (grid[x, y] == 6)
        {
            return Color.cyan;
        }
        if (grid[x, y] == 7)
        {
            return Color.yellow;
        }
        if (grid[x, y] == 8)
        {
            return Color.magenta;
        }
        return Color.white;
    }

    private static void MakeBorder(ref Color[] colors, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            colors[ConvertCoordinatesToIndex(x, 0, width)] = Color.black;
            colors[ConvertCoordinatesToIndex(x, height-1, width)] = Color.black;
        }
        for (int y = 0; y < height; y++)
        {
            colors[ConvertCoordinatesToIndex(0, y, width)] = Color.black;
            colors[ConvertCoordinatesToIndex(width - 1, y, width)] = Color.black;
        }
    }

    private static int ConvertCoordinatesToIndex(int x, int y, int width) { return y * width + x; }
}
