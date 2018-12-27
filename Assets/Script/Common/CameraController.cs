using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    public int TOTAL_TIME = 600;

    public Text timeText;
    private int time;
    private int step;

    public Text stepText;

    private void Start()
    {
        time = 0;
        timeText.text = "";
        step = NeuronStructureFactory.GetStep();
        stepText.text = "Step: " + step;
        TOTAL_TIME = 600 + NeuronStructureFactory.GetStep() * 50;
    }

    private void Update()
    {
        time++;
        timeText.text = "Time: " + time.ToString();

        if (time > TOTAL_TIME)
        {
            MoveToNextStep();
        }
    }

    private void MoveToNextStep()
    {
        NeuronStructureFactory.MoveToNextStep();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public int GetTimer()
    {
        return time;
    }
}