using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PortalGun : MonoBehaviour
{
    private Camera cam;
    
    public GameObject portalPrefab;
    public GameObject portalPrefab2;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // RaycastHit hit;
            // Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            Vector2 cursorPos = cam.ScreenToWorldPoint(Input.mousePosition);
            GameObject bluePortal = GameObject.FindGameObjectWithTag("Portal1");
            
            if (bluePortal != null)
            {
                Destroy(bluePortal);
            }

            Instantiate(portalPrefab, new Vector3(cursorPos.x, cursorPos.y, 0), Quaternion.identity);
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            // RaycastHit hit;
            // Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            Vector2 cursorPos = cam.ScreenToWorldPoint(Input.mousePosition);
            GameObject orangePortal = GameObject.FindGameObjectWithTag("Portal2");
            
            if (orangePortal != null)
            {
                Destroy(orangePortal);
            }

            Instantiate(portalPrefab2, new Vector3(cursorPos.x, cursorPos.y, 0), Quaternion.identity);
        }
    }
}