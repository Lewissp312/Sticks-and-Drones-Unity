using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;

public class Stick : MonoBehaviour
{
    private bool isSuperStick;
    private readonly float speed = 15.0f;
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
            ReturnStickToPool();
        }
    }

    public void SetIsSuperStick()
    {
        isSuperStick = true;
        meshRenderer.material = orange;
    }

    public void SetStickPool(IObjectPool<Stick> poolToSet)
    {
        stickPool = poolToSet;
    }

    public void ReturnStickToPool()
    { 
        if (isSuperStick)
        {
            isSuperStick = false;
            meshRenderer.material = originalColour;
        }
        transform.rotation = Quaternion.Euler(0, -180, 0);
        stickPool.Release(this);
    }

    public bool GetIsSuperStick() { return isSuperStick; }
}
