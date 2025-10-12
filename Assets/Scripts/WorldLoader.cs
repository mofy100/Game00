using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class WorldLoader : MonoBehaviour{

    static public void SaveChunk(Vector2Int chunkId, Chunk chunk){
        string path = Path.Combine(Application.dataPath, $"SaveData/chunk_({chunkId.x})_({chunkId.y}).dat");
        BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create));

        for(int x = 0; x < Chunk.sizeH; x++){
            for(int y = 0; y < Chunk.sizeV; y++){
                for(int z = 0; z < Chunk.sizeH; z++){
                    Block block = chunk.blocks[x, y, z];
                    writer.Write((byte)block.blockType);
                    if(block.IsFense()){
                        writer.Write((byte)block.GetFenseNumber());
                    }else if(block.IsSoil()){
                        writer.Write((byte)block.GetBlockLevelRaw());
                    }
                }
            }
        }
        writer.Close();
        chunk.modified = false;
    }

    static public bool LoadChunk(Chunk chunk){
        var chunkId = chunk.chunkId;
        string path = Path.Combine(Application.dataPath, $"SaveData/chunk_({chunkId.x})_({chunkId.y}).dat");
        if(!File.Exists(path)){
            Debug.LogWarning($"File not fount : {path}");
            return false;
        }

        BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));
        for(int x = 0; x < Chunk.sizeH; x++){
            for(int y = 0; y < Chunk.sizeV; y++){
                for(int z = 0; z < Chunk.sizeH; z++){
                    BlockType blockType = (BlockType)reader.ReadByte();
                    Block b = BlockUtils.CreateBlock(blockType);
                    b.chunkId = chunkId;
                    b.localId = new Vector3Int(x, y, z);
                    chunk.blocks[x, y, z] = b;
                    if(b.IsObject()){
                        chunk.objects[new Vector3Int(x, y, z)] = blockType;
                        if(b.IsFense()){
                            byte fenseNumber = reader.ReadByte();
                            b.SetFenseShape(fenseNumber);
                        }
                    }else if(b.IsSoil()){
                        byte blockLevel = reader.ReadByte();
                        b.SetBlockLevelRaw(blockLevel);
                    }
                }
            }
        }
        chunk.SetGround();
        return true;
    }

    [ContextMenu("Save Field")]
    public void SaveField(){
        Debug.Log("save field");
        var world = GameObject.Find("World").GetComponent<World>();
        foreach(Vector2Int key in world.chunks.Keys){
            Chunk chunk = world.chunks[key];
            if(chunk.modified){
                SaveChunk(key, chunk);
            }
        }
    }

    [ContextMenu("Reset Field")]
    public void ResetField(){
#if UNITY_EDITOR

        string path = Path.Combine(Application.dataPath, $"SaveData/");
        if(Directory.Exists(path)){
            bool confirm = EditorUtility.DisplayDialog(
                "チャンクデータの削除",
                "全チャンクデータを削除しますか？",
                "削除",
                "キャンセル"
            );
            if(confirm){
                string[] files = Directory.GetFiles(path, "*.dat");
                foreach(string file in files){
                    File.Delete(file);
                    string metaFile = file + ".meta";
                    if(File.Exists(metaFile)){
                        File.Delete(metaFile);
                    }
                }
                AssetDatabase.Refresh();
                Debug.Log("チャンクデータ削除完了");
            }
        }
    }
#endif
}
