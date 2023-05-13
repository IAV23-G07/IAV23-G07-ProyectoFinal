using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Hardware;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public static class MeshGenerator{
    static bool createVert(bool seaBottom, float height)
    {
        if (seaBottom && height < 0.4f) return true;
        else if (!seaBottom && height > 0.4f) return true;
        return false;
    }
    public static void GenerateTerrainMesh(Cell[,] mapaCells,GameObject basee,float heightPerBlock,bool seaBottom){
        int size = mapaCells.GetLength(0);
       
        float topLeftX = (size - 1) / -2f;
        float topLeftZ = (size - 1) / -2f;

        Mesh BaseMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();//Almacenar los vertices y triangulos de la malla
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>(); //coordenadas de textura

        for (int y = 0; y < size; y++){
            for (int x = 0; x < size; x++){
                Cell cell = mapaCells[x, y];
               // if (createVert(seaBottom,cell.type.height)){
                    float currHeight = heightPerBlock * cell.noise *100;
                    //definir los vertices de la celda
                    Vector3 a = new Vector3(topLeftX + x - 0.5f, currHeight, topLeftZ - y + 0.5f);
                    Vector3 b = new Vector3(topLeftX + x + 0.5f, currHeight, topLeftZ - y + 0.5f);
                    Vector3 c = new Vector3(topLeftX + x - 0.5f, currHeight, topLeftZ - y - 0.5f);
                    Vector3 d = new Vector3(topLeftX + x + 0.5f, currHeight, topLeftZ - y - 0.5f);

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
                //}
            }
        }

        BaseMesh.vertices = vertices.ToArray();// se le asignan los vertices a la malla q hemos creado
        BaseMesh.triangles = triangles.ToArray();// y los triangulos q forman dichos vertices
        BaseMesh.uv = uvs.ToArray();
        BaseMesh.RecalculateNormals();//para que la iluminación y sombreado en la malla se calculen correctamente

        MeshFilter meshFilter = basee.GetComponent<MeshFilter>();//Renderizar la malla
        meshFilter.mesh = BaseMesh;
    
        var renderer = basee.GetComponent<MeshRenderer>();
        DrawTexture(mapaCells, renderer, seaBottom); 
        
    }

    public static void DrawTexture(Cell[,] mapaCells,MeshRenderer renderer,bool seaBottom){
        int size = mapaCells.GetLength(0);

        Texture2D texture = new Texture2D(size, size);
        for (int y = 0; y < size; y++){
            for (int x = 0; x < size; x++){
                Cell cell = mapaCells[x, y]; 
                //if(createVert(seaBottom,cell.type.height))
                 texture.SetPixel(x, y, cell.type.color);                               
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        renderer.sharedMaterial.mainTexture = texture;
    }
    public static void DrawEdges(Cell[,] mapaCells,GameObject edges, float heightPerBlock, bool seaBottom)
    {
        int size = mapaCells.GetLength(0);

        float topLeftX = (size - 1) / -2f;
        float topLeftZ = (size - 1) / -2f;

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>(); //coordenadas de textura
        for (int y = 0; y < size; y++){
            for (int x = 0; x < size; x++){
                Cell cell = mapaCells[x, y];
                float currHeight = heightPerBlock * cell.noise * 100;
                //if (createVert(false,cell.noise)){
                    if (x > 0){
                        Cell left = mapaCells[x - 1, y];//izquierda
                        if (left.noise<cell.noise){
                            float leftHeight = currHeight - (heightPerBlock * left.noise * 100);
                            Vector3 a = new Vector3(topLeftX + x - .5f, currHeight, topLeftZ - y + .5f);
                            Vector3 b = new Vector3(topLeftX + x - .5f, currHeight, topLeftZ - y - .5f);
                            Vector3 c = new Vector3(topLeftX + x - .5f, currHeight - leftHeight, topLeftZ - y + .5f);
                            Vector3 d = new Vector3(topLeftX + x - .5f, currHeight - leftHeight, topLeftZ - y - .5f);

                            Vector2 uvA = new Vector2(x / (float)size, y / (float)size); //definir coordenadas de textura correspondientes a cada v
                            Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                            Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                            Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                            for (int k = 0; k < 6; k++){
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                                uvs.Add(uv[k]);
                            }
                            
                        }
                    }
                    if (x < size - 1){
                        Cell right = mapaCells[x + 1, y];//derecha
                        if (right.noise<cell.noise){
                            float rightHeight = currHeight - (heightPerBlock * right.noise * 100);
                            Vector3 a = new Vector3(topLeftX + x + .5f, currHeight, topLeftZ - y - .5f);
                            Vector3 b = new Vector3(topLeftX + x + .5f, currHeight, topLeftZ - y + .5f);
                            Vector3 c = new Vector3(topLeftX + x + .5f, currHeight-rightHeight, topLeftZ - y - .5f);
                            Vector3 d = new Vector3(topLeftX + x + .5f, currHeight-rightHeight, topLeftZ - y + .5f);

                            Vector2 uvA = new Vector2(x / (float)size, y / (float)size); //definir coordenadas de textura correspondientes a cada v
                            Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                            Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                            Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                            for (int k = 0; k < 6; k++){
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                                uvs.Add(uv[k]);
                            }
                           
                        }
                    }
                    if (y > 0){
                        Cell down = mapaCells[x, y - 1];//abajo
                        if (down.noise< cell.noise){ 
                            float downHeight = currHeight - (heightPerBlock * down.noise * 100);
                            Vector3 a = new Vector3(topLeftX + x - .5f, currHeight, topLeftZ - y + .5f);
                            Vector3 b = new Vector3(topLeftX + x + .5f, currHeight, topLeftZ - y + .5f);
                            Vector3 c = new Vector3(topLeftX + x - .5f, currHeight-downHeight, topLeftZ - y + .5f);
                            Vector3 d = new Vector3(topLeftX + x + .5f, currHeight-downHeight, topLeftZ - y + .5f);

                            Vector2 uvA = new Vector2(x / (float)size, y / (float)size); //definir coordenadas de textura correspondientes a cada v
                            Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                            Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                            Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                            for (int k = 5; k >= 0; k--){
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                                uvs.Add(uv[k]);
                            }
                            
                        }
                    }
                    if (y < size - 1){
                        Cell up = mapaCells[x, y + 1];//arriba
                        if (up.noise< cell.noise){
                            float upHeight = currHeight - (heightPerBlock * up.noise * 100);
                            Vector3 a = new Vector3(topLeftX + x + .5f, currHeight, topLeftZ - y - .5f);
                            Vector3 b = new Vector3(topLeftX + x - .5f, currHeight, topLeftZ - y - .5f);
                            Vector3 c = new Vector3(topLeftX + x + .5f, currHeight - upHeight, topLeftZ - y - .5f);
                            Vector3 d = new Vector3(topLeftX + x - .5f, currHeight - upHeight, topLeftZ - y - .5f);

                            Vector2 uvA = new Vector2(x / (float)size, y / (float)size); //definir coordenadas de textura correspondientes a cada v
                            Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                            Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                            Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                            for (int k = 5; k >= 0; k--){
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                                uvs.Add(uv[k]);
                            }
                        }
                        
                    //}
                }
            }
        }
        mesh.vertices = vertices.ToArray();// se le asignan los vertices a la malla q hemos creado
        mesh.triangles = triangles.ToArray();// y los triangulos q forman dichos vertices
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();//para que la iluminación y sombreado en la malla se calculen correctamente             

        MeshFilter meshFilter = edges.GetComponent<MeshFilter>();//Renderizar la malla
        meshFilter.mesh = mesh;

        var render = edges.GetComponent<MeshRenderer>();
        DrawTexture(mapaCells, render,false);
    }   
}