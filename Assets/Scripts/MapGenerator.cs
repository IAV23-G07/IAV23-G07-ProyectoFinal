using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }

    public enum DrawMode { NoiseMap,ColorMap,FallOff};
    public DrawMode drawMode;

    public int mapSize;
    const int chunkSize = 50;

    public float HeightPerBlock = 0.5f;

    public float noiseScale;
    public int octaves;
    [Range(0f, 1f)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public TerrainType[] regions;

    public bool useFallOff = false;
    public bool autoUpdate = false;

    Cell[,] mapaCells;
    Dictionary<Vector2, Chunk> map3D= new Dictionary<Vector2, Chunk>();
    private void Awake(){
        GenerateMap();
    }
    public void GenerateMap(){
        float[,] fallOffMap = Noise.GenerateFalloffMap(mapSize);
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, mapSize,seed,noiseScale,octaves,persistance,lacunarity,offset);
        mapaCells = new Cell[mapSize, mapSize];

        Color[] colorMap = new Color[mapSize * mapSize];        
        for (int y = 0; y < mapSize; y++){
            for (int x = 0; x < mapSize; x++){
                if(useFallOff) noiseMap[x,y]=Mathf.Clamp01( noiseMap[x,y] - fallOffMap[x,y]);// calculo el nue o noise con respecto al falloff
                float currentHeight = noiseMap[x, y];
                foreach (var currentRegion in regions){
                    if (currentHeight <= currentRegion.height){
                        colorMap[y* mapSize + x] = currentRegion.color;

                        mapaCells[x, y] = new Cell();
                        mapaCells[x, y].type = currentRegion;
                        mapaCells[x, y].noise = currentHeight;
                        break;
                    }
                }
            }
        }

        MapDisplay display = GetComponent<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap) display.DrawTextureMap(TextureGenerator.TextureFromNoiseMap(noiseMap));
        else if (drawMode == DrawMode.ColorMap) { 
            display.DrawTextureMap(TextureGenerator.TextureFromColorMap(colorMap, mapSize, mapSize));
            map3D.Clear();
            GenerarMapaPorChunks();
        }
        else if (drawMode == DrawMode.FallOff) display.DrawTextureMap(TextureGenerator.TextureFromNoiseMap(Noise.GenerateFalloffMap(mapSize)));
    }
    private void OnValidate()
    {
        if(mapSize < 1) mapSize = 1;
        if(lacunarity<1) lacunarity=1;
        if(octaves< 0) octaves=0;
    }
    void GenerarMapaPorChunks(){
        int numChunks = mapSize / chunkSize;
        if (mapSize % chunkSize != 0) numChunks++;

        for (int y = 0; y < numChunks; y++){
            for (int x = 0; x < numChunks; x++){
                Vector2 chunkPos= new Vector2(x, y);
                if(!map3D.ContainsKey(chunkPos)){
                    map3D[chunkPos] = new Chunk(chunkPos,mapaCells,HeightPerBlock,transform);
                }
                else{
                    map3D[chunkPos].GenerateTerrainMesh(mapaCells, HeightPerBlock);
                }
            }
        }

    }    
}
public class Chunk{
    public Vector2 posMap;
    GameObject suelo;
    GameObject edges;
    public Chunk(Vector2 posMap, Cell[,] mapaCells, float HeightPerBlock,Transform parent){
        this.posMap = posMap;

        suelo = new GameObject("Suelo " + posMap);
        edges = new GameObject("Edges " + posMap);
        setParent(parent);
        edges.transform.SetParent(suelo.transform);             

        Material sueloMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        suelo.AddComponent<MeshFilter>();
        suelo.AddComponent<MeshRenderer>().material=sueloMaterial;

        Material edgesMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        edges.AddComponent<MeshFilter>();
        edges.AddComponent<MeshRenderer>().material=edgesMaterial;

        GenerateTerrainMesh(mapaCells, HeightPerBlock);

        suelo.AddComponent<MeshCollider>();
        edges.AddComponent<MeshCollider>();
    }
    public void GenerateTerrainMesh(Cell[,] mapaCells, float heightPerBlock){
        MeshGenerator.GenerateTerrainMeshChunk(mapaCells,posMap, suelo, heightPerBlock);
        MeshGenerator.DrawEdgesChunk(mapaCells,posMap, edges, heightPerBlock);
    }
    public void setParent(Transform parent){
        suelo.transform.SetParent(parent);
    }
}