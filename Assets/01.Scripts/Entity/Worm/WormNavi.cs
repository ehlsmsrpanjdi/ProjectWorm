using UnityEngine;
using UnityEngine.InputSystem;

public class WormNavi : MonoBehaviour
{
    PlayerInput naviInput = null;

    InputAction verticalAction;
    InputAction horizontalAction;

    Vector3 naviAcc;

    private void Awake()
    {
        naviInput = GetComponent<PlayerInput>();

        verticalAction = naviInput.actions["WS"];
        horizontalAction = naviInput.actions["AD"];

        naviInput.enabled = true;
    }

    private void Update()
    {
        InputFunction();
        transform.position += naviAcc * Time.deltaTime;

        //if (transform.position.y < 0 && naviAcc.y < 0)
        //{
        //    transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        //    naviAcc.y = 0;
        //}
    }

    float maxSpeed = 20f;



    float tempAccX = 0f;
    float tempAccY = 0f;
    float accrate = 15f;

    void InputFunction()
    {
        tempAccX = horizontalAction.ReadValue<float>();
        tempAccY = verticalAction.ReadValue<float>();

        if (transform.position.y > 0)
        {
            tempAccY = 0f;
        }

        CaculateAcc(tempAccX * accrate, tempAccY * accrate);
    }

    float tempX = 0f;
    float tempY = 0f;

    float noneInputRatio = 5f;

    void CaculateAcc(float _x, float _y)
    {
        tempX = _x;
        tempY = _y;

        if (tempX != 0f)
        {
            naviAcc.x = Mathf.Clamp(naviAcc.x + tempX * Time.deltaTime, -maxSpeed, maxSpeed);
        }

        if (tempY != 0f)
        {
            naviAcc.y = Mathf.Clamp(naviAcc.y + tempY * Time.deltaTime, -maxSpeed, maxSpeed);
        }

        float damping = noneInputRatio * Time.deltaTime;
        naviAcc.x = Mathf.MoveTowards(naviAcc.x, 0, damping);

        if (transform.position.y < 0)
        {
            naviAcc.y = Mathf.MoveTowards(naviAcc.y, 0, damping);
        }

        CalculateGravity();
    }

    float gravityValue = -10f;

    void CalculateGravity()
    {
        if (transform.position.y > 0)
        {
            naviAcc.y = Mathf.Clamp(naviAcc.y + gravityValue * Time.deltaTime, -maxSpeed, maxSpeed);
        }
    }
}
