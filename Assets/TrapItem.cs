using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapItem : MonoBehaviour
{
    public IEnumerator SpringTrap() {
        print("Trap sprung!");

        Canvas jumpscare = GameObject.Find("jumpscare").GetComponent<Canvas>();
        print(jumpscare);
        AudioSource scream = GetComponent<AudioSource>();
        jumpscare.enabled = true;
        scream.Play();
        yield return new WaitForSeconds(2);
        jumpscare.enabled = false;
    }
}
