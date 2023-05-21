using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalWandering : MonoBehaviour
{
    double tiempoIdle;           // Segundos que estara en el idle seleccionado
    double tiempoComienzoIdle;     // Segundo en el que empezo ese idle
    Vector3 newPos;
    NavMeshAgent agent;
    Enemy enemy;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        tiempoComienzoIdle = 0;
        tiempoIdle = 8.2;
        newPos = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //Si el enemigo esta cerca, y ademas le esta atacando, para
        if(enemy!=null && Vector2.SqrMagnitude(transform.position - enemy.transform.position) < 2.8f && enemy.IsAttacking())
        {
            agent.enabled = false;
        }
        else
        {
            tiempoComienzoIdle += Time.deltaTime; //Contador para el cambio de idle
            Merodeo();
        }
    }
    public void Merodeo()
    {
        //Si ha pasado el tiempo
        if (tiempoComienzoIdle >= tiempoIdle ||
            Vector2.SqrMagnitude(transform.position - newPos) < 2.8f ||
            agent.pathStatus == NavMeshPathStatus.PathPartial ||
            agent.pathStatus == NavMeshPathStatus.PathInvalid) 
        { 
            tiempoComienzoIdle = 0;
            GenerateRandomPos();
        }
        else
            agent.SetDestination(newPos);
    }
    private void GenerateRandomPos()
    {
        //Gnero una nueva posicion aleatoria
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
