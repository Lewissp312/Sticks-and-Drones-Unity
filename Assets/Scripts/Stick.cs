using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Stick : MonoBehaviour
{
    private bool isSuperStick;
    private readonly float speed = 15.0f;
    private readonly float zBound = 8.0f;
    [SerializeField] private Material orange;

    void Start()
    {
        // playerController=GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.forward);
        if (transform.position.z > zBound)
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
