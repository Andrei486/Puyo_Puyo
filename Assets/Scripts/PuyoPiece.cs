using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class PuyoPiece : MonoBehaviour
{
    public List<Block> blocks;
    public Vector2 pivot;
    public Vector2Int absolutePosition; //The board position represented by (0, 0) in local coordinates.
    public int framesUntilDrop = 0;
    public BoardManager board;

    void Start()
    {
        Color color = board.controller.GetRandomColor();
        Vector2Int blockPosition;
        foreach (Transform child in transform){
            blockPosition = Vector2Int.RoundToInt(child.localPosition);
            blocks.Add(new Block(color, blockPosition, child.gameObject));
        }
    }

    void LateUpdate()
    {
        if (framesUntilDrop == 0){
            //make piece fall
            framesUntilDrop = board.framesPerDrop;
            board.FallPiece();
        } else {
            framesUntilDrop--;
        }
    }

    public virtual void RotateCounterclockwise(){
        /**Rotates this piece counterclockwise by 90 degrees, or whatever alternate behavior
        is mapped to rotation for this piece, if possible.!--*/
        Vector2Int newPosition;
        Vector2Int movement;
        bool canRotate = true;
        foreach (Block block in blocks){
            //rotate relative to pivot
            newPosition = block.position.RotateAround(pivot, 90) + absolutePosition;
            if (!board.IsFree(newPosition)){
                canRotate = false;
                break; // if one block cannot be placed, stop there
            }
        }
        if (canRotate){
            foreach (Block block in blocks){
                movement = block.position.RotateAround(pivot, 90) - block.position;
                block.position = block.position.RotateAround(pivot, 90);
                block.Move(movement);
            }
        }
    }

    public virtual void RotateClockwise(){
        /**Rotates this piece clockwise by 90 degrees, or whatever alternate behavior
        is mapped to rotation for this piece, if possible.!--*/
        Vector2Int newPosition;
        Vector2Int movement;
        bool canRotate = true;
        foreach (Block block in blocks){
            //rotate relative to pivot
            newPosition = block.position.RotateAround(pivot, -90) + absolutePosition;
            if (!board.IsFree(newPosition)){
                canRotate = false;
                break; // if one block cannot be placed, stop there
            }
        }
        if (canRotate){
            foreach (Block block in blocks){
                movement = block.position.RotateAround(pivot, -90) - block.position;
                block.position = block.position.RotateAround(pivot, -90);
                block.Move(movement);
            }
        }
    }
}
