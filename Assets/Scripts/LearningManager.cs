using System.Collections.Generic;
using UnityEngine;

public class LearningManager : MonoBehaviour
{
    public List<CarAI> Cars = new List<CarAI>();
    public float[] BestWeightsForce = new float[22];
    public float[] BestWeightsAngle = new float[22];

    public float[] BestWeightsForce2 = new float[22];
    public float[] BestWeightsAngle2 = new float[22];

    [SerializeField] int  BestScore;
    
    void Start()
    {
        
    }

    public void RaceFinished()
    {
        Cars.Sort((a, b) => b.Score.CompareTo(a.Score));

        CarAI Best = Cars[0];
        BestWeightsForce = Best.WeightsForce;
        BestWeightsAngle = Best.WeightsAngle;

        BestScore = Best.Score;

        CarAI Best2 = Cars[Random.Range(1,4)];
        BestWeightsForce2 = Best2.WeightsForce;
        BestWeightsAngle2 = Best2.WeightsAngle;

        foreach(CarAI car in Cars)
        {
            car.Destroy();
        }

        Cars.Clear();
    }
}
