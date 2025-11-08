using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Beehive : MonoBehaviour
{
    [Header("Bee Settings")]
    public int BeeAmount = 10;
    public BeeAI BeePrefab;
    public List<BeeAI> bees = new List<BeeAI>();
    public Transform playerTarget;

    [Header("Launch Settings")]
    public float launchForce = 5f;
    public float launchInterval = 0.2f;
    public float spawnRadius = 0.3f;   // where bees spawn around hive
    public float launchSpread = 360f;  // full random direction

    private bool isLaunching;

    private void Start()
    {
        bees.Clear();

        for (int i = 0; i < BeeAmount; i++)
        {
            // 🐝 Create inactive bees around hive
            Vector2 spawnOffset = Random.insideUnitCircle * spawnRadius;
            var bee = Instantiate(BeePrefab, (Vector2)transform.position + spawnOffset, Quaternion.identity, this.transform);
            bee.gameObject.SetActive(false);
            bees.Add(bee);
        }
    }

    /// <summary>
    /// Call this externally to start launching bees.
    /// </summary>
    public void Begin()
    {
        if (!isLaunching)
            StartCoroutine(ActivateBees());
    }

    private IEnumerator ActivateBees()
    {
        isLaunching = true;

        foreach (var bee in bees)
        {
            if (bee == null) continue;

            bee.gameObject.SetActive(true);
            bee.target = playerTarget;

            Rigidbody2D rb = bee.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // 🌀 Launch in random direction
                float angle = Random.Range(0f, 360f);
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                rb.linearVelocity = dir * launchForce;
            }

            yield return new WaitForSeconds(launchInterval);
        }

        isLaunching = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.8f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
