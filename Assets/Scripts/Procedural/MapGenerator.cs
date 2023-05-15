using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour{    
    public enum DrawMode { NoiseMap,ColorMap=0,FallOff,All,NoObjects};

    public DrawMode drawMode;

    public int mapSize;
    const int chunkSize = 50;

    public float sizePerBlock = 0.5f;
    public float heightPerBlock = 0.5f;

    public float noiseScale;
    public int octaves;
    [Range(0f, 1f)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public TerrainType[] regions;
    public ObjectInMap[] objects;

    public bool useFallOff = false;
    public bool autoUpdate = false;
    bool clean = false;
    Cell[,] cellMap;
    Dictionary<Vector2, Chunk> map3D= new Dictionary<Vector2, Chunk>();

    private void Awake(){
        clean = true;
        GenerateMap();
    }

    public void GenerateMap(){
        float[,] fallOffMap = new float[mapSize,mapSize];
        if (useFallOff) fallOffMap = Noise.GenerateFalloffMap(mapSize);

        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, mapSize,seed,noiseScale,octaves,persistance,lacunarity,offset);
        cellMap = new Cell[mapSize, mapSize];

        Color[] colorMap = new Color[mapSize * mapSize];        
        for (int y = 0; y < mapSize; y++){
            for (int x = 0; x < mapSize; x++){
                if(useFallOff) noiseMap[x,y]=Mathf.Clamp01( noiseMap[x,y] - fallOffMap[x,y]);// calculo el nue o noise con respecto al falloff
                float currentHeight = noiseMap[x, y];
                foreach (var currentRegion in regions){
                    if (currentHeight <= currentRegion.height){
                        colorMap[y* mapSize + x] = currentRegion.color;

                        cellMap[x, y] = new Cell();
                        cellMap[x, y].type = currentRegion;
                        cellMap[x, y].noise = currentHeight;
                        cellMap[x, y].Height = heightPerBlock * currentHeight * 100; ;
                        break;
                    }
                }
            }
        }     
        if(clean)foreach (var chunk in map3D) { chunk.Value.delete(); }

        MapDisplay display = GetComponent<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap) display.DrawTextureMap(TextureGenerator.TextureFromNoiseMap(noiseMap));
        else if (drawMode == DrawMode.ColorMap) { 
            display.DrawTextureMap(TextureGenerator.TextureFromColorMap(colorMap, mapSize, mapSize));
            
        }
        else if(drawMode == DrawMode.All){
            display.DrawTextureMap(TextureGenerator.TextureFromColorMap(colorMap, mapSize, mapSize));
            if (!clean) map3D.Clear();
            GenerarMapaPorChunks();
            ObjectsGenerator.GenerarObjectos(mapSize, chunkSize, heightPerBlock, cellMap, map3D, objects);
        }
        else if (drawMode == DrawMode.NoObjects)
        {
            display.DrawTextureMap(TextureGenerator.TextureFromColorMap(colorMap, mapSize, mapSize));
            if (!clean) map3D.Clear();
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
                    map3D[chunkPos] = new Chunk(chunkPos,cellMap,heightPerBlock, sizePerBlock,transform);
                }
                else{
                    map3D[chunkPos].GenerateTerrainMesh(cellMap, heightPerBlock, sizePerBlock);
                }
            }
        }
    }
}
