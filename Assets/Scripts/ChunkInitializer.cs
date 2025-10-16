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
            position = new Vector2Int(100, 0),
            size = 30.0f,
            biome = Biome.Desert
        },
        new BiomeLocation{
            position = new Vector2Int(-100, 0),
            size = 30.0f,
            biome = Biome.Mountain
        },
        new BiomeLocation{
            position = new Vector2Int(0, 100),
            size = 30.0f,
            biome = Biome.Savana
        },
        new BiomeLocation{
            position = new Vector2Int(0, -100),
            size = 30.0f,
            biome = Biome.Grassland
        },
    };
    
    public void InitializeBlocks(){
        System.Random rand = new System.Random(0);

        const int offset = 1000;

        for(int x = 0; x < Chunk.sizeH; x++){
            for(int z = 0; z < Chunk.sizeH; z++){

                int gX = x + chunkId.x * Chunk.sizeH;
                int gZ = z + chunkId.y * Chunk.sizeH;

                float alt = 5.0f;

                var (biome, factor) = GetBiome(gX, gZ);
                gX += offset;
                gZ += offset;

                alt += GetAlt(gX, gZ, biome) * factor;
                int maxY = (int)alt;

                for(int y = 0; y < Chunk.sizeV; y++){
                    Block b;
                    if(y == maxY){
                        b = new Block();
                        b.blockType = BlockType.Soil;
                        if(biome == Biome.Grassland){
                            b.SetBlockSubType((byte)SoilType.Green);
                        }else if(biome == Biome.Savana){
                            b.SetBlockSubType((byte)SoilType.Olive);
                        }else{
                            b.SetBlockSubType((byte)SoilType.Sand);

                        }
                    }else if(y <= maxY - 1){
                        b = new Block();
                        b.blockType = BlockType.Soil;
                        if(biome == Biome.Grassland){
                            b.SetBlockSubType((byte)SoilType.Barren);
                        }else if(biome == Biome.Savana){
                            b.SetBlockSubType((byte)SoilType.Barren);
                        }else{
                            b.SetBlockSubType((byte)SoilType.Sand);
                        }
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

    (Biome, float) GetBiome(int x, int z){
        Biome thisBiome = Biome.Grassland;
        float noiseScale = 500.0f;
        float noiseAmplitude = 100.0f;
        Vector2 thisPoint = new Vector2(x, z);
        float minDistance = float.MaxValue;
        foreach(BiomeLocation location in biomeLocations){
            float dist = Vector2.Distance(thisPoint, location.position);
            float nx = Mathf.PerlinNoise(x / noiseScale + location.position.x, z / noiseScale + location.position.y);
            float noise = (nx - 0.5f) * 2f * noiseAmplitude; // -amplitude〜+amplitude に揺らぐ
            dist += noise;

            if(dist < minDistance){
                minDistance = dist;
                thisBiome = location.biome;
            }
        }
        return (thisBiome, 1.0f);

        /*
        float minDistance = 100000.0f;
        Biome thisBiome = Biome.Desert;
        float factor = 1.0f;
        foreach(BiomeLocation location in biomeLocations){
            float distance = (x - location.position.x) * (x - location.position.x) + (z - location.position.y) * (z - location.position.y);
            distance = Mathf.Sqrt(distance);
            distance /= location.size;
            if(distance < 1.0f){
                if(distance < minDistance){
                    minDistance = distance;
                    thisBiome = location.biome;
                    factor = 1.0f;
                }
            }else{
                thisBiome = Biome.Grassland;
            }
        }
        */
    }

    float GetAlt(int gX, int gZ, Biome biome){
        float alt = 0.0f;
        float scale;
        float height;
        float noise;

        if(biome == Biome.Grassland){
        }else if(biome == Biome.Mountain){
            scale = 10.0f;
            height = 20.0f;
            noise = Mathf.PerlinNoise(gX / scale, gZ / scale);
            alt += ((noise > 0.5f) ? (noise - 0.5f) : 0.0f) * height;

            scale = 30.0f;
            height = 60.0f;
            noise = Mathf.PerlinNoise(gX / scale, gZ / scale);
            alt += ((noise > 0.5f) ? (noise - 0.5f) : 0.0f) * height;
        }else if(biome == Biome.Savana){
            scale = 10.0f;
            height = 10.0f;
            noise = Mathf.PerlinNoise(gX / scale, gZ / scale);
            alt += ((noise > 0.6f) ? (noise - 0.6f) : 0.0f) * height;

            scale = 30.0f;
            height = 10.0f;
            noise = Mathf.PerlinNoise(gX / scale, gZ / scale);
            alt += ((noise > 0.6f) ? (noise - 0.6f) : 0.0f) * height;
        }else if(biome == Biome.Desert){
            scale = 10.0f;
            height = 10.0f;
            noise = Mathf.PerlinNoise(gX / scale, gZ / scale);
            alt += ((noise > 0.6f) ? (noise - 0.6f) : 0.0f) * height;

            scale = 30.0f;
            height = 10.0f;
            noise = Mathf.PerlinNoise(gX / scale, gZ / scale);
            alt += ((noise > 0.6f) ? (noise - 0.6f) : 0.0f) * height;
        }else{
            alt = 5.0f;
        }

        return alt;
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


