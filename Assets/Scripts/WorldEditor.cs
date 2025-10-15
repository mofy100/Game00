using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WorldEditor: MonoBehaviour
{
    // private MyInputAction controll;
    private Vector2 moveDirection;
    private InputAction moveAction, moveupAction, movedownAction, rotateAction, addAction, delAction, selectBlockAction;
    private World world;

    private bool adding;
    private bool deleting;
    private float nextActionTime;
    private float warmupTime = 0.2f;
    private float intervalTime = 0.1f;

    public BlockType holdingBlockType;
    private byte holdingBlockTypeIndex;
    public byte holdingBlockSubType;
    public TextMeshProUGUI holdingBlockText;
    BlockType[] blockTypes;

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float cameraHeight;
    [SerializeField] float minRotationX, maxRotationX;
    [SerializeField] GameObject target;
    public Camera myCamera;

    void Awake(){
        // Input Setting
        var editorActions = inputActions.FindActionMap("Editor");
        moveAction = editorActions.FindAction("Move");
        moveupAction = editorActions.FindAction("MoveUp");
        movedownAction = editorActions.FindAction("MoveDown");
        rotateAction = editorActions.FindAction("Rotate");
        addAction = editorActions.FindAction("Add");
        delAction = editorActions.FindAction("Del");
        selectBlockAction = editorActions.FindAction("SelectBlock");

        // UI Setting
        holdingBlockText.text = holdingBlockType.ToString();

        world = GameObject.Find("World").GetComponent<World>();
        adding = false;
        deleting = false;
        nextActionTime = Mathf.Infinity;

        blockTypes = (BlockType[])Enum.GetValues(typeof(BlockType));
        holdingBlockTypeIndex = 0;
        holdingBlockType = blockTypes[holdingBlockTypeIndex];
    }

    void Start(){
    }

    void Update()
    {
        Vector2 moveDirection = moveAction.ReadValue<Vector2>();
        if(moveDirection != Vector2.zero){
            Move(moveDirection);
        }
        float moveupValue = moveupAction.ReadValue<float>() - movedownAction.ReadValue<float>();
        if(moveupValue != 0){
            Moveup(moveupValue);
        }
        Vector2 rotateDirection = rotateAction.ReadValue<Vector2>();
        if(rotateDirection != Vector2.zero){
            Rotate(rotateDirection);
        }

        Hit hit = world.ReleaseRay(myCamera.transform.position, myCamera.transform.forward);
        if(hit != null){
            target.transform.position = hit.space.GetGlobalPosition();
        }

        if(nextActionTime < Time.time){
            if(adding){
                ExecAdd();
                nextActionTime += intervalTime;
            }else if(deleting){
                ExecDel();
                nextActionTime += intervalTime;
            }
        }

    }

    void OnEnable(){
        moveAction.Enable();
        moveupAction.Enable();
        movedownAction.Enable();
        rotateAction.Enable();
        addAction.Enable();
        addAction.performed += Add;
        addAction.canceled += StopAdd;
        delAction.Enable();
        delAction.performed += Del;
        delAction.canceled += StopDel;
        selectBlockAction.Enable();
        selectBlockAction.performed += SelectBlock;
    }
    void OnDisable(){
        moveAction.Disable();
        moveupAction.Disable();
        movedownAction.Disable();
        rotateAction.Disable();
        addAction.performed -= Add;
        addAction.canceled -= StopAdd;
        addAction.Disable();
        delAction.performed -= Del;
        delAction.canceled -= StopDel;
        delAction.Disable();
        selectBlockAction.Disable();
        selectBlockAction.performed -= SelectBlock;
    }

    void Move(Vector2 direction){
        float rad = -1.0f * transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        direction = new Vector2(direction.x * cos - direction.y * sin,
                                direction.x * sin + direction.y * cos);
        Vector3 delta = new Vector3(direction.x, 0.0f, direction.y) * moveSpeed * Time.deltaTime;
        transform.position += delta;
    }

    void Moveup(float value){
        transform.position += Vector3.up * value * moveSpeed * Time.deltaTime;
    }

    void Rotate(Vector2 direction){
        float angleX = direction.y * rotateSpeed * Time.deltaTime * -1.0f;
        float angleY = direction.x * rotateSpeed * Time.deltaTime;

        transform.Rotate(angleX, 0, 0);
        transform.Rotate(0, angleY, 0, Space.World);
    }

    void Add(InputAction.CallbackContext context){
        ExecAdd();
        adding = true;
        nextActionTime = Time.time + warmupTime;
    }
    void StopAdd(InputAction.CallbackContext context){
        adding = false;
        nextActionTime = Mathf.Infinity;
    }

    void ExecAdd(){
        Hit hit = world.ReleaseRay(myCamera.transform.position,myCamera.transform.forward);
        if(hit != null){
            world.AddBlock(hit.space, holdingBlockType, holdingBlockSubType, this.GetDirection2D());
        }
    }

    void Del(InputAction.CallbackContext context){
        ExecDel();
        deleting = true;
        nextActionTime = Time.time + warmupTime;
    }
    void StopDel(InputAction.CallbackContext context){
        deleting = false;
        nextActionTime = Mathf.Infinity;
    }

    void ExecDel(){
        Hit hit = world.ReleaseRay(myCamera.transform.position,myCamera.transform.forward);
        if(hit != null){
            world.DeleteBlock(hit.block);
        }
    }

    void SelectBlock(InputAction.CallbackContext context){

        holdingBlockTypeIndex++;
        if(holdingBlockTypeIndex == blockTypes.Length){
            holdingBlockTypeIndex = 0;
        }
        holdingBlockType = blockTypes[holdingBlockTypeIndex];
        holdingBlockText.text = holdingBlockType.ToString();
    }

    Direction2D GetDirection2D(){
        float angle = transform.rotation.eulerAngles.y; // 0 ~ 360
        return (Direction2D)(Mathf.Round(angle / 90.0f) % 4); // 0, 90, 180, 270 
    }
}

