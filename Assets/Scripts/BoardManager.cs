using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class BoardManager : MonoBehaviour
{
    public PuyoPiece activePiece;
    public Dictionary<KeyCode, Action> keyMappings = new Dictionary<KeyCode, Action>();
    public int framesPerDrop = 5;
    public GameController controller;
    public InputManager input;
    private Block[][] grid;
    public int spawnColumn = 2; //leftmost column is 0

    void Start()
    {
        controller = GameController.GetController();
        input = gameObject.GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Input-activated commands
    public void FallPiece(){

    }

    public void MovePieceLeft(){
        /**Moves the active piece left by one column if possible.!--*/
        Vector2Int newPosition;
        Vector2Int oldPosition;
        bool canMove = true;
        foreach (Block block in activePiece.blocks){
            newPosition = block.position + Vector2Int.left;
            if (!IsFree(newPosition)){
                canMove = false;
                break;
            }
        }
        if (canMove){
            foreach (Block block in activePiece.blocks){
                oldPosition = block.position;
                block.Move(Vector2Int.left);
                grid[oldPosition.x][oldPosition.y] = null;
                grid[block.position.x][block.position.y] = block;
            }
        }
    }
    public void MovePieceRight(){
        /**Moves the active piece right by one column if possible.!--*/
        Vector2Int newPosition;
        Vector2Int oldPosition;
        bool canMove = true;
        foreach (Block block in activePiece.blocks){
            newPosition = block.position + Vector2Int.right;
            if (!IsFree(newPosition)){
                canMove = false;
                break;
            }
        }
        if (canMove){
            foreach (Block block in activePiece.blocks){
                oldPosition = block.position;
                block.Move(Vector2Int.right);
                grid[oldPosition.x][oldPosition.y] = null;
                grid[block.position.x][block.position.y] = block;
            }
        }
    }

    public void RotatePieceLeft(){
        /**Rotates the active piece left (counterclockwise) by 90 degrees.!--*/
        activePiece.RotateCounterclockwise();
    }

    public void RotatePieceRight(){
        /**Rotates the active piece right (clockwise) by 90 degrees.!--*/
        activePiece.RotateClockwise();
    }

    public Block GetBlock(Vector2Int position){
        /**Returns the Block at the position (x, y).!--*/
        return grid[position.x][position.y];
    }

    public bool IsWithinBounds(Vector2Int position){
        /**Returns true if and only if the (x, y) position specified by
        the vector argument is a valid space for the board grid.!--*/
        if (position.x < 0 || position.x > controller.boardDimensions.x - 1){
            //out of bounds in x
            return false;
        }
        if (position.y < 0 || position.y > controller.boardDimensions.y - 1){
            //out of bounds in y
            return false;
        }
        return true;
    }

    public bool IsFree(Vector2Int position){
        /**Returns true if and only if a block can be placed at the (x, y) position
        specified by the vector argument.!--*/
        return (IsWithinBounds(position) && (GetBlock(position) == null));
    }

    public void CreateNewPiece(){
        /**Creates a new Puyo piece.!--*/
    }

    public void LockPiece(){
        Vector2Int actualPos;
        foreach (Block block in activePiece.blocks){
            actualPos = block.position + activePiece.absolutePosition;
            if (!IsFree(actualPos)){
                Debug.Log("Tried to lock a piece in place over another.");
            } else {
                grid[actualPos.x][actualPos.y] = block;
                block.position = actualPos;
            }
        }
        Destroy(activePiece);

    }

    public bool GroundBlock(Vector2Int startPosition){
        /**Moves the block at position as far down as it can until it hits
        another block or the bottom of the grid. Returns true if and only if the
        block was moved at least once.!--*/
        Vector2Int position = startPosition;
        bool moved = false;
        Block block = GetBlock(position);
        // Safeguard in case of empty space.
        if (block == null){
            return false;
        }
        while (IsFree(position + Vector2Int.down)){
            position += Vector2Int.down;
            moved = true;
        }
        // Move the block's sprite.
        block.Move(position - startPosition);
        // Remove the original block, and add it back at its new position
        grid[position.x][position.y] = block;
        grid[startPosition.x][startPosition.y] = null;
        return moved;
    }

    public void GroundAllBlocks(){
        /**Grounds all blocks on the grid, until none can move down any further.!--*/
        for (int x=0; x < controller.boardDimensions.x; x++){
            // Start with the lowest blocks, so that the higher ones can move into those spaces if needed.
            for (int y=0; y < controller.boardDimensions.y; y++){
                if (GetBlock(new Vector2Int(x, y)) != null){
                    GroundBlock(new Vector2Int(x, y));
                }
            }
        }
    }
}
