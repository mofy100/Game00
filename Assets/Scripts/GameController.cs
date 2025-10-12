using UnityEngine;

public class GameController : MonoBehaviour {
    public bool editorMode;
    private GameObject editor;
    private GameObject player;
    private GameObject editorCanvas;

    void Awake(){
        // Select editor/player mode
        editor = GameObject.Find("WorldEditor");
        player = GameObject.Find("Player");
        editorCanvas = GameObject.Find("EditorCanvas");
        if(editorMode){
            editor.SetActive(true);
            editorCanvas.SetActive(true);
            player.SetActive(false);
            editor.GetComponent<WorldEditor>().myCamera.gameObject.tag = "MainCamera";
        }else{
            editor.SetActive(false);
            editorCanvas.SetActive(false);
            player.SetActive(true);
            player.GetComponent<Player>().myCamera.gameObject.tag = "MainCamera";
        }

    }
}