using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    private bool isDirectionSet;
    private float speed;
    private Vector3 directionVector;
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
            transform.Translate(speed * Time.deltaTime * directionVector,Space.World);
            GameManager.Instance.MovementRestrictions(gameObject);
        }
    }

    public void SetDirection(GameManager.Direction directionToSet)
    {
        direction = directionToSet;
        switch (direction)
            {
                case GameManager.Direction.TOP:
                    directionVector = Vector3.back;
                    break;
                case GameManager.Direction.LEFT:
                    directionVector = Vector3.right;
                    break;
                case GameManager.Direction.RIGHT:
                    directionVector = Vector3.left;
                    break;
            }
        isDirectionSet = true;
    }
}
