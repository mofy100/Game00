using UnityEngine;
using System;
using System.Collections.Generic;

public enum BlockType : byte{
    Soil,
    Water,
    Rock,
    Fense,
    Tree,
    Empty,
}

/* BlockType 
Block
    Cube
        Soil
        Rock
        Water
    Object
        Fense
        Tree
    Empty
*/


public enum Direction : byte{
    Right,
    Left,
    Up,
    Down,
    Forward,
    Back
}

public enum Direction2D : byte{
    Forward,
    Right,
    Back,
    Left
}   // multiply 90.0f => rotationY

public static class BlockUtils{
    private static readonly Dictionary<BlockType, Func<Block>> blockFactory = new()
    {
        { BlockType.Soil, () => new SoilBlock() },
        { BlockType.Fense, () => new FenseBlock() },
        { BlockType.Tree, () => new ObjectBlock() },
    };

    public static Block CreateBlock(BlockType type){
        Block b = blockFactory.TryGetValue(type, out var creator) ? creator() : new Block();
        b.blockType = type;
        return b;
    }

    public static bool IsCube(BlockType blockType){
        return blockType == BlockType.Soil || 
               blockType == BlockType.Water ||
               blockType == BlockType.Rock;
    }

    public static bool IsSoil(BlockType blockType){
        return blockType == BlockType.Soil;
    }
    
    public static bool IsWall(BlockType blockType){
        return blockType == BlockType.Rock;
    }

    public static bool IsObject(BlockType blockType){
        return blockType == BlockType.Fense || 
               blockType == BlockType.Tree;
    }

    public static bool IsFense(BlockType blockType){
        return blockType == BlockType.Fense;
    }

    public static bool IsEmpty(BlockType blockType){
        return blockType == BlockType.Empty;
    }

    public static bool IsSolid(BlockType blockType){
        return blockType == BlockType.Soil ||
               blockType == BlockType.Rock;
    }

    public static bool IsLiquid(BlockType blockType){
        return blockType == BlockType.Water;
    }

    public static bool OutOfBounds(Vector3 position){
        return !((0 - Block.sizeV / 2.0f < position.y) && (position.y < (Chunk.sizeV - 1) * Block.sizeV));
    }
}