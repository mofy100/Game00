using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ChunkMeshGenerator{

    static readonly Vector2 uv00 = new Vector2(0, 0);
    static readonly Vector2 uv01 = new Vector2(0, 1);
    static readonly Vector2 uv10 = new Vector2(1, 0);
    static readonly Vector2 uv11 = new Vector2(1, 1);

    static readonly Dictionary<Direction, Vector3[]> faceOffsets = new()
    {
        [Direction.Right] = new[] {
            new Vector3(1f, 1f, 1f),
            new Vector3(1f, -1f, 1f),
            new Vector3(1f, -1f, -1f),
            new Vector3(1f, 1f, -1f)
        },
        [Direction.Left] = new[] {
            new Vector3(-1f, -1f, -1f),
            new Vector3(-1f, -1f, 1f),
            new Vector3(-1f, 1f, 1f),
            new Vector3(-1f, 1f, -1f)
        },
        [Direction.Up] = new[] {
            new Vector3(1f, 1f, 1f),
            new Vector3(1f, 1f, -1f),
            new Vector3(-1f, 1f, -1f),
            new Vector3(-1f, 1f, 1f)
        },
        [Direction.Down] = new[] {
            new Vector3(-1f, -1f, -1f),
            new Vector3(1f, -1f, -1f),
            new Vector3(1f, -1f, 1f),
            new Vector3(-1f, -1f, 1f)
        },
        [Direction.Forward] = new[] {
            new Vector3(1f, 1f, 1f),
            new Vector3(-1f, 1f, 1f),
            new Vector3(-1f, -1f, 1f),
            new Vector3(1f, -1f, 1f)
        },
        [Direction.Back] = new[] {
            new Vector3(-1f, -1f, -1f),
            new Vector3(-1f, 1f, -1f),
            new Vector3(1f, 1f, -1f),
            new Vector3(1f, -1f, -1f)
        }
    };

    static readonly Dictionary<Direction, Vector2[]> uvPositions = new(){
        [Direction.Right] = new[]{
            new Vector2(0.49f, 1.0f),
            new Vector2(0.49f, 0.67f),
            new Vector2(0.0f, 0.67f),
            new Vector2(0.0f, 1.0f)
        },
        [Direction.Left] = new[]{
            new Vector2(1.0f, 0.67f),
            new Vector2(0.51f, 0.67f),
            new Vector2(0.51f, 1.0f),
            new Vector2(1.0f, 1.0f)
        },
        [Direction.Up] = new[]{
            new Vector2(0.49f, 0.66f),
            new Vector2(0.49f, 0.34f),
            new Vector2(0.01f, 0.34f),
            new Vector2(0.01f, 0.66f)
        },
        [Direction.Down] = new[]{
            new Vector2(0.51f, 0.34f),
            new Vector2(1.0f, 0.34f),
            new Vector2(1.0f, 0.66f),
            new Vector2(0.51f, 0.66f)
        },
        [Direction.Forward] = new[]{
            new Vector2(0.0f, 0.33f),
            new Vector2(0.49f, 0.33f),
            new Vector2(0.49f, 0.0f),
            new Vector2(0.0f, 0.0f)
        },
        [Direction.Back] = new[]{
            new Vector2(0.51f, 0.0f),
            new Vector2(0.51f, 0.33f),
            new Vector2(1.0f, 0.33f),
            new Vector2(1.0f, 0.0f)
        },
    };

    public static void UpdateMeshData(Chunk chunk){

        chunk.meshData = new MeshData();
        // chunk.mesh = new Mesh();

        List<Vector3> vertices = new();
        List<int> faces = new();
        List<Vector2> uvs = new();
        List<List<int>> subFaces = new List<List<int>>();
        var submeshBlockTypes = new List<(BlockType, byte)>();

        for(int x = 0; x < Chunk.sizeH; x++){
            for(int y = 0; y < Chunk.sizeV; y++){
                for(int z = 0; z < Chunk.sizeH; z++){
                    Block block = chunk.blocks[x, y, z];
                    if(block.IsEmpty()){
                        continue;
                    }
                    BlockType blockType = block.blockType;
                    byte blockLevel = block.GetBlockLevel();
                    if(!submeshBlockTypes.Contains((blockType, blockLevel))){
                        submeshBlockTypes.Add((blockType, blockLevel));
                        subFaces.Add(new List<int>());
                    }

                    if(block.IsCube()){
                        int submeshId = submeshBlockTypes.IndexOf((blockType, blockLevel));
                        // check 6 faces of the block
                        if(IsEmpty(chunk.blocks, x + 1, y, z)) AddFace(vertices, faces, uvs, subFaces, submeshId, block, Direction.Right);
                        if(IsEmpty(chunk.blocks, x - 1, y, z)) AddFace(vertices, faces, uvs, subFaces, submeshId, block, Direction.Left);
                        if(IsEmpty(chunk.blocks, x, y + 1, z)) AddFace(vertices, faces, uvs, subFaces, submeshId, block, Direction.Up);
                        if(IsEmpty(chunk.blocks, x, y - 1, z)) AddFace(vertices, faces, uvs, subFaces, submeshId, block, Direction.Down);
                        if(IsEmpty(chunk.blocks, x, y, z + 1)) AddFace(vertices, faces, uvs, subFaces, submeshId, block, Direction.Forward);
                        if(IsEmpty(chunk.blocks, x, y, z - 1)) AddFace(vertices, faces, uvs, subFaces, submeshId, block, Direction.Back);
                    }


                }
            }
        }

        /*
        chunk.mesh.vertices = vertices.ToArray();
        chunk.mesh.triangles = faces.ToArray();
        chunk.mesh.uv = uvs.ToArray();
        chunk.mesh.RecalculateNormals();

        chunk.mesh.subMeshCount = subFaces.Count;
        for(int i = 0; i < subFaces.Count; i++){
            chunk.mesh.SetTriangles(subFaces[i], i);
        }
        */

        chunk.meshData.vertices = vertices;
        chunk.meshData.faces = faces;
        chunk.meshData.uvs = uvs;
        chunk.meshData.subFaces = subFaces;

        chunk.submeshBlockTypes = submeshBlockTypes;
    }

    public static void UpdateMesh(Chunk chunk){
        chunk.mesh = new Mesh();
        MeshData data = chunk.meshData;
        chunk.mesh.vertices = data.vertices.ToArray();
        chunk.mesh.triangles = data.faces.ToArray();
        chunk.mesh.uv = data.uvs.ToArray();
        chunk.mesh.RecalculateNormals();

        chunk.mesh.subMeshCount = data.subFaces.Count;
        for(int i = 0; i < data.subFaces.Count; i++){
            chunk.mesh.SetTriangles(data.subFaces[i], i);
        }
    }

    static bool IsEmpty(Block[,,] blocks, int x, int y, int z){
        if(x < 0 || Chunk.sizeH <= x || y < 0 || Chunk.sizeV <= y || z < 0 || Chunk.sizeH <= z){
            return true;
        }else if(blocks[x, y, z] == null){
            return true;
        }else{
            return !(blocks[x, y, z].IsCube());
        }
    }

    static void AddFace(List<Vector3> vertices, List<int> faces, List<Vector2> uvs, List<List<int>> subFaces, int submeshId, Block block, Direction direction){
        Vector3 pos = block.GetLocalPosition();
        BlockType blockType = block.blockType;

        // four vertices of the face
        Vector3[] verts = {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
        for(int i = 0; i < 4; i++){
            verts[i] += faceOffsets[direction][i];
        }

        // modify block scale
        for(int i = 0; i < 4; i++){
            verts[i].x *= (Block.sizeH / 2.0f);
            verts[i].y *= (Block.sizeV / 2.0f);
            verts[i].z *= (Block.sizeH / 2.0f);
            verts[i] += pos;
        }

        int vertsCount = vertices.Count;
        vertices.AddRange(verts);

        faces.Add(vertsCount);
        faces.Add(vertsCount + 1);
        faces.Add(vertsCount + 2);
        faces.Add(vertsCount);
        faces.Add(vertsCount + 2);
        faces.Add(vertsCount + 3);

        for(int i = 0; i < 4; i++){
            uvs.Add(uvPositions[direction][i]);
        }

        subFaces[submeshId].Add(vertsCount);
        subFaces[submeshId].Add(vertsCount + 1);
        subFaces[submeshId].Add(vertsCount + 2);
        subFaces[submeshId].Add(vertsCount);
        subFaces[submeshId].Add(vertsCount + 2);
        subFaces[submeshId].Add(vertsCount + 3);
    }
}

public class MeshData {
    public List<Vector3> vertices = new();
    public List<int> faces = new();
    public List<Vector2> uvs = new();
    public List<List<int>> subFaces = new List<List<int>>();
}