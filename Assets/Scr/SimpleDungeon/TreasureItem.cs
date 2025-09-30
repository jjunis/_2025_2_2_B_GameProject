using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureItem : MonoBehaviour
{
    public int goldAmount = 100;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"º¸¹° È¹µæ! °ñµå + {goldAmount}");
            Destroy(gameObject);
        }
    }
}
