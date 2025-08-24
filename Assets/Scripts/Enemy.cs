using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    // public float xBound=21.0f; //18
    // public float zBound=50.0f; //11
    private GameManager gameManager;
    private GameManager.Direction direction;
    private AudioSource audioSource;
    // private Animator animator;
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        audioSource=GetComponent<AudioSource>();
        gameManager=GameObject.Find("Game Manager").GetComponent<GameManager>();
        direction = gameManager.enemySelectedDirection;
        speed=gameManager.enemySpeed;
        // animator=GetComponent<Animator>();
        // StartCoroutine(IncreaseSpeed());
    }

    // Update is called once per frame
    void Update()
    {
        gameManager.DirectionalMovement(gameObject,speed,direction);
        // if (name=="Drone 1(Clone)"){
        //     animator.speed=1;
        // }
        // EnemyMovement(speed,gameManager,direction,xBound,zBound);
    }

    // public void EnemyMovement(float speed,GameManager gameManager,GameManager.Direction direction,float xBound, float zBound){
    //     speed=gameManager.enemySpeed;
    //     if (direction==GameManager.Direction.Top){
    //         transform.Translate(Vector3.back*speed*Time.deltaTime);
    //     } else if(direction==GameManager.Direction.Left){
    //         transform.Translate(Vector3.right*speed*Time.deltaTime);
    //     } else if(direction==GameManager.Direction.Right){
    //         transform.Translate(Vector3.left*speed*Time.deltaTime);
    //     }
    //     if (transform.position.x>xBound || transform.position.x<-xBound || transform.position.z<-zBound){
    //         Destroy(gameObject);
    //     }
    // }

    // void OnTriggerEnter(Collider other){
    //     if (other.gameObject.CompareTag("Player")){
    //         // explosion.transform.position=transform.position;
    //         // explosion.Play();
    //         explosion=Instantiate(explosion,transform.position,explosion.transform.rotation);
    //         explosion.Play();
    //         // StartCoroutine(WaitForExplosion());
    //         audioSource.PlayOneShot(crash);
    //         gameManager.lives--;
    //         //gameManager.updateLives();
    //         Debug.Log("Player collision");
    //         Destroy(gameObject);
    //     }
    //     if(other.gameObject.CompareTag("Stick")){
    //         Destroy(other.gameObject);
    //         Debug.Log("Stick collision");
    //         //will need seperate path for super stick, as this will destroy the drone
    //     }
    // }

    void OnCollisionEnter(Collision other){
        if (other.gameObject.CompareTag("Stick")){
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().superSticks){
                Destroy(gameObject);
            } else{
                Destroy(other.gameObject);
            }
        }
    }

    // IEnumerator WaitForExplosion(){
    //     yield return new WaitForSeconds(2);
    //     Destroy(explosion.gameObject);
    // }

    // IEnumerator IncreaseSpeed(){
    //     yield return new WaitForSeconds(15);
    //     speed+=100;
    //     StartCoroutine(IncreaseSpeed());
    // }


}
