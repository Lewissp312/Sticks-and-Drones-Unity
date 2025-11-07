using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Creates and regulates each object pool
/// </summary>
public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;
    private readonly bool _collectionCheck = true;
    private readonly int _defaultStickCapacity = 40;
    private readonly int _defaultEnemyCapacity = 20;
    private readonly int _defaultTreeCapacity = 15;
    private readonly int _defaultSoundOrEffectCapacity = 30;
    private readonly int _defaultLaserCapacity = 20;
    private readonly int _maxStickSize = 60;
    private readonly int _maxEnemySize = 40;
    private readonly int _maxTreeSize = 35;
    private readonly int _maxSoundOrEffectSize = 50;
    private readonly int _maxLaserSize = 40;
    private IObjectPool<Stick> _stickPool;
    private IObjectPool<Enemy> _dronePool;
    private IObjectPool<Tree> _treePool;
    private IObjectPool<SoundOrEffect> _soundOrEffectPool;
    private IObjectPool<Laser> _laserPool;
    [SerializeField] private Stick _stick;
    [SerializeField] private Tree _tree;
    [SerializeField] private SoundOrEffect _soundOrEffect;
    [SerializeField] private Laser _laser;
    [SerializeField] private Enemy[] _drones;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Unity methods

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        _stickPool = new ObjectPool<Stick>(CreateStickObject, OnGetStickFromPool, OnReleaseStickToPool, OnDestroyPooledStick, _collectionCheck, _defaultStickCapacity, _maxStickSize);
        _dronePool = new ObjectPool<Enemy>(CreateDroneObject, OnGetDroneFromPool, OnReleaseDroneToPool, OnDestroyPooledDrone, _collectionCheck, _defaultEnemyCapacity, _maxEnemySize);
        _treePool = new ObjectPool<Tree>(CreateTreeObject, OnGetTreeFromPool, OnReleaseTreeToPool, OnDestroyPooledTree, _collectionCheck, _defaultTreeCapacity, _maxTreeSize);
        _soundOrEffectPool = new ObjectPool<SoundOrEffect>(CreateSoundOrEffectObject, OnGetSoundOrEffectFromPool, OnReleaseSoundOrEffectToPool, OnDestroyPooledSoundOrEffect,
                                                           _collectionCheck, _defaultSoundOrEffectCapacity, _maxSoundOrEffectSize);
        _laserPool = new ObjectPool<Laser>(CreateLaserObject, OnGetLaserFromPool, OnReleaseLaserToPool, OnDestroyPooledLaser, _collectionCheck, _defaultLaserCapacity, _maxLaserSize);
    }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Public class methods

    public IObjectPool<Stick> GetStickPool() { return _stickPool; }
    public IObjectPool<Enemy> GetDronePool() { return _dronePool; }
    public IObjectPool<Tree> GetTreePool() { return _treePool; }
    public IObjectPool<SoundOrEffect> GetSoundOrEffectPool() { return _soundOrEffectPool; }
    public IObjectPool<Laser> GetLaserPool() { return _laserPool; }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Private class methods

    void OnGetStickFromPool(Stick pooledObject) { pooledObject.gameObject.SetActive(true); }
    void OnReleaseStickToPool(Stick pooledObject) { pooledObject.gameObject.SetActive(false); }
    void OnDestroyPooledStick(Stick pooledObject) { Destroy(pooledObject.gameObject); }
    void OnGetDroneFromPool(Enemy pooledObject) { pooledObject.gameObject.SetActive(true); }
    void OnReleaseDroneToPool(Enemy pooledObject) { pooledObject.gameObject.SetActive(false); }
    void OnDestroyPooledDrone(Enemy pooledObject) { Destroy(pooledObject.gameObject); }
    void OnGetTreeFromPool(Tree pooledObject) { pooledObject.gameObject.SetActive(true); }
    void OnReleaseTreeToPool(Tree pooledObject) { pooledObject.gameObject.SetActive(false); }
    void OnDestroyPooledTree(Tree pooledObject) { Destroy(pooledObject.gameObject); }
    void OnGetSoundOrEffectFromPool(SoundOrEffect pooledObject) { pooledObject.gameObject.SetActive(true); }
    void OnReleaseSoundOrEffectToPool(SoundOrEffect pooledObject) { pooledObject.gameObject.SetActive(false); }
    void OnDestroyPooledSoundOrEffect(SoundOrEffect pooledObject) { Destroy(pooledObject.gameObject); }
    void OnGetLaserFromPool(Laser pooledObject) { pooledObject.gameObject.SetActive(true); }
    void OnReleaseLaserToPool(Laser pooledObject) { pooledObject.gameObject.SetActive(false); }
    void OnDestroyPooledLaser(Laser pooledObject) { Destroy(pooledObject.gameObject); }

    Stick CreateStickObject()
    {
        Stick stickCopy = Instantiate(_stick);
        stickCopy.SetStickPool(_stickPool);
        return stickCopy;
    }

    Enemy CreateDroneObject()
    {
        Enemy droneCopy = Instantiate(_drones[Random.Range(0, 3)]);
        droneCopy.SetDronePool(_dronePool);
        droneCopy.SetLaserPool(_laserPool);
        return droneCopy;
    }

    Tree CreateTreeObject()
    {
        Tree treeCopy = Instantiate(_tree);
        treeCopy.SetTreePool(_treePool);
        treeCopy.SetStickPool(_stickPool);
        return treeCopy;
    }

    SoundOrEffect CreateSoundOrEffectObject()
    {
        SoundOrEffect soundOrEffectCopy = Instantiate(_soundOrEffect);
        soundOrEffectCopy.SetSoundOrEffectPool(_soundOrEffectPool);
        return soundOrEffectCopy;
    }

    Laser CreateLaserObject()
    {
        Laser laserCopy = Instantiate(_laser);
        laserCopy.SetLaserPool(_laserPool);
        return laserCopy;
    }
}