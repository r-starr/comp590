using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class TreasureHunter : MonoBehaviour
{
    public TextMesh scoreText;

    public TextMesh winText;

    public LayerMask mask;

    public int score = 0;
    public int itemsCollected = 0;

    public bool gameIsOver = false;

    // private GameObject leftControllerCone;
    private GameObject rightControllerPointer;
    private GameObject collectibleArea;

    private GameObject itemHeld;


    // Start is called before the first frame update
    void Start()
    {
        //references to pointer cone, for easy access
        rightControllerPointer = GameObject.Find("RightControllerAnchor").transform.Find("Pointer").gameObject;

        //inventory initialization
        TreasureHunterInventory inventoryObj = GetComponent<TreasureHunterInventory>();
        Dictionary<Collectible, int> inventory = inventoryObj.inventory;

        inventory.Add((Collectible)AssetDatabase.LoadAssetAtPath("Assets/WoodPiece.prefab", typeof(Collectible)), 0);
        inventory.Add((Collectible)AssetDatabase.LoadAssetAtPath("Assets/BronzeBar.prefab", typeof(Collectible)), 0);
        inventory.Add((Collectible)AssetDatabase.LoadAssetAtPath("Assets/SilverBar.prefab", typeof(Collectible)), 0);
        inventory.Add((Collectible)AssetDatabase.LoadAssetAtPath("Assets/GoldBar.prefab", typeof(Collectible)), 0);
    }

    // Update is called once per frame
    void Update()
    {

        // if the user hasn't won/lost yet
        if (!gameIsOver)
        {
            // if the user presses the right trigger, attempt to grab object controller is pointing to
            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            {
                print("Pressed trigger");
                if (itemHeld == null)
                {
                    GrabNearestCollectible();
                }
            }
            // if you let go of the trigger
            else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
            {
                // and you're actually holding something
                if (itemHeld != null)
                {
                    Transform playerHeadPosition = GameObject.Find("CenterEyeAnchor").transform;
                    float heightFromHead = playerHeadPosition.position.y - itemHeld.transform.position.y;
                    if (heightFromHead >= 0.5f)
                    {
                        CollectHeldObject();
                    }
                    else
                    {
                        LetGo();
                    }
                }
            }
        }
    }

    void GrabNearestCollectible()
    {
        RaycastHit hit; // holds object the ray hits if any

        Debug.DrawRay(rightControllerPointer.transform.position, rightControllerPointer.transform.up, Color.green, 200, false);
        // if the ray hits anything
        if (Physics.Raycast(rightControllerPointer.transform.position, rightControllerPointer.transform.up, out hit, 100.0f, mask))
        {
            Debug.DrawRay(rightControllerPointer.transform.position, rightControllerPointer.transform.up, Color.red, 200, false);
            GameObject objectHit = hit.transform.gameObject;
            attachGameObjectToAChildGameObject(objectHit, GameObject.Find("RightControllerAnchor"), AttachmentRule.SnapToTarget, AttachmentRule.SnapToTarget, AttachmentRule.KeepWorld, true);
            itemHeld = objectHit;
        }
    }

    void LetGo()
    {
        //if you're actually holding something
        if (itemHeld != null)
        {
            // take the object off your hand
            detachGameObject(itemHeld, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld);
            //turn physics back on
            simulatePhysics(itemHeld, true);
            //you're not holding anything anymore
            itemHeld = null;
        }
    }

    void CollectHeldObject()
    {
        // method assumes you've already checked to make sure it's in a collectible state

        // check to see if it's a trap, and if it is, end the game!
        TrapItem trapScript = itemHeld.GetComponent<TrapItem>();
        if (trapScript != null)
        {
            print("trap");
            StartCoroutine(trapScript.SpringTrap());
            winText.color = Color.red;
            winText.text = "You lose!";
            gameIsOver = true;
        }
        //if it's not a trap, do normal collection   
        else
        {

            // grab the inventory
            TreasureHunterInventory inventoryObj = GetComponent<TreasureHunterInventory>();
            Dictionary<Collectible, int> inventory = inventoryObj.inventory;

            // what kind of collectible is it?
            string collectibleType = itemHeld.GetComponent<Collectible>().type;

            // update score
            itemsCollected++;
            score += itemHeld.GetComponent<Collectible>().value;

            // update inventory list and counts
            Collectible prefab = (Collectible)AssetDatabase.LoadAssetAtPath($"Assets/{collectibleType}.prefab", typeof(Collectible));
            int count = 0;
            inventory.TryGetValue(prefab, out count);
            inventory[prefab] = (count == 0) ? 1 : ++count;
            inventoryObj.keys = new Collectible[inventory.Keys.Count];
            inventory.Keys.CopyTo(inventoryObj.keys, 0);
            inventoryObj.values = new int[inventory.Values.Count];
            inventory.Values.CopyTo(inventoryObj.values, 0);

            int woodpieces = inventory[(Collectible)AssetDatabase.LoadAssetAtPath("Assets/WoodPiece.prefab", typeof(Collectible))];
            int bronze = inventory[(Collectible)AssetDatabase.LoadAssetAtPath("Assets/BronzeBar.prefab", typeof(Collectible))];
            int silver = inventory[(Collectible)AssetDatabase.LoadAssetAtPath("Assets/SilverBar.prefab", typeof(Collectible))];
            int gold = inventory[(Collectible)AssetDatabase.LoadAssetAtPath("Assets/GoldBar.prefab", typeof(Collectible))];
            scoreText.text = $"Wood Pieces (10pts): {woodpieces}\nBronze Bars (25pts): {bronze}\nSilver Bars (50pts): {silver}\nGold Bars (100pts): {gold}\nScore: {score}\nTotal Collected: {itemsCollected}";

            // check win condition, end game if won
            // shouldnt be able to get more than 5 items but just in case
            if (itemsCollected >= 5)
            {
                winText.text = "You win!";
                winText.color = Color.green;
                gameIsOver = true;
            }

        }
        // destroy item, clear held item
        Destroy(itemHeld);
        itemHeld = null;

    }

    // Nick's code for attachment and simulating physics

    public static void attachGameObjectToAChildGameObject(GameObject GOToAttach, GameObject newParent, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule, bool weld)
    {
        GOToAttach.transform.parent = newParent.transform;
        handleAttachmentRules(GOToAttach, locationRule, rotationRule, scaleRule);
        if (weld)
        {
            simulatePhysics(GOToAttach, false);
        }
    }

    public static void detachGameObject(GameObject GOToDetach, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule)
    {
        //making the parent null sets its parent to the world origin (meaning relative & global transforms become the same)
        GOToDetach.transform.parent = null;
        handleAttachmentRules(GOToDetach, locationRule, rotationRule, scaleRule);
    }

    public static void handleAttachmentRules(GameObject GOToHandle, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule)
    {
        GOToHandle.transform.localPosition =
            (locationRule == AttachmentRule.KeepRelative) ? GOToHandle.transform.position :
            //technically don't need to change anything but I wanted to compress into ternary
            (locationRule == AttachmentRule.KeepWorld) ? GOToHandle.transform.localPosition :
            new Vector3(0, 0, 0);

        //localRotation in Unity is actually a Quaternion, so we need to specifically ask for Euler angles
        GOToHandle.transform.localEulerAngles =
            (rotationRule == AttachmentRule.KeepRelative) ? GOToHandle.transform.eulerAngles :
            //technically don't need to change anything but I wanted to compress into ternary
            (rotationRule == AttachmentRule.KeepWorld) ? GOToHandle.transform.localEulerAngles :
            new Vector3(0, 0, 0);

        GOToHandle.transform.localScale =
            (scaleRule == AttachmentRule.KeepRelative) ? GOToHandle.transform.lossyScale :
            //technically don't need to change anything but I wanted to compress into ternary
            (scaleRule == AttachmentRule.KeepWorld) ? GOToHandle.transform.localScale :
            new Vector3(1, 1, 1);
    }

    public static void simulatePhysics(GameObject target, bool simulate)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb)
        {
            if (!simulate)
            {
                //forums will recommend setting isKinematic to false. The problem with this is that even if you disable gravity, hitting other physics bodies will cause strange 
                //floaty physics. try it yourself to verify. I couldn't find a better way to replicate my UE4 code than to do it this way, but maybe you'll find a better strategy
                Destroy(rb);
            }
        }
        else
        {
            if (simulate)
            {
                //there's actually a problem here relative to the UE4 version since Unity doesn't have this simple "simulate physics" option
                //The object will NOT preserve momentum when you throw it like in UE4.
                //What you can do is make a kinematic rigidbody on the controller and use its own velocity to set the new rigidbody's velocity.
                //I didn't do it here because it's a Unity-specific thing, but feel free to do this yourself as an exercise
                target.AddComponent<Rigidbody>();
            }
        }
    }

}