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
    private bool isInside;
    private GameObject _destinationOrange;
    private GameObject _destinationBlue;

    void Start()
    {
        isInside = false;
    }

    void Update()
    {
        //_destination = targetPortal.GetComponent<Transform>();

        _destinationOrange = GameObject.FindGameObjectWithTag("Portal1");
        _destinationBlue = GameObject.FindGameObjectWithTag("Portal2");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Vector2.Distance(transform.position, other.transform.position) > distance && !isInside &&
            other.isTrigger)
        {
            if (isOrange)
            {
                _destination = _destinationOrange.GetComponent<Transform>();
                var localPosition = _destination.localPosition;
                other.transform.position =
                    new Vector2(localPosition.x - 2, localPosition.y);
                isInside = true;
            }
            else
            {
                _destination = _destinationBlue.GetComponent<Transform>();
                var localPosition = _destination.localPosition;
                other.transform.position =
                    new Vector2(localPosition.x + 2, localPosition.y);
                isInside = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        isInside = false;
    }
}