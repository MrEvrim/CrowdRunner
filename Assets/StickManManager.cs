using UnityEngine;

public class StickManManager : MonoBehaviour
{
    private enemyManager _enemyManager;

    private void Start()
    {
        _enemyManager = transform.parent.GetComponent<enemyManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("red") && other.transform.parent.childCount > 0)
        {
            Destroy(other.gameObject);
            Destroy(gameObject);

            
        }
    }
}
