using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "BlockData")]
public class BlockData : ScriptableObject{
    public BlockType blockType;
    public Mesh mesh;
    public Mesh[] meshes;
    public Material material;
    public Material[] materials;
    // public Vector3Int scale;

    public Material GetMaterial(byte blockSubType = 0){
        if(material == null){
            if(blockSubType >= materials.Length){
                Debug.Log($"blockType {blockType} subType {blockSubType}");
            }
            return materials[blockSubType];
        }else{
            return material;
        }
    }

}
