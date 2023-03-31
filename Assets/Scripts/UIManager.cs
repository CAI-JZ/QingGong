using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [Header("Start Page")]
    [SerializeField] CanvasGroup startCanvas;
    [SerializeField] Button startGame;
    [SerializeField] Button exitGame;

    [Header("Pause")]
    [SerializeField] CanvasGroup pauseCanvas;
    [SerializeField] Button continueGame;
    [SerializeField] Button exit;

    [Header("Title")]
    [SerializeField] GameObject title;
    [SerializeField] GameObject[] titles;
    private GameObject currentTitle;

    public CanvasGroup StartCanvas => startCanvas;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (PlayerInput._instance.Pause && GameManager.instance.currentState != GameState.PAUSE)
        {
            GameManager.instance.PauseGame();
            ShowUI(pauseCanvas);
        }
    }

    private void Start()
    {
        if (title != null)
        {
            int count = title.transform.childCount;
            titles = new GameObject[count];
            for (int i = 0; i < count; i++)
            {
                titles[i] = title.transform.GetChild(i).gameObject;
            }
        }

        startGame.onClick.AddListener(OnBtnStartGame);
        exitGame.onClick.AddListener(OnBtnExitGame);
        continueGame.onClick.AddListener(OnBtnContinueGame);
        exit.onClick.AddListener(OnBtnExit);

    }

    public void SwitchTitle(int theme)
    {
        if (theme >= titles.Length)
        {
            Debug.Log("≥¨¡À£°");
            return;
        }
        currentTitle = titles[theme];
        currentTitle.SetActive(true);
        Invoke("TitleHide", 3f);
    }

    private void TitleHide()
    {
        currentTitle.SetActive(false);
    }

    private void OnBtnStartGame()
    {
        GameManager.instance.GameStart();
        HideUI(startCanvas, false);
    }

    private void OnBtnExitGame()
    {
        GameManager.instance.ExitGame();
    }

    private void OnBtnContinueGame()
    {
        HideUI(pauseCanvas, true);
        GameManager.instance.ContinueGame();
    }

    private void OnBtnExit()
    {
        GameManager.instance.ExitGame();
    }

    public void HideUI(CanvasGroup canvasGroup, bool isActive)
    {
        canvasGroup.alpha = 0;
        if (!isActive)
        {
            canvasGroup.gameObject.SetActive(isActive);
        }
    }

    private void ShowUI(CanvasGroup canvasGroup)
    {
        if (!canvasGroup.gameObject.activeSelf)
        {
            canvasGroup.gameObject.SetActive(true);
        }
        canvasGroup.alpha = 1;
    }

}
