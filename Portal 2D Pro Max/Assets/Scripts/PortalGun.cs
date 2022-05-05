using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PortalGun : MonoBehaviour
{
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 cursorPos = cam.ScreenToWorldPoint(Input.mousePosition);
            GameObject bluePortal = GameObject.FindGameObjectWithTag("Portal1");
            GameObject bluePortalClone = bluePortal;
            

            if (bluePortal != null)
            {
                Destroy(bluePortal); 
          
            }
            
            bluePortalClone = this.gameObject;
            Instantiate(bluePortalClone, new Vector3(cursorPos.x, cursorPos.y, 0), Quaternion.identity);

        }
    }
}