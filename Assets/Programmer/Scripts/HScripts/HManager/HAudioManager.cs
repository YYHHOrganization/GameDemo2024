using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class HSoundBase
{
    public string name;
    public AudioClip clip;
    
    [Range(0f, 2f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    public string path;
    public bool isLoop;
    public float spatialBlend;
    
    [HideInInspector]
    public AudioSource source;
}

public class HAudioManager : MonoBehaviour
{
    public List<HSoundBase> sounds;
    public static HAudioManager instance;
    public static HAudioManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<HAudioManager>();
            }

            return instance;
        }
    }
    private Dictionary<string, HSoundBase> soundDict = new Dictionary<string, HSoundBase>();

    public void SetAudioSourcesFromDesignTable(string audioDesignTablePath)
    {
        StartCoroutine(ReadAudioDesignTable(audioDesignTablePath));
    }

    IEnumerator ReadAudioDesignTable(string audioDesignTablePath)
    {
        string[] fileData = File.ReadAllLines(audioDesignTablePath);
        for(int i = 2;i < fileData.Length; i++)
        {
            string[] rowData = fileData[i].Split(',');
            HSoundBase sound = new HSoundBase();
            sound.name = rowData[0];
            sound.path = rowData[2];
            sound.volume = float.Parse(rowData[3]);
            sound.pitch = float.Parse(rowData[4]);
            sound.isLoop = bool.Parse(rowData[5]);
            sound.spatialBlend = float.Parse(rowData[6]);
            AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(sound.path);
            yield return handle;
            sound.clip = handle.Result;
            soundDict.Add(sound.name, sound);
        }
        Play("BeginGameMusic", this.gameObject);
        YTriggerEvents.RaiseOnLoadResourceStateChanged(true);
    }
    
    private void Awake()
    {
        // if (instance == null)
        // {
        //     instance = this;
        // }
        // else
        // {
        //     Destroy(gameObject);
        //     return;
        // }
        // DontDestroyOnLoad(gameObject);
        instance = this;

        // foreach (HSoundBase s in sounds)
        // {
        //     s.source = gameObject.AddComponent<AudioSource>();
        //     s.source.clip = s.clip;
        //
        //     s.source.volume = s.volume;
        //     s.source.pitch = s.pitch;
        //     s.source.loop = s.isLoop;
        //     s.source.spatialBlend = s.spatialBlend;
        // }
    }

    private void SetAudioSource(AudioSource source, HSoundBase s)
    {
        source.clip = s.clip;
        source.volume = s.volume;
        source.pitch = s.pitch;
        source.loop = s.isLoop;
        source.spatialBlend = s.spatialBlend;
    }

    public void Play(string name, GameObject go)
    {
        AudioSource audioSource = go.GetComponent<AudioSource>();
        if(audioSource == null)
        {
            audioSource = go.AddComponent<AudioSource>();
        }
        else
        {
            //ease out, ease in
            audioSource.Stop();
        }
        if(!soundDict.ContainsKey(name))
        {
            return;
        }
        HSoundBase s = soundDict[name];
        
        if (s == null)
        {
            Debug.LogWarning("Sound :" + name + " not found!");
            return;
        }
        
        SetAudioSource(audioSource, s);
        audioSource.Play();
    }
    

    public void Stop(GameObject go)
    {
        AudioSource audioSource = go.GetComponent<AudioSource>();
        if (audioSource)
        {
            //stop all audio sources on the game object
            audioSource.Stop();
        }
    }

    public void EaseOutAndStop(GameObject go)
    {
        AudioSource audioSource = go.GetComponent<AudioSource>();
        if (audioSource)
        {
            StartCoroutine(EaseOutAudioAndStop(audioSource));
        }
    }
    
    IEnumerator EaseOutAudioAndStop(AudioSource audioSource)
    {
        audioSource.DOFade(0, 1);
        yield return new WaitForSeconds(1);
        audioSource.Stop();
    }
    
}
