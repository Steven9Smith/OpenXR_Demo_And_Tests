using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnLamp : MonoBehaviour
{
    [SerializeField] Light mLight;
    bool lightOn => mLight.isActiveAndEnabled;
    private void OnTriggerStay(Collider other)
    {
        var inputs = other.gameObject.GetComponent<StarterAssetsInputs>();
        if (inputs != null)
        {
            if (inputs.interact)
            {
                if (lightOn) mLight.enabled = false;
                else mLight.enabled = true;
            }
            inputs.interact = false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.parent.transform.position.y < 0) mLight.enabled = false;
    }
}
