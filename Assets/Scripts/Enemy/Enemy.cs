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

    int action, //Accion aleatoria del idle
        food, //Numero de comida
        weaponLevel; //Nivel del arma
    bool interact, attack;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("Day and Night Controller").GetComponent<DayAndNightControl>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        interact = false;
        attack = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Dejara de hacer lo que este haciendo para tumbarse y dormir
    public void Sleep()
    {
        //Quita el navMeshAgent
        //Animacion de dormir
        animator.SetBool("IsNight", true);
    }
    public void WakeUp()
    {
        //Activa el navMeshAgent
        //Animacion de levantarse
        animator.SetBool("IsNight", false);
    }
    //Comprobacion de si es de noche
    public bool IsNight()
    {
        if (controller.TimeOfDay() == "Midnight" || controller.TimeOfDay() == "Night") { /*Debug.Log("NOCHE");*/ return true; }
        //Debug.Log("DIA");
        return false;
    }

    public void SelecIdleAction()
    {
        action = Random.Range(0, 4);
        switch (action)
        {
            //Accion de Idle normal
            case 0:
                Cook();
                break;
            //Merodeo
            case 1:
                Merodeo();
                break;
            //Bailar
            case 2:

                break;
            //Cocinar
            case 3:

                break;
        }
    }
    private void Cook()
    {
        if (food > 0)
        {
            //Animacion de cocinar
            Debug.Log("Cocina");
        }
    }
    private void Merodeo()
    {
        Debug.Log("Merodeo");
    }

    public void OnTriggerEnter(Collider other)
    {
        //Si el objeto se puede recoger
        if (other.gameObject.GetComponent<Pickable>() != null)
        {
            interact = true;
            target = other.gameObject;
        }

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
    }
    public void Chase()
    {
        if (interact || attack)
        {
            agent.SetDestination(target.transform.position);
        }
    }
    public void pickUpFood()
    {
        food++;
        Debug.Log("Comida: "+ food);
    }
    public int getWeaponLevel()
    {
        return weaponLevel;
    }
    public void setWeaponLevel(int level)
    {
        weaponLevel = level;
    }

    public void Hunt()
    {
        //Animacion de atacar
        food += 3;
    }
}
