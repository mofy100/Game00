using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviour
{
    // private MyInputAction controll;
    private Vector2 moveDirection;
    private InputAction moveAction, rotateAction, jumpAction, nearAction, farAction;
    private World world;

    private Vector2Int chunkId;
    private Vector3Int localId;
    private Vector3 localPos;
    private Block currentBlock;

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float nearSpeed;
    [SerializeField] float cameraHeight;
    [SerializeField] float minRotationX, maxRotationX;
    [SerializeField] GameObject character;
    private Animator characterAnimator;

    private const float nearestDistance = 3.0f;
    private const float farthestDistance = 1000.0f;

    // Capsule Collider
    private const float colliderHeight = 0.25f;
    private const float colliderRadius = 0.25f;
    
    private bool isJumping = false;
    private float velocityY = 0.0f;
    private float gravity = 9.8f;

    public Camera myCamera;

    void Awake(){
        var playerActions = inputActions.FindActionMap("Player");
        moveAction = playerActions.FindAction("Move");
        rotateAction = playerActions.FindAction("Rotate");
        jumpAction = playerActions.FindAction("Jump");
        nearAction = playerActions.FindAction("CameraNear");
        farAction = playerActions.FindAction("CameraFar");

        world = GameObject.Find("World").GetComponent<World>();

        characterAnimator = character.GetComponent<Animator>();
    }

    void Start(){
        transform.position = new Vector3(transform.position.x, 10.0f, transform.position.z);
    }

    void Update()
    {
        Vector2 moveDirection = moveAction.ReadValue<Vector2>();
        if(moveDirection != Vector2.zero){
            Move(moveDirection);
            characterAnimator.SetBool("moving", true);
        }else{
            characterAnimator.SetBool("moving", false);
        }

        bool jump = (jumpAction.ReadValue<float>() >= 0.1f);
        if(jump){
            if(!isJumping){
                isJumping = true;
                velocityY = 4.0f;
                characterAnimator.SetTrigger("jump");
            }
        }

        transform.position = SolveCollisionXZ(transform.position);

        transform.position += Vector3.up * velocityY * Time.deltaTime;
        if(isJumping){
            velocityY -= gravity * Time.deltaTime;
        }else{
            velocityY = 0.0f;
        }

        Debug.Log($"jumping {isJumping} velocity {velocityY}");

        transform.position = SolveCollisionY(transform.position);
        // transform.position = SolveCollision(transform.position);

        Vector2 rotateDirection = rotateAction.ReadValue<Vector2>();
        if(rotateDirection != Vector2.zero){
            Rotate(rotateDirection);
        }

        float nearValue = nearAction.ReadValue<float>() - farAction.ReadValue<float>();
        if(nearValue != 0){
            MoveNear(nearValue);
        }

    }

    void OnEnable(){
        moveAction.Enable();
        rotateAction.Enable();
        jumpAction.Enable();
        nearAction.Enable();
        farAction.Enable();
    }
    void OnDisable(){
        moveAction.Disable();
        rotateAction.Disable();
        jumpAction.Disable();
        nearAction.Disable();
        farAction.Disable();
    }

    void Move(Vector2 direction){
        float rad = -1.0f * transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        Vector3 currentPosition = transform.position;
        direction = new Vector2(direction.x * cos - direction.y * sin,
                                direction.x * sin + direction.y * cos);
        // Vector2 delta = GetMoveDistance(currentPosition, direction * moveSpeed * Time.deltaTime);
        Vector2 delta = direction * moveSpeed * Time.deltaTime;

        currentPosition.x += delta.x;
        currentPosition.z += delta.y;

        transform.position = currentPosition;

        // rotate character mesh
        Vector3 dir = new Vector3(direction.x, 0.0f, direction.y);
        character.transform.rotation = Quaternion.LookRotation(dir);
    }

    void Rotate(Vector2 direction){
        float angleX = direction.y * rotateSpeed * Time.deltaTime * -1.0f;
        float angleY = direction.x * rotateSpeed * Time.deltaTime;

        myCamera.transform.RotateAround(this.transform.position, myCamera.transform.right, angleX);
        transform.Rotate(0, angleY, 0, Space.World);
        character.transform.Rotate(0, -angleY, 0, Space.World);
    }

    void Jump(InputAction.CallbackContext context){
    }


    void MoveNear(float value){
        Vector3 offset = myCamera.transform.position - this.transform.position;
        if((offset.magnitude < nearestDistance && value < 0) || (farthestDistance < offset.magnitude && value > 0)) return;
        offset += value * offset * nearSpeed * Time.deltaTime;
        myCamera.transform.position = this.transform.position + offset;
    }

    Vector3 SolveCollisionXZ(Vector3 position){

        Vector3 newPosition = position;

        Vector2Int chunkId = world.GetChunkId(position);
        Vector3Int localId = world.GetLocalId(position);
        Vector3 localPos = world.GetLocalPos(position);

        int[] dxs = new int[] {-1, 0, 1};
        int[] dzs = new int[] {-1, 0, 1};
        Block block;

        for(int i = 0; i < 3; i++){
            int dx = dxs[i];
            for(int j = 0; j < 3; j++){
                int dz = dzs[j];

                // block = world.GetBlock(chunkId, new Vector3Int(localId.x + dx, localId.y, localId.z + dz));
                block = world.GetBlock(newPosition + new Vector3(dx, -colliderHeight + 0.01f, dz));
                if(block != null){
                    if(block.IsSolid()){
                        Vector2 back2D = Collision(new Vector2(localPos.x, localPos.z), new Vector2(localId.x + dx, localId.z + dz), colliderRadius);
                        newPosition -= new Vector3(back2D.x, 0.0f, back2D.y);
                        localPos = world.GetLocalPos(newPosition);
                    }
                }
            }
        }
        return newPosition;
    }

    Vector3 SolveCollisionY(Vector3 position){
        Vector3 newPosition = position;
        Vector2Int chunkId = world.GetChunkId(position);
        Vector3Int localId = world.GetLocalId(position);
        Vector3 localPos = world.GetLocalPos(position);

        int[] dxs = new int[] {-1, 0, 1};
        int[] dzs = new int[] {-1, 0, 1};
        Block block;

        for(int i = 0; i < 3; i++){
            int dx = dxs[i];
            for(int j = 0; j < 3; j++){
                int dz = dzs[j];

                block = world.GetBlock(chunkId, new Vector3Int(localId.x + dx, localId.y - 1, localId.z + dz));
                if(block != null){
                    if(block.IsSolid()){
                        if(newPosition.y - block.GetGlobalPosition().y < colliderHeight + Block.sizeV / 2.0f){
                            Vector2 back = Collision(new Vector2(localPos.x, localPos.z), new Vector2(localId.x + dx, localId.z + dz), colliderRadius);
                            Debug.Log($"localId {localId} block {block.blockType} back {back}");
                            if(back != Vector2.zero || ((dx == 0) && (dz == 0))){
                                newPosition.y = block.GetGlobalPosition().y + Block.sizeV / 2.0f + colliderHeight;
                                isJumping = false;
                                return newPosition;
                            }
                        }
                    }
                }
            }
        }
        isJumping = true;

        return newPosition;

    }


    Vector3 SolveCollision(Vector3 position){

        Vector3 newPosition = position;

        Vector2Int chunkId = world.GetChunkId(position);
        Vector3Int localId = world.GetLocalId(position);
        Vector3 localPos = world.GetLocalPos(position);

        int[] dxs = new int[] {-1, 0, 1};
        int[] dzs = new int[] {-1, 0, 1};
        Block block;

        for(int i = 0; i < 3; i++){
            int dx = dxs[i];
            for(int j = 0; j < 3; j++){
                int dz = dzs[j];

                // block = world.GetBlock(chunkId, new Vector3Int(localId.x + dx, localId.y, localId.z + dz));
                block = world.GetBlock(newPosition + new Vector3(dx, -colliderHeight + 0.01f, dz));
                if(block != null){
                    if(block.IsSolid()){
                        Vector2 back2D = Collision(new Vector2(localPos.x, localPos.z), new Vector2(localId.x + dx, localId.z + dz), colliderRadius);
                        newPosition -= new Vector3(back2D.x, 0.0f, back2D.y);
                        localPos = world.GetLocalPos(newPosition);
                    }
                }
            }
        }

        for(int i = 0; i < 3; i++){
            int dx = dxs[i];
            for(int j = 0; j < 3; j++){
                int dz = dzs[j];

                block = world.GetBlock(chunkId, new Vector3Int(localId.x + dx, localId.y - 1, localId.z + dz));
                if(block != null){
                    if(block.IsSolid()){
                        if(newPosition.y - block.GetGlobalPosition().y < colliderHeight + Block.sizeV / 2.0f){
                            Vector2 back = Collision(new Vector2(localPos.x, localPos.z), new Vector2(localId.x + dx, localId.z + dz), colliderRadius);
                            Debug.Log($"localId {localId} block {block.blockType} back {back}");
                            if(back != Vector2.zero || ((dx == 0) && (dz == 0))){
                                newPosition.y = block.GetGlobalPosition().y + Block.sizeV / 2.0f + colliderHeight;
                                isJumping = false;
                                return newPosition;
                            }
                        }
                    }
                }
            }
        }
        isJumping = true;

        return newPosition;

    }

    Vector2 Collision(Vector2 circle, Vector2 square, float radius){
        float closeX = Mathf.Clamp(circle.x, square.x - 0.5f, square.x + 0.5f);
        float closeY = Mathf.Clamp(circle.y, square.y - 0.5f, square.y + 0.5f);
        float dx = circle.x - closeX;
        float dy = circle.y - closeY;
        // Debug.Log($"dx {dx} dy {dy} closeX {closeX} closeY {closeY} circle {circle} square {square}");
        if(dx * dx + dy * dy <= radius * radius - 0.01f){
            float overlap = radius - Mathf.Sqrt(dx * dx + dy * dy);
            if(overlap > 0.0f){
                return new Vector2(dx, dy).normalized * -1.0f * overlap;
            }
        }
        return Vector2.zero;
    }
}

