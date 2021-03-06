﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageGameOverMenu : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private string quitScene = "Title";
    [SerializeField]
    private GameObject menuItems;
    [SerializeField]
    private Text scoreNumberText, highScoreNumberText;
    [SerializeField]
    private GameObject highScoreIndicator;
    [SerializeField]
    private Image fadeBG;
    [SerializeField]
    private float fadeSpeed;
    [SerializeField]
    private float quitShiftTime;
    [SerializeField]
    private MenuButton[] menuButtons;
    [SerializeField]
    private FadingMusic fadingMusic;
    [SerializeField]
    private float showcaseModeCooldown = 60f;
#pragma warning restore 0649

    private float BGGoalAlpha;
    private float lastGameOverTime;
    private bool hasQuit;

    private State state;
    private enum State
    {
        FadeIn,
        Menu,
        FadeOut
    }


    public void initialize(int score)
    {
        lastGameOverTime = Time.time;

        if (BGGoalAlpha == 0f)
            BGGoalAlpha = getBGAlpha();
        setBGAlpha(0f);

        state = State.FadeIn;
        gameObject.SetActive(true);
        PauseManager.disablePause = true;

        fadingMusic.GetComponent<AudioSource>().time = 0f;
        fadingMusic.startFade();

        foreach (MenuButton menuButton in menuButtons)
        {
            menuButton.forceDisable = false;
        }

        int currentHighScore = PrefsHelper.getHighScore(gameObject.scene.name);
        if (highScoreIndicator != null)
            highScoreIndicator.SetActive(score > currentHighScore);
        if (score > currentHighScore)
        {
            currentHighScore = score;
            PrefsHelper.setHighScore(gameObject.scene.name, currentHighScore);
        }
        setNumber(scoreNumberText, score);
        setNumber(highScoreNumberText, currentHighScore);
	}

    void Update()
    {
        switch(state)
        {
            case (State.FadeIn):
                UpdateFade(true);
                break;
            case (State.Menu):
                if (GameController.instance.ShowcaseMode
                    && !hasQuit
                    && Time.time > lastGameOverTime + showcaseModeCooldown)
                {
                    GameMenu.subMenu = GameMenu.SubMenu.Splash;
                    quit();
                }
                break;
            case (State.FadeOut):
                UpdateFade(false);
                break;
            default:
                break;
        }
    }

    void UpdateFade(bool fadeIn)
    {
        float diff = fadeSpeed * Time.deltaTime,
            alpha = getBGAlpha();
        if (fadeIn)
        {
            if (alpha + diff >= BGGoalAlpha)
            {
                setBGAlpha(BGGoalAlpha);
                menuItems.SetActive(true);
                foreach (MenuButton menuButton in menuButtons)
                {
                    menuButton.GetComponent<Animator>().Play("Normal");
                }
                state = State.Menu;
            }
            else
                setBGAlpha(alpha + diff);
        }
        else
        {
            if (alpha - diff <= 0f)
            {
                setBGAlpha(0f);
                PauseManager.disablePause = false;
            }
            else
                setBGAlpha(alpha - diff);
        }
    }

    public void retry()
    {
        if (state != State.Menu)
            return;

        Invoke("disableMenuItems", .15f);
        GameObject.Find("Stage Controller").GetComponent<StageController>().retry();
        state = State.FadeOut;
        fadingMusic.startFade();
    }

    public void quit()
    {
        if (state != State.Menu)
            return;

        GameController.instance.sceneShifter.startShift(quitScene, quitShiftTime);
        fadingMusic.startFade();
        hasQuit = true;
    }

    void disableMenuItems()
    {
        menuItems.SetActive(false);
    }

    void setNumber(Text textComponent, int score)
    {
        textComponent.text = textComponent.text.Substring(0, textComponent.text.Length - 3);

        int number = score;
        textComponent.text += number.ToString("D3");
    }

    void setBGAlpha(float alpha)
    {
        Color color = fadeBG.color;
        color.a = alpha;
        fadeBG.color = color;
    }

    float getBGAlpha()
    {
        return fadeBG.color.a;
    }
}
