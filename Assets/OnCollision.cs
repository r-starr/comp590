using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class OnCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if(other.transform.tag == "Collectible") {
            TreasureHunter.itemHeldIsCollectible = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.transform.tag == "Collectible") {
            TreasureHunter.itemHeldIsCollectible = false;
        }
    }

}