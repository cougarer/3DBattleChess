using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

public class SoundManager : MonoBehaviour {

    private Queue<AudioSource> freeAS;
    private Queue<AudioSource> usedAS;
    private Dictionary<string, AudioClip> soundDictionary;
    private AudioSource bgmAS;

    private float audioVol;

    void Awake()
    {
        bgmAS = this.gameObject.AddComponent<AudioSource>() as AudioSource;
        soundDictionary = new Dictionary<string, AudioClip>();
        freeAS = new Queue<AudioSource>();
        usedAS = new Queue<AudioSource>();
        for (int i = 0; i < 10; i++) freeAS.Enqueue(new AudioSource());
        AudioClip[] audioArray = Resources.LoadAll<AudioClip>("Audios");
        foreach (AudioClip ac in audioArray)
        {
            soundDictionary.Add(ac.name, ac);
        }
        bgmAS.volume = PlayerPrefs.GetFloat("BGMVol");
        audioVol = PlayerPrefs.GetFloat("audioVol");
    }
    public void PlayBGM(string name)
    {
        if (!soundDictionary.ContainsKey(name))
        {
            Debug.Log(name + "声音不存在！");
            return;
        }
        if (bgmAS.volume != 0)
        {
            bgmAS.clip = null;
            bgmAS.clip = soundDictionary[name];
            bgmAS.loop = true;
            bgmAS.Play();
        }
    }
    public bool isPlayingBGM
    {
        get { return bgmAS.isPlaying; }
    }
    public void PlayAudio(string name)
    {
        if (!soundDictionary.ContainsKey(name))
        {
            Debug.Log(name + "声音不存在！");
            return;
        }
        if (audioVol != 0 && freeAS.Count > 0)
        {
            AudioSource aS = freeAS.Dequeue();
            aS = this.gameObject.AddComponent<AudioSource>() as AudioSource;
            aS.clip = soundDictionary[name];
            aS.volume = audioVol;
            aS.Play();
            usedAS.Enqueue(aS);
            StartCoroutine("ICollectFreeAS");
        }
    }
    IEnumerator ICollectFreeAS()
    {
        yield return new WaitForSeconds(5.0f);
        AudioSource aS = usedAS.Dequeue();
        aS.Stop();
        aS.clip = null;
        Object.Destroy(aS);
        freeAS.Enqueue(aS);
    }
    public void StopBGM()
    {
        if (bgmAS.isPlaying)
        {
            bgmAS.Stop();
        }
    }
    public void ChangeBGMVol(float value)
    {
        //PlayerPrefs.SetFloat("BGMVol", value);
        bgmAS.volume = value;
    }
    public void ChangeAudioVol(float value)
    {
        //PlayerPrefs.SetFloat("audioVol", value);
        audioVol = value;
        //Global.instance.SoundManager.PlayAudio("BtnClickSound");
    }
}
