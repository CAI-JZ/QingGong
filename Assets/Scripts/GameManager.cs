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
    public GameObject firstTutorial;

    [Header("TEST")]
    public bool isTesting;
    public int collectionCount;

    [SerializeField] private FollowCamera _mainCamera;
    private GameObject _player;
    private Vector3 currentCheckPoint;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gameState = GameState.BEFORE_START;
        _mainCamera = GetComponent<FollowCamera>();
        _player = GameObject.FindGameObjectWithTag("Player");
        firstTutorial.SetActive(false);

#if UNITY_EDITOR
        if (isTesting)
        {
            TestFunction();
        }
#endif

        collectionCount = 0;
    }

    private void TestFunction()
    { 
        UIManager.instance.ForTest();
    }

    public void AddCollection()
    {
        collectionCount += 1;
    }

    public int GetCollectiong()
    {
        return collectionCount;
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
        firstTutorial.SetActive(true);
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
