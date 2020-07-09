using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class BoardManager : MonoBehaviour
{
    public PuyoPiece activePiece;
    public Dictionary<KeyCode, Action> keyMappings = new Dictionary<KeyCode, Action>();
    public int framesPerDrop = 60;
    public GameController controller;
    public InputManager input;
    private PuyoBlock[,] grid;
    public int spawnColumn = 2; //leftmost column is 0
    public Vector3 anchorPosition; //the world-space position of the bottom-left corner of the board.
    private List<GameObject> background;
    private static Vector2Int[] ADJACENCY_DIRECTIONS = new Vector2Int[]{Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};

    void Awake()
    {
        controller = GameController.GetController();
        input = gameObject.GetComponent<InputManager>();
        grid = new PuyoBlock[controller.boardDimensions.x, controller.boardDimensions.y];
        SetupBackground(controller.backgroundTile);
        SpawnNewPiece();
    }
    void Update()
    {
        
    }

    // Input-activated commands
    public void MovePieceLeft(){
        /**Moves the active piece left by one column if possible.!--*/
        activePiece.Move(Vector2Int.left);
    }
    public void MovePieceRight(){
        /**Moves the active piece right by one column if possible.!--*/
        activePiece.Move(Vector2Int.right);
    }

    public void RotatePieceLeft(){
        /**Rotates the active piece left (counterclockwise) by 90 degrees.!--*/
        activePiece.RotateCounterclockwise();
    }

    public void RotatePieceRight(){
        /**Rotates the active piece right (clockwise) by 90 degrees.!--*/
        activePiece.RotateClockwise();
    }

    public PuyoBlock GetBlock(Vector2Int position){
        /**Returns the PuyoBlock at the position (x, y).!--*/
        return grid[position.x, position.y];
    }

    public bool IsWithinBounds(Vector2Int pos){
        /**Returns true if and only if the (x, y) position specified by
        the vector argument is a valid space for the board grid.!--*/
        if (pos.x < 0 || (pos.x > (controller.boardDimensions.x - 1))){
            //out of bounds in x
            return false;
        }
        if (pos.y < 0 || (pos.y > (controller.boardDimensions.y - 1))){
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

    public bool IsFull(Vector2Int position){
        /**Returns true if and only if a block already exists at the (x, y) position
        specified by the vector argument.!--*/
        return (IsWithinBounds(position) && (GetBlock(position) != null));
    }

    public void SpawnNewPiece(){
        /**Creates a new Puyo piece.!--*/
        if (activePiece != null){
            LockPiece(); //first lock and destroy the old one if it exists
        }
        GroundAllBlocks(); //ground everything
        ResolveChains(controller.minChainSize);
        activePiece = controller.GetRandomPiece().GetComponent<PuyoPiece>();
        activePiece.board = this;
        activePiece.transform.parent = transform;
    }

    public void FallPiece(){
        /**Moves the active piece down by one column if possible. If not,
        lock the piece in place and check for chains.!--*/
        if (!activePiece.Move(Vector2Int.down)) {
            SpawnNewPiece();
        }
    }

    public void LockPiece(){
        Vector2Int actualPos;
        foreach (PuyoBlock block in activePiece.blocks){
            actualPos = block.position + activePiece.absolutePosition;
            if (!IsFree(actualPos)){
                Debug.Log("Tried to lock a piece in place over another.");
            } else {
                grid[actualPos.x, actualPos.y] = block;
                block.position = actualPos;
            }
        }
        activePiece.transform.DetachChildren();
        Destroy(activePiece);
    }

    public bool GroundBlock(Vector2Int startPosition){
        /**Moves the block at position as far down as it can until it hits
        another block or the bottom of the grid. Returns true if and only if the
        block was moved at least once.!--*/
        Vector2Int position = startPosition;
        bool moved = false;
        PuyoBlock block = GetBlock(position);
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
        grid[startPosition.x, startPosition.y] = null;
        grid[position.x, position.y] = block;
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

    public List<HashSet<PuyoBlock>> DestroyColorGroups(int minSize){
        /**Returns a list of Puyo groups on the board (contiguous, same color), and destroys them.
        Ignores those groups smaller than minSize.!--*/
        HashSet<PuyoBlock> seenBlocks = new HashSet<PuyoBlock>();
        List<HashSet<PuyoBlock>> seenGroups = new List<HashSet<PuyoBlock>>();
        Vector2Int pos;
        HashSet<PuyoBlock> newGroup;
        for (int x=0; x < controller.boardDimensions.x; x++){
            for (int y=0; y < controller.boardDimensions.y; y++){
                pos = new Vector2Int(x, y);
                if (IsFull(pos) && !seenBlocks.Contains(GetBlock(pos))){
                    newGroup = ColorGroup(pos);
                    seenGroups.Add(newGroup);
                    seenBlocks.UnionWith(newGroup);
                }
            }
        }

        seenGroups = new List<HashSet<PuyoBlock>>(from seenGroup in seenGroups where seenGroup.Count >= minSize select seenGroup);
        //remove groups from the board
        foreach (HashSet<PuyoBlock> group in seenGroups){
            foreach (PuyoBlock block in group){
                grid[block.position.x, block.position.y] = null; //remove the block from the board
                Destroy(block.gameObject);
            }
        }
        return seenGroups;
    }

    private bool SameColor(Vector2Int first, Vector2Int second){
        /**Returns true if and only if the block at first and the block toOther
        away from first (relative position) are the same color. Assumes that both
        spaces are within bounds, returning false if at least one is empty.!--*/
        if (!IsFull(first) || !IsFull(second)){
            return false;
        } else {
            return (GetBlock(first).color == GetBlock(second).color);
        }
    }

    private HashSet<PuyoBlock> ColorGroup(Vector2Int startPosition){
        /**Returns the list of Puyo blocks in the grid that form a same-color
        group attached to startPosition. Contains at least one PuyoBlock, 
        the one at startPosition.!-- */
        Vector2Int basePos;
        PuyoBlock basePuyoBlock;
        PuyoBlock newPuyoBlock;
        Debug.Log("started group at: " + startPosition);

        HashSet<PuyoBlock> groupedPuyoBlocks = new HashSet<PuyoBlock>();
        Stack<Vector2Int> toAdd = new Stack<Vector2Int>();
        toAdd.Push(startPosition);
        while (toAdd.Count > 0){
            basePos = toAdd.Pop();
            basePuyoBlock = GetBlock(basePos);
            groupedPuyoBlocks.Add(basePuyoBlock);
            foreach (Vector2Int direction in ADJACENCY_DIRECTIONS){
                if (IsFull(basePos + direction)){
                    newPuyoBlock = GetBlock(basePos + direction);
                    if (!groupedPuyoBlocks.Contains(newPuyoBlock) && SameColor(basePos, basePos + direction)){
                        //if the block has not been seen yet and it is the right color, add it
                        toAdd.Push(basePos + direction);
                        Debug.Log("added to group: " + (basePos + direction));
                    }
                }
            }
        }
        Debug.Log("group of size: " + groupedPuyoBlocks.Count);
        return groupedPuyoBlocks;
    }

    public List<HashSet<PuyoBlock>> ResolveChains(int minSize){
        /**Resolves all chains, destroying Puyo groups and grounding pieces until
        there are no more chains.!--*/
        List<HashSet<PuyoBlock>> resolvedGroups = new List<HashSet<PuyoBlock>>();
        List<HashSet<PuyoBlock>> newGroups = DestroyColorGroups(minSize);
        while (newGroups.Count > 0){
            resolvedGroups.AddRange(newGroups);
            GroundAllBlocks();
            newGroups = DestroyColorGroups(minSize);
        }
        return resolvedGroups;
    }

    public void SetupBackground(GameObject bgTile){
        /**Sets up the background for the board.!--*/
        GameObject tile;
        background = new List<GameObject>();
        for (int x=0; x < controller.boardDimensions.x; x++){
            for (int y=0; y < controller.boardDimensions.y; y++){
                tile = Instantiate(bgTile, transform);
                tile.transform.position = anchorPosition + Vector3.forward; //reference position is anchor, but behind the rest
                tile.transform.localScale = (Vector3) controller.spaceDimensions;
                tile.transform.Translate(new Vector3(x, y, 0), Space.Self);
                background.Add(tile);
            }
        }
    }
}
