using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapItem : MonoBehaviour
{

    public TextMesh debugText;

    // Start is called before the first frame update
    public IEnumerator SpringTrap() {
        print("Trap sprung!");
        debugText.text = "It's a trap!";

        Canvas jumpscare = GameObject.Find("jumpscare").GetComponent<Canvas>();
        print(jumpscare);
        jumpscare.enabled = true;
        yield return new WaitForSeconds(2);
        jumpscare.enabled = false;
    }
}
