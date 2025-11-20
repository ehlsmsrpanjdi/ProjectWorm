using UnityEngine;

public class PoisonComponent : MonoBehaviour
{
    float poisonTime;
    float attackDelay = 1f;
    float currentAttackDelay = 0f;

    public void AddTime(float _Time)
    {

    }
    private void Update()
    {
        if (poisonTime > 0 && Worm.Instance != null)
        {
            currentAttackDelay -= Time.deltaTime;
            if (currentAttackDelay < 0)
            {
                currentAttackDelay += attackDelay;
                poisonTime -= attackDelay;
                Worm.Instance.TakeDamage(2f);

                if(poisonTime <= 0)
                {
                    Destroy(this);
                }
            }
        }
    }
}
