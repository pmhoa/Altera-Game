﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 zoomedOut;
    public Vector3 zoomedIn;
    public Transform follow;
    private bool zoom;
    private bool personCam;
    private bool changing;
    public float xAxisClamp;
    private string mouseXInputName, mouseYInputName;
    public float mouseSensitivity;
    public Transform rotatorObj;
    private PlayerControl pc;
    private UserInterface ui;
    
    private void Start()
    {
        offset = zoomedOut;
        zoom = false;
        mouseXInputName = "Mouse X";
        mouseYInputName = "Mouse Y";
        pc = PlayerControl.Instance;
        ui = UserInterface.Instance;
    }
    void Update()
    {
        if (!personCam)
            transform.position = follow.transform.position + offset;
        else
        {
            CameraRotation();
        }
        //CameraRotation();
        if (Input.GetMouseButtonDown(1))
        {
            ChangeCam();
        }
    }
    private void FixedUpdate()
    {
        if (personCam)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
            {
                if (hit.transform.gameObject.tag == "Enemy")
                {
                    TargetPart tp = hit.transform.GetComponent<TargetPart>();
                    float hitchange = pc.HitChange(pc.stats.aim, pc.weapon.accuracy, pc.HitRange(pc.weapon, hit.transform), tp.parent.stats.dodge) * tp.hitMultiplier;
                    ui.targetText.text = $"{tp.partName} {hitchange * 100:F1}%";
                    ui.targetText.color = new Color32(240, 100, 100, 255);
                }
                else
                {
                    ui.targetText.text = " ";
                    ui.targetText.color = new Color32(240, 100, 100, 255);
                }
            }
        }
    }
    public void ChangeCam()
    {
        if (!changing && !pc.moving)
        {
            if (zoom)
            {
                StartCoroutine(ChangeCamRoutine(zoomedOut, 65f));
                zoom = false;
            }
            else
            {
                StartCoroutine(ChangeCamRoutine(zoomedIn, 0));
                zoom = true;
            }
        }

    }
    public IEnumerator ChangeCamRoutine(Vector3 newset, float rotation)
    {
        changing = true;
        float t = 0;
        Vector3 rvector = new Vector3(rotation, 0, 0);
        while (changing && t < 1f)
        {
            t += Time.deltaTime * 5f;
            offset = Vector3.Lerp(offset, newset, t);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, rvector, t);
            yield return null;
        }
        personCam = zoom;
        if (personCam)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pc.agent.enabled = false;
            transform.SetParent(rotatorObj);
            transform.localPosition = rotatorObj.localPosition + offset;
            transform.localEulerAngles = Vector3.zero;
            ui.crossHair.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pc.agent.enabled = true;
            transform.SetParent(null);
            rotatorObj.localEulerAngles = Vector3.zero;
            ui.crossHair.SetActive(false);
        }
        changing = false;
    }
    private void CameraRotation()
    {
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensitivity * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) * mouseSensitivity * Time.fixedDeltaTime;
        xAxisClamp += mouseY;

        if (xAxisClamp > 65)
        {
            xAxisClamp = 65;
            mouseY = 0;
            clampXAxisRotationToValue(295);
        }
        else if (xAxisClamp < -65)
        {
            xAxisClamp = -65;
            mouseY = 0;
            clampXAxisRotationToValue(65);
        }
        rotatorObj.Rotate(Vector3.left * mouseY);
        //rotatorObj.Rotate(Vector3.up * mouseX);
        //transform.Rotate(Vector3.up * mouseX);
        follow.Rotate(Vector3.up * mouseX);
    }
    private void clampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        rotatorObj.eulerAngles = eulerRotation;
    }
}