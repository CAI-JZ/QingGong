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
    [SerializeField] CanvasGroup[] titles;
    private CanvasGroup currentTitle;
    [SerializeField] private float fadeMul;
    [SerializeField] private float titleHideDelay;

    [Header("Gaming")]
    [SerializeField] CanvasGroup Gaming;
    [SerializeField] Text collection;

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
        CollectionUpdate();
    }

    private void Start()
    {
        startGame.onClick.AddListener(OnBtnStartGame);
        exitGame.onClick.AddListener(OnBtnExitGame);
        continueGame.onClick.AddListener(OnBtnContinueGame);
        exit.onClick.AddListener(OnBtnExit);

    }

    private void CollectionUpdate()
    {
        collection.text = GameManager.instance.GetCollectiong().ToString();
    }

    public void SwitchTitle(int theme)
    {
        if (theme >= titles.Length)
        {
            Debug.Log("ณฌมหฃก");
            return;
        }
        currentTitle = titles[theme];
        ShowUI(currentTitle);
        Invoke("TitleHide", titleHideDelay);
    }

    private void TitleHide()
    {
        HideUI(currentTitle.GetComponent<CanvasGroup>(),true);
    }

    public void ForTest()
    {
        OnBtnStartGame();
    }

    private void OnBtnStartGame()
    {
        GameManager.instance.GameStart();
        HideUI(startCanvas, false);
        ShowUI(Gaming);
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
        StartCoroutine(DecreaseAlpha(canvasGroup));
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
        StartCoroutine(IncreaseAlpha(canvasGroup));
    }


    IEnumerator DecreaseAlpha(CanvasGroup canvas)
    {
        while (canvas.alpha > 0)
        {
            canvas.alpha -= Time.deltaTime * fadeMul;
            yield return new WaitForFixedUpdate();
        }

        canvas.alpha = 0;
    }

    IEnumerator IncreaseAlpha(CanvasGroup canvas)
    {
        while (canvas.alpha < 1)
        {
            canvas.alpha += Time.deltaTime * fadeMul;
            yield return new WaitForFixedUpdate();
        }

        canvas.alpha = 1;
    }
}
