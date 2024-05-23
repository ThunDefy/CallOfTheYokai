using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    //private Health hp;
    public bool canDrop = true;
    public bool randomDrop = true;
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;
    }


    public List<Drops> drops;

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded)
        {
            return;
        }

        if (canDrop)
        {
            PlayerStats playerStats = FindAnyObjectByType<PlayerStats>();

            if (randomDrop)
            {
                float randomNumber = UnityEngine.Random.Range(0f, 100f);
                List<Drops> possibleDrops = new List<Drops>();
                foreach (Drops rate in drops)
                {
                    if (randomNumber <= rate.dropRate)
                    {
                        possibleDrops.Add(rate);
                    }
                }
                if (possibleDrops.Count > 0)
                {
                    foreach (Drops drop in possibleDrops)
                    {
                        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
                        Vector3 spawnPosition = transform.position + randomOffset;

                        Instantiate(drop.itemPrefab, spawnPosition, Quaternion.identity);

                        if (playerStats)
                        {
                            float randomValue = Random.value;
                            if (randomValue < playerStats.actualStats.luck)
                                Instantiate(drop.itemPrefab, transform.position, Quaternion.identity); // повезло, еще одна копия
                        }
                    }
                }
            }
            else
            {
                foreach (Drops drop in drops)
                {
                    Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
                    Vector3 spawnPosition = transform.position + randomOffset;
                    Instantiate(drop.itemPrefab, spawnPosition, Quaternion.identity);
                    if (playerStats)
                    {
                        float randomValue = Random.value;
                        if (randomValue < playerStats.actualStats.luck)
                            Instantiate(drop.itemPrefab, spawnPosition, Quaternion.identity); // повезло, еще одна копия
                    }
                }
            }
        }
        
    }
}
