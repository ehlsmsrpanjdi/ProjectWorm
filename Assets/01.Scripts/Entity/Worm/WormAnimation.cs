using System.Collections.Generic;
using UnityEngine;

public class WormAnimation : MonoBehaviour
{
    [System.Serializable]
    public class Segment
    {
        public Transform transform;
        public float offset;
        public Transform prevTransform;
    }

    [HideInInspector]
    public List<Segment> segments;
    public List<Transform> tail = new List<Transform>();
    public Transform CaptureCameraTransform;

    public float followSpeed = 30f;

    public float rotationSpeed = 2f;

    public float baseOffset = 2.5f;

    private void Reset()
    {
        CaptureCameraTransform = GameObject.Find("CaptureCamera").transform;
        tail.Clear();

        tail.Add(transform.Find("NavTarget"));
        tail.Add(transform.Find("Head"));
        for (int i = 1; i < 31; ++i)
        {
            if (i < 10)
            {
                string index = "0" + i.ToString();
                Transform child = transform.Find("Spine " + index);
                tail.Add(child);
            }
            else
            {
                string index = i.ToString();
                Transform child = transform.Find("Spine " + index);
                tail.Add(child);
            }
        }
    }

    void Start()
    {
        //followSpeed = 25f;

        //rotationSpeed = 2f;

        //baseOffset = 2f;

        segments.Clear();
        int j = 0;

        foreach (Transform i in tail)
        {
            Segment k = new Segment();
            k.transform = i;

            if (i != tail[0])
            {
                k.prevTransform = tail[j];
                j++;
            }

            segments.Add(k);
        }

        foreach (Segment i in segments)
        {
            if (i.prevTransform)
                i.offset = baseOffset;
        }
    }

    void Update()
    {
        for (int i = 1; i < segments.Count; i++)
        {
            Segment cur = segments[i];
            Segment prev = segments[i - 1];

            if (cur.prevTransform == null)
                continue;

            // 목표 위치
            Vector3 dir = (cur.prevTransform.position - cur.transform.position);
            float distance = dir.magnitude;

            // 이동
            if (distance > cur.offset)
            {
                Vector3 targetPos = cur.prevTransform.position - dir.normalized * cur.offset;
                cur.transform.position = Vector3.Lerp(cur.transform.position, targetPos, followSpeed * Time.deltaTime);
            }


            // 회전
            if (dir.sqrMagnitude > 0.001f)
            {
                Vector3 prevPosition = cur.prevTransform.position;
                Vector3 mainPosition = CaptureCameraTransform.position;
                Vector3 curPosition = cur.transform.position;

                Vector3 AB = mainPosition - prevPosition;
                Vector3 AC = mainPosition - curPosition;

                Vector3 normal = Vector3.Cross(AB, AC).normalized;

                Quaternion targetRot = Quaternion.LookRotation(dir.normalized, normal);
                cur.transform.rotation = targetRot;
                //cur.transform.rotation = Quaternion.Slerp(cur.transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

                //// 대안 1번
                //Vector3 currentDir = cur.transform.forward; 
                //Vector3 targetDir = (cur.prevTransform.position - cur.transform.position).normalized;

                //Quaternion rot = Quaternion.FromToRotation(currentDir, targetDir);
                //cur.transform.rotation = rot;





                //Vector3 currentDir = cur.transform.forward;
                //Vector3 targetDir = (cur.prevTransform.position - cur.transform.position).normalized;

                //Quaternion rot = Quaternion.FromToRotation(currentDir, targetDir);

                //Quaternion rotation = Quaternion.Slerp(cur.transform.rotation, cur.prevTransform.rotation, rotationSpeed * Time.deltaTime);

                //cur.transform.rotation = rotation;
            }
        }
    }

}
