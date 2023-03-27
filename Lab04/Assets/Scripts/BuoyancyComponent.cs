    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuoyancyComponent : MonoBehaviour
{
    public Transform[] floaters;

    public float underWaterDrag = 3;
    public float underWaterAngularDrag = 1;
    public float airDrag = 0;
    public float airAngularDrag = 0.05f;
    public float floatingPower = 15f;

    public float waterHeight = 0f; 

    Rigidbody m_RigidBody;

    int floatersUnderwater;

    [SerializeField] bool includeSelf = true;
    bool underwater;

    // Start is called before the first frame update
    private void OnValidate()
    {
        if(includeSelf && (floaters == null || floaters.Length == 0)) floaters = new Transform[]{ transform};
    }
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        floatersUnderwater = 0;
        for (int i = 0; i < floaters.Length; i++)
        {
            float diff = floaters[i].transform.position.y - waterHeight;
            if (diff < 0)
            {
                floatersUnderwater++;
                m_RigidBody.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(diff), floaters[i].transform.position, ForceMode.Force);
                if (!underwater)
                {
                    underwater = true;
                    SwitchState(underwater);
                }

            }
            if (floatersUnderwater == 0 && underwater)
            {
                underwater = false;
                SwitchState(underwater);
            }
        }
    }
    void SwitchState(bool isUnderwater)
    {
        if (isUnderwater)
        {
            m_RigidBody.drag = underWaterDrag;
            m_RigidBody.angularDrag = underWaterAngularDrag;
        }
        else
        {
            m_RigidBody.drag = airDrag;
            m_RigidBody.angularDrag = airAngularDrag;
        }
    }
}
