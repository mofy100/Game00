using UnityEngine;
using System;
using System.Collections.Generic;

public enum BlockType : byte{
    Soil    = 0b_0000_0000,
    Rock    = 0b_0000_0001,
    Brick   = 0b_0000_0010,
    Water   = 0b_0100_0000,
    Fense   = 0b_1000_0000,
    Stair   = 0b_1000_0001,
    Tree    = 0b_1000_1000,
    Empty   = 0b_1111_1111,
}

public enum SoilType : byte{
    Grass   = 0b_0000_0000,
    Barren  = 0b_0000_0001,
    Sand    = 0b_0000_0010,
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
    public static Block CreateBlock(BlockType type){
        Block b;
        if(IsCube(type)){
            b = new Block();
        }else if(IsObject(type)){
            if(IsFense(type)) b = new FenseBlock();
            else b = new ObjectBlock();
        }else{
            b = new Block();
        }
        b.blockType = type;
        return b;
    }

    public static bool IsCube(BlockType blockType){
        return ((byte)((byte)blockType & 0b_1011_0000) == 0b_0000_0000);
    }

    public static bool IsWall(BlockType blockType){
        return blockType == BlockType.Rock;
    }

    public static bool IsObject(BlockType blockType){
        return ((byte)((byte)blockType & 0b_1111_0000) == 0b_1000_0000);
    }

    public static bool IsFense(BlockType blockType){
        return blockType == BlockType.Fense;
    }

    public static bool IsEmpty(BlockType blockType){
        return blockType == BlockType.Empty;
    }

    public static bool IsSolid(BlockType blockType){
        return ((byte)((byte)blockType & 0b_1111_0000) == 0b_0000_0000);
    }

    public static bool IsLiquid(BlockType blockType){
        return blockType == BlockType.Water;
    }

    public static bool OutOfBounds(Vector3 position){
        return !((0 - Block.sizeV / 2.0f < position.y) && (position.y < (Chunk.sizeV - 1) * Block.sizeV));
    }
}