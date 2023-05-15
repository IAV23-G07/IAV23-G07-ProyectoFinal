using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UIElements;

public static  class Noise {
    /// <summary>
    /// Genera un mapa Procedural en base a los siguienets parametros
    /// </summary>
    /// <param name="Width">La anchura del mapa de ruido a generar</param>
    /// <param name="Height">La altura del mapa de ruido a generar</param>
    /// <param name="seed">La semilla aleatoria utilizada para generar el ruido. Dos semillas diferentes generarán ruido diferente</param>
    /// <param name="scale">El factor de escala del ruido generado.Un valor mayor producirá un ruido con detalles más finos</param>
    /// <param name="octaves"> El número de octavas utilizadas en el algoritmo de ruido. Cada octava es una capa de ruido que se suma al resultado final.A medida que se agregan más octavas, el ruido generado se vuelve más detallado</param>
    /// <param name="persistance">La persistencia controla la amplitud de cada octava. Un valor más bajo reducirá el efecto de las octavas posteriores de las octavas posteriores</param>
    /// <param name="lacunarity">El lacunaridad controla la frecuencia de cada octava. Un valor más alto aumentará la frecuencia</param>
    /// <param name="offset">La posición inicial del ruido generado</param>
    /// <returns></returns>
    public static float[,] GenerateNoiseMap
        (int Width, int Height,int seed,float scale,int octaves, float persistance, float lacunarity,Vector2 offset){
        if (scale <= 0) scale = 0.0001f;
        float[,] noiseMap= new float[Width,Height];       

        System.Random r = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++){
            float offsetX = r.Next(-10000, 10000) + offset.x;
            float offsetY = r.Next(-10000, 10000) + offset.y;
            octaveOffsets[i]= new Vector2(offsetX,offsetY);  
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = Width/ 2f;
        float halfHeight = Height/ 2f;
        for (int y = 0; y < Height; y++){
            for (int x = 0; x < Width; x++){
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    float smpleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float smpleY = (y - halfHeight)/ scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(smpleX, smpleY)* 2-1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;

                }

                if(noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if(noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

                noiseMap[x,y] = noiseHeight;
            }
        }
        for (int y = 0; y < Height; y++){
            for (int x = 0; x < Width; x++){
                noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,y]);
            }
        }
        return noiseMap;
    }
    public static float[,] GenerateFalloffMap(int size){
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++){
            for (int j = 0; j < size; j++){
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }
    static float Evaluate(float value){
        float a = 3;
        float b = 2.2f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
