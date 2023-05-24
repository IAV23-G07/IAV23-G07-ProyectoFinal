using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Pickable;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.GraphicsBuffer;

public class TriggerObject : MonoBehaviour
{
  

    Pickable padre;
    bool stop;
    float time;
    
    private void Start()
    {
        padre= GetComponentInParent<Pickable>();
        if (padre.getObjectType() == ObjectType.ANIMAL)
            time = 2;
        else
            time = 1.3f;
    }
    private void OnTriggerStay(Collider collision)
    {
        padre.target = collision.gameObject.GetComponent<Enemy>();
        if (padre.target.IsNight()||padre.myType==ObjectType.FIRE) return; //Si es de noche no cuenta
        if (padre.enemy != padre.target.gameObject) return; //Si el que colisiona es distinto al que entro en el trigger
        if (padre.target != null /*&& target.IsInteracting()*/) //Si ese enemigo esta yendo a por el
        {
            padre.target.StopEnemy(); //Paro al enemigo
            if (stop) return;
            stop = true;
            //Dependiento del tipo de objeto que sea hago una cosa u otra
            switch (padre.getObjectType())
            {
                case ObjectType.ANIMAL: //Si es un animal el enemigo hace la animacion de atacar
                    //Si no tiene arma no ataca
                    if (padre.target.getWeaponLevel() > 0)
                    {
                        padre.target.setAttacking(true); //Paso al estado de atacar
                        Invoke("Deactivate", time);
                        Debug.Log("Animal Atrapado");
                        GetComponent<Collider>().enabled = false; //Ya no colisiona mas con ese objeto
                    }
                    else padre.target.setInteract(false);
                    break;
                case ObjectType.FOOD: //Si es comida hace la animacion de recoger y avisa al enemigo de que tiene comida
                    padre.target.setPicking(true);
                    padre.target.pickUpFood();
                    Invoke("Deactivate", time);
                    GetComponent<Collider>().enabled = false; //Ya no colisiona mas con ese objeto
                    break;
                case ObjectType.WEAPON: //Si es un arma, si la recoge, hace la animacion de recoger
                    GetComponent<Collider>().enabled = false; //Ya no tendra en cuenta ese objeto
                    if (padre.level >= padre.target.getWeaponLevel()) //Si el arma es mejor
                    {
                        padre.target.setPicking(true);
                        Debug.Log("Arma Cogida");
                        Invoke("Deactivate", time);
                    }
                    //else target.setInteract(false);
                    break;
            }
            padre.target.setInteract(false); //Me salgo del estado de Interactuar
        }
    }
    private void Deactivate()
    {
        if (padre.target.IsAttacking()) padre.target.setAttacking(false); //Si esta en el estado de atacar sale
        if (padre.myType == ObjectType.WEAPON) //Cambio de la espada
        {
            GameObject sword = padre.target.GetComponentInChildren<MeshFilter>().gameObject;
            sword.GetComponent<MeshFilter>().mesh = padre.myMesh;
            sword.GetComponent<Renderer>().material = padre.myMaterial;
            sword.transform.localScale = Vector3.one;
            sword.transform.localScale *= 0.01f;
            padre.target.setWeaponLevel(padre.level); //Actualizo el nivel del arma del enemigo
        }
        if (padre.target.isPickingUp()) padre.target.setPicking(false); //Me salgo del estado de recoger
        Destroy(padre.gameObject); //Destruyo este objeto
    }
}
