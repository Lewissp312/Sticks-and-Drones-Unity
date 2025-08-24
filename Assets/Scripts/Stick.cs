using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour
{
    private float speed=15.0f;
    private float zBound=8.0f;
    private PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        playerController=GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Debug.Log(playerController);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward*speed*Time.deltaTime);
        if (transform.position.z>zBound){
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision other){
        if (other.gameObject.CompareTag("Enemy") && playerController.superSticks){
            ParticleSystem explosionMod = Instantiate(playerController.explosion,other.transform.position,playerController.explosion.transform.rotation);
            explosionMod.Play();
            playerController.audioSource.PlayOneShot(playerController.crash);
            StartCoroutine(playerController.WaitForExplosion(explosionMod));
        }
    }
}
