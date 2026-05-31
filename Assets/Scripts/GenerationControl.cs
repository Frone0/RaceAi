using UnityEngine;

public class GenerationControl : MonoBehaviour
{
    public int CountPerGeneration;
    public int CurrentCount;
    public int GenerationNumber;

    [SerializeField] GameObject CarPref;
    [SerializeField] Transform SpawnPos;

    [SerializeField] LearningManager LM;


    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        for (int i = 0; i < CountPerGeneration; i++)
        {
            CarAI ai = Instantiate(CarPref, SpawnPos.position, transform.rotation).GetComponent<CarAI>();
            ai.Type = CarAI.TypeOfMutation.Default;
        }

        CurrentCount = CountPerGeneration;
    }
    
    public void DiedOne()
    {
        if (CurrentCount <= 0)
        {
            LM.RaceFinished();
            Restart();
        } 
    }
    
    public void Restart()
    {
        GenerationNumber++;

        for (int i = 0; i < CountPerGeneration; i++)
        {
            CarAI ai = Instantiate(CarPref, SpawnPos.position, transform.rotation).GetComponent<CarAI>();
            if (i < 15)
            {
                ai.Type = CarAI.TypeOfMutation.Default;
            }
            else if (i < 20)
            {
                ai.Type = CarAI.TypeOfMutation.LitleMutation;
            }
            else if (i < 25)
            {
                ai.Type = CarAI.TypeOfMutation.MediumMutation;
            }
            else if (i < 30)
            {
                ai.Type = CarAI.TypeOfMutation.BigMutation;
            }
            else if (i < 35)
            {
                ai.Type = CarAI.TypeOfMutation.CrossMutation;
            }
            else if (i < 38)
            {
                ai.Type = CarAI.TypeOfMutation.MidMutation;
            }
            else if (i < 40)
            {
                ai.Type = CarAI.TypeOfMutation.Exact;
            }



        }

        CurrentCount = CountPerGeneration;
    }
}
