using UnityEngine;

public class Pickable : MonoBehaviour
{
    public enum ObjectType { ANIMAL, FOOD, WEAPON, FIRE };
    public ObjectType myType;

    public int level; //Nivel del arma, vale 0 si no es un arma
    [HideInInspector]
    public Enemy target;
    [HideInInspector]
    public Mesh myMesh;
    [HideInInspector]
    public Material myMaterial;
    [HideInInspector]
    public GameObject enemy; //Enemigo asociado a ese objeto
    //float time, distance;
   
    void Awake()
    {
        enemy=null;
        if (myType == ObjectType.WEAPON) //Si es una espada
        {
            myMesh = GetComponent<MeshFilter>().mesh;       //Me guardo la malla para sustituir la espada
            myMaterial = GetComponent<Renderer>().material; //Me guardo el material par sustituirlo tambien
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (enemy != null) return; //Si ya tiene un enemigo asociado
        if(other.GetComponent<Enemy>()!=null) //Si es un enemigo

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
            other.GetComponent<Enemy>().deleteTarget(this.gameObject); //Lo saco de la lista
            enemy = null; //Desasocio al personaje para que pueda cogerlo otro
        }
    }
    public ObjectType getObjectType() { return myType; }
}