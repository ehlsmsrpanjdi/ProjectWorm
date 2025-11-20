using Unity.VisualScripting;
using UnityEngine;

public class BulletBase : MonoBehaviour
{

    float speed = 1;

    private void Reset()
    {
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = transform.AddComponent<CircleCollider2D>();
        }
        col.isTrigger = true;
    }


    Vector3 direction;

    bool isInit = false;
    public void Init(Vector3 _Direction)
    {
        isInit = true;

        direction = _Direction.normalized;
    }

    private void Update()
    {
        if (false == isInit)
        {
            return;
        }

        transform.position += Time.timeScale * direction * speed;
    }

    private void OnDisable()
    {
        isInit = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Worm worm = collision.GetComponentInParent<Worm>();

        if (worm != null)
        {
            worm.TakeDamage(1);
        }
    }
}
