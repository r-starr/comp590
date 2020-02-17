using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TreasureHunter : MonoBehaviour
{
    public TextMesh scoreText;

    public int score = 0;
    public int itemsCollected = 0;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // if the user presses Q, attempt to collect the closet object hit by raycast
        if(Input.GetKeyDown(KeyCode.Q)) {

            RaycastHit hit; // holds object the ray hits if any
            Debug.Log("Pressed Q");

            // create a ray that points in the direction the camera is facing
            Camera camera = GameObject.Find("Player Camera").GetComponent<Camera>();
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            // if the ray hits anything
            if(Physics.Raycast(ray, out hit, 10)) {
                // and it's a collectible
                if(hit.transform.tag == "Collectible") {
                    // grab the inventory
                    TreasureHunterInventory inventoryObj = GetComponent<TreasureHunterInventory>();
                    Dictionary<Collectible, int> inventory = inventoryObj.inventory;
                    GameObject objectHit = hit.transform.gameObject;
                    string collectibleType = objectHit.GetComponent<Collectible>().type;

                    itemsCollected++;
                    score += objectHit.GetComponent<Collectible>().value;

                    Collectible prefab = (Collectible)AssetDatabase.LoadAssetAtPath($"Assets/{collectibleType}.prefab", typeof(Collectible));
                    int count = 0;
                    inventory.TryGetValue(prefab, out count);
                    inventory[prefab] = (count == 0) ? 1 : ++count;
                    inventoryObj.keys = new Collectible[inventory.Keys.Count];
                    inventory.Keys.CopyTo(inventoryObj.keys, 0);
                    inventoryObj.values = new int[inventory.Values.Count];
                    inventory.Values.CopyTo(inventoryObj.values, 0);

                    scoreText.text = $"Score: {score}\nItems Collected: {itemsCollected}";

                    Destroy(objectHit);
                }
            }
        }
    }
}
