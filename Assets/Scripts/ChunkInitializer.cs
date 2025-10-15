using UnityEngine;
using System.Collections.Generic;

public partial class Chunk{
    public void InitializeBlocks(){
        System.Random rand = new System.Random(0);

        const int offset = 1000;

        for(int x = 0; x < Chunk.sizeH; x++){
            for(int z = 0; z < Chunk.sizeH; z++){

                int gX = x + chunkId.x * Chunk.sizeH + offset;
                int gZ = z + chunkId.y * Chunk.sizeH + offset;
                float alt = 5.0f;

                var (biome, factor) = GetBiome(gX, gZ);
                alt += GetAlt(gX, gZ, biome) * factor;
                int maxY = (int)alt;

                for(int y = 0; y < Chunk.sizeV; y++){
                    Block b;
                    if(y == maxY){
                        b = new Block();
                        b.blockType = BlockType.Soil;
                        if(biome == Biome.Grassland){
                            b.SetBlockSubType((byte)SoilType.Grass);
                        }else{
                            b.SetBlockSubType((byte)SoilType.Sand);
                        }
                    }else if(y <= maxY - 1){
                        b = new Block();
                        b.blockType = BlockType.Soil;
                        if(biome == Biome.Grassland){
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

    (Biome, float) GetBiome(int gX, int gZ){
        Biome biome;
        float factor = 1.0f;

        float scale = 150.0f;
        float noise = Mathf.PerlinNoise(gX / scale, gZ / scale);
        biome = (noise > 0.5f) ? Biome.Grassland : Biome.Desert;
        float noiseAbs = Mathf.Abs(noise - 0.5f);
        if(noiseAbs < 0.1f){
            factor *= noiseAbs / 0.1f;
        }

        return (biome, factor);
    }

    float GetAlt(int gX, int gZ, Biome biome){
        float alt = 0.0f;
        float scale;
        float height;
        float noise;

        if(biome == Biome.Grassland){
            scale = 10.0f;
            height = 20.0f;
            noise = Mathf.PerlinNoise(gX / scale, gZ / scale);
            alt += ((noise > 0.6f) ? (noise - 0.6f) : 0.0f) * height;

            scale = 30.0f;
            height = 60.0f;
            noise = Mathf.PerlinNoise(gX / scale, gZ / scale);
            alt += ((noise > 0.6f) ? (noise - 0.6f) : 0.0f) * height;
        }else if(biome == Biome.Desert){
            scale = 5.0f;
            height = 10.0f;
            noise = Mathf.PerlinNoise(gX / scale, gZ / scale);
            alt += ((noise > 0.6f) ? (noise - 0.6f) : 0.0f) * height;

            scale = 10.0f;
            height = 20.0f;
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
    Forest,
    Desert,
    Snowfiled,
    Mountain
}


