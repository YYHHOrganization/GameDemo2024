using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Cinemachine;
using UnityEngine;

public class HBigGunThrowCharacter : MonoBehaviour
{
    [Header("References")] private Transform camera;
    public Transform attackPoint;
    public GameObject objectToThrow;

    [Header("Settings")] public int totalThrows;
    public float throwCoolDown;

    [Header("Throwing")] public KeyCode throwKey = KeyCode.X;
    public float throwForce;
    public float throwUpwardForce;
    
    bool readyToThrow = true;

    private void Start()
    {
        readyToThrow = true;
    }

    private void Throw()
    {
        if (!camera)
        {
            camera = GameObject.FindWithTag("PlayerCamera").transform;
        }
        readyToThrow = false;
        GameObject throwObject = Instantiate(objectToThrow, attackPoint.position, camera.rotation);
        //get rigidbody
        Rigidbody rb = throwObject.GetComponent<Rigidbody>();

        //add force
        
        Vector3 forceToAdd = camera.transform.forward * throwForce + transform.up * throwUpwardForce;
        rb.AddForce(forceToAdd, ForceMode.Impulse);

        totalThrows--;
        //implement throw cooldown
        StartCoroutine(ResetThrow(throwObject));
        
    }
    
    private void SetPlayerCameraAndAnotherCameraEachHalf(Camera otherCamera)
    {
        Camera playerCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
        playerCamera.gameObject.SetActive(true);
        otherCamera.gameObject.SetActive(true);
        playerCamera.rect = new Rect(0,0,0.5f,1);
        otherCamera.rect = new Rect(0.5f,0,0.5f,1);
    }

    private void Update()
    {
        if(Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
        {
            Throw();
        }
    }
    
    IEnumerator ResetThrow(GameObject throwObject)
    {
        //发射角色，角色击中可破坏物之后摧毁角色
        var virtualCamera = throwObject.gameObject.GetComponentInChildren<CinemachineVirtualCamera>().gameObject;
        var camera = throwObject.gameObject.GetComponentInChildren<Camera>().gameObject;
        virtualCamera.SetActive(false);
        camera.SetActive(false);
        yield return new WaitForSeconds(1f);
        virtualCamera.SetActive(true);
        camera.SetActive(true);
        yield return new WaitForSeconds(5f);
        virtualCamera.SetActive(false);
        camera.SetActive(false);
        if(throwObject)
            Destroy(throwObject, 4f);
        HCameraLayoutManager.Instance.SetPlayerCameraWholeScreen();
        yield return new WaitForSeconds(throwCoolDown);
        readyToThrow = true;
    }
    
}

