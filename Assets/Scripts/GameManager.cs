using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    [SerializeField] public static Language language;
    public int level;
    public int levelToLoad;
    public Transform player;
    public SoundManager sm;
    public AudioClip[] levelsBGM = new AudioClip[4];
    public int numOfFloors = 2;
    public bool isPlayerHiding;
    public List<Transform> subLevels ;
    public  Transform achievementsUI;
    //   public Transform spawnPoint;
    public TextStrings text;
    public int killedGhosts = 0;
  [SerializeField]  private List<string> achievementsUnlocked= new List<string>();
    Savedata savedata;
 
    [ContextMenu("Force achievement")]
    public void killGhost()
    { 
        achievementsUI = GameObject.Find("Achievement").transform;

        if (killedGhosts==0)
        {
            Debug.Log("Kill first ghost achievement");
           UnlockAchievement("KillFirstGhost");
          
        }
        killedGhosts++;
    }

    public void UnlockAchievement(string name)
    {
        StartCoroutine(nameof(PutAchievement),Resources.Load<Achievement>("Achievements/"+ name));

    }
    public Transform GetARandomTarget()
    {
        var currentSublevel = UnityEngine.Random.Range(0, subLevels.Count - 1);
        return subLevels[currentSublevel].GetChild(UnityEngine.Random.Range(0, subLevels[currentSublevel].childCount));
    }

    public Transform GetSpecificTarget(int sublevel, int npoint)
    {
        var currentSublevel = UnityEngine.Random.Range(0, subLevels.Count - 1);
        return subLevels[sublevel].GetChild(npoint);
    }

    private void Awake()
    {
        if (GameManager._instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }

     }

    void OnEnable()
    {
         SceneManager.sceneLoaded += OnLevelFinishedLoading;
        
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    // Start is called before the first frame update
    void Start()    
    {
        Application.targetFrameRate = 60;
        sm = FindObjectOfType<SoundManager>();
        ChangeLevel(levelToLoad);
    }


    public bool LoadSubLevels()
    {
        Debug.Log("sublevels Length" + subLevels.Count);
        for (int i = 0; i < 3; i++)
        {
            Transform levelTransform= new RectTransform();
            Debug.Log("Busco en posicion" +i );
            try
            {
                  levelTransform =
                    GameObject.Find("Level" + i).transform.Find("LevelTargetPoints").transform;
                //  subLevels[i] = levelTransform;

                Debug.Log("LevelTransform is" + levelTransform.name);
                if (levelTransform!=null )
                {
             
                    subLevels.Add(levelTransform);   
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                    Debug.LogError("Error: leveltransform is"  + levelTransform + "for level " + i 
                                    );
                throw;
            }
          
            
        }

        if (subLevels == null)
        {
            Debug.LogError("No sublevelsToLoad");
            return false;
        }

        return true;
    }

    public void FixBugButtons()
    {
     /*   if (level != 0) return;
        FindObjectOfType<MainMenuBehaviour>().optionsMenuMamut.transform.parent.GetChild(1).GetChild(0)
            .GetComponent<Button>().onClick.AddListener(() => { IncreaseLevel(); });
        FindObjectOfType<MainMenuBehaviour>().optionsMenuPez.transform.parent.GetChild(1).GetChild(0)
            .GetComponent<Button>().onClick.AddListener(() => { IncreaseLevel(); });
        FindObjectOfType<MainMenuBehaviour>().optionsMenuIntern.transform.parent.GetChild(1).GetChild(0)
            .GetComponent<Button>().onClick.AddListener(() => { IncreaseLevel(); });
    */}


    public void ChangeLevel(int newLevel)
    {
        levelToLoad = newLevel;
        SceneManager.LoadScene(4);
    }

    public void IncreaseLevel()
    {
        StartCoroutine(nameof(LoadFromJSON));
    }


    public static List<string> getStrings(TextStrings text)
    {
        switch (language)
        {
            case Language.En:
                return text.stringsEn;
                break;

            case Language.Es:
                return text.stringsEs;
                break;
        }

        return null;
    }

    public void SaveIntoJson()
    {
         SerializedTransform serializedTransformPlayer = new SerializedTransform(player);
        savedata = new Savedata(level, levelToLoad, killedGhosts, serializedTransformPlayer,achievementsUnlocked);
        string saveGame = "";
        saveGame += JsonUtility.ToJson(savedata);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveGame.json", saveGame);
        Debug.Log("Game saved correctly at " + Application.persistentDataPath + "/SaveGame.json");
    }

    private bool loadedFromJSON=false;
    public IEnumerator LoadFromJSON()
    {
        PauseMenuBehaviour.isPaused = true;
        try
        {
            savedata = JsonUtility.FromJson<Savedata>(
                System.IO.File.ReadAllText(Application.persistentDataPath + "/SaveGame.json"));
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e);
            Debug.Log(e.Message);
            levelToLoad = 3;
        }

        yield return new WaitForSeconds(0.5f);
        if (savedata != null&&!loadedFromJSON)
        {
            
            loadedFromJSON = true;
            killedGhosts = savedata.killedGhosts;
            levelToLoad = savedata.levelToLoad;
            achievementsUnlocked = savedata.achievementUnlocked;
       

          
           

            ChangeLevel(levelToLoad);
            Debug.Log("lEVEL IS "+ level + "  y the current scene number is" + SceneManager.GetActiveScene().buildIndex);
            while (true)
            {
                while (levelToLoad != level)
                {
                    yield return null;
                }

                if (levelToLoad==level)
                {
                    break;
                }
            }
            Debug.Log("lEVEL IS " + level + "  y level to load is " + levelToLoad);
            Debug.Log("Player is " + player);
           savedata.lastPlayerState.DeserialTransform(player);
             Debug.Log("player pos is" + player.transform.position);
            Debug.Log("Loaded JSON successfully");
            
            PauseMenuBehaviour.isPaused = false;
        }else if (loadedFromJSON)
        {
            Debug.Log("No cargues 2 veces lo mismo atontao");
        }
        else
        {
            Debug.LogError("JSON is null");
            ChangeLevel(1);
        }
     
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        subLevels = new List<Transform>();
        Debug.Log("Level Loaded");
        Debug.Log(scene.name);
        Debug.Log(mode);
        level = scene.buildIndex;

   
        
        Debug.Log(level);
                SoundManager._instance.PlayBGM(levelsBGM[level]);

        
        switch (scene.buildIndex)
        {case 0:
                loadedFromJSON = false;
                Cursor.visible=true;
                Cursor.lockState = CursorLockMode.None;
                
            break;
            case 1:
            case 2:
            case 3:
            case 5:
            case 6:
  //todo fix this         
          /*       achievementsUI = GameObject.Find("Achievement").transform;
                 Debug.Log("aCHIEVEMENTS IS " + achievementsUI);
                if (savedata == null)
                {
                    Debug.LogError("SaveDataIsNull");
                    level = SceneManager.GetActiveScene().buildIndex;
                }
               
                

                 PauseMenuBehaviour.isPaused = false;

                LoadSubLevels();
                Debug.Log("SUBlevelsLoaded");
                
*/
 
                break;
        }
    }


    public enum Language
    {
        En,
        Es
    }
    public IEnumerator PutAchievement(Achievement achievement)
    {
        if (achievementsUnlocked.Contains(achievement.name)) yield break;
        achievementsUI.GetChild(1).GetComponent<Text>().text= getStrings(achievement.MessageText)[achievement.MessagePos];
        Animator animator = achievementsUI.GetComponent<Animator>();
        animator.Play("Appear");
        yield return new WaitForSeconds(2);
        animator.Play("Disapear");
        achievement.IsObtained = true;
        achievementsUnlocked.Add(achievement.name);
    }
    [Serializable]
    internal class Savedata
    {
        public int level;
        public int levelToLoad;
        public int killedGhosts;
         public SerializedTransform lastPlayerState;
        public List<string> achievementUnlocked;
        public Savedata(int level, int levelToLoad, int killedGhosts, SerializedTransform player    ,List<string> achievementsUnlocked)
        {
            this.level = level;
            this.levelToLoad = levelToLoad;
            this.killedGhosts = killedGhosts;
            this.lastPlayerState = player;
             this.achievementUnlocked = achievementsUnlocked;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

  

}