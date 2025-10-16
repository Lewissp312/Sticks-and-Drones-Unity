using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    private float speed;
    private Vector3 originalPosition;
    private BoxCollider boxCollider;

    void Awake()
    {
        originalPosition = transform.position;
        boxCollider = GetComponent<BoxCollider>();
    } 

    void Update()
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            transform.Translate(speed * Time.deltaTime * Vector3.back);
            if (transform.position.z < originalPosition.z - boxCollider.size.z / 2)
            {
                transform.position = originalPosition;
            }
        }
    }

    public void SetSpeed(float speedToSet) { speed = speedToSet; }
}