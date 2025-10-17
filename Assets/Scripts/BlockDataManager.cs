using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.IO;

public class BlockDataManager : MonoBehaviour{
    public static Dictionary<BlockType, BlockData> blockDataBase;

    void Awake(){
        blockDataBase = Resources.LoadAll<BlockData>("BlockData").ToDictionary(data => data.blockType);
    }

    public static BlockData GetBlockData(BlockType blockType){
        return blockDataBase[blockType];
    }

    [ContextMenu("Set Soil Material")]
    void SetSoilMaterials(){
        string textureFolder = "Assets/Textures/Soils";
        string materialFolder = "Assets/Materials/Soils";
        string soilDataPath = "Assets/Resources/BlockData/Soil.asset";
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { textureFolder });
        BlockData block = AssetDatabase.LoadAssetAtPath<BlockData>(soilDataPath);

        if(block == null){
            Debug.Log($"soil block data not found path : {soilDataPath}");
            return;
        }

        foreach (string guid in guids){
            string texturePath = AssetDatabase.GUIDToAssetPath(guid);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (texture == null) continue;

            string textureName = Path.GetFileNameWithoutExtension(texturePath);
            string materialPath = Path.Combine(materialFolder, textureName + ".mat");

            if (File.Exists(materialPath)){
                AssetDatabase.DeleteAsset(materialPath);
            }

            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.SetTexture("_BaseMap", texture);
            AssetDatabase.CreateAsset(mat, materialPath);
            Debug.Log($"Create Soil Materials: {materialPath}");

        }

        guids = AssetDatabase.FindAssets("t:Material", new[] { materialFolder });
        Material[] materials = guids
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Select(path => AssetDatabase.LoadAssetAtPath<Material>(path))
            .Where(mat => mat != null)
            .ToArray();

        block.materials = materials;
        EditorUtility.SetDirty(block);
        AssetDatabase.SaveAssets();
        Debug.Log($"Set Materials: {soilDataPath}");





    }

}
