using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollowPlayer : MonoBehaviour {
    GameObject player;
    int[,] grid;
    List<PCGMapCreation.IntPoint> path = new List<PCGMapCreation.IntPoint>();
    public int framesBetweenPathUpdate = 1000;
    public float closeEnough = 0.3f;
    public float movementSpeed = 1;
    public int AStarLoopsOK = 10;
    int framesToPathUpdate = 0;
    CharacterController charController;
    Coroutine co = null;
	// Use this for initialization
	void Start () {
        grid = PCGMapCreation.grid;
        player = GameObject.FindGameObjectWithTag("Player");
        charController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (framesToPathUpdate <= 0)
        {
            PCGMapCreation.IntPoint start = PCGMapCreation.singleton.ConvertVector3ToGridPosition(transform.position);
            PCGMapCreation.IntPoint goal = PCGMapCreation.singleton.ConvertVector3ToGridPosition(player.transform.position);
            if (PCGMapCreation.IsPointInsideGrid(start) && PCGMapCreation.IsPointInsideGrid(goal))
            {
                if (co != null)
                {
                    StopCoroutine(co);
                }
                co = StartCoroutine(AStar(start, goal));
                framesToPathUpdate = framesBetweenPathUpdate;
            }
        }
        framesToPathUpdate--;

        if (path.Count != 0)
        {
            Vector3 target = PCGMapCreation.singleton.ConvertGridPositionToVector2(path[path.Count - 1]);
            target.z = target.y;
            target.y = transform.position.y;
            // Are we close enough? remove the point from the list
            if ((target - transform.position).magnitude <= closeEnough)
            {
                path.RemoveAt(path.Count - 1);
                return;
            }
            Vector3 direction = transform.position - target;
            direction.Normalize();
            charController.SimpleMove(-direction * movementSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation , Quaternion.LookRotation(- direction, Vector3.up), Time.deltaTime*6);
        }
	}

    IEnumerator AStar(PCGMapCreation.IntPoint start, PCGMapCreation.IntPoint goal)
    {
        List<PCGMapCreation.IntPoint> closedSet = new List<PCGMapCreation.IntPoint>();
        List<PCGMapCreation.IntPoint> openSet = new List<PCGMapCreation.IntPoint>();
        openSet.Add(start);
        Dictionary<PCGMapCreation.IntPoint, PCGMapCreation.IntPoint> cameFrom = new Dictionary<PCGMapCreation.IntPoint, PCGMapCreation.IntPoint>(); // This is not really right..
        Dictionary<PCGMapCreation.IntPoint, float> gScore = new Dictionary<PCGMapCreation.IntPoint, float>();
        gScore[start] = 0;
        Dictionary<PCGMapCreation.IntPoint, float> fScore = new Dictionary<PCGMapCreation.IntPoint, float>();
        fScore[start] = HeurisitcCostEvaluation(start,goal);

        int loopsDone = 0;
        while (openSet.Count != 0)
        {
            PCGMapCreation.IntPoint current = FindLowestScoreInDictionaryThatRecidesInList(fScore, openSet);
            if (current == goal)
            {
                ReconstructPath(cameFrom, current);
                yield break;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            for (int x = -1; x < 2; x++)
            {
                PCGMapCreation.IntPoint neighbor = new PCGMapCreation.IntPoint(current.x + x, current.y);
                if (closedSet.Exists((t => t == neighbor)) || !PCGMapCreation.IsPointInsideGrid(neighbor) || grid[neighbor.x , neighbor.y] == 2 )
                {
                    continue;
                }
                if (!openSet.Exists((t => t == neighbor)))
                {
                    openSet.Add(neighbor);
                }

                if (!gScore.ContainsKey(neighbor))
                {
                    gScore.Add(neighbor, float.MaxValue);
                }

                float tentativeScore = gScore[current] + HeurisitcCostEvaluation(current, neighbor);
                // Not a better path
                if (tentativeScore >= gScore[neighbor])
                {
                    continue;
                }
                if (!cameFrom.ContainsKey(neighbor))
                {
                    cameFrom.Add(neighbor, new PCGMapCreation.IntPoint(0, 0));
                }

                if (!fScore.ContainsKey(neighbor))
                {
                    fScore.Add(neighbor, float.MaxValue);
                }
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeScore;
                fScore[neighbor] = tentativeScore + HeurisitcCostEvaluation(neighbor, goal);      
            }
            for (int y = -1; y < 2; y++)
            {
                PCGMapCreation.IntPoint neighbor = new PCGMapCreation.IntPoint(current.x, current.y + y);
                if (closedSet.Exists((t => t == neighbor)) || !PCGMapCreation.IsPointInsideGrid(neighbor) || grid[neighbor.x, neighbor.y] == 2)
                {
                    continue;
                }
                if (!openSet.Exists((t => t == neighbor)))
                {
                    openSet.Add(neighbor);
                }

                if (!gScore.ContainsKey(neighbor))
                {
                    gScore.Add(neighbor, float.MaxValue);
                }

                float tentativeScore = gScore[current] + HeurisitcCostEvaluation(current, neighbor);
                // Not a better path
                if (tentativeScore >= gScore[neighbor])
                {
                    continue;
                }
                if (!cameFrom.ContainsKey(neighbor))
                {
                    cameFrom.Add(neighbor, new PCGMapCreation.IntPoint(0, 0));
                }

                if (!fScore.ContainsKey(neighbor))
                {
                    fScore.Add(neighbor, float.MaxValue);
                }
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeScore;
                fScore[neighbor] = tentativeScore + HeurisitcCostEvaluation(neighbor, goal);
            }
            loopsDone++;
            if (loopsDone >= AStarLoopsOK)
            {
                yield return new WaitForEndOfFrame();
                loopsDone = 0;
            }
        }
    }


    float HeurisitcCostEvaluation(PCGMapCreation.IntPoint a, PCGMapCreation.IntPoint b)
    {
        Vector2 avec = new Vector2(a.x, a.y);
        Vector2 bvec = new Vector2(b.x, b.y);

        return (avec - bvec).magnitude;
    }

    PCGMapCreation.IntPoint FindLowestScoreInDictionaryThatRecidesInList(Dictionary<PCGMapCreation.IntPoint, float> dic, List<PCGMapCreation.IntPoint> list)
    {
        float minScore = float.MaxValue;
        PCGMapCreation.IntPoint point = new PCGMapCreation.IntPoint(0,0);
        foreach (var item in list)
        {
            if (dic.ContainsKey(item))
            {
                if (dic[item] < minScore)
                {
                    minScore = dic[item];
                    point = item;
                }
            }

        }
        return point;
    }

    void ReconstructPath(Dictionary<PCGMapCreation.IntPoint, PCGMapCreation.IntPoint> cameFrom, PCGMapCreation.IntPoint current)
    {
        path.Clear();
        while (cameFrom.ContainsKey(current))
        {
            Vector2 positionCur = PCGMapCreation.singleton.ConvertGridPositionToVector2(current);
            Vector2 posisitonCame = PCGMapCreation.singleton.ConvertGridPositionToVector2(cameFrom[current]);
            Debug.DrawLine(new Vector3(positionCur.x, 1, positionCur.y), new Vector3(posisitonCame.x, 1, posisitonCame.y), Color.red, 1);
            current = cameFrom[current];
            path.Add(current);
        }
    }
}
