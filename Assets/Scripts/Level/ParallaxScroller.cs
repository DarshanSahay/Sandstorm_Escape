using UnityEngine;

public class ParallaxScroller : MonoBehaviour
{
    [Tooltip("Speed multiplier relative to world scroll speed (0 = static, 1 = full speed)")]
    [Range(0f, 1f)] public float parallaxMultiplier = 0.5f;

    [SerializeField] private Material mat;
    [SerializeField] private float distance;

    private void Start()
    {
        mat.SetTextureOffset("_MainTex", Vector2.zero);
    }

    private void Update()
    {
        distance += parallaxMultiplier * Time.deltaTime;
        mat.SetTextureOffset("_MainTex", Vector2.right * distance);
    }
}
