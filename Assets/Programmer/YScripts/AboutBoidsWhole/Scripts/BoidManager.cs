using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour {

    const int threadGroupSize = 1024;

    public BoidSettings settings;
    public ComputeShader compute;
    Boid[] boids;
    public Transform target;
    public Spawner FishspSawner;
    bool canStartSimu = false;
    void Start ()
    {
        //FishspSawner = gameObject.GetComponentInChildren<Spawner>();
        return;
        boids = FindObjectsOfType<Boid> ();
        foreach (Boid b in boids) 
        {
            b.Initialize (settings, null);
            //b.Initialize (settings, target);
        }
    }
    
    void Update () 
    {
        if(!canStartSimu)return;
        if (boids != null)
        {
            int numBoids = boids.Length;
            var boidData = new BoidData[numBoids];

            for (int i = 0; i < boids.Length; i++) 
            {
                boidData[i].position = boids[i].position;
                boidData[i].direction = boids[i].forward;
            }

            var boidBuffer = new ComputeBuffer (numBoids, BoidData.Size);
            boidBuffer.SetData (boidData);

            compute.SetBuffer (0, "boids", boidBuffer);
            compute.SetInt ("numBoids", boids.Length);
            compute.SetFloat ("viewRadius", settings.perceptionRadius);
            compute.SetFloat ("avoidRadius", settings.avoidanceRadius);
            
            //threadGroupSize=1024 是线程组的大小，这里的计算是为了确保每个线程组都有1024个线程
            int threadGroups = Mathf.CeilToInt (numBoids / (float) threadGroupSize);
            
            //使用一维向量是因为需要处理的数据没有类似材质、体积之类的三维结构体；1024是为了直接把线程拉满
            compute.Dispatch (0, threadGroups, 1, 1);

            boidBuffer.GetData (boidData);

            for (int i = 0; i < boids.Length; i++) 
            {
                boids[i].avgFlockHeading = boidData[i].flockHeading;
                boids[i].centreOfFlockmates = boidData[i].flockCentre;
                boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

                boids[i].UpdateBoid ();
            }

            boidBuffer.Release ();
        }
    }

    public struct BoidData
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

        public static int Size 
        {
            get 
            {
                //5个Vector3和一个int
                return sizeof (float) * 3 * 5 + sizeof (int);
            }
        }
    }

    public void SetAndStartBoids(Boid[] boids1)
    {
        boids = boids1;
        //开始模拟
        StartSimulation();
    }
    public void StartBoids(Transform target=null)
    {
        this.target = target;
        canStartSimu = true;
        FishspSawner.SpawnBoid(this);
    }
    public void StopBoids()
    {
        canStartSimu = false;
        DestroyBoid();
    }

    private void StartSimulation()
    {
        canStartSimu = true;
        foreach (Boid b in boids) 
        {
            //b.Initialize (settings, null);
            b.Initialize (settings, target);
        }
    }
    public void DestroyBoid()
    {
        for (int i = 0; i < boids.Length; i++)
        {
            Destroy(boids[i].gameObject);
        }
    }
}