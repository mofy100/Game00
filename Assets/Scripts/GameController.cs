using UnityEngine;

public class GameController : MonoBehaviour {
    public bool editorMode;
    private GameObject editor;
    private GameObject player;

    void Awake(){
        // Select editor/player mode
        editor = GameObject.Find("WorldEditor");
        player = GameObject.Find("Player");
        if(editorMode){
            editor.SetActive(true);
            player.SetActive(false);
            editor.GetComponent<WorldEditor>().myCamera.gameObject.tag = "MainCamera";
        }else{
            editor.SetActive(false);
            player.SetActive(true);
            player.GetComponent<Player>().myCamera.gameObject.tag = "MainCamera";
        }

    }
}