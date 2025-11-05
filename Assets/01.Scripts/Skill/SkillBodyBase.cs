using UnityEngine;

public class SkillBodyBase : MonoBehaviour
{
    float skillDuration;
    float skillDamage;
    float skillScaleRatio;


    public virtual void Init(ActiveSkillBase _SkillContext)
    {
        skillDuration = _SkillContext.currentDuration;
        skillDamage = _SkillContext.currentDamage;
        skillScaleRatio = _SkillContext.currentRange;

        transform.localScale *= skillScaleRatio;
    }

    protected virtual void Update()
    {
        skillDuration -= Time.deltaTime;
        if (skillDuration < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (null != collision.GetComponent<Worm>())
        {
            return;
        }


        Entity entity = collision.GetComponent<Entity>();

        if (entity != null)
        {
            entity.TakeDamage(skillDamage);
        }
    }
}
