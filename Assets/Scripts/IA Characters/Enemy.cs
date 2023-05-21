using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class Enemy : MonoBehaviour
{
    DayAndNightControl controller; //Controlador del ciclo de dia y noche
    Animator animator;
    NavMeshAgent agent;
    GameObject target;
    GameObject nearestFire;
    double tiempoIdle;           // Segundos que estara en el idle seleccionado
    double tiempoComienzoIdle;     // Segundo en el que empezo ese idle
    double cookingTime;
    double iniCookingTime;

    int action, //Accion aleatoria del idle
        food, //Numero de comida que tiene
        weaponLevel; //Nivel del arma
    bool interact, attack, idle, isPicking, isCooking, isWandering;
    Vector3 newPos;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("Day and Night Controller").GetComponent<DayAndNightControl>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        setInteract(false);
        idle = true;
        setAttacking(false);
        setPicking(false);
        setCooking(false);
        tiempoComienzoIdle = 0;
        tiempoIdle = 3;
        weaponLevel = 0;
        cookingTime = 2.5;
        iniCookingTime = 0;
        isWandering = false;
        newPos = Vector3.zero;
    }
    // Update is called once per frame
    void Update()
    {
        if (idle) tiempoComienzoIdle += Time.deltaTime; //Contador para el cambio de idle
        if (isCooking) iniCookingTime += Time.deltaTime; //Contador para cocinar piezas de comida
    }
    //Dejara de hacer lo que este haciendo para tumbarse y dormir
    public void Sleep()
    {
        idle = false;
        setInteract(false); setAnim("IsWalking", false);
        setAttacking(false); setAnim("IsAttacking", false);
        setCooking(false);
        setPicking(false);
        isWandering=false;
        //Animacion de dormir
        setAnim("IsNight", true);
        agent.enabled = false;
    }
    public void WakeUp()
    {
        agent.enabled = true;
        //Animacion de levantarse
        setAnim("IsNight", false);
    }
    //Comprobacion de si es de noche
    public bool IsNight()
    {
        if (controller.TimeOfDay() == "Midnight" || controller.TimeOfDay() == "Night") return true;
        else return false;
    }
    public void Idle()
    {
        idle = true;
        agent.enabled = false;
        setAnim("IsAttacking", false);
        setAnim("IsWalking", false);
        setAnim("IsPicking", false);
        //Si ha pasado el tiempo cambio de accion
        if (tiempoComienzoIdle >= tiempoIdle)
        {
            SelecIdleAction();
            tiempoComienzoIdle = 0;
        }
    }
    void SelecIdleAction()
    {
        action = Random.Range(0, 5); //Elijo de forma aleatoria una accion
        tiempoIdle = 3;
        setAnim("IsWalking", false);
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
                StartMerodeo();
                break;
        }
    }
    public void StartCook()
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
    }
    public void Cook()
    {
        if(food <= 0) isCooking = false; //Cuando se le acaba la comida para
        else
        {
            if (nearestFire != null && Vector3.SqrMagnitude(transform.position - nearestFire.transform.position) < 4.2f)
            {
                StopEnemy();
                setAnim("IsWalking", false);
                //Animacion de cocinar
                animator.SetInteger("Action", 3);
                //Cocino cada ciertos segundos
                if (iniCookingTime >= cookingTime)
                {
                    food--;
                    Debug.Log("Comida: " + food);
                    iniCookingTime = 0; //Reseteo el timer
                }
            }
        }
    }
    public void StartMerodeo()
    {
        agent.enabled = true;
        //Gnero una nueva posicion aleatoria
        float x = Random.Range(-75, 75);
        float z = Random.Range(-75, 75);
        newPos = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
       
        tiempoIdle = 10; //Tiempo que estara merodeando
        tiempoComienzoIdle = 0; //Timer
        setAnim("IsWalking", true); //Animacion de caminar
        Debug.Log(newPos);
        isWandering = true; //Cambio de estado
    }
    public void Merodeo() //En el Update del estado Wandering
    {
        setAnim("IsWalking", true); //Animacion de caminar
        //Si ha pasado el tiempo cambio de estado y reseteor el contador
        if (tiempoComienzoIdle >= tiempoIdle ||
            Vector2.SqrMagnitude(transform.position - newPos) < 2.8f ||
            agent.pathStatus == NavMeshPathStatus.PathPartial || 
            agent.pathStatus == NavMeshPathStatus.PathInvalid) { tiempoComienzoIdle = 0; isWandering = false; }
        else
            agent.SetDestination(newPos);
    }
    public void OnTriggerEnter(Collider other)
    {
        Pickable p = other.gameObject.GetComponent<Pickable>();
        //Me guardo el fuego mas cercano para luego cocinar
        if (p!=null && p.myType == Pickable.ObjectType.FIRE)
        {
            nearestFire = other.gameObject;
        }
    }
    public void OnTriggerStay(Collider other)
    {
        Pickable p = other.gameObject.GetComponent<Pickable>();
        //Si esta persiguiendo a un bicho y no tiene arma no entra
        if (p.myType == Pickable.ObjectType.ANIMAL && getWeaponLevel() == 0) return;
        //Si el objeto se puede recoger o es el jugador
        if ((p != null && p.myType!=Pickable.ObjectType.FIRE && !isPicking && !IsAttacking()))
        {
            //Pasa al estado de perseguir
            target = other.gameObject;
            setAnim("IsWalking", true);
            setInteract(true);
            idle = false;
        }
    }
    public bool isCookingFood()
    {
        return isCooking;
    }
    public void setCooking(bool c)
    {
        isCooking = c;
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
    public bool IsAttacking()
    {
        return attack;
    }
    public void setAttacking(bool a)
    {
        attack = a;
        setAnim("IsAttacking", a);
    }
    public bool IsInteracting()
    {
        return interact;
    }
    public void setInteract(bool b)
    {
        interact = b;
    }
    public bool Wandering()
    {
        return isWandering;
    }
    //Update del estado Interact
    public void Chase()
    {
        idle = false;
        if (target!=null)
        {
            agent.enabled = true;
            agent.SetDestination(target.transform.position);
        }
    }
    public void pickUpFood()
    {
        agent.enabled=false;
        idle = false;
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
    public void Attack()
    {
        agent.enabled = false;
        idle = false;
        //Animacion de atacar
        setAnim("IsAttacking", true);
        setAnim("IsWalking", false);
        //Cantidad aleatoria de comida que recibira al cazar
        int amount = Random.Range(1, 6);
        food += amount;
        Debug.Log("Comida: " + food);
    }
    public void StopEnemy()
    {
        agent.enabled = false;
    }
    public void setAnim(string name, bool action)
    {
        animator.SetBool(name, action);
    }
}
