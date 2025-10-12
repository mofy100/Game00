using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BlockDataManager : MonoBehaviour{
    public static Dictionary<BlockType, BlockData> blockDataBase;

    void Awake(){
        blockDataBase = Resources.LoadAll<BlockData>("BlockData").ToDictionary(data => data.blockType);
    }

    public static BlockData GetBlockData(BlockType blockType){
        return blockDataBase[blockType];
    }

}
