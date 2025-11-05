using UnityEngine;

public class ShokeWaveBody : SkillBodyBase
{
    [SerializeField] float skillSizeSpeed = 2f;

    public override void Init(ActiveSkillBase _SkillContext)
    {
        base.Init(_SkillContext);
    }
    protected override void Update()
    {
        base.Update();
        transform.localScale += Vector3.one * skillSizeSpeed * Time.deltaTime;
    }
}
