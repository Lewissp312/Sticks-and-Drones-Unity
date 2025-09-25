using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    private bool isDirectionSet;
    private float speed;
    private GameManager.Direction direction;
    // Start is called before the first frame update
    void Start()
    {
        speed = GameManager.Instance.GetEnemySpeed();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDirectionSet)
        {
            GameManager.Instance.DirectionalMovement(gameObject, speed, direction);   
        }
    }

    public void SetDirection(GameManager.Direction directionToSet)
    {
        direction = directionToSet;
        isDirectionSet = true;
    }
}
