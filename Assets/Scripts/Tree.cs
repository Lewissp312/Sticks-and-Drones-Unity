using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class Tree : MonoBehaviour
{
    private bool isRebuilt;
    private int sticksNeeded;
    private int originalSticksNeeded;
    private float speed;
    private readonly float zBound = GameManager.zBound;
    private readonly float xBound = GameManager.xBound;
    private TextMesh treeText;
    private IObjectPool<Tree> treePool;

    [SerializeField] private AudioClip completedSound;

    void Awake()
    {
        treeText = transform.GetChild(0).gameObject.GetComponent<TextMesh>();
        ResetObject();
    } 

    void Start()
    {
        treePool = ObjectPooler.Instance.GetTreePool();
        speed = GameManager.Instance.GetTreeSpeed();
    }

    void Update()
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            transform.Translate(speed * Time.deltaTime * Vector3.forward);
        }
        if (transform.position.x> xBound ||
        transform.position.x<-xBound ||
        transform.position.z<-zBound)
        {
            if(!isRebuilt)
            {
                GameManager.Instance.UpdateLives(-1,GameManager.CauseOfFailure.MISSED_TREE);
            }
            ResetObject();
            treePool.Release(this);
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
                    if (sticksNeeded <= 0)
                    {
                        GameManager.Instance.PlaySound(completedSound);
                        GameManager.Instance.UpdateScore(originalSticksNeeded);
                        treeText.text = "Done!";
                        treeText.fontSize = 20;
                        isRebuilt = true;
                    }
                    else { treeText.text = $"{sticksNeeded}"; }
                    otherObject.GetComponent<Stick>().ReturnStickToPool();
                }
            }
            else if (otherObject.CompareTag("TreeDangerWall") && !isRebuilt)
            {
                treeText.fontSize = 70;
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

    public void SetTreePool(IObjectPool<Tree> poolToSet)
    {
        treePool = poolToSet;
    }

    public void ResetObject()
    {
        sticksNeeded = Random.Range(1, 6);
        originalSticksNeeded = sticksNeeded;
        treeText.text = $"{sticksNeeded}";
        treeText.fontSize = 20;
        isRebuilt = false;
    }
}
