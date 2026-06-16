using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Tutorial_Script : MonoBehaviour
{

    [SerializeField] GameObject TutorialSkip;
    [SerializeField] GameObject TutorialVid;
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "TutorialScene")
            { TutorialVid.SetActive(true);
              TutorialSkip.SetActive(true);

        }
        else
            { TutorialVid.SetActive(false);
            TutorialSkip.SetActive(false);

        }
    }

}
