using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private static PauseMenu instance;
    public static PauseMenu Instance { get { return instance; } }

    bool gamePaused;
    public bool GamePaused { get { return gamePaused; } }
    [SerializeField] GameObject pauseMenu;

    [SerializeField] RectTransform selectionPrefab;
    #region Initialise
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        Application.targetFrameRate = 30;
        gamePaused = false;
        pauseMenu.SetActive(false);
    }
    #endregion

    #region Input
    public void OnPause()
    {
        if (gamePaused)
        {
            if (currentMenu == Submenus.root)
            {
                UnpauseGame();
            }
            else
            {
                //change to the root menu
            }
        }
        else
        {
            PauseGame();
        }
    }
    void OnInteract()
    {
        //press current button
        PressButton();
    }

    void OnDirectional(InputValue val)
    {
        //change selected button. if the player presses the right direction, also count that as a selection
        Vector2 dir = val.Get<Vector2>();
        if (dir.y > 0.8f)
        {
            ChangeSelection(false);
        }
        else if (dir.y < -0.8f)
        {
            ChangeSelection(true);
        }
    }
    #endregion

    void PauseGame()
    {
        gamePaused = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        currentMenu = Submenus.root;

        currentRootSelection = 0;
        selectionPrefab.anchoredPosition = new Vector2(rootButtons[currentRootSelection].location.anchoredPosition.x, rootButtons[currentRootSelection].location.anchoredPosition.y);
    }
    void UnpauseGame()
    {
        gamePaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }


    enum Submenus
    {
        root,
        controls
    }
    Submenus currentMenu;
    [SerializeField] private GameObject rootMenu;
    [SerializeField] private GameObject controlsMenu;

    [System.Serializable]
    struct NewButton
    {
        public RectTransform location;
        public ButtonFunctions function;
    }
    [SerializeField] NewButton[] rootButtons;
    int currentRootSelection;

    enum ButtonFunctions
    {
        resumeGame,
        controlMenu,
        quitGame
    }



    void ChangeSelection(bool increase)
    {
        if (increase)
        {
            currentRootSelection++;
        }
        else
        {
            currentRootSelection--;
        }

        currentRootSelection = Mathf.Clamp(currentRootSelection, 0, rootButtons.Length - 1);

        selectionPrefab.anchoredPosition = new Vector2(rootButtons[currentRootSelection].location.anchoredPosition.x, rootButtons[currentRootSelection].location.anchoredPosition.y);
    }

    void PressButton()
    {
        switch (rootButtons[currentRootSelection].function)
        {
            case ButtonFunctions.resumeGame:
                UnpauseGame(); break;
            case ButtonFunctions.controlMenu:
                ControlsMenu(); break;
            case ButtonFunctions.quitGame:
                QuitGame(); break;
        }
    }

    void ControlsMenu()
    {

    }

    void QuitGame()
    {
        Application.Quit();
    }
}
