using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    DayAndNightControl controller; //Controlador del ciclo de dia y noche
    Animator animator;
    NavMeshAgent agent;
    GameObject target;             //Objeto que se fija como destino
    GameObject nearestFire;        //Fuego mas cercano
    double idleTime;             //Cada x segundo se se cambia de idle
    double iniIdleTime;     //Timer de idle
    double cookingTime;            //Cada x segundo se cocina una pieza de comida
    double iniCookingTime;         //Timer de cocinar

    int action,     //Accion aleatoria del idle
        food,       //Numero de comida que tiene
        weaponLevel;//Nivel del arma

    bool interact,   //Estado Interact
        attack,      //Estado attack
        idle,        //Estado idle
        isPicking,   //Estado pickup 
        isCooking,   //Estado cook
        isWandering; //Estado wandering

    Vector3 newPos; //Nueva posicion del merodeo
    Vector3 lastPos;

    List<GameObject> targets = new List<GameObject>();

    //Inicio todas las variables
    void Awake()
    {
        controller = GameObject.Find("Day and Night Controller").GetComponent<DayAndNightControl>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        setInteract(false);
        idle = true;
        setAttacking(false);
        setPicking(false);
        setCooking(false);
        iniIdleTime = 0;
        idleTime = 3;
        weaponLevel = 0;
        cookingTime = 2.5;
        iniCookingTime = 0;
        isWandering = false;
        newPos = Vector3.zero;
        lastPos = Vector3.zero;
        targets=new List<GameObject>();
    }

    void Update()
    {
        if (idle) iniIdleTime += Time.deltaTime; //Contador para el cambio de idle
        if (isCooking) iniCookingTime += Time.deltaTime; //Contador para cocinar piezas de comida
    }
    //DORMIR
    public void Sleep() //Dejara de hacer lo que este haciendo para tumbarse y dormir
    {
        idle = false;
        setInteract(false); setAnim("IsWalking", false);
        setAttacking(false); setAnim("IsAttacking", false);
        setCooking(false);
        setPicking(false);
        isWandering=false;
        //Animacion de dormir
        setAnim("IsNight", true);
        StopEnemy();
    }
    public void WakeUp() //Salir del estado Sleeping
    {
        if (agent == null) { Debug.Log("2gfgdfg"); agent= GetComponent<NavMeshAgent>(); }
        agent.enabled = true;
        //Animacion de levantarse
        setAnim("IsNight", false);
    }
    public bool IsNight()
    {
        if (controller.TimeOfDay() == "Midnight" || controller.TimeOfDay() == "Night") return true;
        else return false;
    } //Comprobacion de si es de noche

    //IDLE
    public void Idle()
    {
        idle = true;
        StopEnemy();
        setAnim("IsAttacking", false);
        setAnim("IsPicking", false);
        //Si ha pasado el tiempo cambio de accion
        if (iniIdleTime >= idleTime)
        {
            SelecIdleAction();
            iniIdleTime = 0;
        }
    }
    private void SelecIdleAction() //Seleccionador de accion de idle que se llama cada 3 segundos
    {
        action = Random.Range(0, 5); //Elijo de forma aleatoria una accion
        idleTime = 3;
        //setAnim("IsWalking", false);
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
                StartCook();
                break;
            //Merodear
            case 4:
                StartWandering();
                break;
        }
    }
    private void StartCook()
    {
        //Si sale la accion de cocinar, pero no hay comida o no ha encontrado un fuego busca otra cosa que hacer
        if (food > 0 && nearestFire!=null)
        {
            isCooking = true; //Paso al estado Cook
            setAnim("IsWalking", true); //Animacion de andar
            agent.enabled = true;
            agent.SetDestination(nearestFire.transform.position); //Destino el fuego
            Debug.Log("Cocina");
        }
        else SelecIdleAction();
    } //Accion de cocinar sin pasar de estado
    public void StartWandering() //Accion de merodear sin pasar de estado
    {
        Debug.Log("Merodear");
        agent.enabled = true;
        //Genero una nueva posicion aleatoria
        GenerateRandomPos();
       
        idleTime = 10; //Tiempo que estara merodeando
        iniIdleTime = 0; //Timer
        isWandering = true; //Cambio de estado
        lastPos=transform.position;
    }

    //COOK
    public void Cook()
    {
        if (food <= 0) isCooking = false; //Cuando se le acaba la comida para y sale del estado
        else
        {
            //Si esta lo suficientemente cerca del fuego
            if (nearestFire != null && 
                /*Vector3.SqrMagnitude(transform.position - nearestFire.transform.position)*/
                Vector3.Distance(transform.position, nearestFire.transform.position) < 3.2f)
            {
                StopEnemy();
                setAnim("IsWalking", false);
                animator.SetInteger("Action", 3); //Animacion de cocinar

                if (iniCookingTime >= cookingTime) //Cocino cada ciertos segundos
                {
                    food--;
                    //Debug.Log("Comida: " + food);
                    iniCookingTime = 0; //Reseteo el timer
                }
            }
        }
    }
    public bool isCookingFood()
    {
        return isCooking;
    }
    private void setCooking(bool c)
    {
        isCooking = c;
    }

    //WANDERING
    public void Wander() //En el Update del estado Wandering
    {
        setAnim("IsWalking", true); //Animacion de caminar
        //Si ha pasado el tiempo o el camino es inalcanzable cambio de estado y reseteo el contador
        if (iniIdleTime >= idleTime ||
            agent.pathStatus == NavMeshPathStatus.PathPartial ||
            agent.pathStatus == NavMeshPathStatus.PathInvalid) 
        {
            iniIdleTime = 0; 
            isWandering = false; 
        }
        else
            agent.SetDestination(newPos);

        if (lastPos == transform.position) GenerateRandomPos();
        else lastPos = transform.position;
    }
    public bool Wandering()
    {
        return isWandering;
    }
    private void GenerateRandomPos()
    {
        float x = Random.Range(-75, 75);
        float z = Random.Range(-75, 75);
        newPos = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
    }

    public void OnTriggerEnter(Collider other) //Para guardarme el el fuego mas cercano
    {
        //Cada vez que entra en un trigger de un fuego significa que ese es el ultimo mas cercano
        Pickable p = other.gameObject.GetComponent<Pickable>();
        //Me guardo el fuego mas cercano para luego cocinar
        if (p!=null && p.myType == Pickable.ObjectType.FIRE)
        {
            nearestFire = other.gameObject;
        }
    }
    //INTERACT
    public void Chase()
    {
        if (targets.Count <= 0) return; //Si la lista de objetos esta vacia me salgo
        idle = false;
        target = targets[0];
        foreach (var t in targets) //Busco el objeto de la lista mas cercanos
        {
            if (Vector3.Distance(t.transform.position, transform.position) <
                Vector3.Distance(target.transform.position, transform.position))
                target = t;
        }
        if (target!=null) //Persigo el objeto
        {
            agent.enabled = true;
            agent.SetDestination(target.transform.position);
            if (lastPos == transform.position) setInteract(false);
            else lastPos = transform.position;
        }       
    }
    public void StartChasing()
    {
        lastPos = transform.position;
        idle = false;
    }
    public bool IsInteracting()
    {
        return  targets.Count>0; //Si hay elementos con los que ha interactuado voy a por ellos
    }
    public void setInteract(bool b)
    {
        interact = b;
    }

    //PICKUP
    public void pickUpFood()
    {
        StopEnemy();
        idle = false;
        food++;
        Debug.Log("Comida: "+ food);
    }
    public bool isPickingUp()
    {
        return isPicking;
    }
    public void setPicking(bool p)
    {
        isPicking = p;
        setAnim("IsPicking", p);
    }

    //WEAPON
    public int getWeaponLevel()
    {
        return weaponLevel;
    }
    public void setWeaponLevel(int level)
    {
        weaponLevel = level;
    }

    //ATTACK
    public void Attack()
    {
        StopEnemy();
        idle = false;
        //Animacion de atacar
        setAnim("IsAttacking", true);
        setAnim("IsWalking", false);
        //Cantidad aleatoria de comida que recibira al cazar
        int amount = Random.Range(1, 6);
        food += amount;
        //Debug.Log("Comida: " + food);
    }
    public bool IsAttacking()
    {
        return attack;
    }
    public void setAttacking(bool a)
    {
        attack = a;
        setAnim("IsAttacking", a);
    }

    public void StopEnemy() //Desactiva el NavMeshAgent
    {
        agent.enabled = false;
        setAnim("IsWalking", false);
    }
    public void setAnim(string name, bool action) //Establecer una animacion
    {
        animator.SetBool(name, action);
    }
    public void setIdleAnim()
    {
        animator.SetInteger("Action", 0);
    }
    public void addTarget(GameObject go) //Añadir un elemento a la lista
    {
        if (!targets.Contains(go))
            targets.Add(go);
    }
    public void deleteTarget(GameObject go) //Quitar un elemento de la lista
    {
        if(targets.Contains(go))
            targets.Remove(go);
    }
}