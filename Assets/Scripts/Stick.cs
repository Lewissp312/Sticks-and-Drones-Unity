using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour
{
    private readonly float speed=15.0f;
    private readonly float zBound=8.0f;
    private PlayerController playerController;
    
    void Start()
    {
        playerController=GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Debug.Log(playerController);
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.forward);
        if (transform.position.z>zBound){
            Destroy(gameObject);
        }
    }
}
