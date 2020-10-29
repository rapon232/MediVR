using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectQuadCollision : MonoBehaviour
{
    private bool collide = false;

    private Transform objectLocation = null;
    private Vector3 dir = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //objectLocation = this.gameObject.transform;
        if(dir != null && collide)
        {
            this.transform.Translate(dir, Space.Self);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        if(collision.gameObject.layer == 9) // layer: Room
        {
            Debug.Log($"Collision detected!");

            Vector3 dir = collision.contacts[0].point - transform.position;
            // We then get the opposite (-Vector3) and normalize it
            dir = -dir.normalized;

            collide = true;

        } 
    }

    void OnCollisionExit(Collision collision)
    {
        this.gameObject.GetComponent<Rigidbody>().isKinematic = false;

        if(collision.gameObject.layer == 9) // layer: Room
        {
            Debug.Log($"Collision exited!");

            collide = false;
        } 
    }

}
