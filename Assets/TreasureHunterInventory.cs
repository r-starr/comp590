using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureHunterInventory : MonoBehaviour
{
   public Dictionary<Collectible, int> inventory = new Dictionary<Collectible, int>();

   public Collectible[] keys;
   public int[] values;
}
