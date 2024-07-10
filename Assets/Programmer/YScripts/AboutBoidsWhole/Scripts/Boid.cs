using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {

    BoidSettings settings;

    // State
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    Vector3 velocity;

    // To update:
    Vector3 acceleration;
    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;
    [HideInInspector]
    public int numPerceivedFlockmates;

    // Cached
    Material material;
    Transform cachedTransform;
    Transform target;

    void Awake () 
    {
        material = transform.GetComponentInChildren<MeshRenderer> ().material;
        cachedTransform = transform;
    }

    public void Initialize (BoidSettings settings, Transform target) 
    {
        this.target = target;
        this.settings = settings;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }

    public void SetColour (Color col) 
    {
        if (material != null) 
        {
            material.color = col;
        }
    }

    public void UpdateBoid () 
    {
        Vector3 acceleration = Vector3.zero;

        if (target != null) 
        {
            Vector3 offsetToTarget = (target.position - position);
            acceleration = SteerTowards (offsetToTarget) * settings.targetWeight;
        }

        
        //numPerceivedFlockmates是感知到的群体成员数量，如果这个数量不为0，那么就计算出群体的中心位置centreOfFlockmates。
        if (numPerceivedFlockmates != 0) 
        {
            centreOfFlockmates /= numPerceivedFlockmates;

            Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            var alignmentForce = SteerTowards (avgFlockHeading) * settings.alignWeight;
            //cohesionForce是指向群体中心的力
            var cohesionForce = SteerTowards (offsetToFlockmatesCentre) * settings.cohesionWeight;
            //seperationForce是避免与群体成员碰撞的力。
            var seperationForce = SteerTowards (avgAvoidanceHeading) * settings.seperateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }

        if (IsHeadingForCollision ()) 
        {
            //collisionAvoidDir是一个方向向量，这个方向向量是避免碰撞的方向。
            Vector3 collisionAvoidDir = ObstacleRays ();
            Vector3 collisionAvoidForce = SteerTowards (collisionAvoidDir) * settings.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp (speed, settings.minSpeed, settings.maxSpeed);
        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
        forward = dir;
    }

    /// <summary>
    /// 检测是否会发生碰撞
    /// </summary>
    /// <returns></returns>
    bool IsHeadingForCollision () 
    {
        RaycastHit hit;
        if (Physics.SphereCast (position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask)) {
            return true;
        } else { }
        return false;
    }

    // 在进行正面避障时，如果提供的射线少了则会使得鱼的速度方向变化过大，不够平滑，
    // 所以正面避障直接采用黄金螺旋法在球面上等距取点，从正面开始遍历这些点，一旦取到一个可转向的范围就break即可
    Vector3 ObstacleRays () 
    {
        //BoidHelper计算黄金螺旋法那些东西
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++) 
        {
            Vector3 dir = cachedTransform.TransformDirection (rayDirections[i]);
            Ray ray = new Ray (position, dir);
            if (!Physics.SphereCast (ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask)) 
            {
                return dir;
            }
        }

        return forward;
    }

    /// <summary>
    ///  SteerTowards作用是计算
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    Vector3 SteerTowards (Vector3 vector)
    {
        //这个向量的方向是朝向输入的vector，但是其大小（长度）被限制在了settings.maxSteerForce的范围内。 
        //这个方向是由velocity末尾指向vector末尾，也就是说，这个方向是从当前位置指向目标位置的方向。
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        //ClampMagnitude将结果向量的长度限制在settings.maxSteerForce的范围（http://www.vfkjsd.cn/unity/Script/Vector3/Vector3.ClampMagnitude.html）
        return Vector3.ClampMagnitude (v, settings.maxSteerForce);
    }

}