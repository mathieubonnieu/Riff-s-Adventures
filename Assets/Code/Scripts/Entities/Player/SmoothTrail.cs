using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using UnityEditor;

[RequireComponent(typeof(TrailRenderer))]
[ExecuteInEditMode]
public class SmoothTrail : MonoBehaviour
{
    [Header("Smoothing Settings")]
    [Range(1, 10)] public int smoothingIterations = 3;
    [Range(0.1f, 1f)] public float smoothingFactor = 0.5f;
    [Range(2, 20)] public int interpolationPoints = 5;
    [Range(10, 1000)] public float maxNumberOfPoint = 10f;

    public bool activated;
    
    private TrailRenderer trailRenderer;
    private List<Vector3> originalPositions = new List<Vector3>();
    private List<Vector3> debugGizmo = new List<Vector3>();
    private List<Vector3> smoothedPositions = new List<Vector3>();

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        DeactivateTrail();
    }

    public void ActivateTrail()
    {
        activated = true;
        trailRenderer.enabled = true;
    }

    public void DeactivateTrail()
    {
        activated = false;
        trailRenderer.Clear();
        trailRenderer.enabled = false;
        originalPositions.Clear();
    }

    void Update()
    {
        if (!activated)
        {
            return;
        }
        // Get current positions from the trail renderer
        Vector3[] positions = new Vector3[trailRenderer.positionCount];
        trailRenderer.GetPositions(positions);

        if (positions.Length == 0) return;
        // if last position is different from original, add it to the list
        if (originalPositions.Count == 0 || Vector3.Distance(originalPositions.Last(), positions.Last()) > 0.01f)
        {
            originalPositions.Add(positions.Last());
        }

        // Until size of originalPositions is more than positions, remove
        while (originalPositions.Count > positions.Length)
        {
            originalPositions.RemoveAt(0);
        }
        
        
        if (originalPositions.Count < 2) return;

        debugGizmo.Clear();
        debugGizmo.AddRange(originalPositions);


        // Apply smoothing
        smoothedPositions = SmoothTrailPositions(originalPositions);
        
        // maxNumberOfPoint
        if (smoothedPositions.Count > maxNumberOfPoint)
        {
            smoothedPositions = smoothedPositions.Skip(smoothedPositions.Count - (int)maxNumberOfPoint).ToList();
        }

        trailRenderer.Clear();
        foreach (Vector3 point in smoothedPositions)
        {
            trailRenderer.AddPosition(point);
        }
    }

    List<Vector3> SmoothTrailPositions(List<Vector3> inputPositions)
    {
        List<Vector3> smoothed = new(inputPositions);
        
        // Apply multiple smoothing passes
        for (int i = 0; i < smoothingIterations; i++)
        {
            smoothed = ApplySmoothingPass(smoothed);
        }
        
        // Apply Bézier curve interpolation
        return ApplyBezierInterpolation(smoothed);
    }

    List<Vector3> ApplySmoothingPass(List<Vector3> positions)
    {
        if (positions.Count < 3) return positions;
        
        List<Vector3> smoothed = new List<Vector3>();
        
        // Always keep the first point
        smoothed.Add(positions[0]);
        
        // Smooth middle points
        for (int i = 1; i < positions.Count - 1; i++)
        {
            Vector3 prev = positions[i - 1];
            Vector3 current = positions[i];
            Vector3 next = positions[i + 1];
            
            // Calculate smoothed position
            Vector3 smoothedPos = current * (1 - smoothingFactor) + 
                                 (prev + next) * 0.5f * smoothingFactor;
            
            smoothed.Add(smoothedPos);
        }
        
        // Always keep the last point
        smoothed.Add(positions[positions.Count - 1]);
        
        return smoothed;
    }

    List<Vector3> ApplyBezierInterpolation(List<Vector3> positions)
    {
        if (positions.Count < 2) return positions;
        
        List<Vector3> interpolated = new List<Vector3>();
        
        // Add first point
        interpolated.Add(positions[0]);
        
        // Interpolate between points
        for (int i = 0; i < positions.Count - 1; i++)
        {
            Vector3 p0 = positions[i];
            Vector3 p1 = positions[i + 1];
            
            // For quadratic Bézier, we need a control point
            Vector3 controlPoint = Vector3.Lerp(p0, p1, 0.5f);
            
            // If we have a next point, adjust control point for smoother curve
            if (i < positions.Count - 2)
            {
                Vector3 p2 = positions[i + 2];
                controlPoint += (p1 - p2) * 0.1f;
            }
            
            // Generate interpolated points
            for (int j = 1; j <= interpolationPoints; j++)
            {
                float t = j / (float)interpolationPoints;
                Vector3 point = CalculateQuadraticBezierPoint(t, p0, controlPoint, p1);
                interpolated.Add(point);
            }
        }
        
        return interpolated;
    }

    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        
        Vector3 p = uu * p0; // First term
        p += 2 * u * t * p1; // Second term
        p += tt * p2; // Third term
        
        return p;
    }

    // void OnDrawGizmos()
    // {
    //     if (debugGizmo.Count < 2) return;
    //     // Draw sphere on each point in the original trail
    //     Gizmos.color = Color.red;
    //     foreach (Vector3 point in debugGizmo)
    //     {
    //         Gizmos.DrawSphere(point, 0.1f);
    //         // Draw a number on the sphere
    //         GUIStyle style = new GUIStyle();
    //         style.normal.textColor = Color.white;
    //         style.fontSize = 20;
    //         Handles.Label(point, debugGizmo.IndexOf(point).ToString(), style);
    //     }

    //     foreach (Vector3 point in smoothedPositions)
    //     {
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawSphere(point, 0.05f);
    //     }
    // }
}
