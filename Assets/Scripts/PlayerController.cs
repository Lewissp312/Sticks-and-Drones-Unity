using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlayerController : MonoBehaviour
{
    private bool canShoot;
    private bool hasArmour;
    private bool hasPowerUp;
    private bool hasSuperSticks;
    private bool hasFourWaySticks;
    private float speed;
    private float stickSpeed;
    private readonly float zBound = 7f;
    private readonly float xBound = 13;
    private Animator playerAnim;
    private IObjectPool<Stick> stickPool;
    private InputAction _moveAction;
    private InputAction _shootAction;    
    private static readonly WaitForSeconds _waitForSeconds0_1 = new(0.1f);
    private static readonly WaitForSeconds _waitForSeconds10 = new(10);
    [SerializeField] private GameObject stick;
    [SerializeField] private GameObject armourRing;
    [SerializeField] private GameObject fourWaySticksRing;
    [SerializeField] private ParticleSystem explosion;

    void Awake()
    {
        canShoot = true;
        speed = 11.5f;
        stickSpeed = speed + 5;
        playerAnim = GetComponent<Animator>();
    }

    void Start()
    {
        //WASD / arrow keys or left stick on a controller
        _moveAction = InputSystem.actions.FindAction("Move");
        //The space bar / left click or west gamepad button / right trigger  (e.g Y, Triangle)
        _shootAction = InputSystem.actions.FindAction("Shoot");
        stickPool = ObjectPooler.Instance.GetStickPool();
    }

    void Update()
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            Vector2 moveValue = _moveAction.ReadValue<Vector2>();
            moveValue = Vector3.Normalize(moveValue);
            Vector3 newMoveValue = new(moveValue.x, 0, moveValue.y);
            transform.Translate(speed * Time.deltaTime * newMoveValue);
            PlayerMovementConstraints();
            if (_shootAction.WasPressedThisFrame() && canShoot && !GameManager.Instance.GetIsGamePaused())
            {
                Stick stickCopy = stickPool.Get();
                stickCopy.transform.position = transform.position + new Vector3(0, 0.3f, 1);
                if (hasSuperSticks)
                {
                    stickCopy.SetIsSuperStick();
                    stickCopy.SetSpeed(stickSpeed);
                }
                else if (hasFourWaySticks)
                {
                    Stick stickCopy2 = stickPool.Get();
                    //Downwards
                    stickCopy2.transform.SetPositionAndRotation(transform.position + new Vector3(0, 0.3f, -1), Quaternion.identity);
                    stickCopy2.SetSpeed(stickSpeed);
                    Stick stickCopy3 = stickPool.Get();
                    //Right
                    stickCopy3.transform.SetPositionAndRotation(transform.position + new Vector3(1, 0.3f, 0), Quaternion.Euler(0, 90, 0));
                    stickCopy3.SetSpeed(stickSpeed);
                    Stick stickCopy4 = stickPool.Get();
                    //Left
                    stickCopy4.transform.SetPositionAndRotation(transform.position + new Vector3(-1, 0.3f, 0), Quaternion.Euler(0, -90, 0));
                    stickCopy4.SetSpeed(stickSpeed);
                }
                canShoot = false;
                StartCoroutine(WaitToShoot());
            }
        }
    }

    void PlayerMovementConstraints(){
        if (transform.position.z > zBound)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zBound);
        }
        if (transform.position.z<-zBound){
            transform.position=new Vector3(transform.position.x,transform.position.y,-zBound);
        }
        if (transform.position.x>xBound){
            transform.position=new Vector3(xBound,transform.position.y,transform.position.z);
        }
        if (transform.position.x<-xBound){
            transform.position=new Vector3(-xBound,transform.position.y,transform.position.z);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        if (otherObject.CompareTag("Enemy") && !hasArmour)
        {
            GameManager.Instance.UpdateLives(-1, GameManager.CauseOfFailure.DRONE);
            if (GameManager.Instance.GetIsGameActive()){PlayAnimation("Hit Trigger");}
            else{PlayAnimation("Death Trigger");}
        }
        else if (otherObject.CompareTag("Laser"))
        {
            if (!hasArmour)
            {
                GameManager.Instance.UpdateLives(-1, GameManager.CauseOfFailure.LASER);
                if (GameManager.Instance.GetIsGameActive()){PlayAnimation("Hit Trigger");}
                else{PlayAnimation("Death Trigger");}
            }
        }
        else if (otherObject.CompareTag("Heal"))
        {
            GameManager.Instance.UpdateLives(3 - GameManager.Instance.GetLives());
            PlayAnimation("Spin Trigger");
        }
        else if (otherObject.CompareTag("DroneSpeedDown"))
        {
            GameManager.Instance.DecreaseSpeed(1);
            PlayAnimation("Spin Trigger");
        }
        else if (otherObject.CompareTag("TreeSpeedDown"))
        {
            GameManager.Instance.DecreaseSpeed(2);
            PlayAnimation("Spin Trigger");
        }
        else if (otherObject.CompareTag("SuperSticks") || otherObject.CompareTag("Armor") || otherObject.CompareTag("FourWaySticks"))
        {
            if (hasPowerUp)
            {
                StopAllCoroutines();
                canShoot = true;
                hasSuperSticks = false;
                hasArmour = false;
                hasFourWaySticks = false;
                armourRing.SetActive(false);
                fourWaySticksRing.SetActive(false);
            }
            else { hasPowerUp = true; }
            if (otherObject.CompareTag("SuperSticks"))
            {
                hasSuperSticks = true;
            }
            else if (otherObject.CompareTag("Armor"))
            {
                fourWaySticksRing.SetActive(false);
                armourRing.SetActive(true);
                hasArmour = true;
            }
            else
            {
                armourRing.SetActive(false);
                fourWaySticksRing.SetActive(true);
                hasFourWaySticks = true;
            }
            PlayAnimation("Spin Trigger");
            StartCoroutine(WaitForPowerupCooldown());
        }
    }

    private void PlayAnimation(string animationTrigger)
    {
        StopCoroutine(nameof(WaitforAnimationToFinish));
        playerAnim.ResetTrigger("Spin Trigger");
        playerAnim.ResetTrigger("Hit Trigger");
        playerAnim.SetTrigger("Fly Trigger");
        playerAnim.SetTrigger(animationTrigger);
        StartCoroutine(WaitforAnimationToFinish(playerAnim.GetCurrentAnimatorStateInfo(0).length,animationTrigger));
    }

    public bool GetHasArmour() { return hasArmour; }

    public void IncreaseSpeed(float numToAdd)
    {
        speed += numToAdd;
        stickSpeed = speed + 5;
    }

    IEnumerator WaitforAnimationToFinish(float timeToFinish, string animationTrigger)
    {
        yield return new WaitForSeconds(timeToFinish);
        playerAnim.ResetTrigger(animationTrigger);
        playerAnim.SetTrigger("Fly Trigger");
    }

    IEnumerator WaitToShoot(){
        yield return _waitForSeconds0_1;
        canShoot=true;
    }

    IEnumerator WaitForPowerupCooldown()
    {
        yield return _waitForSeconds10;
        hasPowerUp = false;
        hasSuperSticks = false;
        hasArmour = false;
        hasFourWaySticks = false;
        armourRing.SetActive(false);
        fourWaySticksRing.SetActive(false);
    }
}
