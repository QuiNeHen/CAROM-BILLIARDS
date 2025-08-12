using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public Camera mainCamera;
    public Camera otherCamera;
    public Camera menuCamera;
    public GameObject UIButton; // Tham chiếu đến GameObject chứa UI buttons
    public GameObject UIButtonIMG;
    public GameObject UIIntroduce;
    public GameObject UIReplay;
    public GameObject UIScore;
    public GameObject UICurrentPlayer;

    public Image powerFilled;

    int scorePlayer1;
    int scorePlayer2;
    bool temp_isGameover;
    string currentPlayer;

    public int ScorePlayer1 { get => scorePlayer1; set => scorePlayer1 = value; }
    public int ScorePlayer2 { get => scorePlayer2; set => scorePlayer2 = value; }

    public bool isGameover { get => temp_isGameover; set => temp_isGameover = value; }
    public string CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }

    public override void Awake()
    {
        MakeSingleton(false);
    }


    // Start is called before the first frame update
    public override void Start()
    {

        mainCamera.enabled = false;
        otherCamera.enabled = false;
        menuCamera.enabled = true;

        // Ẩn các UI buttons
        UIButton.SetActive(false);
        UIButtonIMG.SetActive(true);
        UIReplay.SetActive(false);
        UIScore.SetActive(false);
        UICurrentPlayer.SetActive(false);
        UIIntroduce.SetActive(false);

        UIManager.Ins.UpdateScorePlayer1(scorePlayer1);
        UIManager.Ins.UpdateScorePlayer2(scorePlayer2);

        UIManager.Ins.CurrentPlayer(currentPlayer);
    }

    public void IntroGame()
    {
        mainCamera.enabled = false;
        otherCamera.enabled = false;
        menuCamera.enabled = true;

        UIButtonIMG.SetActive(false);
        UIIntroduce.SetActive(true);
    }

    public void StartGame()
    {

        mainCamera.enabled = true;
        otherCamera.enabled = true;
        menuCamera.enabled = false;

        UIButton.SetActive(true);
        UIButtonIMG.SetActive(false);
        UIScore.SetActive(true);
        UICurrentPlayer.SetActive(true);
        UIIntroduce.SetActive(false);
    }

    public void SwitchCamera()
    {
        mainCamera.enabled = !mainCamera.enabled;
        otherCamera.enabled = !otherCamera.enabled;
    }

    public void ResetCamera()
    {
        mainCamera.enabled = true;
        otherCamera.enabled = false;

        powerFilled.fillAmount = 0;
    }

    public void UpdatePower(float power)
    {
        if (powerFilled)
        {
            powerFilled.fillAmount = power;
        }
    }
}