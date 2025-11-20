using UnityEngine;

public class Citizen : Edible
{
    public float moveSpeed = 2f;

    float distance = 0;
    float moveDirection = 0;

    private void Update()
    {
        if (Worm.Instance != null)
        {
            if (Worm.Instance.wormHead.transform.position.y > 0f)
            {
                distance = Worm.Instance.wormHead.transform.position.x - transform.position.x;

                if (distance > 0)
                {
                    moveDirection = -1f;
                }
                else
                {
                    moveDirection = 1f;
                }

                distance = Mathf.Abs(distance);

                if (distance < 10)
                {
                    rb.linearVelocity = new Vector2(moveDirection * moveSpeed, 0f);
                }
                else
                {
                    rb.linearVelocity = new Vector2(0f, 0f);
                }
            }
            else
            {
                if (distance < 10)
                {
                    rb.linearVelocity = new Vector2(moveDirection * moveSpeed, 0f);
                }
                else
                {
                    rb.linearVelocity = new Vector2(0f, 0f);
                }
            }
        }
    }
}
