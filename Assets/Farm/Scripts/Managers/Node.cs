using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Farm
{
    [System.Serializable]
    public class NodeData
    {
        public int id;

        // Grid coordinates (x = column, y = row)
        public Vector2Int gridPosition;
        public List<Node> neighborNodes = new List<Node>();
        public Dictionary<string, object> values = new Dictionary<string, object>();
        public Sprite sprite;

        public NodeData(int id, Vector2Int gridPosition)
        {
            this.id = id;
            this.gridPosition = gridPosition;
        }
    }

    public class Node : MonoBehaviour
    {
        [SerializeField]
        private NodeData data;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Collider2D nodeCollider;

        public Sprite defaultSprite;

        // Node events
        public UnityEvent<Node> OnNodeClicked = new UnityEvent<Node>();
        public UnityEvent<Node> OnNodeDragStart = new UnityEvent<Node>();
        public UnityEvent<Node> OnNodeDragEnd = new UnityEvent<Node>();
        public UnityEvent<Node> OnNodeHold = new UnityEvent<Node>();

        // Reference to actual neighbor nodes (not serialized)
        private List<Node> neighbors = new List<Node>();

        // Input tracking
        private bool isDragging = false;
        private bool isHolding = false;
        private float holdTimer = 0f;
        private float holdThreshold = 0.5f; // Time in seconds to register a hold

        // Drag visual elements
        private GameObject dragClone;
        private SpriteRenderer dragSpriteRenderer;
        private float originalSpriteAlpha = 1f;
        private float draggedSpriteAlpha = 0.5f;

        // Board reference for node swapping
        private Board board;

        // Debug settings
        [Header("Debug")]
        [SerializeField]
        private bool debugInputEvents = true;

        private void Awake()
        {
            // Ensure sprite renderer component exists
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            // Ensure collider exists
            if (nodeCollider == null)
                nodeCollider = GetComponent<Collider2D>();

            // Get board reference
            board = GetComponentInParent<Board>();
        }

        private void Update()
        {
            HandleInput();
        }

        public void InitializeNode(int id, Vector2Int gridPosition)
        {
            data = new NodeData(id, gridPosition);
            UpdateNodeVisuals();
        }

        public void SetNeighbors(List<Node> neighborNodes)
        {
            neighbors = neighborNodes;
            data.neighborNodes = neighborNodes;
        }

        public NodeData GetData()
        {
            return data;
        }

        public void SetData(NodeData newData)
        {
            data = newData;
            UpdateNodeVisuals();
        }

        public void UpdateNodeVisuals()
        {
            if (spriteRenderer != null)
            {
                // Use default sprite if node sprite is null
                if (data.sprite == null)
                {
                    spriteRenderer.sprite = defaultSprite;
                }
                else
                {
                    spriteRenderer.sprite = data.sprite;
                }
            }
        }

        private void HandleInput()
        {
            // Handle input for both editor and mobile
            bool isMouseDown = Input.GetMouseButtonDown(0);
            bool isMouseUp = Input.GetMouseButtonUp(0);
            bool isMouseHeld = Input.GetMouseButton(0);
            bool isTouchBegan = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
            bool isTouchEnded = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;
            bool isTouchHeld =
                Input.touchCount > 0
                && (
                    Input.GetTouch(0).phase == TouchPhase.Moved
                    || Input.GetTouch(0).phase == TouchPhase.Stationary
                );

            // Combined input states
            bool inputDown = isMouseDown || isTouchBegan;
            bool inputUp = isMouseUp || isTouchEnded;
            bool inputHeld = isMouseHeld || isTouchHeld;

            // Debug input states
            if (debugInputEvents)
            {
                if (inputDown)
                    Debug.Log(
                        $"[{gameObject.name}] Input DOWN - Mouse:{isMouseDown} Touch:{isTouchBegan}"
                    );
                if (inputUp)
                    Debug.Log(
                        $"[{gameObject.name}] Input UP - Mouse:{isMouseUp} Touch:{isTouchEnded}"
                    );
                if (inputHeld && !isHolding && !isDragging)
                    Debug.Log(
                        $"[{gameObject.name}] Input HELD - Mouse:{isMouseHeld} Touch:{isTouchHeld}"
                    );
            }

            // Raycast from camera to screen position to detect nodes
            if (inputDown)
            {
                Vector2 screenPoint = GetInputPosition();
                if (screenPoint != null && IsRaycastHittingThisNode(screenPoint))
                {
                    isDragging = true;
                    isHolding = true;
                    holdTimer = 0f;

                    if (debugInputEvents)
                        Debug.Log($"[{gameObject.name}] Node SELECTED!");

                    OnNodeDragStart.Invoke(this);
                    OnNodeClicked.Invoke(this);

                    // Create drag clone
                    CreateDragClone();
                }
            }

            // Update drag clone position
            if (isDragging && dragClone != null)
            {
                UpdateDragClonePosition();
            }

            // Handle hold event
            if (inputHeld && isHolding)
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdThreshold)
                {
                    if (debugInputEvents)
                        Debug.Log($"[{gameObject.name}] Node HOLD triggered after {holdTimer:F2}s");

                    OnNodeHold.Invoke(this);
                    isHolding = false; // Only trigger once
                }
            }

            // Handle drag end and drop
            if (inputUp && isDragging)
            {
                if (debugInputEvents)
                    Debug.Log($"[{gameObject.name}] Node DRAG ENDED");

                Vector2 screenPoint = GetInputPosition();

                // Find the node under the drop position
                RaycastHit2D hit = Physics2D.Raycast(
                    Camera.main.ScreenToWorldPoint(screenPoint),
                    Vector2.zero
                );

                if (hit.collider != null)
                {
                    Node targetNode = hit.collider.GetComponent<Node>();
                    if (targetNode != null && targetNode != this)
                    {
                        if (debugInputEvents)
                            Debug.Log(
                                $"[{gameObject.name}] Dropped on {targetNode.gameObject.name}"
                            );
                        if (targetNode != this)
                        {
                            // Check if target is a neighbor
                            SwapWithNode(targetNode);
                            if (debugInputEvents)
                                Debug.Log(
                                    $"[{gameObject.name}] SWAPPING with {targetNode.gameObject.name}"
                                );
                        }
                    }
                }

                isDragging = false;
                isHolding = false;
                OnNodeDragEnd.Invoke(this);

                // Destroy drag clone and restore original sprite
                DestroyDragClone();
            }
        }

        private Vector2 GetInputPosition()
        {
            Vector2 position;
            if (Input.touchCount > 0)
            {
                position = Input.GetTouch(0).position;
                if (debugInputEvents)
                    Debug.Log($"[{gameObject.name}] Touch Position: {position}");
            }
            else
            {
                position = Input.mousePosition;
                if (debugInputEvents && isDragging)
                    Debug.Log($"[{gameObject.name}] Mouse Position: {position}");
            }
            return position;
        }

        private bool IsRaycastHittingThisNode(Vector2 screenPoint)
        {
            if (screenPoint == null)
                return false;

            RaycastHit2D hit = Physics2D.Raycast(
                Camera.main.ScreenToWorldPoint(screenPoint),
                Vector2.zero
            );

            bool isHit = hit.collider != null && hit.collider.gameObject == gameObject;

            if (debugInputEvents && isHit)
                Debug.Log($"[{gameObject.name}] Raycast HIT this node");

            return isHit;
        }

        private void SwapWithNode(Node otherNode)
        {
            // Save data references
            NodeData thisData = new NodeData(data.id, data.gridPosition);
            thisData.values = new Dictionary<string, object>(data.values);
            thisData.sprite = data.sprite != null ? data.sprite : defaultSprite;

            NodeData otherData = new NodeData(otherNode.data.id, otherNode.data.gridPosition);
            otherData.values = new Dictionary<string, object>(otherNode.data.values);
            otherData.sprite =
                otherNode.data.sprite != null ? otherNode.data.sprite : otherNode.defaultSprite;

            // Swap grid positions and values
            data.values = otherData.values;
            data.sprite = otherData.sprite;

            otherNode.data.values = thisData.values;
            otherNode.data.sprite = thisData.sprite;

            // Update visuals
            UpdateNodeVisuals();
            otherNode.UpdateNodeVisuals();

            if (debugInputEvents)
                Debug.Log($"[{gameObject.name}] Swap completed with {otherNode.gameObject.name}");

            // Notify board of the swap if needed
            if (board != null)
            {
                board.OnNodesSwapped(this, otherNode);
            }
        }

        private void CreateDragClone()
        {
            // Create a new GameObject for the clone
            dragClone = new GameObject($"{gameObject.name}_DragClone");
            dragClone.transform.SetParent(transform.parent);

            // Copy transform properties
            dragClone.transform.localScale = transform.localScale;

            // Add sprite renderer
            dragSpriteRenderer = dragClone.AddComponent<SpriteRenderer>();

            // Copy sprite from original
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                dragSpriteRenderer.sprite = spriteRenderer.sprite;
                dragSpriteRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
                dragSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder + 10; // Ensure it renders above
                dragSpriteRenderer.drawMode = spriteRenderer.drawMode;
                dragSpriteRenderer.size = spriteRenderer.size;
                dragSpriteRenderer.maskInteraction = spriteRenderer.maskInteraction;

                // Save original alpha and reduce alpha of the original
                if (spriteRenderer.color != null)
                {
                    originalSpriteAlpha = spriteRenderer.color.a;
                    Color originalColor = spriteRenderer.color;
                    originalColor.a = draggedSpriteAlpha;
                    spriteRenderer.color = originalColor;
                }
            }

            // Initial position
            UpdateDragClonePosition();
        }

        private void UpdateDragClonePosition()
        {
            if (dragClone != null)
            {
                Vector2 screenPoint = GetInputPosition();
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(
                    new Vector3(screenPoint.x, screenPoint.y, 10f)
                );
                worldPos.z = 0; // Ensure it's on the same Z plane
                dragClone.transform.position = worldPos;
            }
        }

        private void DestroyDragClone()
        {
            // Restore original sprite alpha
            if (spriteRenderer != null)
            {
                Color originalColor = spriteRenderer.color;
                originalColor.a = originalSpriteAlpha;
                spriteRenderer.color = originalColor;
            }

            // Destroy clone
            if (dragClone != null)
            {
                Destroy(dragClone);
                dragClone = null;
                dragSpriteRenderer = null;
            }
        }
    }
}
