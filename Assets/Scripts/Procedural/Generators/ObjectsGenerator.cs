using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using static MapGenerator;

public static class ObjectsGenerator {
   public static void GenerarObjectos
        (int mapSize, int chunkSize,float heightPerBlock, Cell[,] cellMap, Dictionary<Vector2,Chunk> chunks, ObjectInMap[] objectsToGenerate){

        float topLeftX = (mapSize - 1) / -2f;
        float topLeftZ = (mapSize - 1) / -2f;

        for (int y = 0; y < mapSize; y++){
            for (int x = 0; x < mapSize; x++){
                if (cellMap[x, y].objectGenerated==null){// si no hay un gameObject generado anteriormente
                    Cell current = cellMap[x, y];
                    foreach (var obj in objectsToGenerate.OrderBy(o => o.Density))//los ordeno por orden de densidad para q sea equivalente
                    {
                        if (obj.GenerationLayer == current.type.Layer){
                            float noiseValue = Mathf.PerlinNoise(x * obj.NoiseScale, y * obj.NoiseScale);
                            float v = Random.Range(0.0f, obj.Density);
                            if (noiseValue < obj.Density){

                                Vector2 chunkPos = new Vector2(x / chunkSize, y / chunkSize);
                                GameObject generated= GameObject.Instantiate(obj.prefab, chunks[chunkPos].objectos.transform);

                                generated.transform.position = new Vector3(topLeftX+x, heightPerBlock * current.noise * 100, topLeftX - y);
                                generated.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                                generated.transform.localScale =Vector3.one * Random.Range(0.8f, 1.2f);

                                current.objectGenerated= generated;
                                break;
                            }
                        }
                           
                    }
                }
            }
        }
    }
}
