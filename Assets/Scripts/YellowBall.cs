using UnityEngine;
using UnityEngine.Events;

public class YellowBall : Singleton<YellowBall>
{
    private bool isYellow = false;

    private bool hasCollidedWithRed = false;
    private bool hasCollidedWithWhite = false;
    private bool hasHandledCollisions = false;
    bool hasCollided;

    Rigidbody rb;

    // Constants
    private float m = 0.21f; // Mass
    private float R = 0.04048f; // Radius


    public override void Awake()
    {
        MakeSingleton(false);

    }

    public override void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.mass = m;
        transform.localScale = new Vector3(R * 2, R * 2, R * 2);

        Collider collider = GetComponent<Collider>();
    }

    void Update()
    {

    }

    public bool GetYellowBall()
    {
        return isYellow;
    }

    public void MakeYellowBall()
    {
        isYellow = true;
    }

    public UnityEvent onBothBallsHit;

    private void OnCollisionEnter(Collision other)
    {
        if (CameraController.Ins.IsWhiteBall == true)
        {
            return;
        }

        if (!hasHandledCollisions)
        {
            if (other.gameObject.CompareTag("Ball"))
            {
                hasCollidedWithWhite = true;
                hasCollided = true;
                AudioController.Ins.PlaySound(AudioController.Ins.ballHitBall);
            }

            if (other.gameObject.CompareTag("RedBall"))
            {
                hasCollidedWithRed = true;
                hasCollided = true;
                AudioController.Ins.PlaySound(AudioController.Ins.ballHitBall);
            }

            if (other.gameObject.CompareTag("table"))
            {
                hasCollided = true;
            }

            if (hasCollidedWithRed && hasCollidedWithWhite)
            {
                onBothBallsHit.Invoke();
                hasHandledCollisions = true;

                if (CameraController.Ins.IsWhiteBall == false)
                {
                    AddScorePlayer2();
                }

                ResetCollisionState();
            }
        }
    }

    private bool scoreAddedThisTurn = false;

    public bool HasCollided { get => hasCollided; set => hasCollided = value; }

    public void AddScorePlayer2()
    {
        if (!scoreAddedThisTurn) 
        {
            GameManager.Ins.ScorePlayer2++; // Cộng điểm
            UIManager.Ins.UpdateScorePlayer2(GameManager.Ins.ScorePlayer2); 


            if (GameManager.Ins.ScorePlayer2 == 1)
            {
                GameManager.Ins.isGameover = true;
                AudioController.Ins.PlaySound(AudioController.Ins.win);
            }

            if (GameManager.Ins.isGameover)
            {
                GameManager.Ins.UIScore.SetActive(false);
                GameManager.Ins.UICurrentPlayer.SetActive(false);
                GameManager.Ins.UIButton.SetActive(false);
                GameManager.Ins.UIReplay.SetActive(true);
                return;
            }

            scoreAddedThisTurn = true;
        }
    }

    public void ResetCollisionState()
    {
        //hasCollided = false;
        hasCollidedWithRed = false;
        hasCollidedWithWhite = false;
        hasHandledCollisions = false;
        scoreAddedThisTurn = false;
    }
}
