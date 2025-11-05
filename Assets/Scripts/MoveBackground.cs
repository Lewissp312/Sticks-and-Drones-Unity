using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    private float speed;
    float pointToRepeat; 
    private Vector3 originalPosition;
    private BoxCollider boxCollider;

    void Awake()
    {
        originalPosition = transform.position;
        boxCollider = GetComponent<BoxCollider>();
        pointToRepeat = originalPosition.z - boxCollider.size.z / 3.75f;
    } 

    void Update()
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            transform.Translate(speed * Time.deltaTime * Vector3.back);
            if (transform.position.z <= originalPosition.z - boxCollider.size.z / 3.75f)
            {
                transform.position = originalPosition;
            }
        }
    }

    public void SetSpeed(float speed) { this.speed = speed; }
}