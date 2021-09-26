using System;
using UnityEngine;

public class ExplodeCubes : MonoBehaviour
{

    private bool _collisionSet; 
        
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Cube") && !_collisionSet) return;
        
        for (var i = collision.transform.childCount - 1; i >= 1; i--) 
        {
            var child = collision.transform.GetChild(i);
            child.gameObject.AddComponent<Rigidbody>();
            child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(70f, Vector3.up, 5f);
            child.SetParent(null);
        }
        Destroy(collision.gameObject);
    }
}
