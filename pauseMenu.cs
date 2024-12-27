using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseMenu : MonoBehaviour
{
    public bool gamePaused = false;
    void Update()
    {
        if(Input.GetKey(KeyCode.Escape) && !gamePaused)
        {
            Time.timeScale = 0;
            gamePaused = true;
        }
        else if(Input.GetKey(KeyCode.Escape) && gamePaused)
        {
            Time.timeScale = 1;
            gamePaused = false;
        }
    }
}
