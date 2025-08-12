using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] Text scorePlayer1Text;
    [SerializeField] Text scorePlayer2Text;
    [SerializeField] Text currentPlayerText;
    [SerializeField] Text messageText;
    [SerializeField] Text winText;
    [SerializeField] InputField namePlayer1;
    [SerializeField] InputField namePlayer2;
    [SerializeField] GameObject cueStick;

    public string name1;
    public string name2;

    public override void Awake()
    {
        MakeSingleton(false);

    }

    // Start is called before the first frame update
    public override void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        name1 = GetNamePlayer1();
        name2 = GetNamePlayer2();

        UpdateScorePlayer1(GameManager.Ins.ScorePlayer1);
        UpdateScorePlayer2(GameManager.Ins.ScorePlayer2);

        if (CameraController.Ins.IsWhiteBall)
        {
            CurrentPlayer(name1);
            MessageText(name1);
        }
        else
        {
            CurrentPlayer(name2);
            MessageText(name2);
        }

        if (GameManager.Ins.ScorePlayer1 == 1)
        {
            Win(name1 + " Win");
        }
        else
        {
            Win(name2 + " Win");
        }
    }

    public string GetNamePlayer1()
    {
        return namePlayer1.text;
        
    }

    public string GetNamePlayer2()
    {
        return namePlayer2.text;
    }

    public void UpdateScorePlayer1(int score)
    {
        if (scorePlayer1Text)
        {
            scorePlayer1Text.text = name1 + ": " + score.ToString();
        }
    }

    public void UpdateScorePlayer2(int score)
    {
        if (scorePlayer2Text)
        {
            scorePlayer2Text.text = name2 + ": " + score.ToString();
        }
    }

    public void CurrentPlayer(string player)
    {
        currentPlayerText.text = "Current Turn: " + player;
    }

    public void MessageText(string message)
    {
        StartCoroutine(DisplayMessage(message, 2f));
    }

    IEnumerator DisplayMessage(string message, float duration)
    {
        messageText.text = message;
        yield return new WaitForSeconds(duration);
        messageText.text = "";
    }

    public void Win(string playerWin)
    {
        winText.text = playerWin;
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        Application.Quit();
    }
}
