using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Stick : MonoBehaviour
{
    private bool isSuperStick;
    private readonly float speed = 15.0f;
    private readonly float zBound = 8.0f;
    private readonly float xBound = 21;
    [SerializeField] private Material orange;

    void Start()
    {
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.back);
        if (transform.position.z > zBound || transform.position.z < -zBound || transform.position.x < -xBound || transform.position.x > xBound)
        {
            Destroy(gameObject);
        }
    }

    public void SetIsSuperStick()
    {
        isSuperStick = true;
        GetComponent<MeshRenderer>().material = orange;
    }

    public bool GetIsSuperStick() { return isSuperStick; }
}
