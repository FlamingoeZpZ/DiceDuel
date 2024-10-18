using System;
using UnityEngine;

[ExecuteInEditMode]
public class Stamper : MonoBehaviour
{
    [SerializeField, Range(0,100)] private float scalar;
    private Vector3 normal;
    private Vector3 center;
    [SerializeField] private bool valid;
    [SerializeField] private bool useCenter;

    [ContextMenu("FlipX")]
    void FlipX()
    {
        transform.eulerAngles += new Vector3(180, 0, 0);
    }

    [ContextMenu("Generate Stamper")]
    public void Stamp()
    {
        valid = false;
        if (!Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, Mathf.Infinity))
        {
            if (!Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
            {
                Debug.LogWarning("Hit nothing", gameObject);
                
                return;
            }
            Debug.LogWarning("internal Flip: ", gameObject);
            //transform.localScale = new Vector3(-1, 1, 1);
        }
        
        Debug.Log(hit.collider.name, hit.rigidbody);

        
        
        valid = true;
        normal = hit.normal;
        center = (useCenter?hit.transform.position:transform.position);
        
        transform.rotation = Quaternion.LookRotation(normal, transform.up);
    }

    private void Awake()
    {
        enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        if(!valid) Debug.DrawRay(transform.position, -transform.forward * scalar, Color.red);
        else Debug.DrawRay(transform.position, normal * scalar, Color.green);
    }

    private void Update()
    {
        if(valid) transform.position =  center + normal * scalar;
    }
}
