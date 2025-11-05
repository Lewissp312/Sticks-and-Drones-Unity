using UnityEngine;
using UnityEngine.Pool;


public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;
    private readonly System.Random rand = new();    
    public enum PoolType { STICK, DRONE, SOUND, EFFECT };
    private IObjectPool<Stick> stickPool;
    private IObjectPool<Enemy> dronePool;
    private IObjectPool<Tree> treePool;
    private IObjectPool<SoundOrEffect> soundOrEffectPool;
    private IObjectPool<Laser> laserPool;
    [SerializeField] private Stick stick;
    [SerializeField] private Tree tree;
    [SerializeField] private SoundOrEffect soundOrEffect;
    [SerializeField] private Laser laser;
    [SerializeField] private Enemy[] drones;
    [SerializeField] private Tree[] trees;
    [SerializeField] private bool collectionCheck = true;
    [SerializeField] private int defaultStickCapacity = 40;
    [SerializeField] private int maxSize = 60;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        stickPool = new ObjectPool<Stick>(CreateStickObject, OnGetStickFromPool, OnReleaseStickToPool, OnDestroyPooledStick, collectionCheck, defaultStickCapacity, maxSize);
        dronePool = new ObjectPool<Enemy>(CreateDroneObject, OnGetDroneFromPool, OnReleaseDroneToPool, OnDestroyPooledDrone, collectionCheck, defaultStickCapacity, maxSize);
        treePool = new ObjectPool<Tree>(CreateTreeObject, OnGetTreeFromPool, OnReleaseTreeToPool, OnDestroyPooledTree, collectionCheck, defaultStickCapacity, maxSize);
        soundOrEffectPool = new ObjectPool<SoundOrEffect>(CreateSoundOrEffectObject, OnGetSoundOrEffectFromPool, OnReleaseSoundOrEffectToPool, OnDestroyPooledSoundOrEffect, collectionCheck, defaultStickCapacity, maxSize);
        laserPool = new ObjectPool<Laser>(CreateLaserObject, OnGetLaserFromPool, OnReleaseLaserToPool, OnDestroyPooledLaser, collectionCheck, defaultStickCapacity, maxSize);
    }

    private Stick CreateStickObject()
    {
        Stick stickCopy = Instantiate(stick);
        stickCopy.SetStickPool(stickPool);
        return stickCopy;
    }
    private Enemy CreateDroneObject()
    {
        Enemy droneCopy = Instantiate(drones[rand.Next(0, 3)]);
        droneCopy.SetDronePool(dronePool);
        droneCopy.SetLaserPool(laserPool);
        return droneCopy;
    }

    private Tree CreateTreeObject()
    {
        Tree treeCopy = Instantiate(tree);
        treeCopy.SetTreePool(treePool);
        treeCopy.SetStickPool(stickPool);
        return treeCopy;
    }

    private SoundOrEffect CreateSoundOrEffectObject()
    {
        SoundOrEffect soundOrEffectCopy = Instantiate(soundOrEffect);
        soundOrEffectCopy.SetSoundOrEffectPool(soundOrEffectPool);
        return soundOrEffectCopy;
    }
    
    private Laser CreateLaserObject()
    {
        Laser laserCopy = Instantiate(laser);
        laserCopy.SetLaserPool(laserPool);
        return laserCopy;
    }

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
    public IObjectPool<Stick> GetStickPool() { return stickPool; }
    public IObjectPool<Enemy> GetDronePool() { return dronePool; }
    public IObjectPool<Tree> GetTreePool() { return treePool; }
    public IObjectPool<SoundOrEffect> GetSoundOrEffectPool() { return soundOrEffectPool; }
    public IObjectPool<Laser> GetLaserPool() { return laserPool; }


}