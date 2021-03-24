using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodSpawner : MonoBehaviour
{
    [SerializeField] GameObject woodPrefab;
    [SerializeField] BoxCollider floorPlaneCollider;
    public void SpawnTree()
    {
        float randomXPos = Random.Range(floorPlaneCollider.bounds.min.x, floorPlaneCollider.bounds.max.x);
        float randomZPos = Random.Range(floorPlaneCollider.bounds.min.z, floorPlaneCollider.bounds.max.z);
        Vector3 woodPosition = new Vector3(randomXPos, 0, randomZPos);
        Instantiate(woodPrefab, woodPosition, Quaternion.identity);
    }
}
