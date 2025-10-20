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
    public MeshData meshData;
    public Mesh mesh;
    // public List<Matrix4x4> matrices = new List<Matrix4x4>();

    // Objects
    // public Dictionary<Vector3Int, BlockType> objects = new Dictionary<Vector3Int, BlockType>();
    public Dictionary<BlockType, List<Matrix4x4>> objects = new Dictionary<BlockType, List<Matrix4x4>>();

    // BlockType of submesh
    public List<(BlockType, byte)> submeshBlockTypes = new List<(BlockType, byte)>();

    public Vector3 GetPosition(){
        return new Vector3(chunkId.x * Chunk.sizeH * Block.sizeH, 0.0f, chunkId.y * Chunk.sizeH * Block.sizeH);
    }

    public Block GetBlock(Vector3Int localId){
        return blocks[localId.x, localId.y, localId.z];
    }

    public void SetBlock(Block b, Vector3Int localId){
        blocks[localId.x, localId.y, localId.z] = b;
    }

    public Vector3 GetGlobalPosition(Vector3Int localId){
        return GetPosition() + new Vector3(localId.x, localId.y * Block.sizeV, localId.z);
    }

    public void SetObject(BlockType type, Vector3Int localId){
        if(!objects.ContainsKey(type)){
            objects[type] = new List<Matrix4x4>();
        }
        Matrix4x4 matrix = Matrix4x4.TRS(GetGlobalPosition(localId), Quaternion.identity, Vector3.one);
        objects[type].Add(matrix);
    }

    public void RemoveObject(BlockType type, Vector3Int localId){
        if(!objects.ContainsKey(type)){
            return;
        }
        Vector3 globalPosition = GetGlobalPosition(localId);
        objects[type].RemoveAll(mat => Vector3.Distance(mat.GetColumn(3), globalPosition) < 0.1f);
    }

}
