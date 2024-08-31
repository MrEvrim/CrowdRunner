using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class enemyManager : MonoBehaviour
{
    [SerializeField] public TextMeshPro CounterTxt;
    [SerializeField] private GameObject stickMan;
    [Range(0f, 1f)][SerializeField] private float DistanceFactor, Radius;

    public Transform enemy;
    public bool attack;

    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();

    void Start()
    {
        for (int i = 0; i < UnityEngine.Random.Range(20, 100); i++)
        {
            Instantiate(stickMan, transform.position, Quaternion.identity, transform);
        }

        // Her karakterin orijinal pozisyonunu sakla
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            originalPositions[child] = child.localPosition; // Pozisyonu sakla
        }

        CounterTxt.text = (transform.childCount - 1).ToString();
        FormatStickMan();
    }

    public void UpdateCounterText()
    {
        CounterTxt.text = (transform.childCount - 1).ToString();
    }

    private void EndAttack()
    {
        // Saldırı sona erdiğinde karakterleri eski pozisyonlarına döndür
        attack = false;

        foreach (var kvp in originalPositions)
        {
            Transform child = kvp.Key;
            Vector3 originalPos = kvp.Value;

            // Karakterleri eski pozisyonlarına dönüştür
            child.DOLocalMove(originalPos, 1f).SetEase(Ease.OutBack);
            child.GetComponent<Animator>().SetBool("run", false);
        }

        if (enemy != null)
        {
            Destroy(enemy.gameObject);
        }
    }

    private void FormatStickMan()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            var x = DistanceFactor * Mathf.Sqrt(i) * Mathf.Cos(i * Radius);
            var z = DistanceFactor * Mathf.Sqrt(i) * Mathf.Sin(i * Radius);

            var newPos = new Vector3(x, -0.08801528f, z);
            transform.GetChild(i).localPosition = newPos;
        }
    }

    void Update()
    {
        if (attack && transform.childCount > 1)
        {
            var enemyPos = new Vector3(enemy.position.x, transform.position.y, enemy.position.z);

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                // Rotate characters towards the enemy
                Vector3 directionToEnemy = (enemyPos - child.position).normalized;
                if (directionToEnemy != Vector3.zero)
                {
                    child.rotation = Quaternion.Slerp(
                        child.rotation,
                        Quaternion.LookRotation(directionToEnemy, Vector3.up),
                        Time.deltaTime * 3f);
                }

                // Move the character towards the enemy
                float distanceToEnemy = Vector3.Distance(child.position, enemy.position);
                if (distanceToEnemy < 1.5f)
                {
                    child.position = Vector3.Lerp(child.position, enemy.position, Time.deltaTime * 3f);
                }
                else
                {
                    child.position = Vector3.MoveTowards(child.position, enemyPos, Time.deltaTime * 7f);
                }
            }

            // Update count on screen
            UpdateCounterText();

            if (enemy.childCount == 0)
            {
                EndAttack();
            }
        }

        // Eğer childCount 0 ise hem kendini hem de parent'ı yok et
        if (transform.childCount == 0)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject); // Üst nesneyi yok et
            }
            Destroy(gameObject); // Kendini yok et
        }
    }

    public void AttackThem(Transform enemyForge)
    {
        enemy = enemyForge;
        attack = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Animator>().SetBool("run", true);
        }
    }

    private void DestroyEnemyArea()
    {
        // Stop the attack and destroy the enemy GameObject
        attack = false;
        if (enemy != null)
        {
            Destroy(enemy.gameObject);
        }
    }
}
