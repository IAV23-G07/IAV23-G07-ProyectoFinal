using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class Cell{
    public TerrainType type;
    public float noise;
    public float Height;
    public GameObject objectGenerated=null;
}
