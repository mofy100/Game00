using UnityEngine;
using System;
using System.Collections.Generic;

public enum BlockType : byte{
    Soil,
    Fense,
    Empty,
}

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
    };

    public static Block CreateBlock(BlockType type){
        return blockFactory.TryGetValue(type, out var creator) ? creator() : new Block();
    }

    public static bool IsCube(BlockType blockType){
        byte id = (byte)blockType;
        return (id == 0);
    }
    
    public static bool IsWall(BlockType blockType){
        byte id = (byte)blockType;
        return false;
    }

    public static bool IsObject(BlockType blockType){
        return blockType == BlockType.Fense;
    }

    public static bool IsFense(BlockType blockType){
        return blockType == BlockType.Fense;
    }

    public static bool IsEmpty(BlockType blockType){
        return blockType == BlockType.Empty;
    }

    public static bool OutOfBounds(Vector3 position){
        return !((0 - Block.sizeV / 2.0f < position.y) && (position.y < (Chunk.sizeV - 1) * Block.sizeV));
    }
}