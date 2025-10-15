using UnityEngine;
using System.Collections.Generic;

public partial class Chunk{
    public const int sizeH = 32;
    public const int sizeV = 32;
    // private const float updateInterval = 3.0f;
    public bool modified = false;
    public Vector2Int chunkId;

    // Blocks
    public Block[,,] blocks = new Block[sizeH, sizeV, sizeH];
    public int[,] grounds = new int[sizeH, sizeH];
    public MeshData meshData;
    public Mesh mesh;
    public List<Matrix4x4> matrices = new List<Matrix4x4>();

    // Objects
    public Dictionary<Vector3Int, BlockType> objects = new Dictionary<Vector3Int, BlockType>();

    // BlockType of submesh
    public List<(BlockType, byte)> submeshBlockTypes = new List<(BlockType, byte)>();

    /*
    public bool Update(){
        time += Time.deltaTime * Random.Range(0.0f, 1.0f);
        if(time < updateInterval){
            return false;
        }
        time -= updateInterval;
        for(int x = 0; x < sizeH; x++){
            for(int z = 0; z < sizeH; z++){
                blocks[x, grounds[x, z], z].Update();
            }
        }
        ChunkMeshGenerator.UpdateMeshData(this);
        ChunkMeshGenerator.UpdateMesh(this);
        return true;
    }
    */

    public Vector3 GetPosition(){
        return new Vector3(chunkId.x * Chunk.sizeH * Block.sizeH, 0.0f, chunkId.y * Chunk.sizeH * Block.sizeH);
    }

    public Block GetBlock(Vector3Int localId){
        return blocks[localId.x, localId.y, localId.z];
    }

    public void SetBlock(Block b, Vector3Int localId){
        blocks[localId.x, localId.y, localId.z] = b;
    }

    public int GetGround(Vector2Int localId){
        return grounds[localId.x, localId.y];
    }

    public void SetGround(){
        for(int x = 0; x < sizeH; x++){
            for(int z = 0; z < sizeH; z++){
                bool flag = false;
                for(int y = sizeV - 1; y >= 0; y--){
                    if(!blocks[x, y, z].IsEmpty()){
                        grounds[x, z] = y;
                        flag = true;
                    }
                    if(flag) break;
                }
            }
        }
    }
    
}
