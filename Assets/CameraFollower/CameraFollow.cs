using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform objectTransform;
    private Vector3 objectVelocity = Vector3.zero;

    [SerializeField]
    private float smoothTime = 0.125f;

    [SerializeField]
    private Camera cameraComponent;

    //[SerializeField]
    //private bool followPhysicsTarget = false;
    
    public List<Transform> transformTargets;
    //public List<Rigidbody> rigidbodyTargets;

    private float zOffset = 10f;

    [SerializeField, Range(0.0f, 1.0f)]
    private float minXViewportThreshold = 0.3f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float maxXViewportThreshold = 0.7f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float minYViewportThreshold = 0.3f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float maxYViewportThreshold = 0.7f;
    
    private void Awake()
    {
        this.objectTransform = GetComponent<Transform>();
    }

    private bool IsHorizontalThresholdCrossed()
    {        
        for (int i = 0; i < this.transformTargets.Count; i++)
        {
            Vector3 viewportPosition = this.cameraComponent.WorldToViewportPoint(this.transformTargets[i].position);

            if (viewportPosition.x <= this.minXViewportThreshold ||
                viewportPosition.x >= this.maxXViewportThreshold)
            {
                return true;
            }            
        }

        return false;
    }

    private bool IsVerticalThresholdCrossed()
    {
        for (int i = 0; i < this.transformTargets.Count; i++)
        {
            Vector3 viewportPosition = this.cameraComponent.WorldToViewportPoint(this.transformTargets[i].position);

            if (viewportPosition.y <= this.minYViewportThreshold ||
                viewportPosition.y >= this.maxYViewportThreshold)
            {
                return true;
            }
        }

        return false;
    }

    private List<Vector2> GetMinMaxValues()
    {
        List<Vector2> minMaxValues = new List<Vector2>();

        Vector2 minMaxX = new Vector2(Mathf.Infinity, Mathf.NegativeInfinity);
        Vector2 minMaxY = new Vector2(Mathf.Infinity, Mathf.NegativeInfinity);
        Vector2 minMaxZ = new Vector2(Mathf.Infinity, Mathf.NegativeInfinity);

        for (int i = 0; i < this.transformTargets.Count; i++)
        {
            //X Values
            if (this.transformTargets[i].position.x < minMaxX[0])
            {
                minMaxX[0] = this.transformTargets[i].position.x;
            }
            if (this.transformTargets[i].position.x < minMaxX[1])
            {
                minMaxX[1] = this.transformTargets[i].position.x;
            }

            //Y Values
            if (this.transformTargets[i].position.y < minMaxY[0])
            {
                minMaxY[0] = this.transformTargets[i].position.y;
            }
            if (this.transformTargets[i].position.y < minMaxY[1])
            {
                minMaxY[1] = this.transformTargets[i].position.y;
            }

            //Z Values
            if (this.transformTargets[i].position.z < minMaxZ[0])
            {
                minMaxZ[0] = this.transformTargets[i].position.z;
            }
            if (this.transformTargets[i].position.z < minMaxZ[1])
            {
                minMaxZ[1] = this.transformTargets[i].position.z;
            }
        }

        minMaxValues.Add(minMaxX);
        minMaxValues.Add(minMaxY);
        minMaxValues.Add(minMaxZ);

        return minMaxValues;
    }
    
    private Vector3 GetCentralizedPoint()
    {
        if (this.transformTargets.Count == 1)
        {
            return this.transformTargets[0].position;
        }

        List<Vector2> minMaxValues = this.GetMinMaxValues();
        Vector3 centralPoint = Vector3.zero;

        centralPoint.x = ((minMaxValues[0][0] + minMaxValues[0][1]) / 2.0f);
        centralPoint.y = ((minMaxValues[1][0] + minMaxValues[1][1]) / 2.0f);
        centralPoint.z = ((minMaxValues[2][0] + minMaxValues[2][1]) / 2.0f);

        return centralPoint;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetPosition = this.GetCentralizedPoint();
        this.objectTransform.position = Vector3.SmoothDamp(this.objectTransform.position, targetPosition, ref this.objectVelocity, this.smoothTime);
    }
}
