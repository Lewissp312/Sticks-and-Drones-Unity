using System;
using UnityEngine;
using UnityEngine.Pool;

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
    private IObjectPool<Stick> stickPool;

    [SerializeField] private AudioClip completedSound;

    void Awake()
    {
        treeText = transform.GetChild(0).gameObject.GetComponent<TextMesh>();
        ResetObject();
    } 

    void Start()
    {
        treePool = ObjectPooler.Instance.GetTreePool();
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
                    Stick stickInstance = otherObject.GetComponent<Stick>();
                    sticksNeeded -= stickInstance.GetIsSuperStick() ? 2 : 1;
                    if (sticksNeeded <= 0)
                    {
                        GameManager.Instance.PlaySound(completedSound);
                        GameManager.Instance.UpdateScore(originalSticksNeeded);
                        treeText.text = "Done!";
                        if (treeText.fontSize != 20) { treeText.fontSize = 20;}
                        isRebuilt = true;
                    }
                    else { treeText.text = $"{sticksNeeded}"; }
                    try { stickPool.Release(stickInstance); } catch (Exception) { }
                }
            }
            else if (otherObject.CompareTag("TreeDangerWall") && !isRebuilt)
            {
                treeText.fontSize = 70;
            }
        }
    }

    void OnDisable()
    {
        ResetObject();
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

    public void SetTreePool(IObjectPool<Tree> treePool)
    {
        this.treePool = treePool;
    }

    public void SetStickPool(IObjectPool<Stick> stickPool)
    {
        this.stickPool = stickPool;
    }

    public void ResetObject()
    {
        sticksNeeded = UnityEngine.Random.Range(1, 6);
        originalSticksNeeded = sticksNeeded;
        treeText.text = $"{sticksNeeded}";
        treeText.fontSize = 20;
        isRebuilt = false;
    }
}
