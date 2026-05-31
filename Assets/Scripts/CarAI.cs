using UnityEngine;

public class CarAI : MonoBehaviour
{
    public float[] WeightsForce = new float[22];
    public float[] WeightsAngle = new float[22];
    public float[] EnteredData = new float[22];

    float[] angles = {
    0f, 18f, 36f, 54f, 72f, 90f, 108f, 126f, 144f, 162f,
    180f, 198f, 216f, 234f, 252f, 270f, 288f, 306f, 324f, 342f
    };


    [SerializeField] float speed = 0.1f;
    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;

    public int Score;
    public LearningManager LM;
    public GenerationControl GC;

    int LastPoint;

    public enum TypeOfMutation
    {
        Default, LitleMutation, MediumMutation, BigMutation, CrossMutation, MidMutation,Exact
    }

    public TypeOfMutation Type;

    void Start()
    {

        GameObject controller = GameObject.Find("Manager");

        LM = controller.GetComponent<LearningManager>();
        GC = controller.GetComponent<GenerationControl>();

        switch (Type)
        {
            case TypeOfMutation.Default:
                for (int i = 0; i < WeightsForce.Length; i++)
                {
                    WeightsForce[i] = Random.Range(-1f, 1f);
                    WeightsAngle[i] = Random.Range(-1f, 1f);
                }
                break;
            case TypeOfMutation.LitleMutation:
                for (int i = 0; i < WeightsForce.Length; i++)
                {
                    WeightsForce[i] = LM.BestWeightsForce[i] + Random.Range(-0.05f,0.05f);
                    WeightsAngle[i] = LM.BestWeightsAngle[i] + Random.Range(-0.05f,0.05f);
                }
                break;
            case TypeOfMutation.MediumMutation:
                for (int i = 0; i < WeightsForce.Length; i++)
                {
                    WeightsForce[i] = LM.BestWeightsForce[i] + Random.Range(-0.2f,0.2f);
                    WeightsAngle[i] = LM.BestWeightsAngle[i] + Random.Range(-0.2f,0.2f);
                }
                break;
            case TypeOfMutation.BigMutation:
                for (int i = 0; i < WeightsForce.Length; i++)
                {
                    WeightsForce[i] = LM.BestWeightsForce[i] + Random.Range(-0.3f,0.3f);
                    WeightsAngle[i] = LM.BestWeightsAngle[i] + Random.Range(-0.3f,0.3f);
                }
                break;
            case TypeOfMutation.CrossMutation:
                for (int i = 0; i < WeightsForce.Length; i++)
                {
                    WeightsForce[i] = Random.Range(0,2) == 0 ? LM.BestWeightsForce[i] : LM.BestWeightsForce2[i];
                    WeightsAngle[i] = Random.Range(0,2) == 0 ? LM.BestWeightsAngle[i] : LM.BestWeightsAngle2[i];
                }
                break;
            case TypeOfMutation.MidMutation:
                for (int i = 0; i < WeightsForce.Length; i++)
                {
                    WeightsForce[i] = (LM.BestWeightsForce[i] + LM.BestWeightsForce2[i]) / 2;
                    WeightsAngle[i] = (LM.BestWeightsAngle[i] + LM.BestWeightsAngle2[i]) / 2;
                }
                break;
            case TypeOfMutation.Exact:
                for (int i = 0; i < WeightsForce.Length; i++)
                {
                    WeightsForce[i] = LM.BestWeightsForce[i];
                    WeightsAngle[i] = LM.BestWeightsAngle[i];
                }
                break;
        }
    }

    float timer;
    float interval = 0.1f;

    [SerializeField] float firstOut;
    [SerializeField] float SecondOut;

    public float LifeTime;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            Scan();
            EnteredData[20] = speed / maxSpeed;
            EnteredData[21] = transform.eulerAngles.z / 360f;
            timer = 0;
        }

        firstOut = NeironSpeed();
        SecondOut = NeironAngle();

        Move(firstOut);
        Rotate(SecondOut);

        LifeTime += Time.deltaTime;

       transform.Translate(Vector3.right * speed * Time.deltaTime);
   

    }

    void Move(float force)
    {
        if (force > 0)
        {
            if (speed < maxSpeed)
            {
                speed += 0.05f * force;
            }
            else
            {
                speed = maxSpeed - 0.1f;
            }
        }
        else
        {
            if (speed > minSpeed)
            {
                speed += 0.05f * force;
            }
            else
            {
                speed = minSpeed + 0.1f;
            }
        }
    }

    void Rotate(float Angle)
    {
         transform.Rotate(0, 0, Angle * 100f * Time.deltaTime);
    }

    float resultSp;
    float resultAn;
    float NeironSpeed()
    {
        resultSp = 0;
        for (int i = 0; i < EnteredData.Length; i++)
        {
            resultSp += EnteredData[i] * WeightsForce[i];
        }

        return Tanh(resultSp);
    }

    float NeironAngle()
    {
        resultAn = 0;
        for (int i = 0; i < EnteredData.Length; i++)
        {
            resultAn += EnteredData[i] * WeightsAngle[i];
        }

        return Tanh(resultAn);
    }

    float Tanh(float x)
    {
        float e2x = Mathf.Exp(2f * x);
        return (e2x - 1f) / (e2x + 1f);
    }

    [SerializeField] LayerMask lm;

    void Scan()
    {
        for (int i = 0; i < angles.Length; i++)
        {
            Vector2 direction = Quaternion.Euler(0, 0, angles[i]) * transform.right;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 5, lm);

            // визуализация лучей
            Color rayColor = hit ? Color.red : Color.green;
            Debug.DrawRay(transform.position, direction * 5, rayColor, interval);

            // записываем расстояние

            
            if (hit)
                EnteredData[i] = hit.distance / 5; // нормализованное значение [0..1]
            else
                EnteredData[i] = 1f; // ничего не попало — считаем максимум
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("way"))
        {
            WayPoint wp = other.gameObject.GetComponent<WayPoint>();

            LastPoint = wp.Index;
            if (wp.Index == 100) // финиш
            {
                if (speed > 3)
                {

                    Score += (int)(25 * speed);
                }
                else
                {
                    Score += 10;
                } 
                Die();
            }
            else
            {
               if (speed > 3)
                {
                    Score += (int)(15 * speed);
                }
                else
                {
                    Score += 1;
                }  
            }
            
        }
        else
        {
             Die();
        }
       
    }


    void Die()
    {
        Score *= (int)(LifeTime / 2);
        LM.Cars.Add(this);
        GC.DiedOne();
        gameObject.SetActive(false);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
