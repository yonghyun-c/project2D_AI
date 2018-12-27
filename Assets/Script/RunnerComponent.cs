using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerComponent : MonoBehaviour {
    private const int MOVE_CNT = 42;

    private Rigidbody2D rb2d;

    private float thrust;

    private int count;


	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        thrust = 1f;
    }
	
	// Update is called once per frame
	void Update () {


        if (count < MOVE_CNT * 2)
        {
            rb2d.AddForce(-1 * transform.right * thrust);
            rb2d.AddForce(-1 * transform.up * thrust);
        }
        else if (count < MOVE_CNT * 6)
        {
            rb2d.AddForce(transform.right * thrust);
            rb2d.AddForce(transform.up * thrust);
        }
        else if (count < MOVE_CNT * 8)
        {
            rb2d.AddForce(-1 * transform.right * thrust);
            rb2d.AddForce(-1 * transform.up * thrust);
        }
        else
        {
            count = -1;
        }

        count++;
    }
}
