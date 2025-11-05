using UnityEngine;

public class AirCannonBody : SkillBodyBase
{
    Vector3 Direction;
    [SerializeField] float skillSpeed = 50f;

    public override void Init(ActiveSkillBase _SkillContext)
    {
        base.Init(_SkillContext);
        Direction = Worm.Instance.GetDirection();
    }
    protected override void Update()
    {
        base.Update();
        transform.position += Direction * skillSpeed * Time.deltaTime;
    }
}
