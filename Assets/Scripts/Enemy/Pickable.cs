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
    bool collide;
    // Start is called before the first frame update
    void Start()
    {
        myMesh = GetComponent<MeshFilter>().mesh;
        myMaterial = GetComponent<Renderer>().material;
        collide = false;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject enemy = GameObject.Find("boko");
        Vector2 enemyPos = new Vector2(enemy.transform.position.x, enemy.transform.position.z);
        Vector2 myPos = new Vector2(transform.position.x, transform.position.z);
        if (!collide && Vector2.SqrMagnitude(enemyPos- myPos) < 1.5f)
        {
            Enemy boko=enemy.GetComponent<Enemy>();
            target = boko;
            collide = true;
            boko.StopEnemy(); //Lo paro
            GetComponent<Collider>().enabled = false;
            //Dependiento del tipo de objeto que sea hago una cosa u otra
            //Si es un animal el enemigo hace la animacion de atacar
            //Si es comida hace la animacion de recoger y avisa al enemigo de que tiene comida
            //Si es un arma, si la recoge, hace la animacion de recoger
            switch (myType)
            {
                case ObjectType.ANIMAL:
                    boko.Hunt();
                    Debug.Log("Animal Atrapado");
                    break;
                case ObjectType.FOOD:
                    target.pickUpFood();
                    break;
                case ObjectType.WEAPON:
                    if (level >= target.getWeaponLevel()) //Si el arma es mejor
                    {
                        boko.setWeaponLevel(level); //Actualizo el nivel del arma del enemigo
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
