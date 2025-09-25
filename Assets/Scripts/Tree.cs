using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tree : MonoBehaviour
{
    private bool isRebuilt;
    private int sticksNeeded;
    private int originalSticksNeeded;
    private float speed;
    private PlayerController playerController;
    private AudioSource treeAudio;
    [SerializeField] private GameObject treeText;
    [SerializeField] private AudioClip completedSound;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        treeAudio = GetComponent<AudioSource>();
        treeAudio.volume = GameManager.Instance.GetSoundEffectsVolume();
        sticksNeeded = Random.Range(1, 6);
        originalSticksNeeded = sticksNeeded;
        treeText = Instantiate(treeText, transform.position, treeText.transform.rotation);
        treeText.GetComponent<TextMesh>().text = $"{sticksNeeded}";
        speed = GameManager.Instance.GetTreeSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            treeText.transform.position = transform.position;
            GameManager.Instance.DirectionalMovement(gameObject, speed, GameManager.Direction.Top);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            GameObject otherObject = other.gameObject;
            if (otherObject.CompareTag("Stick"))
            {
                if (!isRebuilt)
                {
                    sticksNeeded -= otherObject.GetComponent<Stick>().GetIsSuperStick() ? 2 : 1;
                    print($"I have been hit by a stick, I now need {sticksNeeded} more sticks");
                    if (sticksNeeded <= 0)
                    {
                        AudioSource.PlayClipAtPoint(completedSound, Camera.main.transform.position, GameManager.Instance.GetSoundEffectsVolume());
                        GameManager.Instance.UpdateScore(originalSticksNeeded);
                        treeText.GetComponent<TextMesh>().text = "Done!";
                        isRebuilt = true;
                    }
                    else{ treeText.GetComponent<TextMesh>().text = $"{sticksNeeded}";}
                }
                Destroy(otherObject);
            }
        }
    }

    public bool GetIsRebuilt()
    {
        return isRebuilt;
    }

    public GameObject GetTreeText()
    {
        return treeText;
    }
    
    public void SetSpeed(float speedToSet) { speed = speedToSet; }
}
