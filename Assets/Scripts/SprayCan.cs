using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayCan : MonoBehaviour
{
    [SerializeField] private GameObject particles;
    [SerializeField] private GameObject audio;
    [SerializeField] private GameObject canvas;             // The canvas to paint on
    [SerializeField] private float paintStrength = 1.0f;    // Intensity of the paint
    [SerializeField] private float paintRadius = 5.0f;      // Radius of the painted area
    [SerializeField] private Texture2D canvasTexture;       // The texture to paint on
    private Color paintColor;
    private Renderer canvasRenderer;
    private ParticleSystem sprayParticles;

    private void Start()
    {
        sprayParticles = particles.GetComponent<ParticleSystem>();
        paintColor = sprayParticles.main.startColor.color;
        if (canvas != null)
        {
            canvasRenderer = canvas.GetComponent<Renderer>();
            if (canvasRenderer != null)
            {
                canvasTexture = (Texture2D)canvasRenderer.material.mainTexture;
            }
        }
    }

    private void Update()
    {
        // Check if the RTouch trigger is pressed
        if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            ActivateSpray();
        }
        // Check if the RTouch trigger is released
        else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
        {
            DeactivateSpray();
        }
    }

    private void ActivateSpray()
    {
        particles.SetActive(true);
        audio.SetActive(true);
    }

    private void DeactivateSpray()
    {
        particles.SetActive(false);
        audio.SetActive(false);
    }

    private void OnParticleCollision(GameObject other)
    {
        // Ensure collisions are only handled for the canvas
        if (canvas == null || canvasTexture == null || other != canvas) return;

        // Get collision events
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        int eventCount = sprayParticles.GetCollisionEvents(canvas, collisionEvents);

        for (int i = 0; i < eventCount; i++)
        {
            // Get the collision point in world space
            Vector3 collisionPoint = collisionEvents[i].intersection;

            // Convert world position to texture UV
            if (GetTextureUV(collisionPoint, out Vector2 uv))
            {
                // Apply paint to the texture at the UV coordinates
                PaintTexture(uv, paintColor);
            }
        }
    }

    private bool GetTextureUV(Vector3 worldPosition, out Vector2 uv)
    {
        uv = Vector2.zero;

        // Use raycasting to find the UV coordinates on the canvas
        Ray ray = new Ray(worldPosition + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == canvas)
            {
                uv = hit.textureCoord;
                return true;
            }
        }

        return false;
    }

    private void PaintTexture(Vector2 uv, Color color)
    {
        int x = Mathf.FloorToInt(uv.x * canvasTexture.width);
        int y = Mathf.FloorToInt(uv.y * canvasTexture.height);

        int radius = Mathf.CeilToInt(paintRadius * canvasTexture.width / 10); // Adjust radius for visibility

        // Paint in a radius around the point
        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                if (i * i + j * j <= radius * radius) // Ensure circular area
                {
                    int px = Mathf.Clamp(x + i, 0, canvasTexture.width - 1);
                    int py = Mathf.Clamp(y + j, 0, canvasTexture.height - 1);
                    canvasTexture.SetPixel(px, py, color);
                }
            }
        }

        canvasTexture.Apply();

        // Debug log to confirm painting
        Debug.Log($"Painted at UV: {uv}, Color: {color}, Radius: {radius}");
    }

}
