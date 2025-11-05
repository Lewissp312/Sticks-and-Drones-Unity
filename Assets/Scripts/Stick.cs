using System;
using UnityEngine;
using UnityEngine.Pool;

public class Stick : MonoBehaviour
{
    private bool isSuperStick;
    private float speed = 15.0f;
    private readonly float zBound = GameManager.zBound;
    private readonly float xBound = GameManager.xBound;
    private MeshRenderer meshRenderer;
    private IObjectPool<Stick> stickPool;
    [SerializeField] private Material originalColour;
    [SerializeField] private Material orange;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.back);
        if (transform.position.z > zBound || transform.position.z < -zBound || transform.position.x < -xBound || transform.position.x > xBound)
        {
            try{stickPool.Release(this);} catch (Exception){}
        }   
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        if (otherObject.CompareTag("Enemy") && !isSuperStick)
        {
            try{stickPool.Release(this);} catch (Exception){}
        }
    }

    void OnDisable()
    {
        if (isSuperStick)
        {
            isSuperStick = false;
            meshRenderer.material = originalColour;
        }
        transform.rotation = Quaternion.Euler(0, -180, 0);
    }

    public void ReturnStickToPool()
    {
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetIsSuperStick()
    {
        isSuperStick = true;
        meshRenderer.material = orange;
    }

    public void SetStickPool(IObjectPool<Stick> stickPool)
    {
        this.stickPool = stickPool;
    }

    public bool GetIsSuperStick() { return isSuperStick; }
}
