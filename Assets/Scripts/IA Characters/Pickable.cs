using UnityEngine;

public class Pickable : MonoBehaviour
{
    public enum ObjectType { ANIMAL, FOOD, WEAPON, FIRE };
    public ObjectType myType;
    public int level; //Nivel del arma, vale 0 si no es un arma

    Enemy target;
    Mesh myMesh;
    Material myMaterial;
    GameObject enemy; //Enemigo asociado a ese objeto
    float time;
    bool stop = false;
    void Awake()
    {
        enemy=null;
        if (myType == ObjectType.WEAPON)
        {
            myMesh = GetComponent<MeshFilter>().mesh;       //Me guardo la malla para sustituir la espada
            myMaterial = GetComponent<Renderer>().material; //Me guardo el material par sustituirlo tambien
        }
        enemy = GameObject.Find("boko");                //Me guardo la referencia al enemigo
        
        if (myType == ObjectType.ANIMAL)
        {
            time = 2;
        }
        else 
        {
            time = 1.3f;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (enemy != null) return;
        if(other.GetComponent<Enemy>()!=null)
        {
            enemy = other.gameObject;
            if(transform.parent!=null)
                Debug.Log(transform.parent.name+" "+this.gameObject.name + " tiene asociado: "+enemy.name);
            else Debug.Log(this.gameObject.name + " tiene asociado: " + enemy.name);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Enemy>() != null && enemy == other.GetComponent<Enemy>().gameObject)
        {
            enemy = null;
        }
    }
    public GameObject getTarget()
    {
        return enemy;
    }
    private void OnCollisionStay(Collision collision)
    {
        target=collision.gameObject.GetComponent<Enemy>();
        if (target.IsNight()) return; //Si es de noche no cuenta
        if (enemy != target.gameObject) return; //Si el que colisiona es distinto al que entro en el trigger
        if (target != null /*&& target.IsInteracting()*/) //Si ese enemigo esta yendo a por el
        {
            target.StopEnemy(); //Paro al enemigo
            if (stop) return;
            stop = true;
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
                        Debug.Log("Arma Cogida");
                        Invoke("Deactivate", time);
                    }
                    //else target.setInteract(false);
                    break;
            }
            target.setInteract(false); //Me salgo del estado de Interactuar
        }
        
    }
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
    private void Deactivate()
    {
        if (target.IsAttacking()) target.setAttacking(false); //Si esta en el estado de atacar sale
        if (myType==ObjectType.WEAPON) //Cambio de la espada
        {
            GameObject sword = target.GetComponentInChildren<MeshFilter>().gameObject;
            sword.GetComponent<MeshFilter>().mesh = myMesh;
            sword.GetComponent<Renderer>().material = myMaterial;
            sword.transform.localScale = Vector3.one;
            sword.transform.localScale *= 0.01f;
            target.setWeaponLevel(level); //Actualizo el nivel del arma del enemigo
        }
        if (target.isPickingUp()) target.setPicking(false); //Me salgo del estado de recoger
        Destroy(this.gameObject); //Destruyo este objeto
    }
}