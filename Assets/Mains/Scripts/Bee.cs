using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class BeeAI : MonoBehaviour
{
    public Transform target;
    public float speed = 3f;
    public float avoidForce = 5f;
    public float rayDistance = 1f;
    public float wanderStrength = 0.5f; // randomness in movement

    [Header("Bounce Settings")]
    public float bounceDistance = 0.5f;
    public float bounceDuration = 0.3f;
    public float bounceRandomness = 0.5f;

    private Rigidbody2D rb;
    private bool isBouncing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        if (isBouncing || target == null)
            return;

        Vector2 toTarget = ((Vector2)target.position - rb.position).normalized;
        Vector2 avoid = Vector2.zero;

        // Multi-ray obstacle avoidance (like a "feelers" system)
        int rayCount = 5;
        float spread = 60f;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = -spread / 2 + (spread / (rayCount - 1)) * i;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * toTarget;

            RaycastHit2D hit = Physics2D.Raycast(rb.position, dir, rayDistance, LayerMask.GetMask("Wall"));
            if (hit.collider != null)
            {
                avoid += hit.normal * avoidForce / rayCount;
            }
        }

        // Add a little wander so they don’t all follow identical paths
        Vector2 wander = Random.insideUnitCircle * wanderStrength;

        Vector2 finalDir = (toTarget + avoid + wander).normalized;
        rb.linearVelocity = finalDir * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBouncing) return;
        if (collision.collider.CompareTag("Wall"))
        {
            Vector2 normal = collision.contacts[0].normal;
            StartCoroutine(SmoothBounce(normal));
        }
    }

    private IEnumerator SmoothBounce(Vector2 normal)
    {
        isBouncing = true;
        rb.linearVelocity = Vector2.zero;

        Vector2 randomDir = (normal + Random.insideUnitCircle * bounceRandomness).normalized;
        Vector2 startPos = rb.position;
        Vector2 endPos = startPos + randomDir * bounceDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / bounceDuration;
            rb.MovePosition(Vector2.Lerp(startPos, endPos, t));
            yield return null;
        }

        isBouncing = false;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector2 dir = (target.position - transform.position);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (rb == null) return;
        Gizmos.color = Color.yellow;
        Vector2 dir = target ? ((Vector2)target.position - rb.position).normalized : (Vector2)transform.right;
        Gizmos.DrawLine(rb.position, rb.position + dir * rayDistance);
    }
}
