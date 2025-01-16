using System;
using UnityEngine;

public class PaintingManager : MonoBehaviour
{
    [SerializeField] private Transform pen;
    [SerializeField] private Transform pencil;
    [SerializeField] private Transform brush;
    [SerializeField] private Transform can;
    [SerializeField] private Transform pipe;
    [SerializeField] private ParticleSystem sprayParticles;

    private Vector3 penInitialPosition;
    private Vector3 pencilInitialPosition;
    private Vector3 brushInitialPosition;
    private Vector3 canInitialPosition;
    private Vector3 pipeInitialPosition;

    private Quaternion penInitialRotation;
    private Quaternion pencilInitialRotation;
    private Quaternion brushInitialRotation;
    private Quaternion canInitialRotation;
    private Quaternion pipeInitialRotation;

    private Transform activeTool;

    private void Start()
    {
        // Store initial positions and rotations of the tools
        penInitialPosition = pen.position;
        penInitialRotation = pen.rotation;

        pencilInitialPosition = pencil.position;
        pencilInitialRotation = pencil.rotation;

        brushInitialPosition = brush.position;
        brushInitialRotation = brush.rotation;

        canInitialPosition = can.position;
        canInitialRotation = can.rotation;

        pipeInitialPosition = pipe.position;
        pipeInitialRotation = pipe.rotation;
    }

    private void ResetTransformExcept(Transform active)
    {
        // Reset position and rotation of all tools except the active one
        if (active != pen)
        {
            pen.position = penInitialPosition;
            pen.rotation = penInitialRotation;
        }

        if (active != pencil)
        {
            pencil.position = pencilInitialPosition;
            pencil.rotation = pencilInitialRotation;
        }

        if (active != brush)
        {
            brush.position = brushInitialPosition;
            brush.rotation = brushInitialRotation;
        }

        if (active != can)
        {
            can.position = canInitialPosition;
            can.rotation = canInitialRotation;
        }

        if (active != pipe)
        {
            pipe.position = pipeInitialPosition;
            pipe.rotation = pipeInitialRotation;
        }
    }

    public void SelectTool(string toolName)
    {
        // Deactivate all tools first
        pen.gameObject.SetActive(false);
        pencil.gameObject.SetActive(false);
        brush.gameObject.SetActive(false);
        can.gameObject.SetActive(false);
        pipe.gameObject.SetActive(false);

        // Find and activate the selected tool
        switch (toolName.ToLower())
        {
            case "pen":
                activeTool = pen;
                break;
            case "pencil":
                activeTool = pencil;
                break;
            case "brush":
                activeTool = brush;
                break;
            case "can":
                activeTool = can;
                break;
            case "pipe":
                activeTool = pipe;
                break;
            default:
                Debug.LogError($"Tool {toolName} not found!");
                return;
        }

        activeTool.gameObject.SetActive(true);
        ResetTransformExcept(activeTool);
    }
    
    public void ChangeLineColor(string colorHex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(colorHex, out color))
        {
            if (activeTool.GetComponent<DrawingPen>())
            {
                activeTool.GetComponent<DrawingPen>().lineRendererPreset.startColor = color;
                activeTool.GetComponent<DrawingPen>().lineRendererPreset.endColor = color;
            }
            else if (activeTool.GetComponent<SprayCan>())
            {
                ChangeSprayParticlesColor(color);
            }
        }
        else
        {
            Debug.LogError($"Invalid color hex code: {colorHex}");
        }
    }

    private void ChangeSprayParticlesColor(Color color)
    {
        sprayParticles.startColor = color;
    }
}
