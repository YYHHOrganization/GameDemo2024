using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBallControll : MonoBehaviour
{
    [SerializeField] bool _update;
    [SerializeField] Transform _CreationPoint;
    [SerializeField] WaterBall WaterBallPrefab;
    WaterBall waterBall;
    List<WaterBall> waterBalls;
    public int ballCount = 1;
    private void Update()
    {
        if (!_update)
        {
            return;
        }

        // if (Input.GetMouseButtonDown(0))
        // {
        //     if (WaterBallCreated())
        //     {
        //         CreateWaterBall();
        //     }
        //     else
        //     {
        //         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //         RaycastHit hit;
        //         if (Physics.Raycast(ray, out hit))
        //         {
        //             if (waterBall != null)
        //             {
        //                 ThrowWaterBall(hit.point);
        //             }
        //         }
        //     }
        // }
    }
    public bool WaterBallCreated()
    {
        return waterBall != null;
    }
    public void CreateWaterBall()
    {
        waterBall = Instantiate(WaterBallPrefab, _CreationPoint.position, Quaternion.identity);
    }

    public void BossCreateWaterBall(int count)
    {
        ballCount = count;
        DestroyWaterBall();
        
        waterBalls = new List<WaterBall>();
        //waterBall = Instantiate(WaterBallPrefab, _CreationPoint.position, Quaternion.identity);
        for (int i = 0; i < ballCount; i++)
        {
            //排成一行 中心点为_CreationPoint
            // Vector3 pos = _CreationPoint.position + new Vector3(i * 2, 0, i * 2)- new Vector3((ballCount-1)*2/2, 0, (ballCount-1)*2/2);
            Vector3 pos = _CreationPoint.position;
            WaterBall waterBall1 = Instantiate(WaterBallPrefab, pos, Quaternion.identity);
            waterBalls.Add(waterBall1);
            //waterBall = Instantiate(WaterBallPrefab, _CreationPoint.position, Quaternion.identity);
        }
    }
    
    public void ThrowWaterBall(Vector3 pos)
    {
        //waterBall.Throw(pos);
        //扔在pos周围
        for (int i = 0; i < ballCount; i++)
        {
            WaterBall waterBalli = waterBalls[i];
            if (waterBalli == null)
            {
                continue;
            }
            //排成一行 中心点为_CreationPoint
            //在x和z方向随机加一点偏移值
            float randomX = Random.Range(-2f, 2f);
            float randomZ = Random.Range(-2f,2f);
            Vector3 targetPos = pos + new Vector3(i * 2 + randomX, 0, i * 2 + randomZ)- new Vector3((ballCount-1)*2/2, 0, (ballCount-1)*2/2);
            waterBalli.Throw(targetPos);
        }
    }
    public void DestroyWaterBall()
    {
        if(waterBalls == null)
           return;
        // if(waterBall != null)
        //     Destroy(waterBall.gameObject);
        foreach (var waterBall2 in waterBalls)
        {
            if(waterBall2 != null)
                Destroy(waterBall2.gameObject);
            
        }
        waterBalls.Clear();
    }
}
