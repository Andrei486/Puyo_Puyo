using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class GameController : MonoBehaviour
{
    [SerializeField] //show colorMappings in the editor for easier setup
    private Color[] colors = new Color[4];
    [SerializeField]
    private GameObject[] piecePrefabs;
    public GameObject backgroundTile;
    // Dimensions as (width, height)
    [SerializeField]
    public Vector2Int boardDimensions; //(x, y) size of grid in spaces.
    public Vector2 spaceDimensions; //(x, y) size of one space in world space.
    public int minChainSize = 4;
    System.Random rng = new System.Random();
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Color GetColor(int n){
        /**Returns the color corresponding to the integer n,
        as specified in this controller's color map.!--*/
        return colors[n];
    }

    public int GetRandomColorIndex(){
        /**Returns a random valid color index from the color map.!--*/
        return rng.Next(colors.Length);
    }

    public GameObject GetRandomPiece(){
        /**Returns a new instance of a random Puyo piece from the given prefabs.!--*/
        return Instantiate(piecePrefabs[rng.Next(piecePrefabs.Length)]);
    }

    public static GameController GetController(){
        return GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }
}
