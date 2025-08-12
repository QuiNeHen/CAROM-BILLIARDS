using UnityEngine;
using UnityEngine.Events;

public class Ball : Singleton<Ball>
{
    bool isCue = false;

    bool hasCollidedWithRed = false;
    bool hasCollidedWithYellow = false;
    bool hasHandledCollisions = false;
    bool hasCollided;


    private Rigidbody myRB;


    // Constants
    public float m { get; set; } = 0.21f;
    public float R { get; set; } = 0.04048f;
    public bool HasCollided { get => hasCollided; set => hasCollided = value; }

    public override void Awake()
    {
        MakeSingleton(false);

    }

    // Start is called before the first frame update
    public override void Start()
    {
        myRB = GetComponent<Rigidbody>();

        // Set initial properties
        myRB.mass = m;
        transform.localScale = new Vector3(R * 2, R * 2, R * 2);
        Collider collider = GetComponent<Collider>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool GetCueBall()
    {
        return isCue;
    }

    public void MakeCueBall()
    {
        isCue = true;
    }

    public UnityEvent onBothBallsHit;

    private void OnCollisionEnter(Collision other)
    {
        if (CameraController.Ins.IsWhiteBall == false)
        {
            return;
        }

        if (!hasHandledCollisions && !hasCollided)
        {
            if (other.gameObject.CompareTag("RedBall"))
            {
                hasCollidedWithRed = true;
                hasCollided = true;
                AudioController.Ins.PlaySound(AudioController.Ins.ballHitBall);
            }

            if (other.gameObject.CompareTag("YellowBall"))
            {
                hasCollidedWithYellow = true;
                hasCollided = true;
                AudioController.Ins.PlaySound(AudioController.Ins.ballHitBall);
            }

            if (other.gameObject.CompareTag("table"))
            {
                hasCollided = true;
            }

            if (hasCollidedWithRed && hasCollidedWithYellow)
            {
                onBothBallsHit.Invoke();
                hasHandledCollisions = true;

                if (CameraController.Ins.IsWhiteBall == true)
                {
                    AddScorePlayer1();
                }

                ResetCollisionState();
            }
        }
    }
    private bool scoreAddedThisTurn = false;

    public void AddScorePlayer1()
    {
        if (!scoreAddedThisTurn) 
        {
            GameManager.Ins.ScorePlayer1++;
            UIManager.Ins.UpdateScorePlayer1(GameManager.Ins.ScorePlayer1); 

            if (GameManager.Ins.ScorePlayer1 == 1)
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
        hasCollidedWithYellow = false;
        hasHandledCollisions = false;
        scoreAddedThisTurn = false;
    }
}
