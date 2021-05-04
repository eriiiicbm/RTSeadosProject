using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]  public AudioSource[] AS;  // 0 bgm 1 bgs 2 se
    public static SoundManager _instance;
    private void Awake()
    {
        if (SoundManager._instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
            this.gameObject.name = "SoundManagerSupreme";
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isSEPlaying() {
        return AS[2].isPlaying;
    }
    public void PlayBGM(AudioClip _clip)
    {
        AS[0].clip = _clip;
        AS[0].Play();


    }  public void PlayBGS(AudioClip _clip,bool _loop)
    {
        AS[1].clip = _clip;
        AS[1].loop = _loop;
        AS[1].Play();


    }  public void PlaySE(AudioClip _clip, float _pitch)
    {
        if(_pitch!=null)
        ChangePitch(2, _pitch);
        AS[2].PlayOneShot(_clip,5f);


    }  public void PlaySEIfNotPlaying(AudioClip _clip, float _pitch)
    {
        if (isSEPlaying()) return;
        if(_pitch!=null)
            ChangePitch(2, _pitch);
        AS[2].PlayOneShot(_clip,5f);



    }public void PlaySE(AudioClip _clip, float _pitch, float volume)
    {
        ChangePitch(2, _pitch);
        AS[2].PlayOneShot(_clip,volume);


    }
    public void ChangePitch(int num, float _pitch) {
        AS[num].pitch = _pitch;
    }
   
}
