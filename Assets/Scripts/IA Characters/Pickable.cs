using System;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    public enum ObjectType { ANIMAL, FOOD, WEAPON, FIRE };
    public ObjectType myType;

    public int level; //Nivel del arma, vale 0 si no es un arma
    public Enemy target;
    public Mesh myMesh;
    public Material myMaterial;
    public GameObject enemy; //Enemigo asociado a ese objeto
    //float time, distance;
   
    void Awake()
    {
        enemy=null;
        if (myType == ObjectType.WEAPON)
        {
            myMesh = GetComponent<MeshFilter>().mesh;       //Me guardo la malla para sustituir la espada
            myMaterial = GetComponent<Renderer>().material; //Me guardo el material par sustituirlo tambien
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (enemy != null) return; //Si ya tiene un enemigo asociado
        if(other.GetComponent<Enemy>()!=null /*&& !collide*/) //

        {
            Enemy e = other.GetComponent<Enemy>();
            
            //No interrumpe otro estado
            if (e.IsNight() || e.IsInteracting() || e.isPickingUp() || e.IsAttacking()) return;
            //Si esta persiguiendo a un bicho y no tiene arma no entra
            if (myType == ObjectType.ANIMAL && e.getWeaponLevel() == 0) return;
            //Si el objeto se puede recoger o es el jugador
            if (myType != ObjectType.FIRE)
            {
                enemy = e.gameObject;
                //Pasa al estado de perseguir (Interact)
                if(myType==ObjectType.WEAPON && level>e.getWeaponLevel())
                    e.addTarget(this.gameObject);
                else if(myType == ObjectType.ANIMAL || myType == ObjectType.FOOD) e.addTarget(this.gameObject);
                e.setAnim("IsWalking", true);
                e.setInteract(true);

                if (transform.parent != null)
                    Debug.Log(transform.parent.name + " " + this.gameObject.name + " tiene asociado: " + enemy.name);
                else Debug.Log(this.gameObject.name + " tiene asociado: " + enemy.name);
            }
            
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Enemy>() != null && enemy == other.GetComponent<Enemy>().gameObject){
            other.GetComponent<Enemy>().deleteTarget(this.gameObject);
            enemy = null;
          
        }
    }
    public GameObject getTarget()
    {
        return enemy;
    }
    //private void OnCollisionStay(Collision collision)
    //{
    //    target = collision.gameObject.GetComponent<Enemy>();
    //    if (target.IsNight()) return; //Si es de noche no cuenta
    //    if (enemy != target.gameObject) return; //Si el que colisiona es distinto al que entro en el trigger
    //    if (target != null /*&& target.IsInteracting()*/) //Si ese enemigo esta yendo a por el
    //    {
    //        target.StopEnemy(); //Paro al enemigo
    //        if (stop) return;
    //        stop = true;
    //        //Dependiento del tipo de objeto que sea hago una cosa u otra
    //        switch (myType)
    //        {
    //            case ObjectType.ANIMAL: //Si es un animal el enemigo hace la animacion de atacar
    //                //Si no tiene arma no ataca
    //                if (target.getWeaponLevel() > 0)
    //                {
    //                    target.setAttacking(true); //Paso al estado de atacar
    //                    Invoke("Deactivate", time);
    //                    Debug.Log("Animal Atrapado");
    //                    GetComponent<Collider>().enabled = false; //Ya no colisiona mas con ese objeto
    //                }
    //                else target.setInteract(false);
    //                break;
    //            case ObjectType.FOOD: //Si es comida hace la animacion de recoger y avisa al enemigo de que tiene comida
    //                target.setPicking(true);
    //                target.pickUpFood();
    //                Invoke("Deactivate", time);
    //                GetComponent<Collider>().enabled = false; //Ya no colisiona mas con ese objeto
    //                break;
    //            case ObjectType.WEAPON: //Si es un arma, si la recoge, hace la animacion de recoger
    //                GetComponent<Collider>().enabled = false; //Ya no tendra en cuenta ese objeto
    //                if (level >= target.getWeaponLevel()) //Si el arma es mejor
    //                {
    //                    target.setPicking(true);
    //                    Debug.Log("Arma Cogida");
    //                    Invoke("Deactivate", time);
    //                }
    //                //else target.setInteract(false);
    //                break;
    //        }
    //        target.setInteract(false); //Me salgo del estado de Interactuar
    //    }

    //}
    void Update()
    {
        if(enemy==null) return;
        ////Vector2 enemyPos = new Vector2(enemy.transform.position.x, enemy.transform.position.z); //Posicion x,z del enemigo en ese momento
        ////Vector2 myPos = new Vector2(transform.position.x, transform.position.z);                //Mi posicion x,z en ese momento
        ////Si esta lo suficientemente cerca del enemigo y no es un fuego
        ////Debug.Log(Vector3.Distance(enemy.transform.position, transform.position));
        //if (!collide && myType != ObjectType.FIRE && /*Vector3.SqrMagnitude(enemy.transform.position - transform.position) < distance*/
        //    Vector3.Distance(enemy.transform.position, transform.position) <distance)
        //{
        //    target = enemy.GetComponent<Enemy>(); //Me guardo el componente
        //    if(target != null && target.IsInteracting())
        //    {
        //        if (target.IsNight()) return; //Si es de noche no cuenta

        //        collide = true; //Variable de control
               
        //        target.StopEnemy();

        //        //Dependiento del tipo de objeto que sea hago una cosa u otra
        //        switch (myType)
        //        {
        //            case ObjectType.ANIMAL: //Si es un animal el enemigo hace la animacion de atacar
        //                //Si no tiene arma no ataca
        //                if (target.getWeaponLevel() > 0)
        //                {
        //                    target.setAttacking(true); //Paso al estado de atacar
        //                    Invoke("Deactivate", time);
        //                    Debug.Log("Animal Atrapado");
        //                    GetComponent<Collider>().enabled = false; //Ya no colisiona mas con ese objeto
        //                }
        //                else target.setInteract(false);
        //                break;
        //            case ObjectType.FOOD: //Si es comida hace la animacion de recoger y avisa al enemigo de que tiene comida
        //                target.setPicking(true);
        //                target.pickUpFood();
        //                Invoke("Deactivate", time);
        //                GetComponent<Collider>().enabled = false; //Ya no colisiona mas con ese objeto
        //                break;
        //            case ObjectType.WEAPON: //Si es un arma, si la recoge, hace la animacion de recoger
        //                GetComponent<Collider>().enabled = false; //Ya no tendra en cuenta ese objeto
        //                if (level >= target.getWeaponLevel()) //Si el arma es mejor
        //                {
        //                    target.setPicking(true);
        //                    Debug.Log("Arma Cogida");
        //                    Invoke("Deactivate", time);
        //                }
        //                //else target.setInteract(false);
        //                break;
        //        }
        //        target.setInteract(false); //Me salgo del estado de Interactuar
        //    }
        //}
    }
  
   public ObjectType getObjectType() { return myType; }
}