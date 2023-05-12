using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Grid : MonoBehaviour
{
    public float scale = 0.1f;
    public int size = 100;
    CellTMP[,] grid;
    MeshRenderer meshRenderer;


    public float waterLevel = 0.35f;

    public Material terrainMaterial;
    public Material edgesMaterial;
    public Material waterMaterial;
    private void Start(){
        float[,] noiseMap = new float[size,size];
        //Creacion de una semilla unica por ejecucion
        float xOffset= Random.Range(-10000f,10000f);
        float yOffset= Random.Range(-10000f,10000f);

        for (int i = 0; i< size; i++){
            for (int j = 0; j < size; j++){
                float noiseValue= Mathf.PerlinNoise(j*scale+xOffset,i*scale+yOffset);
                noiseMap[j,i] = noiseValue;
            }
        }
        //Para q sea como una isla
        float[,] falloffMap = new float[size,size];
        for (int x = 0; x < size; x++){
            for (int y = 0; y < size; y++){

                float xv = x / (float)size * 2 - 1;
                float yv = y / (float)size * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv),Mathf.Abs(yv));
                falloffMap[x,y] = Mathf.Pow(v,3f)/(Mathf.Pow(v,3f)+Mathf.Pow(2.2f-2.2f*v,3f));
            }
        }



        grid = new CellTMP[size,size];
        for (int y = 0; y < size; y++){
            for (int x = 0; x < size; x++){
                CellTMP cell = new CellTMP();
                float noise = noiseMap[x, y]- falloffMap[y,x];
                cell.isWater= noise <waterLevel;
                grid[y,x] = cell;
                //grid[y,x].height = noise;
            }
        }

        drawTerrainMesh();
        drawTexture();
        drawEdgeMesh();
    }

    void drawTerrainMesh(){
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();//Almacenar los vertices y triangulos de la malla
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>(); //coordenadas de textura

        for (int y = 0; y < size; y++){ 
            for (int x = 0; x < size; x++){
                CellTMP cell = grid[x, y];
                if (!cell.isWater){
                    //definir los vertices de la celda
                    Vector3 a = new Vector3(x - 0.5f,cell.height,y + 0.5f);
                    Vector3 b = new Vector3(x + 0.5f, cell.height, y + 0.5f);
                    Vector3 c = new Vector3(x - 0.5f, cell.height, y - 0.5f);
                    Vector3 d = new Vector3(x + 0.5f,cell.height,y - 0.5f);
                    Vector2 uvA = new Vector2(x / (float)size, y / (float)size); //definir coordenadas de textura correspondientes a cada v
                    Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                    Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                    Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);
                    Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                    Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                    for (int k = 0; k < 6; k++) //crear los triangulos 
                    {
                        vertices.Add(v[k]);
                        triangles.Add(triangles.Count);
                        uvs.Add(uv[k]);
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();// se le asignan los vertices a la malla q hemos creado
        mesh.triangles = triangles.ToArray();// y los triangulos q forman dichos vertices
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();//para que la iluminación y sombreado en la malla se calculen correctamente

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();//Renderizar la malla
        meshFilter.mesh = mesh;

        meshRenderer= gameObject.AddComponent<MeshRenderer>();
    }
    void drawTexture(){
        Texture2D texture = new Texture2D(size, size);
        for (int y = 0; y < size; y++){
            for (int x = 0; x < size; x++){

                CellTMP cell = grid[x, y];
                if (cell.isWater) {
                    texture.SetPixel(x, y, Color.blue);                    
                }
                else{
                    texture.SetPixel(x, y, Color.green);
                }
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();      
        meshRenderer.material.mainTexture= texture;
    }
    void drawEdgeMesh(){
        //Para representar q la superficie tiene volumen, si hay alguna celda q tenga agua al lado se crea una maya
        //que representaran aquellos lados de la celda la cual es nexa al agua
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int y = 0; y < size; y++){
            for (int x = 0; x < size; x++){
                CellTMP cell = grid[x, y];
                if (!cell.isWater){
                    if (x > 0){
                        CellTMP left = grid[x - 1, y];//izquierda
                        if (left.isWater){
                            Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                            Vector3 b = new Vector3(x - .5f, 0, y - .5f);
                            Vector3 c = new Vector3(x - .5f, -1, y + .5f);
                            Vector3 d = new Vector3(x - .5f, -1, y - .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++){
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if (x < size - 1){
                        CellTMP right = grid[x + 1, y];//derecha
                        if (right.isWater){
                            Vector3 a = new Vector3(x + .5f, 0, y - .5f);
                            Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                            Vector3 c = new Vector3(x + .5f, -1, y - .5f);
                            Vector3 d = new Vector3(x + .5f, -1, y + .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++){
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if (y > 0){
                        CellTMP down = grid[x, y - 1];//abajo
                        if (down.isWater){
                            Vector3 a = new Vector3(x - .5f, 0, y - .5f);
                            Vector3 b = new Vector3(x + .5f, 0, y - .5f);
                            Vector3 c = new Vector3(x - .5f, -1, y - .5f);
                            Vector3 d = new Vector3(x + .5f, -1, y - .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++){
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if (y < size - 1){
                        CellTMP up = grid[x, y + 1];//arriba
                        if (up.isWater){
                            Vector3 a = new Vector3(x + .5f, 0, y + .5f);
                            Vector3 b = new Vector3(x - .5f, 0, y + .5f);
                            Vector3 c = new Vector3(x + .5f, -1, y + .5f);
                            Vector3 d = new Vector3(x - .5f, -1, y + .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++){
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                }
            }
        }
        mesh.vertices = vertices.ToArray();// se le asignan los vertices a la malla q hemos creado
        mesh.triangles = triangles.ToArray();// y los triangulos q forman dichos vertices
        mesh.RecalculateNormals();//para que la iluminación y sombreado en la malla se calculen correctamente     

        GameObject edgeObj = new GameObject("Edge");
        edgeObj.transform.SetParent(transform);

        MeshFilter meshFilter = edgeObj.AddComponent<MeshFilter>();//Renderizar la malla
        meshFilter.mesh = mesh;

        var render = edgeObj.AddComponent<MeshRenderer>();
        render.material = edgesMaterial;       
    }

    //private void OnDrawGizmos()
    //{
    //    if (!Application.isPlaying) return;

    //    for (int i = 0; i < size; i++)
    //    {
    //        for (int j = 0; j < size; j++)
    //        {
    //            Cell cell = grid[i, j];
    //            if (cell.isWater) Gizmos.color = Color.blue;
    //            else Gizmos.color = Color.green;

    //            Vector3 pos = new Vector3(j, 0, i);
    //            Gizmos.DrawCube(pos, Vector3.one);
    //        }
    //    }

    //}
}
