using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBall : Singleton<RedBall>
{
    private bool isRed = false;

    Rigidbody rb;

    // Constants
    private float m = 0.21f; // Mass
    private float R = 0.04048f; // Radius


    public override void Awake()
    {
        MakeSingleton(false);

    }

    // Start is called before the first frame update
    public override void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.mass = m;
        transform.localScale = new Vector3(R * 2, R * 2, R * 2); 

        // Set material properties for friction
        Collider collider = GetComponent<Collider>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            AudioController.Ins.PlaySound(AudioController.Ins.ballHitBall);
        }

        if (other.gameObject.CompareTag("RedBall"))
        {
            AudioController.Ins.PlaySound(AudioController.Ins.ballHitBall);
        }
    }
    public bool GetRedBall()
    {
        return isRed;
    }

    public void MakeRedBall()
    {
        isRed = true;
        /*GetComponent<Renderer>().material.color = Color.red;*/
    }

    private void FixedUpdate()
    {
        if (rb.velocity.y > 0)
        {
            Vector3 newvelocity = rb.velocity;
            newvelocity.y = 0f;
            rb.velocity = newvelocity;
        }
    }
}
