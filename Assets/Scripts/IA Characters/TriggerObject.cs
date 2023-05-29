using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Pickable;

public class TriggerObject : MonoBehaviour
{
    Pickable parent;
    bool stop;
    float time;
    
    private void Start()
    {
        parent = GetComponentInParent<Pickable>();
        if (parent.getObjectType() == ObjectType.ANIMAL)
            time = 2;
        else
            time = 1.3f;
    }
    private void OnTriggerStay(Collider collision)
    {
        parent.target = collision.gameObject.GetComponent<Enemy>();
        if (parent.target.IsNight()||parent.myType==ObjectType.FIRE) return; //Si es de noche no cuenta
        if (parent.enemy != parent.target.gameObject) return; //Si el que colisiona es distinto al que entro en el trigger
        if (parent.target != null /*&& target.IsInteracting()*/) //Si ese enemigo esta yendo a por el
        {
            parent.target.StopEnemy(); //Paro al enemigo
            if (stop) return;
            stop = true;
            //Dependiento del tipo de objeto que sea hago una cosa u otra
            switch (parent.getObjectType())
            {
                case ObjectType.ANIMAL: //Si es un animal el enemigo hace la animacion de atacar
                    //Si no tiene arma no ataca
                    if (parent.target.getWeaponLevel() > 0)
                    {
                        parent.target.setAttacking(true); //Paso al estado de atacar
                        Invoke("Deactivate", time);
                        //Debug.Log("Animal Atrapado");
                        GetComponent<Collider>().enabled = false; //Ya no colisiona mas con ese objeto
                    }
                    else parent.target.setInteract(false);
                    break;
                case ObjectType.FOOD: //Si es comida hace la animacion de recoger y avisa al enemigo de que tiene comida
                    parent.target.setPicking(true);
                    parent.target.pickUpFood();
                    Invoke("Deactivate", time);
                    GetComponent<Collider>().enabled = false; //Ya no colisiona mas con ese objeto
                    break;
                case ObjectType.WEAPON: //Si es un arma, si la recoge, hace la animacion de recoger
                    GetComponent<Collider>().enabled = false; //Ya no tendra en cuenta ese objeto
                    if (parent.level >= parent.target.getWeaponLevel()) //Si el arma es mejor
                    {
                        parent.target.setPicking(true);
                        //Debug.Log("Arma Cogida");
                        Invoke("Deactivate", time);
                    }
                    //else target.setInteract(false);
                    break;
            }
            parent.target.setInteract(false); //Me salgo del estado de Interactuar
        }
    }
    private void Deactivate()
    {
        if (parent.target.IsAttacking()) parent.target.setAttacking(false); //Si esta en el estado de atacar sale
        if (parent.myType == ObjectType.WEAPON) //Cambio de la espada
        {
            GameObject sword = parent.target.GetComponentInChildren<MeshFilter>().gameObject;
            sword.GetComponent<MeshFilter>().mesh = parent.myMesh;
            sword.GetComponent<Renderer>().material = parent.myMaterial;
            sword.transform.localScale = Vector3.one;
            sword.transform.localScale *= 0.01f;
            parent.target.setWeaponLevel(parent.level); //Actualizo el nivel del arma del enemigo
        }
        if (parent.target.isPickingUp()) parent.target.setPicking(false); //Me salgo del estado de recoger
        parent.target.deleteTarget(parent.gameObject);
        Destroy(parent.gameObject); //Destruyo este objeto
    }
}
