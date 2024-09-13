using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public SoundType type;
    public SoundPriority priority;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0f, 1f)]
    public float pitchVariance = 0.1f;
}

public enum SoundType
{
    BGM,
    SFX
}

public enum SoundPriority
{
    Low,
    Medium,
    High
}

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource BGMSource;
    public AudioSource BGMSource2; // ũ�ν����̵带 ���� �߰�
    public int SFXSourcesCount = 10; // �ִ� ���� ��� ��
    private List<AudioSource> SFXSources;

    public SoundBank currentSoundBank;

    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float bgmVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private List<AudioSource> activeSFXSources;
    private Sound currentBGM;
    private string currentBGMName;

    protected override void Awake()
    {
        base.Awake();
        BGMSource = CreateAudioSource("BGM Source");
        BGMSource2 = CreateAudioSource("BGM Source 2");

        SFXSources = new List<AudioSource>();
        for (int i = 0; i < SFXSourcesCount; i++)
        {
            SFXSources.Add(gameObject.AddComponent<AudioSource>());
        }

        activeSFXSources = new List<AudioSource>();
    }

    private AudioSource CreateAudioSource(string name)
    {
        GameObject audioSourceObj = new GameObject(name);
        audioSourceObj.transform.SetParent(this.transform);
        AudioSource source = audioSourceObj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        return source;
    }


    public void LoadSoundBank(string soundBankName)
    {
        SoundBank newSoundBank = Resources.Load<SoundBank>("SoundBank/" + soundBankName);
        if (newSoundBank == null)
        {
            Debug.LogError("Failed to load SoundBank: " + soundBankName);
            return;
        }

        currentSoundBank = newSoundBank;
        //Debug.Log("Loaded SoundBank: " + soundBankName);    
    }

    public void PlaySound(string name, float fadeTime = 1f, bool loop = false)
    {
        Sound sound = currentSoundBank.sounds.Find(s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (sound.type == SoundType.BGM)
        {
            StartCoroutine(CrossfadeBGM(sound, fadeTime, loop));
        }
        else
        {
            PlaySFX(sound, loop);
        }
    }


    private IEnumerator CrossfadeBGM(Sound newBGM, float fadeTime, bool loop)
    {
        AudioSource fadeOutSource = BGMSource.isPlaying ? BGMSource : BGMSource2;
        AudioSource fadeInSource = BGMSource.isPlaying ? BGMSource2 : BGMSource;

        fadeInSource.clip = newBGM.clip;
        fadeInSource.volume = 0;
        fadeInSource.loop = loop;
        fadeInSource.Play();

        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            if (currentBGM != null)
            {
                fadeOutSource.volume = Mathf.Lerp(currentBGM.volume * bgmVolume, 0, t / fadeTime) * masterVolume;
            }
            fadeInSource.volume = Mathf.Lerp(0, newBGM.volume * bgmVolume, t / fadeTime) * masterVolume;
            yield return null;
        }

        fadeOutSource.Stop();
        currentBGM = newBGM;
        currentBGMName = newBGM.name;
    }



    private void PlaySFX(Sound sound, bool loop)
    {
        AudioSource source = GetFreeSFXSource();
        if (source != null)
        {
            StartCoroutine(PlaySFXWithFade(source, sound, loop));
        }
    }

    private IEnumerator PlaySFXWithFade(AudioSource source, Sound sound, bool loop)
    {
        source.clip = sound.clip;
        source.volume = 0f;
        source.pitch = 1f + Random.Range(-sound.pitchVariance, sound.pitchVariance);
        source.loop = loop;
        source.Play();

        activeSFXSources.Add(source);
        AdjustSFXVolumes();

        float fadeTime = 0.1f; // SFX ���̵� �ð�
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(0, sound.volume * sfxVolume * masterVolume, t / fadeTime);
            yield return null;
        }

        if (!loop)
        {
            // ������ �ƴ� ��쿡�� ���尡 ���� ������ ���
            yield return new WaitForSeconds(source.clip.length - source.time - fadeTime);

            // ���̵� �ƿ�
            t = 0;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                source.volume = Mathf.Lerp(sound.volume * sfxVolume * masterVolume, 0, t / fadeTime);
                yield return null;
            }

            source.Stop();
            activeSFXSources.Remove(source);
        }
    }
    private AudioSource GetFreeSFXSource()
    {
        return SFXSources.FirstOrDefault(s => !s.isPlaying) ??
               (activeSFXSources.Count < SFXSourcesCount ? SFXSources.First(s => !activeSFXSources.Contains(s)) : null);
    }

    private void AdjustSFXVolumes()
    {
        if (activeSFXSources.Count > SFXSourcesCount)
        {
            var lowPrioritySounds = activeSFXSources
                .Where(s => currentSoundBank.sounds.Find(sound => sound.clip == s.clip).priority == SoundPriority.Low)
                .ToList();

            foreach (var source in lowPrioritySounds)
            {
                source.volume *= 0.5f;
            }
        }
    }

    void Update()
    {
        // ���� �ֽ�ȭ
        if (currentBGM != null)
        {
            BGMSource.volume = currentBGM.volume * bgmVolume * masterVolume;
            BGMSource2.volume = currentBGM.volume * bgmVolume * masterVolume;
        }
        foreach (var source in activeSFXSources)
        {
            Sound sound = currentSoundBank.sounds.Find(s => s.clip == source.clip);
            if (sound != null)
                source.volume = sound.volume * sfxVolume * masterVolume;
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    public string GetCurrentBGMName()
    {
        return currentBGMName;
    }

    // Ư�� BGM�� ���� ��� ������ Ȯ���ϴ� �޼���
    public bool IsBGMPlaying(string bgmName)
    {
        return currentBGMName == bgmName && (BGMSource.isPlaying || BGMSource2.isPlaying);
    }

    public void StopAllSounds()
    {
        StopAllCoroutines();
        BGMSource.Stop();
        BGMSource2.Stop();
        foreach (var source in SFXSources)
        {
            source.Stop();
        }
        activeSFXSources.Clear();
        currentBGM = null;
    }
}