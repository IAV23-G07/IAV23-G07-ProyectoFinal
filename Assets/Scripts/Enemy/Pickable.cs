using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    public enum ObjectType { ANIMAL, FOOD, WEAPON };
    public ObjectType myType;
    Enemy target;
    public int level; //Nivel del arma, vale 0 si no es un arma
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        //Si colisiona con el enemigo
        if (enemy != null)
        {
            target = enemy; //Me guardo al enemigo
            enemy.StopEnemy(); //Lo paro
            //Dependiento del tipo de objeto que sea hago una cosa u otra
            //Si es un animal el enemigo hace la animacion de atacar
            //Si es comida hace la animacion de recoger y avisa al enemigo de que tiene comida
            //Si es un arma, si la recoge, hace la animacion de recoger
            switch (myType)
            {
                case ObjectType.ANIMAL:
                    enemy.Hunt();
                    Debug.Log("Animal Atrapado");
                    break;
                case ObjectType.FOOD:
                    enemy.pickUpFood();
                    break;
                case ObjectType.WEAPON:
                    if (level >= enemy.getWeaponLevel()) //Si el arma es mejor
                    {
                        enemy.setWeaponLevel(level); //Actualizo el nivel del arma del enemigo
                        Debug.Log("Arma Cogida");
                    }
                    break;
            }
            Invoke("Deactivate", 0.7f);
        }
    }
    private void Deactivate()
    {
        target.setInteract(false);
        target.setAnim("IsPicking", false);
        Destroy(this.gameObject);
    }

}
