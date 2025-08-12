using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] float rotatiospeed;
    [SerializeField] Vector3 offset;
    [SerializeField] float downAngle;
    [SerializeField] GameObject cueStick;
    [SerializeField] Camera mainCamera; 

    private bool isFixedFocus = false; 

    public float maxPower;
    public float powerIncrementSpeed;
    float temp_power;

    private float power;
    public float myPower
    {
        get { return power; }
        set { power = value; }
    }
    
    private bool chargingPower = false; 


    private float horizontalInput;
    private Camera currentCamera;

    private bool isSpacePressed = false;

    private Transform cueBall;

    public Transform myCueBall
    {
        get { return cueBall; }
        set { cueBall = value; }
    }

    Vector3 lastPosition;

    GameManager gameManager;

    private int eventCount = 0;

    private bool isWhiteBall = false;
    public bool IsWhiteBall { get => isWhiteBall; set => isWhiteBall = value; }

    private bool isInitialTurnComplete = false; 

    private bool isCameraLocked = false;

    private bool hasExecutedHello = false;
    public override void Awake()
    {
        MakeSingleton(false);

    }

    public override void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball"))
        {
            if (ball.GetComponent<Ball>().GetCueBall())
            {
                cueBall = ball.transform;
                break;
            }
        }

        foreach (Ball ball in FindObjectsOfType<Ball>())
        {
            ball.onBothBallsHit.AddListener(OnBothBallsHit);
        }

        foreach (YellowBall ball in FindObjectsOfType<YellowBall>())
        {
            ball.onBothBallsHit.AddListener(OnBothBallsHit);
        }

        ResetCamera();

        currentCamera = mainCamera;

        lastPosition = cueBall.position;

        isInitialTurnComplete = false;

    }

    private void OnBothBallsHit()
    {
        eventCount++;

        Debug.Log("Đã va chạm cả hai bi");
    }

    void Update()
    {
        Rigidbody cueBallRigidbody = cueBall.gameObject.GetComponent<Rigidbody>();
        bool hasCollided;
        if (isWhiteBall)
        {   
            hasCollided = Ball.Ins.HasCollided;
        }
        else
        {
            hasCollided = YellowBall.Ins.HasCollided;
        }
            
        if (GameManager.Ins.isGameover)
            return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleCameraActivity();
        }

        if (!isCameraLocked)
        {
            if (cueBall != null)
            {
                if (!isSpacePressed)
                {
                    if (CueStick.Ins.isColliding)
                    {
                        Vector3 mouseDirection = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));
                        RaycastHit hit;
                        if (Physics.Raycast(cueBall.position, mouseDirection, out hit))
                        {

                            Vector3 newDirection = Vector3.Reflect(mouseDirection, hit.normal);
                            transform.RotateAround(cueBall.position, Vector3.up, Vector3.SignedAngle(mouseDirection, newDirection, Vector3.up));
                        }
                    }
                    else
                    {
                        horizontalInput = Input.GetAxis("Mouse X") * rotatiospeed * Time.deltaTime;
                        transform.RotateAround(cueBall.position, Vector3.up, horizontalInput);
                    }
                }
            }

            if (cueBall != null && cueBall.GetComponent<Rigidbody>().velocity.magnitude <= 0.01f)
            {
                if (!isFixedFocus)
                {
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        if (!isSpacePressed && isInitialTurnComplete)
                        {
                            ResetCamera();
                            hasExecutedHello = false;
                        }
                            
                    }
                    else
                    {
                        Ball.Ins.HasCollided = false;
                        YellowBall.Ins.HasCollided = false;
                    }
                }
            }

            if (!isFixedFocus)
            {
                
                if (Input.GetKey(KeyCode.Space) && gameObject.GetComponent<Camera>().enabled)
                {
                    chargingPower = true;
                    isSpacePressed = true;
                }
                else
                {
                    isSpacePressed = false;
                }
            }


            
            if (!Input.GetKey(KeyCode.Space) || !gameObject.GetComponent<Camera>().enabled)
            {        
                if (chargingPower)
                {
                    Vector3 hitDir = transform.forward;
                    hitDir = new Vector3(hitDir.x, 0, hitDir.z).normalized;

                    cueBallRigidbody.AddForce(hitDir * power, ForceMode.Impulse);
                    cueStick.SetActive(false);
                    
                    gameManager.SwitchCamera();

                    chargingPower = false; 
                    power = 0f; 

                    isInitialTurnComplete = true;

                }
            }

            
            if (chargingPower && power < maxPower)
            {
                power += powerIncrementSpeed * Time.deltaTime; 
                power = Mathf.Min(power, maxPower); 
                temp_power = power;
                gameManager.UpdatePower(power / maxPower);
            }

            if (Input.GetKeyDown(KeyCode.F) && gameObject.GetComponent<Camera>().enabled)
            {
                if (!isSpacePressed)
                    ToggleFixedFocus(); 
            }
        }

        Debug.Log(hasExecutedHello);

        if (!hasExecutedHello)
        {
            
            hello(cueBallRigidbody, hasCollided);
            
            if (hasCollided)
            {
                hasExecutedHello = true;
            }
        }
            
        /*// Vẽ đường di chuyển của quả bi bằng cách vẽ các đoạn thẳng
        Debug.DrawLine(lastPosition, cueBall.position, Color.red, Mathf.Infinity);
        lastPosition = cueBall.position;*/
    }

    void ToggleCameraActivity()
    {
        isCameraLocked = !isCameraLocked;
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying) 
        {
            Debug.unityLogger.logEnabled = true;
        }
        else
        {
            Debug.unityLogger.logEnabled = false;
        }
    }

    private Vector3 previousCameraPosition;
    private Quaternion previousCameraRotation;


    void ToggleFixedFocus()
    {
        isFixedFocus = !isFixedFocus;
        if (isFixedFocus)
        {
            
            previousCameraPosition = currentCamera.transform.position;
            previousCameraRotation = currentCamera.transform.rotation;

            currentCamera.fieldOfView = 80f;
            rotatiospeed = 100f;

            cueStick.transform.parent = null;
        }
        else
        {
            currentCamera.fieldOfView = 60f;
            rotatiospeed = 1440f;

            currentCamera.transform.position = previousCameraPosition;
            currentCamera.transform.rotation = previousCameraRotation;

            cueStick.transform.parent = currentCamera.transform;
        }
    }


    public void ResetCamera()
    {
        cueStick.SetActive(true);
        if (eventCount == 0)
        {
            IsWhiteBall = !IsWhiteBall;

            GameObject[] balls;
            if (IsWhiteBall)
            {
                balls = GameObject.FindGameObjectsWithTag("Ball");
            }
            else
            {
                balls = GameObject.FindGameObjectsWithTag("YellowBall");
            }

            foreach (GameObject ballObject in balls)
            {
                if ((IsWhiteBall && ballObject.GetComponent<Ball>().GetCueBall()) || (!IsWhiteBall && ballObject.GetComponent<YellowBall>().GetYellowBall()))
                {
                    cueBall = ballObject.transform;
                    break;

                }
            }
        }

        Vector3 newPosition = cueBall.position + offset;
        transform.position = newPosition;

        transform.LookAt(cueBall.position);
        transform.localEulerAngles = new Vector3(downAngle, transform.localEulerAngles.y, 0);

        foreach (Ball ball in FindObjectsOfType<Ball>())
        {
            ball.ResetCollisionState();
        }

        foreach (YellowBall ball in FindObjectsOfType<YellowBall>())
        {
            ball.ResetCollisionState();
        }

        gameManager.ResetCamera();

        isInitialTurnComplete = false;
        eventCount = 0;
    }

    public void hello(Rigidbody rb, bool hasCollided)
    {
        int Area = WhiteBallButton.Ins.AREA;
        float temp_angle = WhiteBallButton.Ins.Temp_angle;
        
        Debug.Log(Area);

        if(Area == 13 && hasCollided)
        {
            Vector3 hitDir = transform.forward;
            hitDir = new Vector3(hitDir.x, 0, hitDir.z).normalized;

            rb.AddForce(hitDir * power, ForceMode.Impulse);
        }
        // Top
        else if (Area == 1 && hasCollided)
        {
            if (temp_angle >= 45f)
                temp_angle = 90 - temp_angle;

            Vector3 hitDirForward = transform.forward;
            float angleRad = temp_angle * Mathf.Deg2Rad; // Chuyển đổi thành radian

            // Tính toán hướng di chuyển mới dựa trên góc
            float newX = hitDirForward.x * Mathf.Cos(angleRad) - hitDirForward.z * Mathf.Sin(angleRad);
            float newZ = hitDirForward.x * Mathf.Sin(angleRad) + hitDirForward.z * Mathf.Cos(angleRad);

            hitDirForward = new Vector3(newX, 0, newZ).normalized;

            rb.AddForce(hitDirForward * temp_power / maxPower, ForceMode.Impulse);
        }
        // Top - Left
        else if (Area == 2 && hasCollided)
        {
            if (temp_angle >= 45f)
                temp_angle = 90 - temp_angle;

            Vector3 hitDirForward = transform.forward;
            Vector3 hitDirLeft = -transform.right;

            float angleRad = temp_angle * Mathf.Deg2Rad;

            // Tính toán hướng di chuyển mới dựa trên góc
            float newXF = hitDirForward.x * Mathf.Cos(angleRad) - hitDirForward.z * Mathf.Sin(angleRad);
            float newZF = hitDirForward.x * Mathf.Sin(angleRad) + hitDirForward.z * Mathf.Cos(angleRad);

            float newXL = hitDirLeft.x * Mathf.Cos(angleRad) - hitDirLeft.z * Mathf.Sin(angleRad);
            float newZL = hitDirLeft.x * Mathf.Sin(angleRad) + hitDirLeft.z * Mathf.Cos(angleRad);

            hitDirForward = new Vector3(newXF, 0, newZF).normalized;
            hitDirLeft = new Vector3(newXL, 0, newZL).normalized;

            rb.AddForce(hitDirForward * temp_power / maxPower, ForceMode.Impulse);
            rb.AddForce(hitDirLeft * temp_power / maxPower, ForceMode.Impulse);

        }
        // Left - Top 
        else if (Area == 3 && hasCollided)
        {
            if (temp_angle >= 45f)
                temp_angle = 90 - temp_angle;

            Vector3 hitDirForward = transform.forward;
            Vector3 hitDirLeft = -transform.right;

            float angleRad = temp_angle * Mathf.Deg2Rad; // Chuyển đổi thành radian

            // Tính toán hướng di chuyển mới dựa trên góc
            float newXF = hitDirForward.x * Mathf.Cos(angleRad) - hitDirForward.z * Mathf.Sin(angleRad);
            float newZF = hitDirForward.x * Mathf.Sin(angleRad) + hitDirForward.z * Mathf.Cos(angleRad);

            float newXL = hitDirLeft.x * Mathf.Cos(angleRad) - hitDirLeft.z * Mathf.Sin(angleRad);
            float newZL = hitDirLeft.x * Mathf.Sin(angleRad) + hitDirLeft.z * Mathf.Cos(angleRad);

            hitDirForward = new Vector3(newXF, 0, newZF).normalized;
            hitDirLeft = new Vector3(newXL, 0, newZL).normalized;
            
            rb.AddForce(hitDirLeft * temp_power / maxPower, ForceMode.Impulse);
            rb.AddForce(hitDirForward * temp_power / maxPower, ForceMode.Impulse);
        }
        // Left
        else if (Area == 4 && hasCollided)
        {
            Debug.Log("yes");
            if (temp_angle >= 45f)
                temp_angle = 90 - temp_angle;

            Vector3 hitDirLeft = -transform.right;

            float angleRad = temp_angle * Mathf.Deg2Rad; // Chuyển đổi thành radian

            float newXL = hitDirLeft.x * Mathf.Cos(angleRad) - hitDirLeft.z * Mathf.Sin(angleRad);
            float newZL = hitDirLeft.x * Mathf.Sin(angleRad) + hitDirLeft.z * Mathf.Cos(angleRad);

            hitDirLeft = new Vector3(newXL, 0, newZL).normalized;
            
            rb.AddForce(hitDirLeft * temp_power / maxPower, ForceMode.Impulse);
        }
        // Left - Bottom
        else if (Area == 5 && hasCollided)
        {
            if (temp_angle >= 45f)
                temp_angle = 90 - temp_angle;

            Vector3 hitDirLeft = -transform.right;
            Vector3 hitDirBack = -transform.forward;

            float angleRad = temp_angle * Mathf.Deg2Rad; // Chuyển đổi thành radian

            // Tính toán hướng di chuyển mới dựa trên góc
            float newXL = hitDirLeft.x * Mathf.Cos(angleRad) - hitDirLeft.z * Mathf.Sin(angleRad);
            float newZL = hitDirLeft.x * Mathf.Sin(angleRad) + hitDirLeft.z * Mathf.Cos(angleRad);

            float newXB = hitDirBack.x * Mathf.Cos(angleRad) - hitDirBack.z * Mathf.Sin(angleRad);
            float newZB = hitDirBack.x * Mathf.Sin(angleRad) + hitDirBack.z * Mathf.Cos(angleRad);

            hitDirLeft = new Vector3(newXL, 0, newZL).normalized;
            hitDirBack = new Vector3(newXB, 0, newZB).normalized;

            rb.AddForce(hitDirLeft * temp_power / maxPower, ForceMode.Impulse);
            rb.AddForce(hitDirBack * temp_power / maxPower, ForceMode.Impulse);
        }
        // Bottom - Left
        else if (Area == 6 && hasCollided)
        {
            if (temp_angle >= 45f)
                temp_angle = 90 - temp_angle;

            Vector3 hitDirBack = -transform.forward;
            Vector3 hitDirLeft = -transform.right;

            float angleRad = temp_angle * Mathf.Deg2Rad; // Chuyển đổi thành radian

            // Tính toán hướng di chuyển mới dựa trên góc
            float newXB = hitDirBack.x * Mathf.Cos(angleRad) - hitDirBack.z * Mathf.Sin(angleRad);
            float newZB = hitDirBack.x * Mathf.Sin(angleRad) + hitDirBack.z * Mathf.Cos(angleRad);

            float newXL = hitDirLeft.x * Mathf.Cos(angleRad) - hitDirLeft.z * Mathf.Sin(angleRad);
            float newZL = hitDirLeft.x * Mathf.Sin(angleRad) + hitDirLeft.z * Mathf.Cos(angleRad);

            hitDirBack = new Vector3(newXB, 0, newZB).normalized;
            hitDirLeft = new Vector3(newXL, 0, newZL).normalized;
            
            rb.AddForce(hitDirBack * temp_power / maxPower, ForceMode.Impulse);
            rb.AddForce(hitDirLeft * temp_power / maxPower, ForceMode.Impulse);
        }
        // Bottom
        else if (Area == 7 && hasCollided)
        {
            if (temp_angle >= 45f)
                temp_angle = 90 - temp_angle;

            Vector3 hitDirBack = -transform.forward;

            float angleRad = temp_angle * Mathf.Deg2Rad; // Chuyển đổi thành radian

            // Tính toán hướng di chuyển mới dựa trên góc
            float newXB = hitDirBack.x * Mathf.Cos(angleRad) - hitDirBack.z * Mathf.Sin(angleRad);
            float newZB = hitDirBack.x * Mathf.Sin(angleRad) + hitDirBack.z * Mathf.Cos(angleRad);

            hitDirBack = new Vector3(newXB, 0, newZB).normalized;
            
            rb.AddForce(hitDirBack * temp_power / maxPower, ForceMode.Impulse);
        }
        // Bottom - Right
        else if (Area == 8 && hasCollided)
        {
            if (temp_angle >= 45f)
                temp_angle = 90 - temp_angle;

            Vector3 hitDirBack = -transform.forward;
            Vector3 hitDirRight = transform.right;

            float angleRad = temp_angle * Mathf.Deg2Rad; // Chuyển đổi thành radian

            // Tính toán hướng di chuyển mới dựa trên góc
            float newXB = hitDirBack.x * Mathf.Cos(angleRad) + hitDirBack.z * Mathf.Sin(angleRad);
            float newZB = -hitDirBack.x * Mathf.Sin(angleRad) + hitDirBack.z * Mathf.Cos(angleRad);

            float newXR = hitDirRight.x * Mathf.Cos(angleRad) + hitDirRight.z * Mathf.Sin(angleRad);
            float newZR = -hitDirRight.x * Mathf.Sin(angleRad) + hitDirRight.z * Mathf.Cos(angleRad);

            hitDirBack = new Vector3(newXB, 0, newZB).normalized;
            hitDirRight = new Vector3(newXR, 0, newZR).normalized;
            
            rb.AddForce(hitDirBack * temp_power / maxPower, ForceMode.Impulse);
            rb.AddForce(hitDirRight * temp_power / maxPower, ForceMode.Impulse);
        }
        // Right - Bottom
        else if (Area == 9 && hasCollided)
        {
            if (temp_angle >= 45f)
                temp_angle = 90 - temp_angle;

            Vector3 hitDirRight = transform.right;
            Vector3 hitDirBack = -transform.forward;

            float angleRad = temp_angle * Mathf.Deg2Rad; // Chuyển đổi thành radian

            // Tính toán hướng di chuyển mới dựa trên góc
            float newXR = hitDirRight.x * Mathf.Cos(angleRad) + hitDirRight.z * Mathf.Sin(angleRad);
            float newZR = -hitDirRight.x * Mathf.Sin(angleRad) + hitDirRight.z * Mathf.Cos(angleRad);

            float newXB = hitDirBack.x * Mathf.Cos(angleRad) + hitDirBack.z * Mathf.Sin(angleRad);
            float newZB = -hitDirBack.x * Mathf.Sin(angleRad) + hitDirBack.z * Mathf.Cos(angleRad);

            hitDirRight = new Vector3(newXR, 0, newZR).normalized;
            hitDirBack = new Vector3(newXB, 0, newZB).normalized;
            
            rb.AddForce(hitDirRight * temp_power / maxPower, ForceMode.Impulse);
            rb.AddForce(hitDirBack * temp_power / maxPower, ForceMode.Impulse);
        }
        // Right
        else if (Area == 10 && hasCollided)
        {
            if (temp_angle >= 45f)
                temp_angle = 90 - temp_angle;

            Vector3 hitDirRight = transform.right;
            float angleRad = temp_angle * Mathf.Deg2Rad; // Chuyển đổi thành radian

            // Tính toán hướng di chuyển mới dựa trên góc
            float newX = hitDirRight.x * Mathf.Cos(angleRad) + hitDirRight.z * Mathf.Sin(angleRad);
            float newZ = -hitDirRight.x * Mathf.Sin(angleRad) + hitDirRight.z * Mathf.Cos(angleRad);

            
            hitDirRight = new Vector3(newX, 0, newZ).normalized;

            rb.AddForce(hitDirRight * temp_power / maxPower, ForceMode.Impulse);
        }
        // Right - Top
        else if (Area == 11 && hasCollided)
        {
            if (temp_angle >= 45f)
                temp_angle = 90 - temp_angle;

            Vector3 hitDirRight = transform.right;
            Vector3 hitDirForward = transform.forward;

            float angleRad = temp_angle * Mathf.Deg2Rad; // Chuyển đổi thành radian

            // Tính toán hướng di chuyển mới dựa trên góc
            float newXR = hitDirRight.x * Mathf.Cos(angleRad) + hitDirRight.z * Mathf.Sin(angleRad);
            float newZR = -hitDirRight.x * Mathf.Sin(angleRad) + hitDirRight.z * Mathf.Cos(angleRad);

            float newXF = hitDirForward.x * Mathf.Cos(angleRad) + hitDirForward.z * Mathf.Sin(angleRad);
            float newZF = -hitDirForward.x * Mathf.Sin(angleRad) + hitDirForward.z * Mathf.Cos(angleRad);

            hitDirRight = new Vector3(newXR, 0, newZR).normalized;
            hitDirForward = new Vector3(newXF, 0, newZF).normalized;

            rb.AddForce(hitDirRight * temp_power / maxPower, ForceMode.Impulse);
            rb.AddForce(hitDirForward * temp_power / maxPower, ForceMode.Impulse);
        }
        // Top - Right
        else if (Area == 12 && hasCollided)
        {
            if (temp_angle >= 45f)
                temp_angle = 90 - temp_angle;

            Vector3 hitDirForward = transform.forward;
            Vector3 hitDirRight = transform.right;

            float angleRad = temp_angle * Mathf.Deg2Rad; // Chuyển đổi thành radian

            // Tính toán hướng di chuyển mới dựa trên góc
            float newXF = hitDirForward.x * Mathf.Cos(angleRad) + hitDirForward.z * Mathf.Sin(angleRad);
            float newZF = -hitDirForward.x * Mathf.Sin(angleRad) + hitDirForward.z * Mathf.Cos(angleRad);

            float newXR = hitDirRight.x * Mathf.Cos(angleRad) + hitDirRight.z * Mathf.Sin(angleRad);
            float newZR = -hitDirRight.x * Mathf.Sin(angleRad) + hitDirRight.z * Mathf.Cos(angleRad);

            hitDirForward = new Vector3(newXF, 0, newZF).normalized;
            hitDirRight = new Vector3(newXR, 0, newZR).normalized;

            rb.AddForce(hitDirForward * temp_power / maxPower, ForceMode.Impulse);
            rb.AddForce(hitDirRight * temp_power / maxPower, ForceMode.Impulse);
        }

        Ball.Ins.HasCollided = false;
        YellowBall.Ins.HasCollided = false;
    }
}