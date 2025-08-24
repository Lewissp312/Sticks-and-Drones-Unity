using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    private Vector3 originalPosition;
    private BoxCollider boxCollider;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition=transform.position;
        boxCollider=GetComponent<BoxCollider>();
        gameManager=GameObject.Find("Game Manager").GetComponent<GameManager>();
        Debug.Log(boxCollider.size.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGameActive){
            transform.Translate(Vector3.back*gameManager.treeSpeed*Time.deltaTime);
            if(transform.position.z<originalPosition.z-boxCollider.size.z/2){
                transform.position=originalPosition;
            }
        }
    }
}