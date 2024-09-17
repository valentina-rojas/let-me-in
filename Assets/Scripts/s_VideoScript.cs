using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class s_VideoScript : MonoBehaviour
{
    [SerializeField] VideoPlayer myVideoPlayer;

    void Start()
    {
        myVideoPlayer.loopPointReached += changeScene;
    }
    void changeScene(VideoPlayer vp)
    {
        SceneManager.LoadScene("Gameplay");
    }
}
