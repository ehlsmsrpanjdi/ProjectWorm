using UnityEngine;

// ⭐ 추가: 지렁이 머리에 붙일 먹기 담당 스크립트
public class WormEating : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ⭐ 추가: Edible 체크
        Edible edible = collision.GetComponent<Edible>();
        if (edible != null)
        {
            Worm.Instance?.TryEat(edible);
            return;
        }

        // ⭐ 추가: Inedible 체크
        Inedible inedible = collision.GetComponent<Inedible>();
        if (inedible != null)
        {
            Worm.Instance?.TouchInedible(inedible);
            return;
        }
    }
}