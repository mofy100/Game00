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

    public Material GetMaterial(byte blockLevel = 0){
        if(material == null){
            return materials[blockLevel];
        }else{
            return material;
        }
    }

}
