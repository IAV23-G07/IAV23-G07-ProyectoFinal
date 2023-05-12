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

    public GameObject basee;
    public GameObject seaBottom;
    public GameObject Edges;
    float[,] noiseMap;
    float[,] fallOffMap;
    Cell[,] mapaCells;
    public void GenerateMap(){
        fallOffMap = Noise.GenerateFalloffMap(mapSize);
        noiseMap= Noise.GenerateNoiseMap(mapSize, mapSize,seed,noiseScale,octaves,persistance,lacunarity,offset);

        Color[] colorMap = new Color[mapSize * mapSize];
        mapaCells= new Cell[mapSize,mapSize];
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
            MeshGenerator.GenerateTerrainMesh(mapaCells,basee,HeightPerBlock,false);
            MeshGenerator.DrawEdges(mapaCells, Edges, HeightPerBlock, false);
            //MeshGenerator.GenerateTerrainMesh(mapaCells,seaBottom,HeightPerBlock,true);          
            //MeshGenerator.DrawTexture(mapaCells);
        }
        else if (drawMode == DrawMode.FallOff) display.DrawTextureMap(TextureGenerator.TextureFromNoiseMap(Noise.GenerateFalloffMap(mapSize)));
    }
    private void OnValidate()
    {
        if(mapSize < 1) mapSize = 1;
        if(lacunarity<1) lacunarity=1;
        if(octaves< 0) octaves=0;
    }
}
