using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm
{
    public class Field : MonoBehaviour
    {
        [SerializeField]
        private GameObject plantPrefab; // Prefab for the plant

        [SerializeField]
        private Sprite plantSprite; // Sprite for the plant

        [SerializeField]
        private Vector2 gridSize = new Vector2(5, 5); // Grid size

        [SerializeField]
        private float spacing = 1.0f; // Spacing between plants

        [SerializeField]
        private Vector2 spriteSize = new Vector2(1f, 1f); // Size of the plant sprites

        [SerializeField]
        private bool useRandomScale = false; // Whether to use random scaling

        [SerializeField]
        private Vector2 randomScaleRange = new Vector2(0.8f, 1.2f); // Min/max scale multiplier

        [SerializeField]
        private bool useRandomOffset = false; // Whether to use random position offsets

        [SerializeField]
        private float randomOffsetRange = 0.2f; // Max position offset in each direction

        [SerializeField]
        private bool useRandomFlip = false; // Whether to randomly flip sprites

        [SerializeField]
        private float flipChance = 0.5f; // Chance of flipping a sprite (0-1)

        private List<GameObject> plants = new List<GameObject>();

        public int fieldIndex;

        // Start is called before the first frame update
        void Start()
        {
            SpawnPlants();
        }

        // Spawn plants in a grid
        private void SpawnPlants()
        {
            // Clear existing plants if any
            ClearPlants();

            // Calculate starting position to center the grid
            Vector2 startPos = new Vector2(
                -((gridSize.x - 1) * spacing) / 2,
                ((gridSize.y - 1) * spacing) / 2 // Positive Y for top position
            );

            // Get sprite bounds if possible to calculate better positioning
            float spriteWidth = 1f;
            float spriteHeight = 1f;

            if (plantSprite != null)
            {
                spriteWidth = plantSprite.bounds.size.x;
                spriteHeight = plantSprite.bounds.size.y;
            }

            // Spawn plants in a grid centered around the field
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    Vector3 position = new Vector3(
                        startPos.x + x * spacing,
                        startPos.y - y * spacing, // Negative Y to go downward
                        0f // Explicitly set Z to 0
                    );

                    // Apply random offset if enabled
                    if (useRandomOffset)
                    {
                        position += new Vector3(
                            Random.Range(-randomOffsetRange, randomOffsetRange),
                            Random.Range(-randomOffsetRange, randomOffsetRange),
                            0f // Maintain Z at 0
                        );
                    }

                    // Calculate scale
                    Vector3 scale = new Vector3(spriteSize.x, spriteSize.y, 1f); // Explicitly set Z to 1

                    // Apply random scale if enabled
                    if (useRandomScale)
                    {
                        float scaleFactor = Random.Range(randomScaleRange.x, randomScaleRange.y);
                        scale.x *= scaleFactor;
                        scale.y *= scaleFactor;
                        // scale.z remains 1
                    }

                    // Determine if sprite should be flipped via negative X scale
                    if (useRandomFlip && Random.value < flipChance)
                    {
                        scale.x *= -1; // Flip by using negative scale on X-axis
                        // scale.z remains 1
                    }

                    GameObject plant;
                    if (plantPrefab != null)
                    {
                        plant = Instantiate(plantPrefab, transform);
                        plant.transform.localPosition = position;
                        plant.transform.localScale = scale;
                        SpriteRenderer spriteRenderer = plant.GetComponent<SpriteRenderer>();
                        spriteRenderer.sprite = plantSprite;

                        // Set sorting layer and order in layer
                        spriteRenderer.sortingLayerName = "Plant";
                        spriteRenderer.sortingOrder = fieldIndex * 1000 + (y * (int)gridSize.x + x); // Increase order based on position

                        // Add FlashController component if it doesn't exist
                        if (plant.GetComponent<FlashController>() == null)
                        {
                            plant.AddComponent<FlashController>().useThisRenderer = true;
                        }
                    }
                    else
                    {
                        // Create a basic GameObject with a SpriteRenderer if no prefab is provided
                        plant = new GameObject("Plant_" + x + "_" + y);
                        plant.transform.parent = transform;
                        plant.transform.localPosition = position;

                        SpriteRenderer spriteRenderer = plant.AddComponent<SpriteRenderer>();
                        spriteRenderer.sprite = plantSprite;

                        // Set sorting layer and order in layer
                        spriteRenderer.sortingLayerName = "Plant";
                        spriteRenderer.sortingOrder = fieldIndex * 1000 + (y * (int)gridSize.x + x); // Increase order based on position

                        plant.transform.localScale = scale;

                        // Add FlashController component if not using prefab
                        plant.AddComponent<FlashController>().useThisRenderer = true;
                    }

                    plants.Add(plant);
                }
            }
        }

        // Clear all plants
        private void ClearPlants()
        {
            foreach (GameObject plant in plants)
            {
                if (plant != null)
                {
                    Destroy(plant);
                }
            }
            plants.Clear();
        }

        // Public method to respawn plants with current settings
        public void RespawnPlants()
        {
            SpawnPlants();
        }

        // Trigger flash effect on a specified number of random plants
        public void TriggerRandomPlantsFlash(int count)
        {
            if (plants.Count == 0)
                return;

            // Ensure count is within valid range
            count = Mathf.Clamp(count, 0, plants.Count);

            // Create a temporary list to shuffle
            List<GameObject> plantsToFlash = new List<GameObject>(plants);

            // Fisher-Yates shuffle algorithm
            for (int i = 0; i < plantsToFlash.Count - 1; i++)
            {
                int j = Random.Range(i, plantsToFlash.Count);
                GameObject temp = plantsToFlash[i];
                plantsToFlash[i] = plantsToFlash[j];
                plantsToFlash[j] = temp;
            }

            // Trigger flash on the first 'count' plants after shuffling
            for (int i = 0; i < count; i++)
            {
                FlashController flashCtrl = plantsToFlash[i].GetComponent<FlashController>();
                if (flashCtrl != null)
                {
                    flashCtrl.TriggerFlash();
                }
            }
        }

        [ContextMenu("Trigger All Plants Flash")]
        // Trigger flash effect on all plants
        public void TriggerAllPlantsFlash()
        {
            foreach (GameObject plant in plants)
            {
                if (plant != null)
                {
                    FlashController flashCtrl = plant.GetComponent<FlashController>();
                    if (flashCtrl != null)
                    {
                        flashCtrl.TriggerFlash();
                    }
                }
            }
        }

        // Trigger flash effect on a specific plant by index
        public void TriggerPlantFlash(int index)
        {
            if (index < 0 || index >= plants.Count)
                return;

            GameObject plant = plants[index];
            if (plant != null)
            {
                FlashController flashCtrl = plant.GetComponent<FlashController>();
                if (flashCtrl != null)
                {
                    flashCtrl.TriggerFlash();
                }
            }
        }
    }
}
