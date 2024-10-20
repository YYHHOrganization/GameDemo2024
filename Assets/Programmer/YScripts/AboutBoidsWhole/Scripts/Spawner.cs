﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这段代码用于在场景中生成一群Boid
/// </summary>
public class Spawner : MonoBehaviour {

    public enum GizmoType { Never, SelectedOnly, Always }

    public Boid prefab;
    public float spawnRadius = 10;
    public int spawnCount = 10;
    public Color colour;
    public GizmoType showSpawnRegion;

    void Awake () 
    {
        return;
        for (int i = 0; i < spawnCount; i++) 
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            Boid boid = Instantiate (prefab);
            boid.transform.position = pos;
            boid.transform.forward = Random.insideUnitSphere;

            boid.SetColour (colour);
        }
    }
    
    Boid[] boids;
    private BoidManager boidManager;
    public void SpawnBoid (BoidManager boidManager) 
    {
        //初始化boids数组，根据其元素个数
        boids = new Boid[spawnCount];
        
        for (int i = 0; i < spawnCount; i++) 
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            Boid boid = Instantiate (prefab,transform);
            boid.transform.position = pos;
            boid.transform.forward = Random.insideUnitSphere;

            boid.SetColour (colour);
            //加入到boids数组中
            boids[i] = boid;
        }
        
        //将boids数组传递给BoidManager
        //在其parent  BoidManager节点之下，寻找BoidManager组件，
        
        if (boidManager != null) 
        {
            boidManager.SetAndStartBoids (boids);
        }
    }

    //GizmoType.Always表示始终显示，（在Editor模式下）
    //GizmoType.SelectedOnly表示只有选中时显示（在Editor模式下）
    //GizmoType.Never表示永不显示（在Editor模式下）
    
    private void OnDrawGizmos () 
    {
        if (showSpawnRegion == GizmoType.Always) 
        {
            DrawGizmos ();
        }
    }

    void OnDrawGizmosSelected () 
    {
        if (showSpawnRegion == GizmoType.SelectedOnly) 
        {
            DrawGizmos ();
        }
    }

    void DrawGizmos () 
    {

        Gizmos.color = new Color (colour.r, colour.g, colour.b, 0.3f);
        Gizmos.DrawSphere (transform.position, spawnRadius);
    }

    
}