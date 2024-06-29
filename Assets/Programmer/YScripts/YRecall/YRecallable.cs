using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在物体（例如，Rigidbody）上添加一个新的脚本，比如YRecallable.
/// 这个脚本将负责跟踪物体的状态，并在需要时进行回溯
/// </summary>
public class YRecallable : MonoBehaviour
{
    private Rigidbody rb;
    //使用List<YRecallObject>，存储物体在每个FixedUpdate时的状态和时间戳。
    private List<YRecallObject> recallObjects;
    
    //绘制recallObjects中的轨迹
    // Variables for drawing the trajectory
    private LineRenderer lineRenderer;
    public float lineSegmentSpacing = 0.1f;

    public Material Choose_lineRendererMat;//回头可以弄成addlink
    public Material Recall_lineRendererMat;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        recallObjects = new List<YRecallObject>();
        lineRenderer = GetComponent<LineRenderer>();
    }
    
    private bool isMoving = false;
    private Vector3 lastPosition;
    private Vector3 lastVelocity;

    private float lastMoveTime = 0f;
    // The time period to keep recording after the object stops moving
    private float stationaryTime = 0.5f; 
    private void FixedUpdate()
    {
        // Calculate the acceleration
        Vector3 acceleration = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;

        // Check if the object is moving
        if (Vector3.Distance(rb.position, lastPosition) > 0.01f)
        {
            // If the object starts moving, clear the list
            if (!isMoving)
            {
                recallObjects.Clear();
                isMoving = true;
            }

            // Store the object's state
            recallObjects.Add(new YRecallObject
            {
                Position = rb.position,
                Rotation = rb.rotation,
                Velocity = rb.velocity,
                AngularVelocity = rb.angularVelocity,
                Force = rb.mass * acceleration, // Calculate the force using the acceleration
                TimeStamp = Time.time
            });

            // Update the last move time
            lastMoveTime = Time.time;
        }
        else if (Time.time - lastMoveTime <= stationaryTime)
        {
            // If the object is not moving, but the last move time is within the stationary time, keep recording
            recallObjects.Add(new YRecallObject
            {
                Position = rb.position,
                Rotation = rb.rotation,
                Velocity = rb.velocity,
                AngularVelocity = rb.angularVelocity,
                Force = rb.mass * acceleration, // Calculate the force using the acceleration
                TimeStamp = Time.time
            });
        }
        else
        {
            // If the object stops moving and the last move time is beyond the stationary time, stop recording
            isMoving = false;
        }

        // Update the last position and velocity
        lastPosition = rb.position;
        lastVelocity = rb.velocity;
    }
    public IEnumerator Recall()
    {
        // Create a copy of the list to iterate over
        List<YRecallObject> tempRecallObjects = new List<YRecallObject>(recallObjects);
        tempRecallObjects.Reverse();
        foreach (var recallObject in tempRecallObjects)
        {
            // Set the object's position, rotation, velocity, and angular velocity
            rb.position = recallObject.Position;
            rb.rotation = recallObject.Rotation;
            rb.velocity = recallObject.Velocity;
            rb.angularVelocity = recallObject.AngularVelocity;

            // Apply the inverse force
            rb.AddForce(-recallObject.Force, ForceMode.Force);

            // Wait for the next physics update
            yield return new WaitForFixedUpdate();
        }

        // Clear the original list after the recall
        recallObjects.Clear();
    }
    
    
    public void DrawRecallTail()
    {
        lineRenderer.positionCount = recallObjects.Count;
        
        for (int i = 0; i < recallObjects.Count; i++)
        {
            lineRenderer.SetPosition(i, recallObjects[i].Position);
        }
    }
}