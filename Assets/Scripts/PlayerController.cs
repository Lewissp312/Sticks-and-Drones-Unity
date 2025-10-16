using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlayerController : MonoBehaviour
{
    private bool canShoot=true;
    private bool hasArmour;
    private bool hasPowerUp;
    private bool hasSuperSticks;
    private bool hasFourWaySticks;
    private const float speed = 10.0f;
    private const float zBound = 7f;
    private const float xBound=14;
    private IObjectPool<Stick> stickPool;
    private InputAction _moveAction;
    private InputAction _shootAction;    
    private static readonly WaitForSeconds _waitForSeconds0_1 = new(0.1f);
    private static readonly WaitForSeconds _waitForSeconds10 = new(10);
    [SerializeField] private GameObject stick;
    [SerializeField] private GameObject armourRing;
    [SerializeField] private GameObject fourWaySticksRing;
    [SerializeField] private ParticleSystem explosion;

    void Start()
    {
        //WASD / arrow keys or left stick on a controller
        _moveAction = InputSystem.actions.FindAction("Move");
        //The space bar / left click or west gamepad button / right trigger  (e.g Y, Triangle)
        _shootAction = InputSystem.actions.FindAction("Shoot");
        stickPool = ObjectPooler.Instance.GetStickPool();

    }

    // Update is called once per frame
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
                }
                else if (hasFourWaySticks)
                {
                    Stick stickCopy2 = stickPool.Get();
                    //Downwards
                    stickCopy2.transform.SetPositionAndRotation(transform.position + new Vector3(0, 0.3f, -1), Quaternion.identity);
                    Stick stickCopy3 = stickPool.Get();
                    //Right
                    stickCopy3.transform.SetPositionAndRotation(transform.position + new Vector3(1, 0.3f, 0), Quaternion.Euler(0, 90, 0));
                    Stick stickCopy4 = stickPool.Get();
                    //Left
                    stickCopy4.transform.SetPositionAndRotation(transform.position + new Vector3(-1, 0.3f, 0), Quaternion.Euler(0, -90, 0));
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

    void OnTriggerEnter(Collider other){
        GameObject otherObject = other.gameObject;
        if (otherObject.CompareTag("Enemy") && !hasArmour)
        {
            GameManager.Instance.UpdateLives(-1, GameManager.CauseOfFailure.DRONE);
        }
        else if (otherObject.CompareTag("Heal"))
        {
            GameManager.Instance.UpdateLives(3 - GameManager.Instance.GetLives());
            Destroy(otherObject);
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
            Destroy(otherObject);
            StartCoroutine(PowerUpCountdown());
        }
    }

    public bool GetHasArmour() {return hasArmour;}



    IEnumerator WaitToShoot(){
        yield return _waitForSeconds0_1;
        canShoot=true;
    }

    IEnumerator PowerUpCountdown()
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
