using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class MapManager : MonoBehaviour {

    public GameObject[] prefabs;

    public int size;
    public int randomPointsForEachTheme;
    public float frequencyOfMakingLoops;

    private List<GameObject> hexagones = new List<GameObject>(); // TODO: delete hexagones in the future, temporary variable

	void Start () {
        Dictionary<IntVector2, VirtualCell> map = generateMap();
        map = CreatePerfectMaze(map);
        map = MakeLoops(map);
        map = SetNeighbourhoodAndDeadEnds(map);
        map = CreatePaths(map);
        createScene();
	}

    // Creates simple big hexagon from small hexagones
    private Dictionary<IntVector2, VirtualCell> generateMap()
    {
        Dictionary<IntVector2, VirtualCell> map = new Dictionary<IntVector2, VirtualCell>();
        IntVector2 position;

        // Structure of map: http://www.redblobgames.com/grids/hexagons/ - Axial coordinates
        for (int i = -2 * size + 1; i <= 2 * size - 1; i++)
        {
            for (int j = -2 * size + 1; j <= 2 * size - 1; j++)
            {
                if(i + j > 2 * size - 1 || i + j < -2 * size + 1)
                {
                    continue;
                }
                position = new IntVector2(i, j);
                hexagones.Add(new GameObject()); // TODO: delete hexagones in the future, temporary variable
                map[position] = hexagones[hexagones.Count - 1].AddComponent<VirtualCell>();
                map[position].Initialize(position);
            }
        }

        return map;
    }

    private Dictionary<IntVector2, VirtualCell> CreatePerfectMaze(Dictionary<IntVector2, VirtualCell> map)
    {
        Stack<VirtualCell> lastCells = new Stack<VirtualCell>();
        int randomIndexX, randomIndexY;
        IntVector2 randomPosition;
        bool isPositionEven;
        do
        {
            // The maze is generated on the basis of cells, which have an even position
            randomIndexX = UnityEngine.Random.Range(-2 * size + 2, 2 * size - 1);
            randomIndexY = UnityEngine.Random.Range(-2 * size + 2, 2 * size - 1);
            isPositionEven = (randomIndexX % 2 == 0) && (randomIndexY % 2 == 0);
            randomPosition = new IntVector2(randomIndexX, randomIndexY);
        } while (!(map.ContainsKey(randomPosition) && isPositionEven));
        

        VirtualCell currentCell = map[randomPosition];
        VirtualCell neighbour;
        VirtualCell middleCell;
        IntVector2 middleCellPosition;
        currentCell.IsVisited = true;
        currentCell.Type = VirtualCell.CellType.FIELD;
        List<IntVector2> potentialNeighbours;

        do
        {
            neighbour = null;
            potentialNeighbours = currentCell.GetRandomlyPotentialIndirectNeighbours();
            for (int i = 0; i < potentialNeighbours.Count; i++)
            {
                if(map.ContainsKey(potentialNeighbours[i]) && !map[potentialNeighbours[i]].IsVisited)
                {
                    neighbour = map[potentialNeighbours[i]];
                }
            }

            if (neighbour)
            {
                middleCellPosition = currentCell.Position + (neighbour.Position - currentCell.Position) / 2;
                if (map.ContainsKey(middleCellPosition))
                {
                    middleCell = map[middleCellPosition];
                    middleCell.Type = VirtualCell.CellType.FIELD;
                    neighbour.Type = VirtualCell.CellType.FIELD;
                }
                else
                {
                    Debug.Assert(false, "Cell does not exist");
                    //Debug.Break();
                }
                lastCells.Push(currentCell);
                currentCell = neighbour;
                currentCell.IsVisited = true;
            }
            else
            {
                if (!currentCell.Position.Equals(randomPosition))
                {
                    currentCell = lastCells.Pop();
                }
                else
                {
                    if(lastCells.Count > 0)
                    {
                        currentCell = lastCells.Pop();
                    }
                }
                
            }
        } while (lastCells.Count > 0);

        return map;
    }

    // TODO: maybe change to limit the longest path insted randomly make loops
    private Dictionary<IntVector2, VirtualCell> MakeLoops(Dictionary<IntVector2, VirtualCell> map)
    {
        List<IntVector2> potentialNeighbours;
        VirtualCell middleCell;
        VirtualCell neighbour;
        IntVector2 middleCellPosition;

        foreach (KeyValuePair<IntVector2, VirtualCell> currentCell in map)
        {
            if ((currentCell.Value.Position.x % 2 == 0) && (currentCell.Value.Position.y % 2 == 0))
            {
                potentialNeighbours = currentCell.Value.GetRandomlyPotentialIndirectNeighbours();
                for (int i = 0; i < potentialNeighbours.Count; i++)
                {
                    if (map.ContainsKey(potentialNeighbours[i]) && map[potentialNeighbours[i]].Type == VirtualCell.CellType.FIELD)
                    {
                        neighbour = map[potentialNeighbours[i]];
                        middleCellPosition = currentCell.Value.Position + (neighbour.Position - currentCell.Value.Position) / 2;
                        if (map.ContainsKey(middleCellPosition))
                        {
                            if (UnityEngine.Random.value <= frequencyOfMakingLoops)
                            {
                                middleCell = map[middleCellPosition];
                                middleCell.Type = VirtualCell.CellType.FIELD;
                                middleCell.IsVisited = true;
                            }
                        }
                        else
                        {
                            Debug.Assert(false, "Cell does not exist");
                            //Debug.Break();
                        }
                        break;
                    }
                }
            }
        }

        return map;
    }

    private Dictionary<IntVector2, VirtualCell> SetNeighbourhoodAndDeadEnds(Dictionary<IntVector2, VirtualCell> map)
    {
        List<IntVector2> list = new List<IntVector2>();
        for(int i = 0; i < 6; i++)
        {
            list.Add(new IntVector2(0, 0));
        }

        foreach (KeyValuePair<IntVector2, VirtualCell> currentCell in map)
        {
            currentCell.Value.IsVisited = false; // TODO: move to other function, maybe reset?
            currentCell.Value.IsDeadEnd = false;

            list[0].x = currentCell.Value.Position.x + 1;
            list[1].x = currentCell.Value.Position.x;
            list[2].x = currentCell.Value.Position.x - 1;
            list[3].x = list[2].x;
            list[4].x = list[1].x;
            list[5].x = list[0].x;
            list[0].y = currentCell.Value.Position.y;
            list[1].y = currentCell.Value.Position.y + 1;
            list[2].y = list[1].y;
            list[3].y = list[0].y;
            list[4].y = currentCell.Value.Position.y - 1;
            list[5].y = list[4].y;

            for (int i = 0; i < list.Count; i++)
            {
                if (map.ContainsKey(list[i]) && map[list[i]].Type == VirtualCell.CellType.FIELD)
                {
                    currentCell.Value.Neighbours.Add(map[list[i]]);
                }
            }

            if(currentCell.Value.Neighbours.Count == 1)
            {
                currentCell.Value.IsDeadEnd = true;
            }
        }

        return map;
    }

    private Dictionary<IntVector2, VirtualCell> CreatePaths(Dictionary<IntVector2, VirtualCell> map)
    {
        List<Queue<VirtualCell>> lastCells = new List<Queue<VirtualCell>>(randomPointsForEachTheme * (int)VirtualCell.FieldType.COUNT);
        VirtualCell currentCell;
        HashSet<IntVector2> randomIndices = new HashSet<IntVector2>();
        int randomIndexX, randomIndexY;
        IntVector2 randomPosition;
        //bool shouldBeNeutral = false; // TODO: doesn't work, needed region variable in VirtualCell

        for (int i = 0; i < randomPointsForEachTheme * (int)VirtualCell.FieldType.COUNT; i++)
        {
            lastCells.Add(new Queue<VirtualCell>());
            do
            {
                do
                {
                    randomIndexX = UnityEngine.Random.Range(-2 * size + 2, 2 * size - 1);
                    randomIndexY = UnityEngine.Random.Range(-2 * size + 2, 2 * size - 1);
                    randomPosition = new IntVector2(randomIndexX, randomIndexY);
                } while (!map.ContainsKey(randomPosition) || map[randomPosition].Type != VirtualCell.CellType.FIELD);
            } while (!randomIndices.Add(randomPosition));

            lastCells[i].Enqueue(map[randomPosition]);
            map[randomPosition].IsVisited = true;
            map[randomPosition].TypeOfField = (VirtualCell.FieldType)(i % (int)VirtualCell.FieldType.COUNT);
        }

        while (lastCells.Count > 0)
        {
            for (int i = 0; i < lastCells.Count; i++)
            {
                if (lastCells[i].Count == 0)
                {
                    lastCells.RemoveAt(i--);
                    continue;
                }

                currentCell = lastCells[i].Dequeue();
                foreach (VirtualCell neighbour in currentCell.Neighbours) // TODO: randomly add neighbours to queue
                {
                    //shouldBeNeutral = false; // TODO: doesn't work, needed region variable in VirtualCell

                    if (!neighbour.IsVisited)
                    {
                        lastCells[i].Enqueue(neighbour);
                        neighbour.TypeOfField = currentCell.TypeOfField;
                        neighbour.IsVisited = true;
                    }
                    //else if (neighbour.TypeOfField == currentCell.TypeOfField) // TODO: doesn't work, needed region variable in VirtualCell
                    //{
                    //    shouldBeNeutral = true;
                    //}
                }

                //if (shouldBeNeutral) // TODO: doesn't work, needed region variable in VirtualCell
                //{
                //    currentCell.TypeOfField = VirtualCell.FieldType.NEUTRAL;
                //}
            }
        }

        return map;
    }

    private void createScene()
    {
        float horizontal = Mathf.Sqrt(3)/2;
        float vertical = 0.75F;
        float shift = horizontal / 2;

        for (int i = 0; i < hexagones.Count; i++)
        {
            VirtualCell currentCell = hexagones[i].GetComponent<VirtualCell>();  // TODO: delete hexagones in the future, temporary variable
            Vector3 position = new Vector3(
                    currentCell.Position.x * horizontal + currentCell.Position.y * shift,
                    0,
                    -1 * currentCell.Position.y * vertical);

            GameObject choosenPrefab = null;
            switch (currentCell.Type)
            {
                case VirtualCell.CellType.WALL: // TODO: Change to resources
                    choosenPrefab = prefabs[prefabs.Length - 1];
                    break;
                case VirtualCell.CellType.FIELD:
                    if (!currentCell.IsDeadEnd)
                    {
                        if ((int)currentCell.TypeOfField < (int)VirtualCell.FieldType.COUNT)
                        {
                            choosenPrefab = prefabs[(int)currentCell.TypeOfField];
                        }
                        else
                        {
                            choosenPrefab = prefabs[prefabs.Length - 3]; // NEUTRAL
                        }
                    }
                    else
                    {
                        choosenPrefab = prefabs[prefabs.Length - 2];
                    }
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            hexagones[i] = Instantiate(choosenPrefab, position, transform.rotation) as GameObject;  // TODO: delete hexagones in the future, temporary variable
            hexagones[i].transform.parent = transform;
        }

        //foreach (KeyValuePair<IntVector2, VirtualCell> wall in map)
        //{
        //    if(wall.Value.Type == VirtualCell.CellType.WALL)
        //    {
        //        Vector3 position = new Vector3(
        //            wall.Key.x * horizontal + wall.Key.y * shift,
        //            0,
        //            -1 * wall.Key.y * vertical);
        //        GameObject go = Instantiate(prefab, position, transform.rotation) as GameObject;
        //        go.transform.parent = transform;
        //    }
        //}
    }
}
