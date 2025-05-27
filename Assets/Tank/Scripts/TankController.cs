using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tank
{
    public class TankController : MonoBehaviour
    {
        [Header("Movement")]
        public Rigidbody rb;
        public float moveSpeed = 10f;
        public float rotateSpeed = 10f;
        public LayerMask obstacleLayer; // Layers that count as obstacles
        public float collisionCheckDistance = 0.5f; // How far to check for collisions

        public Transform tankModel;
        
        [Header("Shooting")]
        public GameObject bulletPrefab;
        public Transform bulletSpawnPoint;
        public float fireRate = 0.5f; // seconds between shots
        public bool canShootWhileMoving = false;
        public float raycastRange = 100f; // How far the raycast will check
        public LayerMask targetLayerMask; // Layers that can be hit
        public GameObject muzzleFlashPrefab; // Visual effect at shot origin
        public GameObject hitEffectPrefab; // Visual effect at hit point
        public float rayDuration = 0.05f; // How long the visible ray lasts
        public Color rayColor = Color.yellow; // Color of the visible ray
        
        [Header("Rotation Controls")]
        public KeyCode rotateLeftKey = KeyCode.Q;
        public KeyCode rotateRightKey = KeyCode.E;
        public float manualRotateSpeed = 120f; // degrees per second
        public bool cardinalMovementOnly = true; // Use only cardinal directions
        
        private float lastFireTime = -Mathf.Infinity;
        private Vector3 lastMovementDirection = Vector3.zero;
        private bool isManuallyRotating = false;
        private Vector3 currentMovement = Vector3.zero;
        private LineRenderer shotLineRenderer;
        
        // Cardinal direction angles
        private readonly Quaternion facingNorth = Quaternion.Euler(0, 0, 0);
        private readonly Quaternion facingEast = Quaternion.Euler(0, 90, 0);
        private readonly Quaternion facingSouth = Quaternion.Euler(0, 180, 0);
        private readonly Quaternion facingWest = Quaternion.Euler(0, 270, 0);
        private CardinalDirection currentDirection = CardinalDirection.North;

        // Enum to track current direction
        private enum CardinalDirection
        {
            North,
            East,
            South,
            West
        }

        void Awake()
        {
            // Ensure we have a rigidbody
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
            
            // Set default obstacle layer if not set
            if (obstacleLayer.value == 0)
            {
                obstacleLayer = LayerMask.GetMask("Default");
            }
            
            // Set default target layer if not set
            if (targetLayerMask.value == 0)
            {
                targetLayerMask = obstacleLayer;
            }
            
            // Setup line renderer for shot visualization
            shotLineRenderer = gameObject.AddComponent<LineRenderer>();
            shotLineRenderer.startWidth = 0.05f;
            shotLineRenderer.endWidth = 0.05f;
            shotLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            shotLineRenderer.startColor = rayColor;
            shotLineRenderer.endColor = rayColor;
            shotLineRenderer.enabled = false;
        }

        void Update()
        {
            HandleRotation();
            HandleMovement();
            HandleShooting();
        }
        
        void HandleRotation()
        {
            if (cardinalMovementOnly)
            {
                // No smooth rotation in cardinal-only mode
                // Rotation is handled in HandleMovement
                return;
            }
            
            isManuallyRotating = false;
            
            // Manual rotation using horizontal input axis
            float rotateInput = Input.GetAxis("Horizontal");
            if (rotateInput != 0)
            {
                isManuallyRotating = true;
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotateInput * manualRotateSpeed * Time.deltaTime, 0));
            }
            // Still handle the key-based rotation as a fallback
            else if (Input.GetKey(rotateLeftKey))
            {
                isManuallyRotating = true;
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0, -manualRotateSpeed * Time.deltaTime, 0));
            }
            else if (Input.GetKey(rotateRightKey))
            {
                isManuallyRotating = true;
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0, manualRotateSpeed * Time.deltaTime, 0));
            }
        }

        void HandleMovement()
        {
            if (cardinalMovementOnly)
            {
                HandleCardinalMovement();
                return;
            }
            
            // Get forward/backward input (but we'll use it only for speed control, not direction)
            float speedModifier = Input.GetAxis("Vertical");
            
            // Original behavior with continuous movement
            float moveInputY = Input.GetAxis("Vertical");
            float moveInputX = Input.GetAxis("Horizontal");
            
            // Lock movement to cardinal directions
            if (Mathf.Abs(moveInputX) > Mathf.Abs(moveInputY))
            {
                moveInputY = 0f;
            }
            else
            {
                moveInputX = 0f;
            }
            
            // Calculate movement independent of rotation
            Vector3 movementDirection = new Vector3(moveInputX, 0f, moveInputY).normalized;
            
            // Update movement tracking
            if (movementDirection != Vector3.zero)
            {
                lastMovementDirection = movementDirection;
                currentMovement = movementDirection * moveSpeed * Time.deltaTime;
            }
            else if (rb.velocity.sqrMagnitude < 0.1f)
            {
                currentMovement = Vector3.zero;
            }
            
            MoveWithCollisionCheck();
        }
        
        void HandleCardinalMovement()
        {
            float moveInputY = Input.GetAxis("Vertical");
            float moveInputX = Input.GetAxis("Horizontal");
            
            // Reset movement
            currentMovement = Vector3.zero;
            
            // Determine direction based on input
            if (Mathf.Abs(moveInputX) > Mathf.Abs(moveInputY))
            {
                // Horizontal movement takes priority
                if (moveInputX > 0.2f)
                {
                    // East (right)
                    currentDirection = CardinalDirection.East;
                    rb.MoveRotation(facingEast);
                    currentMovement = Vector3.right * moveSpeed * Time.deltaTime;
                }
                else if (moveInputX < -0.2f)
                {
                    // West (left)
                    currentDirection = CardinalDirection.West;
                    rb.MoveRotation(facingWest);
                    currentMovement = Vector3.left * moveSpeed * Time.deltaTime;
                }
            }
            else if (Mathf.Abs(moveInputY) > 0)
            {
                // Vertical movement
                if (moveInputY > 0.2f)
                {
                    // North (forward)
                    currentDirection = CardinalDirection.North;
                    rb.MoveRotation(facingNorth);
                    currentMovement = Vector3.forward * moveSpeed * Time.deltaTime;
                }
                else if (moveInputY < -0.2f)
                {
                    // South (backward)
                    currentDirection = CardinalDirection.South;
                    rb.MoveRotation(facingSouth);
                    currentMovement = Vector3.back * moveSpeed * Time.deltaTime;
                }
            }
            
            // Update last movement direction if moving
            if (currentMovement != Vector3.zero)
            {
                lastMovementDirection = currentMovement.normalized;
            }
            
            MoveWithCollisionCheck();
        }
        
        void MoveWithCollisionCheck()
        {
            // Move using the rigidbody (with collision check)
            if (currentMovement != Vector3.zero)
            {
                // Check for collisions in the movement direction
                if (!WouldCollide(currentMovement))
                {
                    // Apply movement in world space if no collisions
                    rb.MovePosition(rb.position + currentMovement);
                }
                else
                {
                    // Try sliding along walls by checking X and Z components separately
                    Vector3 xMovement = new Vector3(currentMovement.x, 0, 0);
                    Vector3 zMovement = new Vector3(0, 0, currentMovement.z);
                    
                    if (currentMovement.x != 0 && !WouldCollide(xMovement))
                    {
                        rb.MovePosition(rb.position + xMovement);
                    }
                    
                    if (currentMovement.z != 0 && !WouldCollide(zMovement))
                    {
                        rb.MovePosition(rb.position + zMovement);
                    }
                }
            }
        }
        
        bool WouldCollide(Vector3 movementVector)
        {
            // The direction and distance to check
            Vector3 direction = movementVector.normalized;
            float distance = movementVector.magnitude + collisionCheckDistance;
            
            // Get the collider bounds
            Collider tankCollider = GetComponent<Collider>();
            if (tankCollider == null)
            {
                // Simple raycast if no collider
                RaycastHit hit;
                if (Physics.Raycast(transform.position, direction, out hit, distance, obstacleLayer))
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red, 0.1f);
                    return true;
                }
            }
            else
            {
                // BoxCast for better collision detection with the tank's shape
                Vector3 center = tankCollider.bounds.center;
                Vector3 halfExtents = tankCollider.bounds.extents;
                Quaternion orientation = transform.rotation;
                
                RaycastHit hit;
                if (Physics.BoxCast(center, halfExtents, direction, out hit, orientation, distance, obstacleLayer))
                {
                    Debug.DrawRay(center, direction * hit.distance, Color.red, 0.1f);
                    return true;
                }
            }
            
            // No collision detected
            Debug.DrawRay(transform.position, direction * distance, Color.green, 0.1f);
            return false;
        }

        void HandleShooting()
        {
            bool canShoot = Time.time - lastFireTime >= fireRate;
            bool shouldShoot = Input.GetKeyDown(KeyCode.Space);
            bool movementCheck = canShootWhileMoving || lastMovementDirection == Vector3.zero;
            
            if (shouldShoot && canShoot && movementCheck)
            {
                // Choose shooting method
                if (bulletPrefab != null && bulletSpawnPoint != null)
                {
                    ShootProjectile();
                }
                else
                {
                    ShootRaycast();
                }
                
                lastFireTime = Time.time;
            }
        }

        void ShootProjectile()
        {
            if (ProjectilePool.Instance != null && bulletSpawnPoint != null)
            {
                GameObject proj = ProjectilePool.Instance.GetProjectile();
                if (proj != null)
                {
                    proj.transform.position = bulletSpawnPoint.position;
                    proj.transform.rotation = bulletSpawnPoint.rotation;
                    proj.SetActive(true);
                    
                    // Show muzzle flash
                    if (muzzleFlashPrefab != null)
                    {
                        GameObject flash = Instantiate(muzzleFlashPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                        Destroy(flash, 0.1f); // Auto-destroy after a short time
                    }
                }
            }
        }
        
        void ShootRaycast()
        {
            Transform shootOrigin = (tankModel != null) ? tankModel : transform;
            Vector3 shootDirection = shootOrigin.forward;
            Vector3 startPoint = bulletSpawnPoint != null ? bulletSpawnPoint.position : shootOrigin.position;
            
            RaycastHit hit;
            bool hitSomething = Physics.Raycast(startPoint, shootDirection, out hit, raycastRange, targetLayerMask);
            
            // Set end point based on whether something was hit
            Vector3 endPoint = hitSomething ? hit.point : startPoint + (shootDirection * raycastRange);
            
            // Show muzzle flash
            if (muzzleFlashPrefab != null)
            {
                GameObject flash = Instantiate(muzzleFlashPrefab, startPoint, Quaternion.LookRotation(shootDirection));
                Destroy(flash, 0.1f);
            }
            
            // Show hit effect if something was hit
            if (hitSomething && hitEffectPrefab != null)
            {
                GameObject hitEffect = Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitEffect, 1f);
                
                // Apply damage or effects to the hit object here
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(1);
                }
            }
            
            // Visualize the ray
            StartCoroutine(ShowShootingRay(startPoint, endPoint));
        }
        
        IEnumerator ShowShootingRay(Vector3 startPoint, Vector3 endPoint)
        {
            shotLineRenderer.enabled = true;
            shotLineRenderer.SetPosition(0, startPoint);
            shotLineRenderer.SetPosition(1, endPoint);
            
            yield return new WaitForSeconds(rayDuration);
            
            shotLineRenderer.enabled = false;
        }
    }
    
    // Interface for objects that can take damage
    public interface IDamageable
    {
        void TakeDamage(int amount);
    }
}
