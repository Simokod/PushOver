using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public PauseMenu pauseMenu;
    public float slowDownFactor;            // controls how slow the time will move
    public float slowDownLength;            // controls how long the slow motion effect will be

    private float deathTimer = 0;
    private void Start() {
        pauseMenu = GameObject.Find("Canvas").GetComponent<PauseMenu>();
    }
    private void Update()
    {
        if (deathTimer > 0)
            deathTimer -= Time.unscaledDeltaTime;
        else
            if (deathTimer < 0 && Time.timeScale < 1 && !pauseMenu.isPaused())
                Time.timeScale += (1f / slowDownLength) * Time.unscaledDeltaTime * 2f;
    }
    public void SlowMow() {
        Time.timeScale = slowDownFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        deathTimer = .75f;
    }
}
