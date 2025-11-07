using UnityEngine;

[RequireComponent(typeof(BoxCollider))]

/// <summary>
/// Controls the repeating background
/// </summary>
public class MoveBackground : MonoBehaviour
{
    private float _speed;
    private float _pointToRepeat;
    private Vector3 _originalPosition;
    private BoxCollider _boxCollider;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Unity methods

    void Awake()
    {
        _originalPosition = transform.position;
        _boxCollider = GetComponent<BoxCollider>();
        _pointToRepeat = _originalPosition.z - _boxCollider.size.z / 3.75f;
    }

    void Update()
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            transform.Translate(_speed * Time.deltaTime * Vector3.back);
            if (transform.position.z <= _pointToRepeat)
            {
                transform.position = _originalPosition;
            }
        }
    }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Public class methods

    public void SetSpeed(float speed) { _speed = speed; }
}