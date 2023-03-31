using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum GameState
{
    BEFORE_START,
    GAMING,
    PAUSE,
}

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get; private set; }

    private GameState gameState;

    public GameState currentState { get { return gameState; } }

    [Header("TEST")]
    public bool isTesting;

    private FollowCamera _mainCamera;
    private GameObject _player;
    private Vector3 currentCheckPoint;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gameState = GameState.BEFORE_START;
        _mainCamera = GetComponent<FollowCamera>();
        _player = GameObject.FindGameObjectWithTag("Player");

#if UNITY_EDITOR
        if (isTesting)
        {
            TestFunction();
        }
#endif

    }

    private void TestFunction()
    {
        GameStart();
        UIManager.instance.HideUI(UIManager.instance.StartCanvas, true);
    }

    public void WhenPlayerDead()
    {
        Debug.Log("PlayerDead,Respawn");
        _mainCamera.isFollow = false;
        _player.transform.position = currentCheckPoint;
        Invoke("MoveCamera", 0.5f);
    }

    private void MoveCamera()
    {
        _mainCamera.isFollow = true;
    }

    public void UpdateCheckPoint(Vector3 newPos)
    {
        currentCheckPoint = newPos;
    }

    public void GameStart()
    {
        //Set Camera
        gameState = GameState.GAMING;
        _player.GetComponent<MovementController>().GameStart();
        _mainCamera.GameStart();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        gameState = GameState.PAUSE;
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;
        gameState = GameState.GAMING;
    }
}
