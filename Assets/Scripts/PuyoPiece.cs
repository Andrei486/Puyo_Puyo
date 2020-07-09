using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class PuyoPiece : MonoBehaviour
{
    public List<PuyoBlock> blocks;
    public Vector2 pivot;
    public Vector2Int absolutePosition; //The board position represented by (0, 0) in local coordinates.
    public int framesUntilDrop = 0;
    public BoardManager board;

    protected virtual void Start()
    {
        int color = board.controller.GetRandomColorIndex();
        blocks = new List<PuyoBlock>();
        foreach (Transform child in transform){
            child.GetComponent<PuyoBlock>().SetColor(color);
            blocks.Add(child.GetComponent<PuyoBlock>());
        }
        framesUntilDrop = board.framesPerDrop;
        int height = 0;
        foreach (PuyoBlock block in blocks){
            if (block.position.y > height){
                height = block.position.y;
            }
        }
        // highest row is dimensions.y - 1, subtract piece's height to obtain spawn point
        absolutePosition = new Vector2Int(board.spawnColumn, board.controller.boardDimensions.y - 1 - height);
        // set position of visual representation.
        transform.position = board.anchorPosition;
        transform.position += new Vector3(absolutePosition.x * board.controller.spaceDimensions.x,
                                         absolutePosition.y * board.controller.spaceDimensions.y,
                                         0);
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
        foreach (PuyoBlock block in blocks){
            //rotate relative to pivot
            newPosition = block.position.RotateAround(pivot, 90) + absolutePosition;
            Debug.Log("rotated from " + block.position + " to " + block.position.RotateAround(pivot, 90));
            if (!board.IsFree(newPosition)){
                canRotate = false;
                break; // if one block cannot be placed, stop there
            }
        }
        if (canRotate){
            foreach (PuyoBlock block in blocks){
                movement = block.position.RotateAround(pivot, 90) - block.position;
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
        foreach (PuyoBlock block in blocks){
            //rotate relative to pivot
            newPosition = block.position.RotateAround(pivot, -90) + absolutePosition;
            Debug.Log("rotated from " + block.position + " to " + block.position.RotateAround(pivot, -90));
            if (!board.IsFree(newPosition)){
                canRotate = false;
                break; // if one block cannot be placed, stop there
            }
        }
        if (canRotate){
            foreach (PuyoBlock block in blocks){
                movement = block.position.RotateAround(pivot, -90) - block.position;
                block.Move(movement);
            }
        }
    }

    public bool Move(Vector2Int movement){
        /**Moves this piece and its representation by movement along the board's grid,
        if such a movement is possible. Returns true if and only if movement was successful.!--*/
        Vector2Int newPosition;
        bool canMove = true;
        foreach (PuyoBlock block in blocks){
            newPosition = block.position + absolutePosition + movement;
            // Debug.Log("block: " + newPosition);
            if (!board.IsFree(newPosition)){
                canMove = false;
                break;
            }
        }
        if (canMove){
            absolutePosition += movement;
            // Debug.Log("piece: " + absolutePosition);
            transform.Translate(Vector2.Scale(movement, GameController.GetController().spaceDimensions), Space.Self);
        }
        return canMove;
    }
}
