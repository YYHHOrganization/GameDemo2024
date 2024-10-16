using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class HEnemyBulletMoveSpecial : HEnemyBulletMoveBase
{
    public string specialMoveType;
    public bool dontDestroy = false;
    public override void SetBulletMoving(bool moving)
    {
        base.SetBulletMoving(moving);
        if (moving)
        {
            StartBulletCoroutineWithType();
        }
    }

    public void StartBulletCoroutineWithType()
    {
        switch (specialMoveType)
        {
            case "BumpMove":
                StartCoroutine(BulletBumpMove());
                break;
            case "BezierMove":
                StartCoroutine(BulletBezierMove());
                break;
            case "ZigzagMove":

                break;
            case "CircleMove":
                StartCoroutine(BulletCircleMove());
                break;
            case "ThrowOut":
                StartCoroutine(BulletThrowOut());
                break;
        }
    }
    
    IEnumerator BulletThrowOut()
    {
        if (hasShootTarget && shootTarget!= null)
        {
            //朝着target沿着抛物线扔出子弹
            Vector3 targetPos = shootTarget.position;
            Vector3 startPos = transform.position;
            float height = 3f;
            float duration = 1f;
            float time = 0;
            while (time <= 1)
            {
                time += Time.deltaTime / duration;
                float yOffset = height * 4 * (time - time * time);
                transform.position = Vector3.Lerp(startPos, targetPos, time) + yOffset * Vector3.up;
                yield return null;
            }
        }
        //Destroy(this.gameObject, 5f);
    }

    IEnumerator BulletCircleMove()
    {
        //子弹沿着四分之一圆弧进行移动, 相对于自身发射方向的右侧
        float radius = 0.2f;
        float angle = 0;
        while (angle >= -180)
        {
            angle -= 1;
            float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            transform.position += (transform.right * x + transform.forward * y);
            yield return new WaitForSeconds(0.02f);
        }
        Destroy(this.gameObject);
    }

    IEnumerator BulletBezierMove()
    {
        //用四阶贝塞尔曲线实现子弹的移动，末端指向目标，中间的控制点随机生成
        Vector3 p0 = transform.position;
        Vector3 p1 = p0 + new Vector3(Random.Range(-10, 10), Random.Range(1, 5), Random.Range(-10, 10));
        Vector3 p2 = p0 + new Vector3(Random.Range(-10, 10), Random.Range(1, 5), Random.Range(-10, 10));
        Vector3 p3 = transform.position + transform.forward * 10;
        if(hasShootTarget && shootTarget != null)
        {
            p3 = shootTarget.position;
        }
        for (float t = 0; t <= 1; t += 0.02f)
        {
            Vector3 pos = Mathf.Pow(1 - t, 3) * p0 + 3 * Mathf.Pow(1 - t, 2) * t * p1 + 3 * (1 - t) * Mathf.Pow(t, 2) * p2 + Mathf.Pow(t, 3) * p3;
            transform.LookAt(pos);
            transform.position = pos;
            yield return new WaitForSeconds(0.02f);
        }
        Destroy(this.gameObject, 5f);
    }
    
    IEnumerator BulletBumpMove()
    {
        if (bulletSpeed <= 0) yield break;
        //interval必须要比duration大
        float jumpInterval = 0.82f;
        float jumpDuration = 0.8f;

        for (int i = 0; i <= 20; i++)
        {
            Vector3 shootTargetPos = transform.position + transform.forward;
            if (hasShootTarget && shootTarget != null)
            {
                Debug.Log("hasShootTarget!!!");
                shootTargetPos = transform.position + (shootTarget.position - transform.position) * Random.Range(0.2f, 1.2f);
                if (!isChasingYAxis)
                {
                    shootTargetPos = new Vector3(shootTargetPos.x, 0.5f, shootTargetPos.z);
                }
                else
                {
                    shootTargetPos = new Vector3(shootTargetPos.x,shootTargetPos.y + shootTarget.localScale.y * 0.5f, shootTargetPos.z);
                }
                //transform.LookAt(shootTargetPos);
                jumpDuration = Random.Range(0.5f, 1.1f);
                jumpInterval = jumpDuration + Random.Range(0.02f, 0.1f);
                //transform.position += transform.forward * (bulletSpeed * Time.deltaTime);
            }
            // 用Dotween实现子弹的弹跳移动
            transform.DOJump(shootTargetPos, 3f, 1, jumpDuration);
            yield return new WaitForSeconds(jumpInterval);
        }
        Destroy(this.gameObject);
    }
    protected override void BulletMoveLogic()
    {
        if (!BulletMoving) return;
        // 这里对应特殊的子弹移动逻辑，不像普通子弹那样直线移动，比如说螺旋线移动，跳跃移动，贝塞尔移动等
        switch (specialMoveType)
        {
            
        }
    }

    protected override void DealWithBulletAfterCollision(GameObject gameObject)
    {
        if(!dontDestroy)
            Destroy(this.gameObject);
    }
}
