using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tree : MonoBehaviour
{
    private float speed;
    private int sticksNeeded;
    private int originalSticksNeeded;
    // private bool superSticks;
    private PlayerController playerController;
    private AudioSource treeAudio;
    public GameObject treeText;
    public AudioClip completedSound;
    public bool rebuilt;
    [SerializeField] AudioClip crash;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        treeAudio =GetComponent<AudioSource>();
        treeAudio.volume = GameManager.Instance.GetSoundEffectsVolume();
        sticksNeeded = Random.Range(1,6);
        originalSticksNeeded=sticksNeeded;
        treeText=Instantiate(treeText,transform.position,treeText.transform.rotation);
        treeText.GetComponent<TextMesh>().text=$"{sticksNeeded}";
    }

    // Update is called once per frame
    void Update()
    {
        speed=GameManager.Instance.treeSpeed;
        if (GameManager.Instance.isGameActive){
            treeText.transform.position=transform.position;
            GameManager.Instance.DirectionalMovement(gameObject,speed,GameManager.Direction.Top);
        }
    }

    void OnCollisionEnter(Collision other){
        if (GameManager.Instance.isGameActive){
            if (other.gameObject.CompareTag("Stick")){
                Destroy(other.gameObject);
                if (other.gameObject.CompareTag("Stick") && !rebuilt){
                    sticksNeeded--;
                    treeText.GetComponent<TextMesh>().text=$"{sticksNeeded}";
                    if (sticksNeeded==0 ||  playerController.superSticks){
                        AudioSource.PlayClipAtPoint(completedSound, Camera.main.transform.position,GameManager.Instance.GetSoundEffectsVolume());
                        GameManager.Instance.UpdateScore(originalSticksNeeded);
                        treeText.GetComponent<TextMesh>().text="Done!";
                        rebuilt=true;
                    }
                }
            }
        }
    }
}
