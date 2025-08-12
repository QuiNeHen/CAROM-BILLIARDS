using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class CueStick : Singleton<CueStick>
{
    public GameObject cue_stick_focus;
    public GameObject cue_stick;

    public bool isColliding = false;
    private Vector3 originalPosition;

    public override void Awake()
    {
        MakeSingleton(false);
    }

    public override void Start()
    {
        cue_stick = this.gameObject;
        cue_stick_focus = GameObject.FindGameObjectWithTag("Ball");
        originalPosition = cue_stick.transform.position;
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball") || other.gameObject.CompareTag("RedBall") || other.gameObject.CompareTag("YellowBall"))
        {
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ball") || other.gameObject.CompareTag("RedBall") || other.gameObject.CompareTag("YellowBall"))
        {
            isColliding = false;
        }
    }
}