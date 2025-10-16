using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class SoundOrEffect : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private AudioSource audioSource;
    private IObjectPool<SoundOrEffect> soundOrEffectPool;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

    }

    public void SetAsSoundOrEffect(AudioClip audioClip = null)
    {
        print($"The audioclip is {audioClip}");
        if (audioClip != null)
        {
            print($"Audio source at function is {audioSource}");
            audioSource.clip = audioClip;
            audioSource.volume = GameManager.Instance.GetSoundEffectsVolume();
            audioSource.Play();
            StartCoroutine(WaitForSoundOrEffectToFinish(audioClip.length));
        }
        else
        {
            particleSystem.Play();
            StartCoroutine(WaitForSoundOrEffectToFinish(particleSystem.main.duration));
        }
    }

    public void SetAsSound(AudioClip clipToPlay,float volumeToPlayAt)
    {
        audioSource.clip = clipToPlay;
        audioSource.volume = volumeToPlayAt;
        audioSource.Play();
        StartCoroutine(WaitForSoundOrEffectToFinish(clipToPlay.length));
    }

    public void SetAsParticleEffect()
    {
        particleSystem.Play();
        StartCoroutine(WaitForSoundOrEffectToFinish(particleSystem.main.duration));
    }

    public void SetSoundOrEffectPool(IObjectPool<SoundOrEffect> poolToSet)
    {
        soundOrEffectPool = poolToSet;
    }

    IEnumerator WaitForSoundOrEffectToFinish(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        particleSystem.Clear();
        particleSystem.Pause();
        soundOrEffectPool.Release(this);
    }
}
