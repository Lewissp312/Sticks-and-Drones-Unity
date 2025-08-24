using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    private GameManager gameManager;
    private GameManager.Direction direction;
    // Start is called before the first frame update
    void Start()
    {
        gameManager=GameObject.Find("Game Manager").GetComponent<GameManager>();
        // enemy=GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
        direction=gameManager.powerUpSelectedDirection;
    }

    // Update is called once per frame
    void Update()
    {
        gameManager.DirectionalMovement(gameObject,gameManager.enemySpeed,direction);
    }
}
