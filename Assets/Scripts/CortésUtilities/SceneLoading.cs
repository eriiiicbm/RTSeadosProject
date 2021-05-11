using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{
    public int sceneLevel;
    [SerializeField]
    private Image _progressBar;
    private void Awake()
    {
        sceneLevel = GameManager._instance.levelToLoad;
    }
    // Start is called before the first frame update
    void Start()
    {
       //sceneLevel = FindObjectOfType<GameManager>().levelToLoad;
        StartCoroutine(LoadAsyncOperation());
        _progressBar.fillAmount = 50;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator LoadAsyncOperation() {
        yield return null;
        float fakeProgress = 0;
        Debug.Log("Ahora empezara el ciclo");
         AsyncOperation load = SceneManager.LoadSceneAsync(sceneLevel);
        while (load.progress<1 && fakeProgress <1) {
            fakeProgress = load.progress ;
            _progressBar.fillAmount = fakeProgress;
             yield return new WaitForSeconds(40);

            Debug.Log("Ciclo hecho");
        }
        _progressBar.color = Color.red;
        yield return new WaitForSeconds(1);
        Debug.Log("He esperado de verdad");
 
    }
}
