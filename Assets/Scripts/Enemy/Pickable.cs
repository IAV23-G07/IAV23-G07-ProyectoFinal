using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    public enum ObjectType { ANIMAL, FOOD, WEAPON };
    public ObjectType myType;
    public int level; //Nivel del arma, vale 0 si no es un arma

    Enemy target;
    Mesh myMesh;
    Material myMaterial;
    // Start is called before the first frame update
    void Start()
    {
        myMesh = GetComponent<MeshFilter>().mesh;
        myMaterial = GetComponent<Renderer>().material;
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
            GetComponent<Collider>().enabled = false;
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
            Invoke("Deactivate", 1.7f);
        }
    }
    private void Deactivate()
    {
        if(target.IsAttacking()) target.setAttacking(false);
        target.setInteract(false);
        target.setAnim("IsPicking", false);
        if (myType==ObjectType.WEAPON)
        {
            GameObject sword = GameObject.Find("Espada");
            sword.GetComponent<MeshFilter>().mesh = myMesh;
            sword.GetComponent<Renderer>().material = myMaterial;
            sword.transform.localScale = Vector3.one;
            sword.transform.localScale *= 0.01f;
        }
        Destroy(this.gameObject);
    }
}
