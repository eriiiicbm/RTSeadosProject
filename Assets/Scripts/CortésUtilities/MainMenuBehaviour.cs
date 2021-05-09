using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuBehaviour : MonoBehaviour
{
    public bool isPauseMenu;
    public GameObject optionsMenuPez;
    public GameObject optionsMenuMamut;
    public GameObject optionsMenuIntern;
    public SoundManager soundManager;

    public Button mainButton;
   // public Slider masterSlider;
   // public Slider BGMSlider;
   // public Slider BGSSlider;
   // public Slider SESlider;
    int backgroundNumber;
    [Header("Text Resource settings")]
    public TextStrings textStringsMainMenu;

    //TODO comentar todo 
    public void LoadLevelFromSave()
    {
        Time.timeScale = 1;
       GameManager._instance.IncreaseLevel() ;

    }
    private void OnLevelWasLoaded(int level)
    {
    //    StartStuff();
    }

    public void EndGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
    }

    public void OpenOptions()
    {
        OnLanguageChanged();
        optionsMenuIntern.SetActive(true);
    }
    public void CloseOptions()
    {

        optionsMenuIntern.SetActive(false);
    }
    public void UpdateQualityLabel()
    {
        int currentQuality = QualitySettings.GetQualityLevel();
        string qualityName = QualitySettings.names[currentQuality];
        PlayerPrefs.SetInt("currentQuality",currentQuality);
        Debug.Log("HEpaspadoad");
        optionsMenuIntern.transform.Find("QualityLevel").GetComponent<UnityEngine.UI.Text>().text =  GameManager.getStrings(textStringsMainMenu)[0] + qualityName;

    }

    public void UpdateMasterVolumeLabel()
    {
        Debug.Log(AudioListener.volume);
        float MasterAudioVolume = AudioListener.volume * 100;
        PlayerPrefs.SetFloat("masterAudioVolume", MasterAudioVolume);

        optionsMenuIntern.transform.Find("MasterVolume").GetComponent<Text>().text = ""+  GameManager.getStrings(textStringsMainMenu)[1] + "" +  MasterAudioVolume.ToString("f2") + "%";

    }
    public void UpdateBGMVolumeLabel()
    {
        float BgmAudioVolume = soundManager.AS[0].volume * 100;

        PlayerPrefs.SetFloat("bgmAudioVolume", BgmAudioVolume);
        String text = GameManager.getStrings(textStringsMainMenu)[2];
        optionsMenuIntern.transform.Find("BGMVolume").GetComponent<UnityEngine.UI.Text>().text = text + "" + BgmAudioVolume.ToString("f2") + "%";

    }
    public void UpdateBGSVolumeLabel()
    {
        
        float BgsAudioVolume = soundManager.AS[1].volume * 100;

        PlayerPrefs.SetFloat("bgsAudioVolume", BgsAudioVolume);

        optionsMenuIntern.transform.Find("BGSVolume").GetComponent<UnityEngine.UI.Text>().text = GameManager.getStrings(textStringsMainMenu)[3]  +"" + BgsAudioVolume.ToString("f2") + "%";
    }
    public void UpdateSEVolumeLabel()
    {
        float SEAudioVolume = soundManager.AS[2].volume * 100;


        PlayerPrefs.SetFloat("seAudioVolume", SEAudioVolume);
        optionsMenuIntern.transform.Find("SEVolume").GetComponent<UnityEngine.UI.Text>().text = GameManager.getStrings(textStringsMainMenu)[4] + SEAudioVolume.ToString("f2") + "%";
    }



    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void IncreaseQuality()
    {
        QualitySettings.IncreaseLevel();
        UpdateQualityLabel();
    }

    public void DecreaseQuality()
    {
        QualitySettings.DecreaseLevel();
        UpdateQualityLabel();
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
        UpdateMasterVolumeLabel();
    }
    public void SetBGMVolume(float value)
    {
        soundManager.AS[0].volume = value;
        UpdateBGMVolumeLabel();
    }
    public void SetBGSVolume(float value)
    {
        soundManager.AS[1].volume = value;
        UpdateBGSVolumeLabel();
    }
    public void SetSEVolume(float value)
    {
        soundManager.AS[2].volume = value;
        UpdateSEVolumeLabel();
    }
    
    private void Start()
    {
        //Destroy(GameObject.Find("Main Camera").gameObject);
        StartCoroutine("StartStuff");
        GameManager._instance.FixBugButtons();
        OnLanguageChanged();

    }
    public void ChangeLanguage() {
        switch (GameManager.language) {
            case GameManager.Language.En:
                GameManager.language = GameManager.Language.Es;
                break;
            case GameManager.Language.Es:
                GameManager.language = GameManager.Language.En;
                break;
        }
        Debug.Log("Current Language " + GameManager.language  );
          FindObjectOfType<SendMessagesAllLevel>().ChangeLanguage();
    //    FindObjectOfType<SendMessagesAllLevel>().ChangeLanguage();
   //     gameObject.SendMessageUpwards("OnLanguageChanged");
        GameObject.Find("LanguageText").GetComponent<Text>().text= GameManager.getStrings(textStringsMainMenu)[7] ;

    }
    public IEnumerator StartStuff()
    {

        if (!isPauseMenu)
        {
          /*  backgroundNumber = (int)UnityEngine.Random.Range(0, 2);
            if (backgroundNumber == 0)
            {
                optionsMenuMamut.transform.parent.parent.gameObject.SetActive(true);
                optionsMenuPez.transform.parent.parent.gameObject.SetActive(false);
                optionsMenuIntern = optionsMenuMamut;
            }
            else
            {
                optionsMenuMamut.transform.parent.parent.gameObject.SetActive(false);
                optionsMenuPez.transform.parent.parent.gameObject.SetActive(true);
                optionsMenuIntern = optionsMenuPez;
            }*/
        }

        
        soundManager = FindObjectOfType<SoundManager>();
        //   AudioListener.volume = PlayerPrefs.GetFloat("masterAudioVolume") / 100;
        // soundManager.AS[0].volume = PlayerPrefs.GetFloat("bgmAudioVolume") / 100;
        //soundManager.AS[1].volume = PlayerPrefs.GetFloat("bgsAudioVolume") / 100;
        // soundManager.AS[2].volume = PlayerPrefs.GetFloat("seAudioVolume") / 100;
        OnLanguageChanged();   
        Debug.Log("mas vbo" + soundManager.AS[0].volume);

     
        //  masterSlider.value = AudioListener.volume;
        //  BGMSlider.value = soundManager.AS[0].volume;
        // BGSSlider.value = soundManager.AS[1].volume;
        // SESlider.value = soundManager.AS[2].volume;
        //  UpdateBGMVolumeLabel();
        //  UpdateBGSVolumeLabel();
        // UpdateMasterVolumeLabel();
        //  UpdateQualityLabel();
        // UpdateSEVolumeLabel();
        SetMasterVolume(PlayerPrefs.GetFloat("masterAudioVolume") / 100);
        SetBGMVolume(PlayerPrefs.GetFloat("bgmAudioVolume") / 100);
        SetBGSVolume(PlayerPrefs.GetFloat("bgsAudioVolume") / 100);
        SetSEVolume(PlayerPrefs.GetFloat("seAudioVolume") / 100);
        if (!isPauseMenu)
        {
         
            mainButton.onClick.AddListener(LoadLevelFromSave);

        }  yield return new WaitForEndOfFrame();

    }
    public void OnLanguageChanged()
    {
        UpdateBGMVolumeLabel();
        UpdateBGSVolumeLabel();
        UpdateMasterVolumeLabel();
        UpdateQualityLabel();
        UpdateSEVolumeLabel();
         
    }
}
