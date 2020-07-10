using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public Hit hit = new Hit();
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Fire()
    {
        rb.velocity = transform.forward * speed;
    }
    private void FixedUpdate()
    {
        //transform.Translate(transform.forward * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger Coll");
        if (other.gameObject.GetComponent<IHit>() != null)
        {
            IHit hitobj = other.gameObject.GetComponent<IHit>();
            hitobj.TakeHit(hit);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
