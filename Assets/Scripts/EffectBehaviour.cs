using UnityEngine;

public class EffectBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, gameObject.GetComponent<ParticleSystem>().main.duration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
