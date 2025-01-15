using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayCan : MonoBehaviour
{
    [SerializeField] private GameObject particles;
    [SerializeField] private GameObject audio;
    [SerializeField] private GameObject canvas;             // The canvas to paint on
    [SerializeField] private float paintStrength = 1.0f;    // Intensity of the paint
    [SerializeField] private float maxDistance = 5.0f;      // Maximum spray distance
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
        // Check if spray is active (e.g., button press)
        if (sprayParticles.isPlaying)
        {
            PaintOnCanvas();
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
    private void PaintOnCanvas()
    {
        if (canvas == null || canvasTexture == null) return;

        // Raycast to detect canvas
        Ray ray = new Ray(sprayParticles.transform.position, sprayParticles.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            if (hit.collider.gameObject == canvas)
            {
                // Calculate distance-based paint effect
                float distance = hit.distance;
                float paintEffect = Mathf.Clamp(1.0f / distance * paintStrength, 0, 1);

                // Convert hit point to texture UV
                Vector2 uv = hit.textureCoord;

                // Paint the texture
                PaintTexture(uv, paintColor * paintEffect);
            }
        }
    }

    private void PaintTexture(Vector2 uv, Color color)
    {
        int x = Mathf.FloorToInt(uv.x * canvasTexture.width);
        int y = Mathf.FloorToInt(uv.y * canvasTexture.height);

        // Apply paint color to the texture
        canvasTexture.SetPixel(x, y, color);
        canvasTexture.Apply();
    }
}

