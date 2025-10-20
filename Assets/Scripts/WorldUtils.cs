using UnityEngine;

public partial class World{

    public Block GetBlock(Vector3 position){

        var chunkId = GetChunkId(position);
        if(!chunks.ContainsKey(chunkId)){
            // Debug.Log($"chunk {chunkId} does not exist");
            return null;
        }
        Vector3 localPos = position - new Vector3(chunkId.x, 0.0f, chunkId.y) * Chunk.sizeH;
        Vector3Int localId = new Vector3Int(Mathf.RoundToInt(localPos.x / Block.sizeH), Mathf.RoundToInt(localPos.y / Block.sizeV), Mathf.RoundToInt(localPos.z / Block.sizeH));
        if(localId.x < 0 || Chunk.sizeH <= localId.x ||
           localId.y < 0 || Chunk.sizeV <= localId.y ||
           localId.z < 0 || Chunk.sizeH <= localId.z){
            // Debug.Log($"position is out of bounds {position}");
            return null;
        }
        return chunks[chunkId].blocks[localId.x, localId.y, localId.z];
    }

    public Block GetBlock(Vector2Int chunkId, Vector3Int localId){
        if(!chunks.ContainsKey(chunkId)){
            // Debug.Log($"chunk {chunkId} does not exist");
            return null;
        }
        if(localId.x < 0){
            localId.x += Chunk.sizeH;
            chunkId.x -= 1;
        }else if(localId.x >= Chunk.sizeH){
            localId.x -= Chunk.sizeH;
            chunkId.x += 1;
        }
        if(localId.y < 0 || Chunk.sizeV <= localId.y){
            // Debug.Log($"GetBlock() : localId out of bounds {localId}");
            return null;
        }
        if(localId.z < 0){
            localId.z += Chunk.sizeH;
            chunkId.y -= 1;
        }else if(localId.z >= Chunk.sizeH){
            localId.z -= Chunk.sizeH;
            chunkId.y += 1;
        }
        return chunks[chunkId].blocks[localId.x, localId.y, localId.z];
    }

    public Vector2Int GetChunkId(Vector3 position){
        return new Vector2Int(Divide(Mathf.RoundToInt(position.x / Block.sizeH), Chunk.sizeH), Divide(Mathf.RoundToInt(position.z / Block.sizeH), Chunk.sizeH));
    }

    public Vector3Int GetLocalId(Vector3 position){
        return new Vector3Int(Mod(Mathf.RoundToInt(position.x / Block.sizeH), Chunk.sizeH), Mathf.RoundToInt(position.y / Block.sizeV), Mod(Mathf.RoundToInt(position.z / Block.sizeH), Chunk.sizeH));
    }

    public Vector3 GetGlobalPos(Vector2Int chunkId, Vector3Int localId){
        return new Vector3(chunkId.x * Chunk.sizeH + localId.x * Block.sizeH,
                           localId.y * Block.sizeV,
                           chunkId.y * Chunk.sizeH + localId.z * Block.sizeH);
    }

    public Vector3 GetLocalPos(Vector3 position){
        float half = Block.sizeH / 2.0f;
        return new Vector3(Mod((position.x + half), Chunk.sizeH * Block.sizeH) - half, 
                           position.y / Block.sizeV,
                           Mod((position.z + half), Chunk.sizeH * Block.sizeH) - half);
    }

}