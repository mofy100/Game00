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
                float alt = 0.0f;

                float scale = 50.0f;
                float height = 30.0f;
                float noise = Mathf.PerlinNoise(gX / scale, gZ / scale);
                alt += noise * height;
                
                scale = 30.0f;
                height = 120.0f;
                noise = Mathf.PerlinNoise(gX / scale, gZ / scale);
                alt += ((noise > 0.5f) ? (noise - 0.5f) * height : 0.0f);

                int maxY = (int)alt;

                for(int y = 0; y < Chunk.sizeV; y++){

                    Block b;
                    if(maxY - 1 <= y && y <= maxY){
                        b = new SoilBlock();
                        b.blockType = BlockType.Soil;
                        int r = rand.Next(12, 15);
                        b.SetBlockLevel((byte)r);
                    }else if(y < maxY){
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
