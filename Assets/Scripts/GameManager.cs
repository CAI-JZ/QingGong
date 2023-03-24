using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField]public Rect Tutorial;
    private FollowCamera _mainCamera;
    private GameObject _player;


    [SerializeField] CanvasGroup welcome;
    [SerializeField] CanvasGroup gaming;
    [SerializeField] CanvasGroup esc;

    [SerializeField] Button startGame;
    Button Exit;


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
        Debug.Log("PlayerDead");
        Debug.Log("Player Go to Check Point");
    }

    private void OnGUI()
    {
        string content = "W,A,S,D ÒÆ¶¯";
        GUI.Label(Tutorial, "W,A,S,D ÒÆ¶¯");
    }

    private void WhenGameStart()
    {
        //Set Camera
        _player.GetComponent<MovementController>().GameStart();
        _mainCamera.GameStart();
        UIStartHidden();
    }

    private void UIStartHidden()
    {
        welcome.gameObject.SetActive(false);
    }

    
}
