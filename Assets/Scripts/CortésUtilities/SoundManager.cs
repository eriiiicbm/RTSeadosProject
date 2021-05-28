using System.Collections;
using System.Collections.Generic;
using Mirror;
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
    [ClientRpc]
    public bool isSEPlaying() {
        return AS[2].isPlaying;
    }
    [ClientRpc]
    public void PlayBGM(AudioClip _clip)
    {
        if (CheckIfAudioClipIsNull(_clip))
        {
            return; 
        }
        AS[0].clip = _clip;
        AS[0].Play();


    }  [ClientRpc]
    public void PlayBGS(AudioClip _clip,bool _loop)
    {
        if (CheckIfAudioClipIsNull(_clip))
        {
            return; 
        }
        AS[1].clip = _clip;
        AS[1].loop = _loop;
        AS[1].Play();


    }

    private bool CheckIfAudioClipIsNull(AudioClip _clip)
    {
        if (_clip != null) return false;
        Debug.LogWarning("Audioclip is null");
        return true;

    }

    [ClientRpc]
    public void PlaySE(AudioClip _clip, float _pitch)
    {
        if (CheckIfAudioClipIsNull(_clip))
        {
            return; 
        }
        if(_pitch!=null)
        ChangePitch(2, _pitch);
        AS[2].PlayOneShot(_clip,5f);


    }  [ClientRpc]
    public void PlaySEIfNotPlaying(AudioClip _clip, float _pitch)
    {
        if (CheckIfAudioClipIsNull(_clip))
        {
            return; 
        }
        if (isSEPlaying()) return;
        if(_pitch!=null)
            ChangePitch(2, _pitch);
        AS[2].PlayOneShot(_clip,5f);



    }[ClientRpc]
    public void PlaySE(AudioClip _clip, float _pitch, float volume)
    {
        if (CheckIfAudioClipIsNull(_clip))
        {
            return; 
        }
        ChangePitch(2, _pitch);
        AS[2].PlayOneShot(_clip,volume);


    }
    [ClientRpc]
    public void ChangePitch(int num, float _pitch) {
        AS[num].pitch = _pitch;
    }
   
}
