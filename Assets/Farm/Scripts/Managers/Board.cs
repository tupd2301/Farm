using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Farm
{
    public class Board : MonoBehaviour
    {
        [Header("Board Settings")]
        [SerializeField]
        private int width = 3;

        [SerializeField]
        private int height = 9;

        [SerializeField]
        private float nodeSpacing = 1f;

        [SerializeField]
        private GameObject nodePrefab;

        private Node[,] grid;
        private List<Node> allNodes = new List<Node>();

        private void Start()
        {
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            grid = new Node[width, height];
            CreateNodes();
            SetupNodeNeighbors();
        }

        private void CreateNodes()
        {
            // Calculate offset to center the grid
            float totalWidth = (width - 1) * nodeSpacing;
            float totalHeight = (height - 1) * nodeSpacing;
            Vector2 offset = new Vector2(-totalWidth / 2, -totalHeight / 2);

            int nodeId = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector3 position = new Vector3(
                        x * nodeSpacing + offset.x,
                        y * nodeSpacing + offset.y,
                        0
                    );
                    GameObject nodeObject = Instantiate(nodePrefab, transform);
                    nodeObject.transform.localPosition = position;
                    nodeObject.name = $"Node_{x}_{y}";

                    Node node = nodeObject.GetComponent<Node>();
                    if (node != null)
                    {
                        Vector2Int gridPos = new Vector2Int(x, y);
                        node.InitializeNode(nodeId, gridPos);

                        grid[x, y] = node;
                        allNodes.Add(node);
                        nodeId++;
                    }
                }
            }
        }

        private void SetupNodeNeighbors()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Node currentNode = grid[x, y];
                    List<Node> neighbors = new List<Node>();

                    // Check 4 adjacent neighbors (up, down, left, right)
                    Vector2Int[] directions = new Vector2Int[]
                    {
                        new Vector2Int(0, 1), // Up
                        new Vector2Int(0, -1), // Down
                        new Vector2Int(-1, 0), // Left
                        new Vector2Int(1, 0), // Right
                    };

                    foreach (Vector2Int dir in directions)
                    {
                        int checkX = x + dir.x;
                        int checkY = y + dir.y;

                        if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                        {
                            neighbors.Add(grid[checkX, checkY]);
                        }
                    }

                    currentNode.SetNeighbors(neighbors);
                }
            }
        }

        // Called when two nodes are swapped
        public void OnNodesSwapped(Node node1, Node node2)
        {
            // Update grid references if needed
            Vector2Int pos1 = node1.GetData().gridPosition;
            Vector2Int pos2 = node2.GetData().gridPosition;

            grid[pos1.x, pos1.y] = node1;
            grid[pos2.x, pos2.y] = node2;

            // You can add additional logic here such as:
            // - Check for matches
            // - Update game state
            // - Trigger animations
            // - etc.
        }
    }
}
