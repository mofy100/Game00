using System;
using UnityEngine;
using static BlockUtils;


public class Block
{
    public const float sizeH = 1.0f;
    public const float sizeV = 0.5f;

    public BlockType blockType;
    public byte blockSubType = 0;
    public Vector2Int chunkId;
    public Vector3Int localId;

    public virtual Mesh GetMesh(){return null;}
    public virtual byte GetBlockSubType(){return blockSubType;}
    public virtual void SetBlockSubType(byte subType){blockSubType = subType;}
    public virtual byte GetFenseNumber(){return 0;}
    public virtual void SetFenseShape(bool flag, byte index){}
    public virtual void SetFenseShape(byte number){}
    public virtual float GetAngle(){return 0;}
    public virtual float GetTopY(Vector2 position){return GetGlobalPosition().y + Block.sizeV / 2.0f;}
    public virtual void SetAngle(Direction2D angle){}
    public virtual void Update(){}

    public Vector3 GetGlobalPosition(){
        return new Vector3(chunkId.x * Chunk.sizeH, 0.0f, chunkId.y * Chunk.sizeH) + GetLocalPosition();
    }
    public Vector3 GetLocalPosition(){
        return new Vector3(localId.x * Block.sizeH, localId.y * Block.sizeV, localId.z * Block.sizeH);
    }
    public bool IsEmpty(){ return BlockUtils.IsEmpty(blockType); }
    public bool IsCube(){ return BlockUtils.IsCube(blockType); }
    public bool IsWall(){ return BlockUtils.IsWall(blockType); }
    public bool IsObject(){ return BlockUtils.IsObject(blockType); }
    public bool IsFense(){ return BlockUtils.IsFense(blockType); }
    public bool IsSolid(){ return BlockUtils.IsSolid(blockType); }
    public bool IsLiquid(){ return BlockUtils.IsLiquid(blockType); }
}

public class ObjectBlock : Block{
    public Direction2D angle; // (0, 1, 2, 3) = (0, 90, 180, 270)

    public override Mesh GetMesh(){
        Mesh mesh = BlockDataManager.blockDataBase[blockType].mesh;
        return mesh;
    }
    public override void SetAngle(Direction2D angle){
        this.angle = angle;
    }
    public override float GetAngle(){
        return (byte)angle * 90.0f;
    }
}

public class FenseBlock : ObjectBlock{
    public bool[] fenseShape = new bool[4]; // default(0,0,0,0) ~ X shape(1,1,1,1)

    public override Mesh GetMesh(){
        Mesh[] meshes = BlockDataManager.blockDataBase[blockType].meshes;
        byte fenseNumber = GetFenseNumber();
        if(fenseNumber == 0){ // (0,0,0,0) 0
            return meshes[0];
        }else if(fenseNumber == 15){ // (1,1,1,1) 15
            return meshes[5]; 
        }else if(fenseNumber % 3 == 0){ // (1,1,0,0) 3,6,9,12
            if(fenseNumber == 3) angle = Direction2D.Forward;
            else if(fenseNumber == 6) angle = Direction2D.Right;
            else if(fenseNumber == 12) angle = Direction2D.Back;
            else angle = Direction2D.Left;
            return meshes[2];
        }else if(fenseNumber == 7 || fenseNumber >= 11){ // (1,1,1,0) 7,11,13,14
            if(fenseShape[0] == false) angle = Direction2D.Back;
            else if(fenseShape[1] == false) angle = Direction2D.Left;
            else if(fenseShape[2] == false) angle = Direction2D.Forward;
            else angle = Direction2D.Right;
            return meshes[4];
        }else if(fenseNumber % 5 == 0){ // (1,0,1,0) 5,10
            angle = (Direction2D)((fenseNumber / 5) % 2);
            return meshes[3];
        }else{ // (1,0,0,0) 1,2,4,8
            if(fenseNumber == 8) angle = Direction2D.Left;
            else{
                angle = (Direction2D)(fenseNumber / 2);
            }
            return meshes[1];
        }
    }
    public override byte GetFenseNumber(){
        int number = (fenseShape[0] ? 1 : 0)
                    | ((fenseShape[1] ? 1 : 0) << 1)
                    | ((fenseShape[2] ? 1 : 0) << 2)
                    | ((fenseShape[3] ? 1 : 0) << 3);
        return (byte)number;
    }
    public override void SetFenseShape(bool flag, byte index){
        fenseShape[index] = flag;
    }
    public override void SetFenseShape(byte number){
        for(int i = 0; i < 4; i++){
            fenseShape[i] = ((number & (1 << i)) != 0);
        }
    }
}



