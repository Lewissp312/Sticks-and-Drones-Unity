using System;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(MeshRenderer))]

/// <summary>
/// Controls the behaviour of sticks, the projectile that the player fires 
/// </summary>
public class Stick : MonoBehaviour
{
    private bool _isSuperStick;
    private float _speed;
    private readonly float _zBound = GameManager.zBound;
    private readonly float _xBound = GameManager.xBound;
    private MeshRenderer _meshRenderer;
    private IObjectPool<Stick> _stickPool;
    [SerializeField] private Material _originalColour;
    [SerializeField] private Material _orange;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Unity methods

    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.back);
        if (transform.position.x > _xBound ||
            transform.position.x < -_xBound ||
            transform.position.z > _zBound ||
            transform.position.z < -_zBound)
        {
            try { _stickPool.Release(this); } catch (Exception) { }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        if (otherObject.CompareTag("Enemy") && !_isSuperStick)
        {
            try { _stickPool.Release(this); } catch (Exception) { }
        }
    }

    void OnDisable()
    {
        if (_isSuperStick)
        {
            _isSuperStick = false;
            _meshRenderer.material = _originalColour;
        }
        transform.rotation = Quaternion.Euler(0, -180, 0);
    }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Public class methods

    public void SetSpeed(float speed){_speed = speed;}

    public void SetIsSuperStick()
    {
        _isSuperStick = true;
        _meshRenderer.material = _orange;
    }

    public void SetStickPool(IObjectPool<Stick> stickPool){_stickPool = stickPool;}

    public bool GetIsSuperStick() {return _isSuperStick;}
}