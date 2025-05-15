using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// Manages the scrolling of the game world for the endless runner
public class WorldScroller : MonoBehaviour
{
    private enum ScrollMode { Idle, Game }
    private ScrollMode currentMode = ScrollMode.Idle;
    public bool isGameRunning = false;

    [Header("Scrolling Settings")]
    [SerializeField] private float baseScrollSpeed = 5f; // Base speed of world scrolling
    [SerializeField] private float speedIncreaseRate = 0.1f; // Speed increase per second
    [SerializeField] private float maxScrollSpeed = 15f; // Maximum scroll speed

    [Header("Segment Settings")]
    [SerializeField] private GameObject segmentPrefab; // Prefab for ground segments
    [SerializeField] private float segmentWidth = 10f; // Width of each segment
    [SerializeField] private int initialSegmentCount = 3; // Number of segments to initialize
    [SerializeField] private float pivotOffset = 0f; // Offset for non-left-aligned pivots (e.g., segmentWidth/2 for centered)
    [SerializeField] private Transform segmentPos;

    private List<GameObject> segmentPool = new List<GameObject>(); // Pool of ground segments
    private Queue<GameObject> activeSegments = new Queue<GameObject>(); // Currently visible segments
    private float currentScrollSpeed; // Current speed of scrolling
    private float totalDistance; // Total distance traveled (for scoring)
    private float scoreBoosterMultiplier = 1f;
    private Coroutine scoreBoosterCoroutine;


    private void Start()
    {
        EventManager.Instance.OnGameStart += SetGameStatusOn;
        EventManager.Instance.OnGameOver += SetGameStatusOff;
        EventManager.Instance.OnGameRestart += ResetScroller;
        EventManager.Instance.OnScoreBoostCollected += UpdateScoreBooster;

        currentScrollSpeed = baseScrollSpeed;
        // Calculate minimum initialSegmentCount to cover screen
        float viewportWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        initialSegmentCount = Mathf.Max(initialSegmentCount, Mathf.CeilToInt(viewportWidth / segmentWidth) + 1);
        InitializeSegmentPool();
        SpawnInitialSegments();
        ValidateSegmentWidth();
    }

    private void Update()
    {
        float moveDistance = 0f;

        if (currentMode == ScrollMode.Game)
        {
            currentScrollSpeed = Mathf.Min(currentScrollSpeed + speedIncreaseRate * Time.deltaTime, maxScrollSpeed);
            moveDistance = currentScrollSpeed * Time.deltaTime * scoreBoosterMultiplier;

            totalDistance += moveDistance;
            EventManager.Instance.TriggerDistanceUpdated(totalDistance);
            EventManager.Instance.TriggerOnWorldSpeedUpdate(currentScrollSpeed * Time.deltaTime);
        }
        else if (currentMode == ScrollMode.Idle)
        {
            moveDistance = baseScrollSpeed * Time.deltaTime;
        }

        foreach (GameObject segment in activeSegments)
            segment.transform.Translate(Vector3.left * moveDistance);

        TryRecycleSegment();
    }

    private void TryRecycleSegment()
    {
        if (activeSegments.Count == 0) return;

        GameObject oldestSegment = activeSegments.Peek();
        float segmentRightEdge = oldestSegment.transform.position.x + segmentWidth / 2f - pivotOffset;
        float leftViewportEdge = Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect;

        if (segmentRightEdge < leftViewportEdge)
            RecycleSegment();
    }

    private void UpdateScoreBooster(float duration)
    {
        if (scoreBoosterCoroutine == null)
        {
            scoreBoosterCoroutine = StartCoroutine(UpdateScoreBooster_Coroutine(duration));
        }
        else
        {
            StopAllCoroutines();
            scoreBoosterCoroutine = StartCoroutine(UpdateScoreBooster_Coroutine(duration));
        }
    }

    IEnumerator UpdateScoreBooster_Coroutine(float duration)
    {
        scoreBoosterMultiplier = 2;
        yield return new WaitForSeconds(duration);
        scoreBoosterMultiplier = 1;
        scoreBoosterCoroutine = null;
    }


    private void SetGameStatusOn() => currentMode = ScrollMode.Game;
    private void SetGameStatusOff() => currentMode = ScrollMode.Idle;

    // Initialize the object pool with inactive segments
    private void InitializeSegmentPool()
    {
        for (int i = 0; i < initialSegmentCount + 1; i++)
        {
            GameObject segment = Instantiate(segmentPrefab, new Vector3(segmentPos.position.x,segmentPos.position.y, segmentPos.position.z), Quaternion.identity);
            segment.SetActive(false);
            segmentPool.Add(segment);
        }
    }

    // Spawn initial segments to cover the visible area
    private void SpawnInitialSegments()
    {
        Vector3 spawnPos = segmentPos.position;
        for (int i = 0; i < initialSegmentCount; i++)
        {
            GameObject segment = GetPooledSegment();
            if (segment != null)
            {
                segment.transform.position = spawnPos + Vector3.right * pivotOffset;
                segment.SetActive(true);
                activeSegments.Enqueue(segment);
                spawnPos.x += segmentWidth;
            }
        }
    }

    // Recycle the oldest segment to the end
    private void RecycleSegment()
    {
        GameObject segment = activeSegments.Dequeue();
        // Position based on the last active segment's right edge
        Vector3 newPos = Vector3.zero;
        if (activeSegments.Count > 0)
        {
            GameObject lastSegment = activeSegments.ToArray()[activeSegments.Count - 1];
            newPos = lastSegment.transform.position + Vector3.right * segmentWidth;
        }
        else
        {
            newPos = activeSegments.ToArray()[activeSegments.Count - 1].transform.position + Vector3.right * segmentWidth;
        }
        newPos.x += pivotOffset;
        segment.transform.position = newPos;
        activeSegments.Enqueue(segment);
    }

    // Get an inactive segment from the pool
    private GameObject GetPooledSegment()
    {
        foreach (GameObject segment in segmentPool)
        {
            if (!segment.activeInHierarchy)
            {
                return segment;
            }
        }
        // If no inactive segment is available, instantiate a new one
        GameObject newSegment = Instantiate(segmentPrefab, Vector3.zero, Quaternion.identity);
        segmentPool.Add(newSegment);
        return newSegment;
    }

    // Validate segment width using sprite bounds or collider
    private void ValidateSegmentWidth()
    {
        if (segmentPrefab == null) return;

        SpriteRenderer sprite = segmentPrefab.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            float spriteWidth = sprite.bounds.size.x;
            if (Mathf.Abs(spriteWidth - segmentWidth) > 0.01f)
            {
                Debug.LogWarning($"Segment width ({segmentWidth}) does not match sprite width ({spriteWidth}). Consider updating segmentWidth.");
            }
        }

        BoxCollider2D collider = segmentPrefab.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            float colliderWidth = collider.size.x * segmentPrefab.transform.localScale.x;
            if (Mathf.Abs(colliderWidth - segmentWidth) > 0.01f)
            {
                Debug.LogWarning($"Segment width ({segmentWidth}) does not match collider width ({colliderWidth}). Consider updating segmentWidth.");
            }
        }
    }

    // Get the current scroll speed (for other systems)
    public float GetScrollSpeed()
    {
        return currentScrollSpeed;
    }

    // Get the total distance traveled (for scoring)
    public float GetTotalDistance()
    {
        return totalDistance;
    }

    // Reset the scroller for a new game
    public void ResetScroller()
    {
        currentScrollSpeed = baseScrollSpeed;
        totalDistance = 0f;

        // Deactivate all segments
        foreach (GameObject segment in activeSegments)
        {
            segment.SetActive(false);
        }
        activeSegments.Clear();

        // Respawn initial segments
        SpawnInitialSegments();
    }

    // Debug visualization of segment bounds and recycling trigger
    private void OnDrawGizmos()
    {
        if (activeSegments == null) return;
        Gizmos.color = Color.green;
        foreach (GameObject segment in activeSegments)
        {
            if (segment != null)
            {
                Vector3 center = segment.transform.position + Vector3.right * pivotOffset;
                Gizmos.DrawWireCube(center, new Vector3(segmentWidth, 1f, 1f));
            }
        }
        // Draw left viewport edge
        Gizmos.color = Color.red;
        float leftEdge = Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect;
        Gizmos.DrawLine(new Vector3(leftEdge, -10, 0), new Vector3(leftEdge, 10, 0));
    }
}