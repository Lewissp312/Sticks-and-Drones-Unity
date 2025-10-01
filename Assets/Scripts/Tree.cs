using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tree : MonoBehaviour
{
    private bool isRebuilt;
    private int sticksNeeded;
    private int originalSticksNeeded;
    private float speed;
    private AudioSource treeAudio;
    private TextMesh treeText;
    [SerializeField] private AudioClip completedSound;

    // Start is called before the first frame update
    void Start()
    {
        treeAudio = GetComponent<AudioSource>();
        treeAudio.volume = GameManager.Instance.GetSoundEffectsVolume();
        sticksNeeded = Random.Range(1, 6);
        originalSticksNeeded = sticksNeeded;
        treeText = transform.GetChild(0).gameObject.GetComponent<TextMesh>();
        treeText.text = $"{sticksNeeded}";
        speed = GameManager.Instance.GetTreeSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            transform.Translate(speed * Time.deltaTime * Vector3.forward);
        }
        GameManager.Instance.MovementRestrictions(gameObject);
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
                        // treeText.GetComponent<TextMesh>().color = new Color(173,93,4,255);
                        treeText.GetComponent<TextMesh>().fontSize = 20;
                        isRebuilt = true;
                    }
                    else { treeText.GetComponent<TextMesh>().text = $"{sticksNeeded}"; }
                    Destroy(otherObject);
                }
            }
            else if (otherObject.CompareTag("TreeDangerWall") && !isRebuilt)
            {
                // treeText.GetComponent<TextMesh>().color = Color.red;
                treeText.GetComponent<TextMesh>().fontSize = 70;
            }
        }
    }

    public bool GetIsRebuilt()
    {
        return isRebuilt;
    }

    public TextMesh GetTreeText()
    {
        return treeText;
    }
    
    public void SetSpeed(float speedToSet) { speed = speedToSet; }
}
