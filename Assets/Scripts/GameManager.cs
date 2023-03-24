using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    private FollowCamera _mainCamera;
    private GameObject _player;
    private Vector3 currentCheckPoint;

    [SerializeField] CanvasGroup welcome;
    [SerializeField] CanvasGroup gaming;
    [SerializeField] CanvasGroup esc;

    [SerializeField] Button startGame;
    [SerializeField] Button exit;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        _mainCamera = GetComponent<FollowCamera>();
        _player = GameObject.FindGameObjectWithTag("Player");
        startGame.onClick.AddListener(WhenGameStart);
        exit.onClick.AddListener(WhenExit);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WhenPlayerDead()
    {
        Debug.Log("PlayerDead,Respawn");
        _player.transform.position = currentCheckPoint;
    }

    public void UpdateCheckPoint(Vector3 newPos)
    {
        currentCheckPoint = newPos;
    }

    private void WhenGameStart()
    {
        //Set Camera
        _player.GetComponent<MovementController>().GameStart();
        _mainCamera.GameStart();
        UIStartHidden();
    }

    private void WhenExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void UIStartHidden()
    {
        welcome.gameObject.SetActive(false);
    }

    
}
