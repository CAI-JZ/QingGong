using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndUI : MonoBehaviour
{
    public Text score;

    public Button tryAgain;
    public Button Exit;

    private void Start()
    {
        tryAgain.onClick.AddListener(OnBtnTryAgain);
        Exit.onClick.AddListener(OnBtnExit);

        score.text = ScoreSave.instance.GetTreasureCount().ToString();
    }

    private void OnBtnTryAgain()
    {
        SceneManager.LoadScene(0);
    }

    private void OnBtnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
