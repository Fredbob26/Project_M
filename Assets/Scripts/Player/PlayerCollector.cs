using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerScore playerScore;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerScore = GetComponent<PlayerScore>();
    }

    private void OnTriggerEnter(Collider other)
    {
        EssencePickup essence = other.GetComponent<EssencePickup>();
        if (essence != null)
        {
            if (playerHealth != null)
                playerHealth.currentLifeTime += essence.lifeTimeBonus;

            if (playerScore != null)
                playerScore.AddScore(essence.scoreValue);

            Destroy(other.gameObject);
        }
    }
}