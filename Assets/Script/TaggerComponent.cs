using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaggerComponent : MonoBehaviour, Player {
    private const int RIGHT = 0, STAY = 1, LEFT = 2;
    private const int DIRECTION_SPEED = 3;

    private const int PREV_VALID = 0, PREV_X = 1, PREV_Y = 2;
    private const int CURRENT_VALID = 3, CURRENT_X = 4, CURRENT_Y = 5;

    private string UNIT_NAME;

    private CameraController m_MainCameraComponent;

    private Rigidbody2D rb2d;

    private SpriteRenderer m_SpriteRenderer;

    private NeuronStructure ics;

    private VisionComponent visionComponent;

    private TextMesh nameComponent;

    private int detect = 0;

    private const int MEMORY_SIZE = 90;
    private Memory<double[]> memory;

    // Use this for initialization
    void Start () {
        UNIT_NAME = transform.parent.name;

        m_MainCameraComponent = Camera.allCameras[0].GetComponent<CameraController>();

        rb2d = GetComponent<Rigidbody2D>();

        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        visionComponent = gameObject.GetComponentInChildren<VisionComponent>();

        nameComponent = gameObject.GetComponentInChildren<TextMesh>();

        ics = NeuronStructureFactory.CreateStructure(this);

        memory = Memory<double[]>.boom(MEMORY_SIZE, new double[] { 0, 0, 0 });
    }
	
	// Update is called once per frame
	void Update () {
        if (visionComponent.IsObserving())
        {
            lock(this) {
                detect++;
            }
        }

        SetCount();

        int valid = visionComponent.IsObserving() ? 1 : 0;
        Vector3 targetPosition = visionComponent.IsObserving() ? visionComponent.GetTargetLocation() : transform.position;
        Vector3 location = transform.position - targetPosition;
        memory.Memorize(new double[] { valid, location.x, location.y });

        transform.Rotate(new Vector3(0, 0, Between(-60f, ics.GetOuput(DIRECTION_SPEED), 60f)) * Time.deltaTime * DecideDirection(), Space.World);
        //rb2d.velocity = (Vector2)transform.up * ics.GetOuput(SPEED);
    }

    private float Between (float min, float value, float max)
    {
        float result = Math.Max(max, value);
        result = Math.Min(min, result);

        return result;
    }

    private void SetCount()
    {
        if (ics.rank > 0)
        {
            nameComponent.text = ics.rank + " - " + detect;
        }
        else
        {
            nameComponent.text = "" + detect;
        }
    }

    private float DecideDirection()
    {
        double right = ics.GetOuput(RIGHT);
        double stay = ics.GetOuput(STAY);
        double left = ics.GetOuput(LEFT);

        if (right > stay && right > left)
        {
            return 1f;
        }
        if (stay > right && stay > left)
        {
            return 0f;
        }
        if (left > stay && left > right)
        {
            return -1f;
        }

        return 0;
    }

    public double GetError()
    {
        return (m_MainCameraComponent.TOTAL_TIME - detect) * 100 / m_MainCameraComponent.TOTAL_TIME;
    }

    public double GetPriority()
    {
        return detect;
    }
    
    public double GetSensorInput(int idx)
    {
        return memory.Remember(idx / 3)[idx % 3];
    }

    public string GetUnitName()
    {
        return UNIT_NAME;
    }
}
