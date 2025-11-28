using UnityEngine;

public class ParasiteTarget : MonoBehaviour
{
    public Transform head; // Worm의 Head Transform
    public Vector3 localOffset = new Vector3(0, -0.3f, 0);  // 턱 아래로 빼기 (원하는 값으로 조절)

    public Vector3 headForwardVec;

    void LateUpdate()
    {
        if (head == null) return;


        headForwardVec = head.transform.forward;

        LogHelper.Log(head.transform.forward.ToString());

        // 1) Head의 회전값을 가져오되 Z회전만 제거
        Vector3 headEuler = head.rotation.eulerAngles;
        headEuler.z = 0f;

        Quaternion correctedRotation = Quaternion.Euler(headEuler);

        if (headForwardVec.x > 0)
        {
            transform.position = head.position + correctedRotation * localOffset;
        }
        else
        {
            transform.position = head.position - correctedRotation * localOffset;
        }

        // 2) 위치 계산: Head의 위치 + 오프셋(회전에 따라 달라짐)

        // 3) ParasiteWorm의 회전은 correctedRotation 사용
        transform.rotation = correctedRotation;
    }
}
