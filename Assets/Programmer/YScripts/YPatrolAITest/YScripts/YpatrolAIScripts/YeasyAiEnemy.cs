using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YeasyAiEnemy : MonoBehaviour
{
    public Transform playerTrans;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(playerTrans);
        transform.Translate(Vector3.forward*1f*Time.deltaTime);
    }
}
