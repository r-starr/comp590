using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SelectObject : MonoBehaviour
{
    public Material testMaterial;
    public TextMesh testText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //check if user is pressing collect button
        RaycastHit hit;

        if(Input.GetKeyDown(KeyCode.Q)) {
            Debug.Log("Pressed Q");
            Camera camera = GameObject.Find("Player Camera").GetComponent<Camera>();
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, 10)) {
                if(hit.transform.tag == "Collectible") {
                    // change material for testing rn?
                    Debug.Log("Hit something!");
                    GameObject objectHit = hit.transform.gameObject;
                    objectHit.GetComponent<MeshRenderer>().material = testMaterial;
                    testText.text = "Hit!";
                    Debug.DrawLine(ray.origin, hit.point);
                }
            }
        }
    }

    // Below is all Nick's code, from the github
    // https://github.com/nrewkowski/COMP590ClassExampleUnity.git

    Collectible getClosestHitObject(Collider[] hits){
        float closestDistance=10000.0f;
        Collectible closestObjectSoFar=null;
        foreach (Collider hit in hits){
            Collectible c=hit.gameObject.GetComponent<Collectible>();
            if (c){
                float distanceBetweenHandAndObject=(c.gameObject.transform.position-transform.position).magnitude;
                if (distanceBetweenHandAndObject<closestDistance){
                    closestDistance=distanceBetweenHandAndObject;
                    closestObjectSoFar=c;
                }
            }
        }
        return closestObjectSoFar;
    }

    public static void attachGameObjectToAChildGameObject(GameObject GOToAttach, GameObject newParent, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule, bool weld){
        GOToAttach.transform.parent=newParent.transform;
        handleAttachmentRules(GOToAttach,locationRule,rotationRule,scaleRule);
        if (weld){
            simulatePhysics(GOToAttach,false);
        }
    }

    public static void detachGameObject(GameObject GOToDetach, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule){
        //making the parent null sets its parent to the world origin (meaning relative & global transforms become the same)
        GOToDetach.transform.parent=null;
        handleAttachmentRules(GOToDetach,locationRule,rotationRule,scaleRule);
    }

    public static void handleAttachmentRules(GameObject GOToHandle, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule){
        GOToHandle.transform.localPosition=
        (locationRule==AttachmentRule.KeepRelative)?GOToHandle.transform.position:
        //technically don't need to change anything but I wanted to compress into ternary
        (locationRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localPosition:
        new Vector3(0,0,0);

        //localRotation in Unity is actually a Quaternion, so we need to specifically ask for Euler angles
        GOToHandle.transform.localEulerAngles=
        (rotationRule==AttachmentRule.KeepRelative)?GOToHandle.transform.eulerAngles:
        //technically don't need to change anything but I wanted to compress into ternary
        (rotationRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localEulerAngles:
        new Vector3(0,0,0);

        GOToHandle.transform.localScale=
        (scaleRule==AttachmentRule.KeepRelative)?GOToHandle.transform.lossyScale:
        //technically don't need to change anything but I wanted to compress into ternary
        (scaleRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localScale:
        new Vector3(1,1,1);
    }
    public static void simulatePhysics(GameObject target,bool simulate){
        Rigidbody rb=target.GetComponent<Rigidbody>();
        if (rb){
            if (!simulate){
                //forums will recommend setting isKinematic to false. The problem with this is that even if you disable gravity, hitting other physics bodies will cause strange 
                //floaty physics. try it yourself to verify. I couldn't find a better way to replicate my UE4 code than to do it this way, but maybe you'll find a better strategy
                Destroy(rb);
            }
        } else{
            if (simulate){
                //there's actually a problem here relative to the UE4 version since Unity doesn't have this simple "simulate physics" option
                //The object will NOT preserve momentum when you throw it like in UE4.
                //What you can do is make a kinematic rigidbody on the controller and use its own velocity to set the new rigidbody's velocity.
                //I didn't do it here because it's a Unity-specific thing, but feel free to do this yourself as an exercise
                target.AddComponent<Rigidbody>();
            }
        }
    }
}
