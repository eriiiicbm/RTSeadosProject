using System;
using UnityEngine;
using UnityEngine.SceneManagement;
[Serializable]
public class PauseMenuBehaviour : MainMenuBehaviour
{

    public static bool isPaused;

    public GameObject pauseMenu;

    public static PauseMenuBehaviour _instance;
    

    // Use this for initialization
    void Start()
    {
        StartCoroutine("StartStuff");
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        UpdateQualityLabel();
   

        if (PauseMenuBehaviour._instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
        //pauseMenu = GameObject.Find("Pause Panel");
        //	optionsMenu = GameObject.Find("Options Panel");
    }
    public void LoadMainAndSave()
    {
        GameManager._instance.SaveIntoJson();
        GameManager._instance.ChangeLevel(0);
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp("escape"))
        {
            if (!optionsMenuIntern.activeInHierarchy)
            {
                isPaused = !isPaused;
                Time.timeScale = (isPaused ? 0 : 1);

                pauseMenu.SetActive(isPaused);
            }
            else
            {
                OpenPauseMenu();
            }
        }
    }

    public void ContinueGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  //      GameManager._instance.LoadFromJSON();
    }

      
  
 

    public void OpenOptions()
    {
        pauseMenu.SetActive(false);
        optionsMenuIntern.SetActive(true);
    }

    public void OpenPauseMenu()
    {
        
        pauseMenu.SetActive(true);
        pauseMenu.BroadcastMessage("OnLanguageChanged");
        optionsMenuIntern.SetActive(false);
    }

    public void ForcePause()
    {
        isPaused = true;
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
