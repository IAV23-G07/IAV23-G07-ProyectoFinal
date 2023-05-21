using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    public enum ObjectType { ANIMAL, FOOD, WEAPON, FIRE };
    public ObjectType myType;
    public int level; //Nivel del arma, vale 0 si no es un arma

    Enemy target;
    Mesh myMesh;
    Material myMaterial;
    GameObject enemy;
    bool collide; //Variable de control para que solo entre 1 vez 
    float time;
  
    void Awake()
    {
        if (myType == ObjectType.WEAPON)
        {
            myMesh = GetComponent<MeshFilter>().mesh;       //Me guardo la malla para sustituir la espada
            myMaterial = GetComponent<Renderer>().material; //Me guardo el material par sustituirlo tambien
        }
        enemy = GameObject.Find("boko");                //Me guardo la referencia al enemigo
        collide = false;                                //Inicializo la variable de control
        time = 1.3f;
        if (myType == ObjectType.ANIMAL)
        {
            time = 2;
        }
    }
   
    void Update()
    {
        Vector2 enemyPos = new Vector2(enemy.transform.position.x, enemy.transform.position.z); //Posicion x,z del enemigo en ese momento
        Vector2 myPos = new Vector2(transform.position.x, transform.position.z);                //Mi posicion x,z en ese momento
        //Si esta lo suficientemente cerca del enemigo y no es un fuego
        if (!collide && myType!=ObjectType.FIRE && Vector2.SqrMagnitude(enemyPos - myPos) < 2.8f)
        {
            target = enemy.GetComponent<Enemy>(); //Me guardo el componente
            collide = true; //Variable de control
            target.setAnim("IsWalking", false);
            target.setInteract(false); //Me salgo del estado de Interactuar
            target.StopEnemy();
           
            //Dependiento del tipo de objeto que sea hago una cosa u otra
            switch (myType)
            {
                case ObjectType.ANIMAL: //Si es un animal el enemigo hace la animacion de atacar
                    //Si no tiene arma no ataca
                    if (target.getWeaponLevel() > 0)
                    {
                        target.setAttacking(true); //Paso al estado de atacar
                        Invoke("Deactivate", time);
                        Debug.Log("Animal Atrapado");
                        GetComponent<Collider>().enabled = false; //Ya no colisiona mas con ese objeto
                    }
                    else target.setInteract(false);
                    break;
                case ObjectType.FOOD: //Si es comida hace la animacion de recoger y avisa al enemigo de que tiene comida
                    target.setPicking(true);
                    target.pickUpFood();
                    Invoke("Deactivate", time);
                    GetComponent<Collider>().enabled = false; //Ya no colisiona mas con ese objeto
                    break;
                case ObjectType.WEAPON: //Si es un arma, si la recoge, hace la animacion de recoger
                    GetComponent<Collider>().enabled = false; //Ya no tendra en cuenta ese objeto
                    if (level >= target.getWeaponLevel()) //Si el arma es mejor
                    {
                        target.setPicking(true);
                        //target.setAnim("IsWalking", false);
                        Debug.Log("Arma Cogida");
                        Invoke("Deactivate", time);
                    }
                    //else target.setInteract(false);
                    break;
            }
        }
    }
    private void Deactivate()
    {
        if (target.IsAttacking()) target.setAttacking(false);
        if (myType==ObjectType.WEAPON && level >= target.getWeaponLevel())
        {
            GameObject sword = GameObject.Find("Espada");
            sword.GetComponent<MeshFilter>().mesh = myMesh;
            sword.GetComponent<Renderer>().material = myMaterial;
            sword.transform.localScale = Vector3.one;
            sword.transform.localScale *= 0.01f;
            target.setWeaponLevel(level); //Actualizo el nivel del arma del enemigo
        }
        //target.setInteract(false);
        target.setPicking(false); //Me salgo del estado de recoger
        Destroy(this.gameObject); //Destruyo este objeto
    }
}