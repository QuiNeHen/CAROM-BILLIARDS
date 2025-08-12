using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class WhiteBallButton : Singleton<WhiteBallButton>, IPointerDownHandler
{
    private CameraController cameraController;
    private Ball ball;
    CueStick cue;

    public float theta;

    [SerializeField] private RectTransform redDotRectTransform;

    private Vector2 redSpinPosition = new Vector2(-0.83f, -1.68f); 

    public int Area;
    public float temp_angle;

    public float Theta { get => theta; set => theta = value; }
    public int AREA { get => Area; set => Area = value; }
    public float Temp_angle { get => temp_angle; set => temp_angle = value; }

    public bool resetZone;

    public bool reset;

    public override void Awake()
    {
        MakeSingleton(false);

    }

    public override void Start()
    {
        cameraController = FindObjectOfType<CameraController>();
        ball = FindObjectOfType<Ball>();
        cue = FindObjectOfType<CueStick>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Chuyển vị trí click từ Viewport Space sang Local Space của Button trắng
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            redDotRectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        // Giới hạn vị trí của chấm đỏ trong phạm vi Button trắng
        Vector2 clampedPosition = ClampPositionInsideButton(localPoint);

        // Thiết lập vị trí mới cho chấm đỏ
        redDotRectTransform.localPosition = clampedPosition;

        resetZone = IsInsideResetZone(clampedPosition);
        // Kiểm tra nếu chấm đỏ nằm trong vùng reset
        if (IsInsideResetZone(clampedPosition))
        {
            CollisionStickAndBall(clampedPosition);
            // Đặt lại vị trí của chấm đỏ về vị trí ban đầu
            redDotRectTransform.localPosition = redSpinPosition;
        }
        else
        {
            CollisionStickAndBall(clampedPosition);

            theta = angleBetweenCenterBallAndRedDot(clampedPosition);
            Area = DetermineSpinFromRedDotPosition(clampedPosition);
        }
    }

    private void Update()
    {
        reset = resetZone;
    }

    public Vector2 GetRedDotPosition()
    {
        // Trả về vị trí của chấm đỏ
        return redDotRectTransform.localPosition;
    }

    private Vector2 ClampPositionInsideButton(Vector2 position)
    {
        Vector2 center = redSpinPosition;
        float distance = Vector2.Distance(center, position);

        if (distance > 300f / 2f)
        {
            Vector2 direction = (position - center).normalized;
            position = center + direction * (300f / 2f);
        }

        return position;
    }

    public bool IsInsideResetZone(Vector2 position)
    {
        // Tính khoảng cách từ vị trí hiện tại đến tâm của hình tròn
        float distance = Vector2.Distance(redSpinPosition, position);

        // Kiểm tra nếu khoảng cách nhỏ hơn hoặc bằng đường kính của vùng reset
        return distance <= 25f;
    }

    private int DetermineSpinFromRedDotPosition(Vector2 redDotPosition)
    {
        // Tính góc tạo bởi vị trí chấm đỏ và tâm của hình tròn
        float angle = Mathf.Atan2(redDotPosition.y - redSpinPosition.y, redDotPosition.x - redSpinPosition.x) * Mathf.Rad2Deg;

        // Đảm bảo góc nằm trong khoảng từ 0 đến 360 độ
        if (angle < 0) angle += 360;

        temp_angle = angle;
        while (true)
        {
            if (temp_angle > 90)
                temp_angle = temp_angle - 90;

            if (temp_angle < 90)
                break;
        }

        Debug.Log("reset" + reset);
        // Phân vùng và chọn loại spin dựa trên góc
        if (reset)
        {
            Debug.Log("Reset");
            return 0;
        }
        else if (angle >= 80 && angle < 100)
        {
            Debug.Log("Top");
            return 1;
        }
        else if (angle >= 100 && angle < 135)
        {
            Debug.Log("Top-Left");
            return 2;
        }
        else if (angle >= 135 && angle < 170)
        {
            Debug.Log("Left-Top");
            return 3;
        }
        else if (angle >= 170 && angle < 190)
        {
            Debug.Log("Left");
            return 4;
        }
        else if (angle >= 190 && angle < 225)
        {
            Debug.Log("Left-Bottom");
            return 5;
        }
        else if (angle >= 225 && angle < 260)
        {
            Debug.Log("Bottom-Left");
            return 6;
        }
        else if (angle >= 260 && angle < 280)
        {
            Debug.Log("Bottom");
            return 7;
        }
        else if (angle >= 280 && angle < 315)
        {
            Debug.Log("Bottom-Right");
            return 8;
        }
        else if (angle >= 315 && angle < 350)
        {
            Debug.Log("Right-Bottom");
            return 9;
        }
        else if (angle >= 10 && angle < 45)
        {
            Debug.Log("Right-Top");
            return 11;
        }
        else if (angle >= 45 && angle < 80)
        {
            Debug.Log("Top-Right");
            return 12;
        }
        else
        {
            Debug.Log("Right Spin");
            return 10;
        }
    }

    private List<Vector2> redDotPositions = new List<Vector2>(); // Khai báo mảng để lưu trữ các vị trí

    private void CollisionStickAndBall(Vector2 redDotPosition)
    {
        redDotPositions.Add(redDotPosition);

        Vector2 previousRedDotPosition = redDotPositions.Count > 1 ? redDotPositions[redDotPositions.Count - 2] : redSpinPosition;


        Vector3 cameraRotation = CameraController.Ins.transform.rotation.eulerAngles;
        Vector3 cuePosition = cue.cue_stick.transform.position;

        if (IsInsideResetZone(redDotPosition))
        {
            cue.cue_stick.transform.position = cuePosition;
        }

        if (cameraRotation.y >= 45 && cameraRotation.y < 135)
        {
            // Tính toán tọa độ y và z mới của que dựa trên vị trí mới của redspin
            float newY = cuePosition.y + (redDotPosition.y - previousRedDotPosition.y) * ball.R * 2 / 300f;
            float newZ = cuePosition.z - (redDotPosition.x - previousRedDotPosition.x) * ball.R * 2 / 300f;

            cue.cue_stick.transform.position = new Vector3(cuePosition.x, newY, newZ);
        }
        else if (cameraRotation.y >= 135 && cameraRotation.y < 225)
        {
            // Tính toán tọa độ y và x mới của que dựa trên vị trí mới của redspin
            float newY = cuePosition.y + (redDotPosition.y - previousRedDotPosition.y) * ball.R * 2 / 300f;
            float newX = cuePosition.x - (redDotPosition.x - previousRedDotPosition.x) * ball.R * 2 / 300f;

            cue.cue_stick.transform.position = new Vector3(newX, newY, cuePosition.z);
        }
        else if (cameraRotation.y >= 225 && cameraRotation.y < 315)
        {
            // Tính toán tọa độ y và z mới của que dựa trên vị trí mới của redspin
            float newY = cuePosition.y + (redDotPosition.y - previousRedDotPosition.y) * ball.R * 2 / 300f;
            float newZ = cuePosition.z + (redDotPosition.x - previousRedDotPosition.x) * ball.R * 2 / 300f;

            cue.cue_stick.transform.position = new Vector3(cuePosition.x, newY, newZ);
        }
        else
        {
            // Tính toán tọa độ y và x mới của que dựa trên vị trí mới của redspin
            float newY = cuePosition.y + (redDotPosition.y - previousRedDotPosition.y) * ball.R * 2 / 300f;
            float newX = cuePosition.x + (redDotPosition.x - previousRedDotPosition.x) * ball.R * 2 / 300f;

            cue.cue_stick.transform.position = new Vector3(newX, newY, cuePosition.z);
        }
    }

    public float angleBetweenCenterBallAndRedDot(Vector2 redDotPosition)
    {
        redDotPositions.Add(redDotPosition);

        Vector2 previousRedDotPosition = redDotPositions.Count > 1 ? redDotPositions[redDotPositions.Count - 2] : redSpinPosition;

        Vector2 center = redSpinPosition;

        float direction = Vector2.Distance(center, previousRedDotPosition);

        float dis = direction * 90f / 150f;

        return dis;
    }
}