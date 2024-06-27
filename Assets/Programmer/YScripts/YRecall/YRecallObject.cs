using UnityEngine;

/// <summary>
/// 用于存储物体的状态和时间戳。
/// </summary>
public class YRecallObject
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Velocity { get; set; }
    public Vector3 AngularVelocity { get; set; }
    public Vector3 Force { get; set; }
    public float TimeStamp { get; set; }
}