using UnityEngine;
using UnityEngine.AI;

public class AnimalWandering : MonoBehaviour
{
    double timeWandering;         //Segundos cada cuales genera una nueva posicion
    double InitialTimeWandering;  //Timer del merodeo
    Vector3 newPos;               //Nueva posicion generada
    NavMeshAgent agent;
    Enemy enemy;
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        InitialTimeWandering = 0;
        timeWandering = 8.2;
        newPos = Vector3.zero;
    }
    
    void Update()
    {
        //Si el enemigo esta cerca, y ademas le esta atacando, para quito
        if(enemy!=null && Vector3.SqrMagnitude(transform.position - enemy.transform.position) < 6.2f && enemy.IsAttacking())
        {
            agent.enabled = false;
        }
        else
        {
            InitialTimeWandering += Time.deltaTime; //Contador para generar una nueva posicion
            Wandering();
        }
    }
    public void Wandering()
    {
        //Si ha pasado el tiempo, no puede llegar o ya ha llegado se genera otra posicion
        if (InitialTimeWandering >= timeWandering ||
            Vector2.SqrMagnitude(transform.position - newPos) < 2.8f ||
            agent.pathStatus == NavMeshPathStatus.PathPartial ||
            agent.pathStatus == NavMeshPathStatus.PathInvalid) 
        { 
            InitialTimeWandering = 0;
            GenerateRandomPos();
        }
        else
            agent.SetDestination(newPos);
    }
    private void GenerateRandomPos() //Genero una nueva posicion aleatoria
    {
        float x = Random.Range(-50, 50);
        float z = Random.Range(-50, 50);
        newPos = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
    }

    public void OnTriggerEnter(Collider other)
    {
        Enemy p = other.gameObject.GetComponent<Enemy>();
        //Me guardo una referencia al enemigo
        if (p != null)
        {
            enemy = p;
        }
    }
}