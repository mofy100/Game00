using UnityEngine;
using System.Collections.Generic;

public class BiomeLocation {
    public Vector2Int position;
    public float size;
    public Biome biome;
};

public partial class Chunk{
    public List<BiomeLocation> biomeLocations = new List<BiomeLocation>(){
        new BiomeLocation{
            position = new Vector2Int(0, 0),
            size = 30.0f,
            biome = Biome.Savana
        },
        new BiomeLocation{
            position = new Vector2Int(-100, 0),
            size = 30.0f,
            biome = Biome.Mountain
        },
        new BiomeLocation{
            position = new Vector2Int(100, 0),
            size = 30.0f,
            biome = Biome.Desert
        },
    };
    
    public void InitializeBlocks(){
        System.Random rand = new System.Random(0);

        const int offsetX = 1000;
        const int offsetZ = 1000;
        float amp;
        float noise;

        for(int x = 0; x < Chunk.sizeH; x++){
            for(int z = 0; z < Chunk.sizeH; z++){
                float alt = 0.0f;

                int gX = x + chunkId.x * Chunk.sizeH;
                int gZ = z + chunkId.y * Chunk.sizeH;
                gX += offsetX;
                gZ += offsetZ;

                float tempScale = 500.0f;
                noise = Mathf.PerlinNoise(gX / tempScale, gZ / tempScale);
                amp = 0.01f;
                if(Mathf.Abs(noise - 0.5f) < 0.3f){
                    noise = 0.5f + (noise - 0.5f) * 0.9f;
                }
                noise += (float)rand.NextDouble() * (2 * amp) - amp;
                noise = Mathf.Clamp01(noise);
                noise *= (8.0f - 0.000001f);
                float soilTemperature = noise;

                float humidScale = 500.0f;
                noise = Mathf.PerlinNoise(gX / humidScale + 10.0f, gZ / humidScale + 10.0f);
                amp = 0.01f;
                noise = 0.5f + (noise - 0.5f) * 0.9f;
                noise += (float)rand.NextDouble() * (2 * amp) - amp;
                noise = Mathf.Clamp01(noise);
                noise *= (8.0f - 0.000001f);
                float soilHumid = noise;

                if(soilTemperature > 8.0f){
                    Debug.Log(soilTemperature);
                }
                if(soilHumid > 8.0f){
                    Debug.Log(soilHumid + ",");
                }

                byte soilType = (byte)(Mathf.FloorToInt(soilTemperature) * 8 + Mathf.FloorToInt(soilHumid));
                alt += GetAlt(gX, gZ, soilTemperature, soilHumid);
                int maxY = (int)alt;

                for(int y = 0; y < Chunk.sizeV; y++){
                    Block b;
                   if(y <= maxY){
                        b = new Block();
                        b.blockType = BlockType.Soil;
                        b.SetBlockSubType(soilType);
                    }else{
                        b = new Block();
                        b.blockType= BlockType.Empty;
                    }

                    b.chunkId = chunkId;
                    b.localId = new Vector3Int(x, y, z);
                    blocks[x, y, z] = b;
                }
                grounds[x, z] = maxY;
            }
        }
        modified = true;
    }

    float GetAlt(int gX, int gZ, float temp, float humid){
        float alt = 0.0f;
        float freq;
        float height;
        float offset;
        float noise;

        
        // temp, humid 0.0f ~ 8.0f
        temp /= 8.0f;  // 0.0f ~ 1.0f
        humid /= 8.0f; // 0.0f ~ 1.0f

        freq = 0.005f;
        // height = 20.0f;
        height = 10.0f;
        offset = 100.0f;
        noise = Mathf.PerlinNoise(gX * freq + offset, gZ * freq + offset);
        alt += noise * height;

        // alt += ((noise > 0.5f) ? (noise - 0.5f) : 0.0f) * height;
        freq = 0.02f;
        height = 30.0f; 
        offset = 200.0f;
        // height = Mathf.PerlinNoise(gX / 100.0f, gZ / 100.0f) * 30.0f + 30.0f;
        noise = Mathf.PerlinNoise(gX * freq + offset, gZ * freq + offset);
        alt += (noise - 0.2f) * height;

        freq = 0.06f;
        height = 30.0f + 50.0f * temp;
        offset = 300.0f;
        // height = Mathf.PerlinNoise(gX / 100.0f, gZ / 100.0f) * 30.0f + 30.0f;
        noise = Mathf.PerlinNoise(gX * freq + offset, gZ * freq + offset);
        alt += ((noise > 0.6f) ? (noise - 0.6f) : 0.0f) * height;

        freq = 0.1f;
        height = (temp > 0.8f) ? 100.0f : 0.0f;
        offset = 400.0f;
        // height = Mathf.PerlinNoise(gX / 100.0f, gZ / 100.0f) * 30.0f + 30.0f;
        noise = Mathf.PerlinNoise(gX * freq + offset, gZ * freq + offset);
        alt += ((noise > 0.7f) ? (noise - 0.75f) : 0.0f) * height;

        return (alt > 0) ? alt : 0.0f;
    }
}


public enum Biome{
    Grassland,
    Savana,
    Forest,
    Desert,
    Snowfiled,
    Mountain
}


