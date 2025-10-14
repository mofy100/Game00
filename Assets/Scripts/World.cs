using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using static BlockUtils;

public partial class World : MonoBehaviour  // Other functions are written in "WorldUtils.cs"
{
    public Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

    private const int initialDrawRange = 2; // at the beginning of the game
    private const int drawRange = 3;
    private const int drawWidth = 2 * drawRange + 1;
    public Vector2Int[] drawingChunkIds;


    private GameObject user;
    private Player player;

    private bool creatingChunk = false;

    private GameController gameController;

    void Awake(){
        var gameController = GameObject.Find("GameController").GetComponent<GameController>();
        if(gameController.editorMode){
            user = GameObject.Find("WorldEditor");
        }else{
            user = GameObject.Find("Player");
        }
        player = user.GetComponent<Player>();

        drawingChunkIds = new Vector2Int[drawWidth * drawWidth];
        int k = 0;
        for(int i = -initialDrawRange; i <= initialDrawRange; i++){
            for(int j = -initialDrawRange; j <= initialDrawRange; j++){

                drawingChunkIds[k++] = new Vector2Int(i, j);

                // create the world
                Chunk chunk = new();
                chunk.chunkId = new Vector2Int(i, j);
                if(!WorldLoader.LoadChunk(chunk)){
                    chunk.InitializeBlocks();
                }
                ChunkMeshGenerator.UpdateMeshData(chunk);
                ChunkMeshGenerator.UpdateMesh(chunk);
                chunks[new Vector2Int(i, j)] = chunk;
            }
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        var userChunkId = GetChunkId(user.transform.position);
        int i = 0;
        for(int distance = 0; distance < drawRange * 2; distance++){
            for(int x = 0; x <= drawRange; x++){
                int y = distance - x;
                if(y < 0){
                    break;
                }else if(y == 0){
                    drawingChunkIds[i++] = new Vector2Int(userChunkId.x + x,
                                                          userChunkId.y);
                    if(x != 0){
                        drawingChunkIds[i++] = new Vector2Int(userChunkId.x - x,
                                                                userChunkId.y);
                    }
                }else if(y <= drawRange){
                    drawingChunkIds[i++] = new Vector2Int(userChunkId.x + x,
                                                          userChunkId.y + y);
                    drawingChunkIds[i++] = new Vector2Int(userChunkId.x + x,
                                                          userChunkId.y - y);
                    if(x != 0){
                        drawingChunkIds[i++] = new Vector2Int(userChunkId.x - x,
                                                            userChunkId.y + y);
                        drawingChunkIds[i++] = new Vector2Int(userChunkId.x - x,
                                                            userChunkId.y - y);
                    }
                }else{
                    continue;
                }
            }
        }
        /*
        for(int i = 0; i < drawWidth; i++){
            for(int j = 0; j < drawWidth; j++){
                drawingChunkIds[i * drawWidth + j] = new Vector2Int(userChunkId.x - drawRange + i,
                                                                    userChunkId.y - drawRange + j);
            }
        }
        */

        Draw();

    }

    int Mod(int a, int b){
        return (a % b + b) % b;
    }
    float Mod(float a, float b){
        return (a % b + b) % b;
    }
    int Divide(int a, int b){
        return (int)Mathf.FloorToInt((float)a / b);
    }
    int Divide(float a, int b){
        return (int)Mathf.FloorToInt(a / b);
    }
    int Divide(float a, float b){
        return (int)Mathf.FloorToInt(a / b);
    }

    async Task CreateChunk(Vector2Int chunkId){

        Chunk chunk = new();
        chunk.chunkId = chunkId;
        await Task.Run(() =>{
            if(!WorldLoader.LoadChunk(chunk)){
                chunk.InitializeBlocks();
            }
            ChunkMeshGenerator.UpdateMeshData(chunk);
        });
        ChunkMeshGenerator.UpdateMesh(chunk);
        chunks[chunkId] = chunk;
        creatingChunk = false;
    }

    public Hit ReleaseRay(Vector3 origin, Vector3 direction){
        Hit hit = new Hit{};
        Direction face;
        float maxDistance = 100.0f;
        Vector3 currentPos = origin;
        if(OutOfBounds(currentPos)){
            Debug.Log("ReleaseRay() : origin is out of bounds");
            return null;
        }
        Block currentBlock = GetBlock(origin);
        Vector3 currentBlockPosition = currentBlock.GetGlobalPosition();
        direction.Normalize();

        float dirX = (direction.x > 0) ? direction.x : -1 * direction.x;
        float dirY = (direction.y > 0) ? direction.y : -1 * direction.y;
        float dirZ = (direction.z > 0) ? direction.z : -1 * direction.z;

        Vector3 stepX = (direction.x > 0) ? Vector3.right * Block.sizeH : Vector3.left * Block.sizeH;
        Vector3 stepY = (direction.y > 0) ? Vector3.up * Block.sizeV : Vector3.down * Block.sizeV;
        Vector3 stepZ = (direction.z > 0) ? Vector3.forward * Block.sizeH : Vector3.back * Block.sizeH;

        // distance between currentPos and next Block face
        float distanceX = (direction.x > 0) ? 
                        (currentBlockPosition.x + Block.sizeH / 2 - origin.x) :
                        (origin.x - (currentBlockPosition.x - Block.sizeH / 2));
        float distanceY = (direction.y > 0) ? 
                        (currentBlockPosition.y + Block.sizeV / 2 - origin.y) :
                        (origin.y - (currentBlockPosition.y - Block.sizeV / 2));
        float distanceZ = (direction.z > 0) ? 
                        (currentBlockPosition.z + Block.sizeH / 2 - origin.z) :
                        (origin.z - (currentBlockPosition.z - Block.sizeH / 2));
        
        // move to the nearest face
        float nextX = (direction.x == 0) ? Mathf.Infinity : distanceX / dirX;
        float nextY = (direction.y == 0) ? Mathf.Infinity : distanceY / dirY;
        float nextZ = (direction.z == 0) ? Mathf.Infinity : distanceZ / dirZ;

        while(true){
            var chunkId = GetChunkId(currentPos);
            Vector3 localPos = currentPos - new Vector3(chunkId.x, 0.0f, chunkId.y) * Chunk.sizeH;

            // move along X-axis, Y-axis, or Z-axis
            if(nextX < nextY && nextX < nextZ){ 
                nextX += Block.sizeH / dirX;
                currentPos += stepX;
                maxDistance -= Block.sizeH / dirX;
                face = (direction.x < 0) ? Direction.Right : Direction.Left;
            }else if(nextY < nextZ){
                nextY += Block.sizeV / dirY;
                currentPos += stepY;
                maxDistance -= Block.sizeV / dirY;
                face = (direction.y < 0) ? Direction.Up : Direction.Down;
            }else{
                nextZ += Block.sizeH / dirZ;
                currentPos += stepZ;
                maxDistance -= Block.sizeH / dirZ;
                face = (direction.z < 0) ? Direction.Forward : Direction.Back;
            }

            currentBlock = GetBlock(currentPos);
            if(currentBlock == null){
                // Debug.Log("currentBlock is null");
                return null;
            }

            if(currentBlock.blockType != BlockType.Empty){
                hit.block = currentBlock;
                hit.face = face;
                Vector2Int spaceChunkId = currentBlock.chunkId;
                Vector3Int spaceLocalId = currentBlock.localId;
                if(face == Direction.Right){
                    spaceLocalId.x += 1;
                    if(spaceLocalId.x >= Chunk.sizeH){
                        spaceLocalId.x = 0;
                        spaceChunkId.x += 1;
                    }
                }else if(face == Direction.Left){
                    spaceLocalId.x -= 1;
                    if(spaceLocalId.x < 0){
                        spaceLocalId.x = Chunk.sizeH - 1;
                        spaceChunkId.x -= 1;
                    }
                }else if(face == Direction.Up){
                    spaceLocalId.y += 1;
                }else if(face == Direction.Down){
                    spaceLocalId.y -= 1;
                }else if(face == Direction.Forward){
                    spaceLocalId.z += 1;
                    if(spaceLocalId.z >= Chunk.sizeH){
                        spaceLocalId.z = 0;
                        spaceChunkId.y += 1;
                    }
                }else if(face == Direction.Back){
                    spaceLocalId.z -= 1;
                    if(spaceLocalId.z < 0){
                        spaceLocalId.z = Chunk.sizeH - 1;
                        spaceChunkId.y -= 1;
                    }
                }

                hit.space = chunks[spaceChunkId].blocks[spaceLocalId.x, spaceLocalId.y, spaceLocalId.z];
                return hit;

            }else if(maxDistance < 0){
                return null;
            }
        }
    }

    public void AddBlock(Block oldBlock, BlockType blockType, byte blockLevel, Direction2D angle = Direction2D.Forward){
        Vector2Int chunkId = oldBlock.chunkId;
        Vector3Int localId = oldBlock.localId;
        Block block = CreateBlock(blockType);
        block.blockType = blockType;
        block.chunkId = chunkId;
        block.localId = localId;
        chunks[chunkId].SetBlock(block, localId);

        Debug.Log($"AddBlock() : blockType = {blockType}");

        if(IsCube(blockType)){
            block.SetBlockLevel(blockLevel);
            ChunkMeshGenerator.UpdateMeshData(chunks[chunkId]);
            ChunkMeshGenerator.UpdateMesh(chunks[chunkId]);
        }
        else if(IsObject(blockType)){
            chunks[chunkId].objects[localId] = blockType;
            block.SetAngle(angle);
            if(IsFense(blockType)){
                AddFenseBlock((FenseBlock)block);
            }
        }

        chunks[chunkId].modified = true;

    }

    public void DeleteBlock(Block block){
        Vector2Int chunkId = block.chunkId;
        BlockType blockType = block.blockType;
        if(IsCube(blockType)){
            block.blockType = BlockType.Empty;
            ChunkMeshGenerator.UpdateMeshData(chunks[chunkId]);
            ChunkMeshGenerator.UpdateMesh(chunks[chunkId]);
        }else if(IsObject(blockType)){
            if(IsFense(blockType)){
                DeleteFenseBlock((FenseBlock)block);
            }
            block.blockType = BlockType.Empty;
            chunks[chunkId].objects.Remove(block.localId);
        }
        chunks[chunkId].modified = true;

    }

    void AddFenseBlock(FenseBlock block){
        Direction2D[] directions = (Direction2D[])Enum.GetValues(typeof(Direction2D));

        if(!block.IsFense()){
            Debug.Log("This block is not a fense");
                return;
        }

        Vector2Int chunkId = block.chunkId;
        Vector3Int localId = block.localId;

        foreach(Direction2D direction in directions){
            Vector3Int nbrId;
            
            if(direction == Direction2D.Forward){
                nbrId = localId  + new Vector3Int(0, 0, 1);
            }else if(direction == Direction2D.Right){
                nbrId = localId  + new Vector3Int(1, 0, 0);
            }else if(direction == Direction2D.Back){
                nbrId = localId  + new Vector3Int(0, 0, -1);
            }else{
                nbrId = localId  + new Vector3Int(-1, 0, 0);
            }
            Block nbr = GetBlock(chunkId, nbrId);
            if(!nbr.IsEmpty()){
                // block.fenseShape[(byte)direction] = true;
                block.SetFenseShape(true, (byte)direction);
                if(nbr.IsFense()){
                    int oppositeDirection = ((byte)direction ^ 0b_00000010);
                    // nbr.fenseShape[oppositeDirection] = true;
                    nbr.SetFenseShape(true, (byte)oppositeDirection);
                    // nbr.SetFenseShape((byte)(nbr.GetFenseNumber() | 1 << oppositeDirection));

                }
            }else{
                // block.fenseShape[(byte)direction] = false;
                block.SetFenseShape(false, (byte)direction);
            }
        }
    }

    void DeleteFenseBlock(FenseBlock block){
        Direction2D[] directions = (Direction2D[])Enum.GetValues(typeof(Direction2D));

        if(!block.IsFense()){
            Debug.Log("This block is not a fense");
                return;
        }

        Vector2Int chunkId = block.chunkId;
        Vector3Int localId = block.localId;

        foreach(Direction2D direction in directions){
            Vector3Int nbrId;
            
            if(direction == Direction2D.Forward){
                nbrId = localId  + new Vector3Int(0, 0, 1);
            }else if(direction == Direction2D.Right){
                nbrId = localId  + new Vector3Int(1, 0, 0);
            }else if(direction == Direction2D.Back){
                nbrId = localId  + new Vector3Int(0, 0, -1);
            }else{
                nbrId = localId  + new Vector3Int(-1, 0, 0);
            }
            Block nbr = GetBlock(chunkId, nbrId);
            if(nbr.IsFense()){
                // int oppositeDirection = ((int)direction + 2) % 4;
                // nbr.SetFenseShape((byte)(nbr.GetFenseNumber() & 0b_11111110 << oppositeDirection));
                int oppositeDirection = ((byte)direction ^ 0b_00000010);
                nbr.SetFenseShape(false, (byte)oppositeDirection);
            }
        }
    }
}

public class Hit{
    public Block block;
    public Direction face;
    public Block space;
}
