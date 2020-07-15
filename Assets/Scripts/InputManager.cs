using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Dictionary<KeyCode, Action> keyMappings = new Dictionary<KeyCode, Action>();
    public BoardManager board;
    public int delayAutoShift = 5; //in frames
    public int autoRepeatRate = 2; // in frames
    public Dictionary<Action, int> framesHeld = new Dictionary<Action, int>(); //only keep track of actions with DAS enabled
    void Awake()
    {
        SetDefaults();
        board = gameObject.GetComponent<BoardManager>();
    }

    void Update()
    {
        if (!board.activePiece.pause){
            foreach (KeyCode key in keyMappings.Keys){
                KeyUpdate(key);
            }
        }
    }

    void KeyUpdate(KeyCode key){
        /**Updates the state of key, activating its corresponding action if applicable.
        Uses DAS if needed. Assumes that key is actually mapped to an action.!--*/
        Action action = keyMappings[key];
        if (framesHeld.ContainsKey(action)){
            // apply DAS rules
            if (Input.GetKeyDown(key)){
                framesHeld[action] = 1; //pressed for one frame
                action();
                Debug.Log("activated " + action.ToString());
            } else if (Input.GetKey(key)){
                framesHeld[action] += 1; //held for one frame longer
                if ((framesHeld[action] >= delayAutoShift) && ((framesHeld[action] - delayAutoShift) % autoRepeatRate == 0)){
                    // after the action is held enough frames for DAS to engage, repeat every autoRepeatRate frames.
                    action();
                    Debug.Log("activated " + action.ToString() + "from DAS");
                }
            } else {
                // the key is not pressed, reset the count of frames held
                framesHeld[action] = 0;
            }
        } else {
            //no DAS, just regular check for GetKeyDown
            if (Input.GetKeyDown(key)){
                action();
                Debug.Log("activated " + action.ToString() + "from DAS");
            }
        }
    }

    void SetDefaults(){
        keyMappings = new Dictionary<KeyCode, Action>();
        keyMappings.Add(KeyCode.A, board.MovePieceLeft);
        keyMappings.Add(KeyCode.D, board.MovePieceRight);
        keyMappings.Add(KeyCode.S, board.SpawnNewPiece);
        keyMappings.Add(KeyCode.J, board.RotatePieceLeft);
        keyMappings.Add(KeyCode.L, board.RotatePieceRight);
        framesHeld = new Dictionary<Action, int>();
        framesHeld.Add(board.MovePieceLeft, 0);
        framesHeld.Add(board.MovePieceRight, 0);
    }
}
