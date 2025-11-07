using System;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Controls the behaviour of trees, the player's main way of getting points
/// and taking away their lives if they miss one
/// </summary>
public class Tree : MonoBehaviour
{
    private bool _isRebuilt;
    private int _sticksNeeded;
    private int _originalSticksNeeded;
    private float _speed;
    private readonly float _zBound = GameManager.zBound;
    private IObjectPool<Tree> _treePool;
    private IObjectPool<Stick> _stickPool;
    [SerializeField] private TextMesh _treeText;
    [SerializeField] private AudioClip _completedSound;

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Unity methods

    void Awake()
    {
        ResetObject();
    }

    void Update()
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            transform.Translate(_speed * Time.deltaTime * Vector3.forward);
        }
        if (transform.position.z < -_zBound)
        {
            if (!_isRebuilt)
            {
                GameManager.Instance.UpdateLives(-1, GameManager.CauseOfFailure.MISSED_TREE);
            }
            _treePool.Release(this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            GameObject otherObject = other.gameObject;
            if (otherObject.CompareTag("Stick"))
            {
                if (!_isRebuilt)
                {
                    Stick stickInstance = otherObject.GetComponent<Stick>();
                    _sticksNeeded -= stickInstance.GetIsSuperStick() ? 2 : 1;
                    if (_sticksNeeded <= 0)
                    {
                        GameManager.Instance.PlaySound(_completedSound);
                        GameManager.Instance.UpdateScore(_originalSticksNeeded);
                        _treeText.text = "Done!";
                        if (_treeText.fontSize != 20) { _treeText.fontSize = 20; }
                        _isRebuilt = true;
                    }
                    else { _treeText.text = $"{_sticksNeeded}"; }
                    // The stick is returned to the pool here instead of the stick script, as
                    // the isSuperStick value would always be set to false by the time it could
                    // be evaluated here
                    try { _stickPool.Release(stickInstance); } catch (Exception) { }
                }
            }
            else if (otherObject.CompareTag("TreeDangerWall") && !_isRebuilt)
            {
                _treeText.fontSize = 70;
            }
        }
    }

    void OnDisable()
    {
        ResetObject();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Public class methods

    public void SetSpeed(float speed) { _speed = speed; }

    public void SetTreePool(IObjectPool<Tree> treePool) { _treePool = treePool; }

    public void SetStickPool(IObjectPool<Stick> stickPool) { _stickPool = stickPool; }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Private class methods

    void ResetObject()
    {
        _sticksNeeded = UnityEngine.Random.Range(1, 6);
        _originalSticksNeeded = _sticksNeeded;
        _treeText.text = $"{_sticksNeeded}";
        _treeText.fontSize = 20;
        _isRebuilt = false;
    }
}
