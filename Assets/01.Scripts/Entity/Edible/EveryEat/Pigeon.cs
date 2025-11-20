using System.Collections;
using UnityEngine;

public class Pigeon : Edible
{
    public float moveSpeed = 2f;

    private void Start()
    {
        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            // 이동 상태
            float dir = Random.Range(0, 2) == 0 ? -1f : 1f;

            float elapsed = 0f;
            while (elapsed < 5f)
            {
                rb.linearVelocity = new Vector2(dir * moveSpeed, 0f);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // 정지 상태
            rb.linearVelocity = new Vector2(0f, 0f);

            yield return new WaitForSeconds(5f);
        }
    }
}
