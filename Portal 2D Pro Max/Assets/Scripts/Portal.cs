using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.UIElements;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private Transform _destination;

    public bool isOrange;
    public float distance = 0.2f;
    public Portal targetPortal;
    private bool isInside;

    void Start()
    {
        isInside = false;
    }

    void Update()
    {
        _destination = targetPortal.GetComponent<Transform>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Vector2.Distance(transform.position, other.transform.position) > distance && !isInside && other.isTrigger)
        {
            if (isOrange)
            {
                other.transform.position =
                    new Vector2(_destination.localPosition.x - 2, _destination.localPosition.y);
                isInside = true;
            }
            else
            {
                other.transform.position =
                    new Vector2(_destination.localPosition.x + 2, _destination.localPosition.y);
                isInside = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        isInside = false;
    }
}