using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRBeats;

public class LoadTrack : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        PlayableManager.SetSelectedTrackIndex(0);
        //string sceneName = trackInfo.Mode == Mode.Boxing ? boxingStyleSceneName : saberStyleScenName;
        SceneManager.LoadScene(1);
    }

    
}
