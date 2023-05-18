using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    DayAndNightControl controller;
    Animator animator;
    NavMeshAgent agent;
    GameObject target;
    double tiempoIdle;           // Segundos que estara en el idle seleccionado
    double tiempoComienzoIdle;     // Segundo en el que empezo ese idle

    int action, //Accion aleatoria del idle
        food, //Numero de comida
        weaponLevel; //Nivel del arma
    bool interact, attack, idle;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("Day and Night Controller").GetComponent<DayAndNightControl>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        setInteract(false);
        idle = true;
        setAttacking(false);
        tiempoComienzoIdle = 0;
        tiempoIdle = 3;
        weaponLevel = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null && Vector3.SqrMagnitude(transform.position - target.transform.position) < 0.7f)
        {
            //setAnim("IsWalking", false);
            //agent.enabled = false;
            StopEnemy();
        }
        if(idle) tiempoComienzoIdle += Time.deltaTime;
    }
    //Dejara de hacer lo que este haciendo para tumbarse y dormir
    public void Sleep()
    {
        idle = false;
        //Quita el navMeshAgent
        //agent.enabled = false;
        //Animacion de dormir
        setAnim("IsNight", true);
    }
    public void WakeUp()
    {
        //Activa el navMeshAgent
        //Animacion de levantarse
        setAnim("IsNight", false);
    }
    //Comprobacion de si es de noche
    public bool IsNight()
    {
        if (controller.TimeOfDay() == "Midnight" || controller.TimeOfDay() == "Night") { /*Debug.Log("NOCHE");*/ return true; }
        //Debug.Log("DIA");
        return false;
    }
    public void Idle()
    {
        //agent.enabled = false;
        idle = true;
        setAnim("IsAttacking", false);
        setAnim("IsWalking", false);
        if (tiempoComienzoIdle >= tiempoIdle)
        {
            SelecIdleAction();
            tiempoComienzoIdle = 0;
        }
    }
    void SelecIdleAction()
    {
        action = Random.Range(0, 5);
        tiempoIdle = 3;
        switch (action)
        {
            //Accion de Idle normal
            case 0:
                animator.SetInteger("Action", 0);
                break;
            //Bailar
            case 1:
                animator.SetInteger("Action", 1);
                break;
            //Idle2, rascarse
            case 2:
                animator.SetInteger("Action", 2);
                break;
            //Cocinar
            case 3:
                Cook();
                break;
            case 4:
                Merodeo();
                break;
        }
    }
    private void Cook()
    {
        if (food > 0)
        {
            //Animacion de cocinar
            animator.SetInteger("Action", 3);
            food --;   
            Debug.Log("Cocina");
        }
    }
    private void Merodeo()
    {
        tiempoIdle = 10;
        setAnim("IsWalking", true);
        Debug.Log("Merodeo");
        //agent.enabled = true;
        agent.isStopped = false;
        float x = Random.Range(Random.Range(-100, -25), Random.Range(25, 100));
        float z = Random.Range(Random.Range(-100, -25), Random.Range(25, 100));
        Vector3 newPos=new Vector3(transform.position.x + x, transform.position.y, transform.position.z+z);
        agent.SetDestination(newPos);
    }

    public void OnTriggerEnter(Collider other)
    {
        //Si el objeto se puede recoger o es el jugador
        if (other.gameObject.GetComponent<Pickable>() != null || 
            other.gameObject.GetComponent<FirstPersonController_EXAMPLE>() != null)
        {
            target = other.gameObject;
            setAnim("IsWalking", true);
            setInteract(true);
            idle = false;
        }
    }
    public bool IsAttacking()
    {
        return attack;
    }
    public void setAttacking(bool a)
    {
        attack = a;
    }
    public bool IsInteracting()
    {
        return interact;
    }
    public void setInteract(bool b)
    {
        interact = b;
    }
    public void StopEnemy()
    {
        agent.isStopped = true;
        //agent.enabled = false;
        setAnim("IsWalking", false);
        //setInteract(false);
    }
    public void Chase()
    {
        idle = false;
        //agent.enabled = true;
        if ((interact || IsAttacking()) && target!=null)
        {
            agent.SetDestination(target.transform.position);
        }
    }
    public void pickUpFood()
    {
        idle = false;
        //agent.enabled = false;
        //Animacion recoger
        setAnim("IsPicking", true);
        food++;
        Debug.Log("Comida: "+ food);
    }
    public int getWeaponLevel()
    {
        return weaponLevel;
    }
    public void setWeaponLevel(int level)
    {
        setAnim("IsWalking", false);
        //agent.enabled = false;
        setAnim("IsPicking", true);
        weaponLevel = level;
    }
    public void Hunt()
    {
        idle = false;
        //Animacion de atacar
        Attack();
        //Cantidad aleatoria de comida que recibira al cazar
        int amount = Random.Range(1, 6);
        food += amount;
        Debug.Log("Comida: " + food);
    }
    public void Attack()
    {
        //agent.enabled = false;
        setAttacking(true);
        setAnim("IsWalking", false);
        setAnim("IsAttacking", true);
    }
    public void setAnim(string name, bool action)
    {
        animator.SetBool(name, action);
    }
}
