using UnityEngine;
using System.Collections.Generic;

public partial class Chunk{
    public void InitializeBlocks(){
        System.Random rand = new System.Random(0);

        const int offset = 1000;
        for(int x = 0; x < Chunk.sizeH; x++){
            for(int z = 0; z < Chunk.sizeH; z++){
                /*
                int gX = x + chunkId.x * Chunk.sizeH + offset;
                int gZ = z + chunkId.y * Chunk.sizeH + offset;
                float alt = 10.0f;
                bool isWater = false;

                // river
                float scale1 = 200.0f;
                float noise1 = Mathf.Abs(Mathf.PerlinNoise(gX / scale1, gZ / scale1) - 0.5f);

                if(noise1 < 0.02f){
                    isWater = true;
                    alt -= 10;
                }

                float scale2 = 100.0f;
                float height2 = 10.0f;
                float noise2 = Mathf.PerlinNoise(gX / scale2, gZ / scale2);
                alt += noise2 * height2;
                
                float scale3 = 30.0f;
                float height3 = 1000.0f;
                float noise3 = Mathf.PerlinNoise(gX / scale3, gZ / scale3);
                if(noise1 > 0.05f && noise3 > 0.5f){
                    alt += (noise1 - 0.05f) * (noise3 - 0.5f) * height3;
                }

                int maxY = (int)alt;
                */

                bool isWater = false;
                int maxY = 6;
                

                for(int y = 0; y < Chunk.sizeV; y++){

                    Block b;

                    if(isWater && maxY < y && y < maxY + 7){
                        b = new Block();
                        b.blockType = BlockType.Water;
                    }else if(maxY - 1 <= y && y <= maxY){
                        b = new SoilBlock();
                        b.blockType = BlockType.Soil;
                        // int r = rand.Next(12, 16);
                        int r = 15;
                        b.SetBlockLevel((byte)r);
                    }else if(y <= maxY - 2){
                        b = new SoilBlock();
                        b.blockType = BlockType.Soil;
                        b.SetBlockLevel(11);
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
}
