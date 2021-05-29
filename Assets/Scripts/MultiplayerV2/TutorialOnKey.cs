using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOnKey : MonoBehaviour
{
    public GameObject tutorialOnPress;
    public GameObject tutorialWallpaperOnPress;
    public GameObject messageToOpenTutorial;

    // Start is called before the first frame update
    void Start()
    {
        tutorialOnPress.SetActive(true);
        tutorialWallpaperOnPress.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.H))
        {
            tutorialOnPress.SetActive(true);
            tutorialWallpaperOnPress.SetActive(true);
            messageToOpenTutorial.SetActive(false);
        }
        else
        {
            tutorialOnPress.SetActive(false);
            tutorialWallpaperOnPress.SetActive(false);

            messageToOpenTutorial.SetActive(true);
        }
    }
}
