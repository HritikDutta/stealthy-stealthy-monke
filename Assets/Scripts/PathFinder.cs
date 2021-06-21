using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Custom A* pathfinder for level grid
public class PathFinder : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap levelTilemap;

    private HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

    private static int[] neighboursX = new int[] { -1, 0,  0, 1 };
    private static int[] neighboursY = new int[] {  0, 1, -1, 0 };

    public class Node : IHeapItem<Node>
    {
        public Vector3Int position;
        public int distanceRemaining;
        public int distanceTravelled;
        public Node parent = null;
        int heapIndex;

        public Node(Vector3Int _position)
        {
            position = _position;
        }

        public int FullCost
        {
            get
            {
                return distanceRemaining + distanceTravelled;
            }
        }

        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }

        public int CompareTo(Node other)
        {
            int compare = FullCost.CompareTo(other.FullCost);

            if (compare == 0)
                compare = distanceRemaining.CompareTo(other.distanceRemaining);

            return -compare;
        }
    }

    public bool UpdatePath(Vector3Int startPosition, Vector3Int endPosition, ref List<Vector3> gridPath)
    {
        Heap<Node> openSet = new Heap<Node>(10 * groundTilemap.size.x * groundTilemap.size.y);  // @Todo: Reduce this size
        closedSet.Clear();

        Node startNode = new Node(startPosition);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node node = openSet.Pop();
            closedSet.Add(node.position);

            // Reached end
            if (node.position == endPosition)
            {
                // Walk back to fill path
                WalkBack(node, ref gridPath);
                return true;
            }

            for (int i = 0; i < neighboursX.Length; i++)
            {
                Vector3Int neighbourPosition = node.position + new Vector3Int(neighboursX[i], neighboursY[i], 0);

                if (closedSet.Contains(neighbourPosition)     ||
                    !groundTilemap.HasTile(neighbourPosition) ||
                    levelTilemap.HasTile(neighbourPosition))
                {
                    continue;
                }

                int movementCost = node.distanceTravelled + GetWeightedDistance(node.position, neighbourPosition);

                Node neighbourNode = new Node(neighbourPosition);
                neighbourNode.distanceTravelled = movementCost;
                neighbourNode.distanceRemaining = GetWeightedDistance(neighbourPosition, endPosition);
                neighbourNode.parent = node;

                if (!openSet.Contains(neighbourNode))
                    openSet.Add(neighbourNode);
            }
        }

        return false;
    }

    void WalkBack(Node finalNode, ref List<Vector3> gridPath)
    {
        gridPath.Clear();

        Node currentNode = finalNode;

        while (currentNode != null)
        {
            gridPath.Add((Vector3) currentNode.position + new Vector3(0.5f, 0.5f, 0f));
            currentNode = currentNode.parent;
        }

        gridPath.Reverse();
    }

    // Ignores z coordinate
    int GetWeightedDistance(Vector3Int start, Vector3Int end)
    {
        int xDist = Mathf.Abs(end.x - start.x);
        int yDist = Mathf.Abs(end.y - start.y);

        if (xDist > yDist)
            return 14 * yDist + 10 * (xDist - yDist);

        return 14 * xDist + 10 * (yDist - xDist);
    }
}
