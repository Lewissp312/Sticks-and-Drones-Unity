using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tree : MonoBehaviour
{
    private float speed;
    private int sticksNeeded;
    private int originalSticksNeeded;
    private bool superSticks;
    private AudioSource treeAudio;
    private AudioSource cameraAudio;
    private GameManager gameManager;
    private GameObject treeTextMod;
    public GameObject treeText;
    public AudioClip completedSound;
    public bool rebuilt=false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager=GameObject.Find("Game Manager").GetComponent<GameManager>();
        treeAudio=GetComponent<AudioSource>();
        cameraAudio=GameObject.Find("Main Camera").GetComponent<AudioSource>();
        sticksNeeded=Random.Range(1,6);
        originalSticksNeeded=sticksNeeded;
        treeText=Instantiate(treeText,transform.position,treeText.transform.rotation);
        treeText.GetComponent<TextMesh>().text=$"{sticksNeeded}";
    }

    // Update is called once per frame
    void Update()
    {
        speed=gameManager.treeSpeed;
        if (gameManager.isGameActive){
            treeText.transform.position=transform.position;
            gameManager.DirectionalMovement(gameObject,speed,GameManager.Direction.Top);
        }
    }

    void OnCollisionEnter(Collision other){
        if (gameManager.isGameActive){
            if (other.gameObject.CompareTag("Stick")){
                Destroy(other.gameObject);
                if (other.gameObject.CompareTag("Stick") && !rebuilt){
                    sticksNeeded--;
                    treeText.GetComponent<TextMesh>().text=$"{sticksNeeded}";
                    superSticks=GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().superSticks;
                    if (sticksNeeded==0 || superSticks){
                        treeAudio.PlayOneShot(completedSound);
                        gameManager.UpdateScore(originalSticksNeeded);
                        treeText.GetComponent<TextMesh>().text="Done!";
                        rebuilt=true;
                    }
                }
            }
        }
    }

    // IEnumerator IncreaseSpeed(){
    //     yield return new WaitForSeconds(15);
    //     speed+=1;
    //     StartCoroutine(IncreaseSpeed());
    // }
}
