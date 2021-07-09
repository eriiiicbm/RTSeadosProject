using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
[Serializable]
public class PauseMenuBehaviour : MainMenuBehaviour
{

    public static bool isPaused;

    public GameObject pauseMenu;

    public static PauseMenuBehaviour _instance;
    

    // Use this for initialization
    void Start()
    {
        base.Start();
        pauseMenu.SetActive(false);
        //Time.timeScale = 1;
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
        //todo readapt this
        //  GameManager._instance.SaveIntoJson();
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
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
         //       Time.timeScale = (isPaused ? 0 : 1);

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
       // Time.timeScale = 1;
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
        wikiMenu.SetActive(false);
    }

    public void OpenWiki()
    {
        pauseMenu.SetActive(false);
        wikiMenu.SetActive(true);
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
