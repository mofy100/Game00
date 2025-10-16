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

    public float GetGround(Vector3 position){
        /*
        Vector2Int chunkId = GetChunkId(position);
        Vector2Int localId = new Vector2Int(Mathf.RoundToInt((position.x - chunkId.x * Chunk.sizeH) / Block.sizeH), 
                                            Mathf.RoundToInt((position.z - chunkId.y * Chunk.sizeH) / Block.sizeH));
        Debug.Log($"pos {position} localId {localId}");
        float g00 = GetGround(chunkId, localId);
        float g01 = GetGround(chunkId, localId + Vector2Int.right);
        float g10 = GetGround(chunkId, localId + Vector2Int.up);
        float g11 = GetGround(chunkId, localId + Vector2Int.one);

        float dx = position.x - Mathf.FloorToInt(position.x - 0.5f) - 0.5f;
        float dy = position.z - Mathf.FloorToInt(position.z - 0.5f) - 0.5f;

        return g00 * (1 - dx) * (1 - dy) +
               g01 * dx * (1 - dy) +
               g10 * (1 - dx) * dy + 
               g11 * dx * dy;
        */

        Vector2Int chunkId = GetChunkId(position);
        Vector2Int localId = new Vector2Int(Mathf.FloorToInt((position.x - chunkId.x * Chunk.sizeH) / Block.sizeH), 
                                            Mathf.FloorToInt((position.z - chunkId.y * Chunk.sizeH) / Block.sizeH));
        float g00 = GetGround(chunkId, localId);
        float g01 = GetGround(chunkId, localId + Vector2Int.right);
        float g10 = GetGround(chunkId, localId + Vector2Int.up);
        float g11 = GetGround(chunkId, localId + Vector2Int.one);

        float dx = position.x - chunkId.x * Chunk.sizeH - localId.x;
        float dy = position.z - chunkId.y * Chunk.sizeH - localId.y;

        return g00 * (1 - dx) * (1 - dy) +
               g01 * dx * (1 - dy) +
               g10 * (1 - dx) * dy + 
               g11 * dx * dy;
        /*
        Vector2Int x0z0 = new Vector2Int (Mathf.FloorToInt(localPos.x / Block.sizeH), Mathf.FlootToInt(localPos.z / Block.sizeH));
        Vector2Int x1z0 = x0z0 + new Vector2Int(1, 0);
        Vector2Int x0z1 = x0z0 + new Vector2Int(0, 1);
        Vector2Int x1z1 = x0z0 + new Vector2Int(1, 1);
        */
    }

    public int GetGround(Vector2Int chunkId, Vector2Int localId){
        if(localId.x < 0){
            localId.x += Chunk.sizeH;
            chunkId.x -= 1;
        }else if(localId.x >= Chunk.sizeH){
            localId.x -= Chunk.sizeH;
            chunkId.x += 1;
        }
        if(localId.y < 0){
            localId.y += Chunk.sizeH;
            chunkId.y -= 1;
        }else if(localId.y >= Chunk.sizeH){
            localId.y -= Chunk.sizeH;
            chunkId.y += 1;
        }
        return chunks[chunkId].GetGround(localId);
    }



}