using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TerrainType
{
    /// <summary>
    /// Nombre Capa De Terreno
    /// </summary>
    public string Layer;
    /// <summary>
    /// Altura
    /// </summary>
    [Range(0f, 1f)]
    public float height;
    /// <summary>
    /// Color de Capa
    /// </summary>
    public Color color;
}
[System.Serializable]
public class ObjectInMap
{
    public GameObject prefab;
    /// <summary>
    /// Densidad del objecto 
    /// </summary>               
    public float Density = 0.1f;
    /// <summary>
    /// El ruido generado
    /// </summary>
    public float NoiseScale = 0.1f;
    /// <summary>
    /// Capa en la que se puede generar el Objecto
    /// </summary>
    public string GenerationLayer;
}
public class Chunk
{
    public Vector2 posMap;
    GameObject suelo;
    GameObject edges;
    public GameObject objectos;
    public Chunk(Vector2 posMap, Cell[,] mapaCells, float HeightPerBlock,float sizePerBlock, Transform parent)
    {
        this.posMap = posMap;

        suelo = new GameObject("Suelo " + posMap);
        edges = new GameObject("Edges " + posMap);
        objectos = new GameObject("Objectos " + posMap);
        setParent(parent);
        edges.transform.SetParent(suelo.transform);
        objectos.transform.SetParent(suelo.transform);

        Material sueloMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        suelo.AddComponent<MeshFilter>();
        suelo.AddComponent<MeshRenderer>().material = sueloMaterial;

        Material edgesMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        edges.AddComponent<MeshFilter>();
        edges.AddComponent<MeshRenderer>().material = edgesMaterial;

        GenerateTerrainMesh(mapaCells, HeightPerBlock,sizePerBlock);

        suelo.AddComponent<MeshCollider>();
        edges.AddComponent<MeshCollider>();
    }
    public void GenerateTerrainMesh(Cell[,] mapaCells, float heightPerBlock,float sizePerBlock)
    {
        MeshGenerator.GenerateTerrainMeshChunk(mapaCells, posMap, suelo, heightPerBlock, sizePerBlock);
        MeshGenerator.DrawEdgesChunk(mapaCells, posMap, edges, heightPerBlock, sizePerBlock);
    }
    public void setParent(Transform parent)
    {
        suelo.transform.SetParent(parent);
    }
    public void delete()
    {
        GameObject.Destroy(edges.gameObject);
        GameObject.Destroy(objectos.gameObject);
        GameObject.Destroy(suelo.gameObject);
    }
}
