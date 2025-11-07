using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

[RequireComponent(typeof(Animator))]
/// <summary>
/// Controls the behaviour of the player
/// </summary>
public class PlayerController : MonoBehaviour
{
    private bool _canShoot;
    private bool _hasArmour;
    private bool _hasPowerUp;
    private bool _hasSuperSticks;
    private bool _hasFourWaySticks;
    private float _speed;
    private float _stickSpeed;
    private readonly float _zBound = 7f;
    private readonly float _xBound = 13f;
    private readonly Vector3 _upwardsStickVector = new(0, 0, 1);
    private readonly Vector3 _downwardsStickVector = new(0, 0, -1);
    private readonly Vector3 _leftStickVector = new(-1, 0, 0);
    private readonly Vector3 _rightStickVector = new(1, 0, 0);
    private readonly Quaternion _leftStickRotation = Quaternion.Euler(0, -90, 0);
    private readonly Quaternion _rightStickRotation = Quaternion.Euler(0, 90, 0);
    private Animator _playerAnim;
    private IObjectPool<Stick> _stickPool;
    private InputAction _moveAction;
    private InputAction _shootAction;    
    private readonly WaitForSeconds _waitForSeconds0_1 = new(0.1f);
    private readonly WaitForSeconds _waitForSeconds10 = new(10);
    [SerializeField] private GameObject _armourRing;
    [SerializeField] private GameObject _fourWaySticksRing;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Unity methods

    void Awake()
    {
        _canShoot = true;
        _speed = 11.5f;
        _stickSpeed = _speed + 5;
        _playerAnim = GetComponent<Animator>();
    }

    void Start()
    {
        // WASD / arrow keys or left stick on a controller
        _moveAction = InputSystem.actions.FindAction("Move");
        // The space bar / left click or west gamepad button (e.g X / Square) / right trigger
        _shootAction = InputSystem.actions.FindAction("Shoot");
        _stickPool = ObjectPooler.Instance.GetStickPool();
    }

    void Update()
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            Vector2 moveValue = _moveAction.ReadValue<Vector2>();
            moveValue = Vector3.Normalize(moveValue);
            Vector3 newMoveValue = new(moveValue.x, 0, moveValue.y);
            transform.Translate(_speed * Time.deltaTime * newMoveValue);
            PlayerMovementConstraints();
            if (_shootAction.WasPressedThisFrame() && _canShoot && !GameManager.Instance.GetIsGamePaused())
            {
                Stick stickCopy = _stickPool.Get();
                stickCopy.transform.position = transform.position + _upwardsStickVector;
                stickCopy.SetSpeed(_stickSpeed);
                if (_hasSuperSticks){stickCopy.SetIsSuperStick();}
                else if (_hasFourWaySticks)
                {
                    //Downwards
                    Stick stickCopy2 = _stickPool.Get();
                    stickCopy2.transform.SetPositionAndRotation(transform.position + _downwardsStickVector, Quaternion.identity);
                    stickCopy2.SetSpeed(_stickSpeed);
                    //Left
                    Stick stickCopy3 = _stickPool.Get();
                    stickCopy3.transform.SetPositionAndRotation(transform.position + _leftStickVector, _leftStickRotation);
                    stickCopy3.SetSpeed(_stickSpeed);
                    //Right
                    Stick stickCopy4 = _stickPool.Get();
                    stickCopy4.transform.SetPositionAndRotation(transform.position + _rightStickVector, _rightStickRotation);
                    stickCopy4.SetSpeed(_stickSpeed);
                }
                _canShoot = false;
                StartCoroutine(WaitToShoot());
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            GameObject otherObject = other.gameObject;
            if (otherObject.CompareTag("Enemy") && !_hasArmour)
            {
                GameManager.Instance.UpdateLives(-1, GameManager.CauseOfFailure.DRONE);
                if (GameManager.Instance.GetIsGameActive()) { PlayAnimation("Hit Trigger"); }
                else
                {
                    StopAllCoroutines();
                    PlayAnimation("Death Trigger");
                }
            }
            else if (otherObject.CompareTag("Laser") && !_hasArmour)
            {
                GameManager.Instance.UpdateLives(-1, GameManager.CauseOfFailure.LASER);
                if (GameManager.Instance.GetIsGameActive()) { PlayAnimation("Hit Trigger"); }
                else
                {
                    StopAllCoroutines();
                    PlayAnimation("Death Trigger");
                }
            }
            else if (otherObject.CompareTag("Heal"))
            {
                GameManager.Instance.UpdateLives(3 - GameManager.Instance.GetLives());
                PlayAnimation("Spin Trigger");
            }
            else if (otherObject.CompareTag("DroneSpeedDown"))
            {
                GameManager.Instance.ChangeEnemySpeed(-1);
                PlayAnimation("Spin Trigger");
            }
            else if (otherObject.CompareTag("TreeSpeedDown"))
            {
                GameManager.Instance.ChangeTreeSpeed(-1);
                PlayAnimation("Spin Trigger");
            }
            else if (otherObject.CompareTag("SuperSticks") || otherObject.CompareTag("Armour") || otherObject.CompareTag("FourWaySticks"))
            {
                if (_hasPowerUp)
                {
                    StopAllCoroutines();
                    _canShoot = true;
                    _hasSuperSticks = false;
                    _hasArmour = false;
                    _hasFourWaySticks = false;
                    _armourRing.SetActive(false);
                    _fourWaySticksRing.SetActive(false);
                }
                else { _hasPowerUp = true; }
                if (otherObject.CompareTag("SuperSticks")){_hasSuperSticks = true;}
                else if (otherObject.CompareTag("Armour"))
                {
                    _armourRing.SetActive(true);
                    _hasArmour = true;
                }
                else
                {
                    _fourWaySticksRing.SetActive(true);
                    _hasFourWaySticks = true;
                }
                PlayAnimation("Spin Trigger");
                StartCoroutine(WaitForPowerupEnd());
            }   
        }
    }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Public class methods

    public bool GetHasArmour() { return _hasArmour; }

    public void IncreaseSpeed(float numToAdd)
    {
        _speed += numToAdd;
        _stickSpeed = _speed + 5;
    }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Private class methods

    void PlayerMovementConstraints(){
        if (transform.position.x > _xBound){
            transform.position=new Vector3(_xBound,transform.position.y,transform.position.z);
        }
        if (transform.position.x < -_xBound){
            transform.position=new Vector3(-_xBound,transform.position.y,transform.position.z);
        }
        if (transform.position.z > _zBound)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, _zBound);
        }
        if (transform.position.z < -_zBound){
            transform.position=new Vector3(transform.position.x,transform.position.y,-_zBound);
        }
    }

    void PlayAnimation(string animationTrigger)
    {
        // All animations stem from the flying animation
        _playerAnim.SetTrigger("Fly Trigger");
        _playerAnim.SetTrigger(animationTrigger);
        StartCoroutine(WaitforAnimationEnd(_playerAnim.GetCurrentAnimatorStateInfo(0).length, animationTrigger));
    }
    
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Coroutines

    IEnumerator WaitforAnimationEnd(float timeToEnd, string animationTrigger)
    {
        yield return new WaitForSeconds(timeToEnd);
        _playerAnim.ResetTrigger(animationTrigger);
    }

    IEnumerator WaitToShoot(){
        yield return _waitForSeconds0_1;
        _canShoot=true;
    }

    IEnumerator WaitForPowerupEnd()
    {
        yield return _waitForSeconds10;
        _hasPowerUp = false;
        _hasSuperSticks = false;
        _hasArmour = false;
        _hasFourWaySticks = false;
        _armourRing.SetActive(false);
        _fourWaySticksRing.SetActive(false);
    }
}
