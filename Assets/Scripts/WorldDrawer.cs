using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using static BlockUtils;

public partial class World // Other functions are written in "WorldUtils.cs"
{
    void Draw(){

        /*
        var userChunkId = GetChunkId(user.transform.position);
        Vector2Int chunkId = new Vector2Int(0, 0);

        for(int i = userChunkId.x - drawRange; i <= userChunkId.x + drawRange; i++){
            for(int j = userChunkId.y - drawRange; j <= userChunkId.y + drawRange; j++){

                chunkId.x = i;
                chunkId.y = j;
                */
        
        foreach(Vector2Int chunkId in drawingChunkIds){

            if(chunks.ContainsKey(chunkId)){

                Chunk chunk = chunks[chunkId];
                var matrix = Matrix4x4.TRS(chunk.GetPosition(), Quaternion.identity, Vector3.one);

                // Draw Blocks
                for(int k = 0; k < chunk.mesh.subMeshCount; k++){
                    var (blockType, blockLevel) = chunk.submeshBlockTypes[k];
                    Material material = BlockDataManager.GetBlockData((BlockType)blockType).GetMaterial(blockLevel);
                    Graphics.DrawMesh(chunk.mesh, matrix, material, 0, null, k);
                }

                // Draw Objects
                foreach(KeyValuePair<Vector3Int, BlockType> kvp in chunk.objects){
                    var localId = kvp.Key;
                    var blockType = kvp.Value;
                    var block = chunk.GetBlock(localId);
                    var blockData = BlockDataManager.GetBlockData(blockType);
                    var blockRotation = Quaternion.Euler(0.0f, block.GetAngle(), 0.0f);

                    Graphics.DrawMesh(block.GetMesh(), Matrix4x4.TRS(GetGlobalPos(chunkId, localId), blockRotation, Vector3.one), blockData.material, 0);
                }
            }else{
                if(!creatingChunk){
                    creatingChunk = true;
                    _ = CreateChunk(chunkId);
                }
            }

        }
    }
}