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
    public GameObject destroyEffect;
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

    public int ChainPower(int length){
        /**Returns the chain power bonus corresponding to the given chain length.!--*/
        if (length < 4){
            return length * 8;
        } else {
            return length * 32 - 96;
        }
    }

    public int ColorBonus(int distinctColors){
        /**Returns the color bonus corresponding to the given number of distinct colors.!--*/
        switch (distinctColors){
            case 1:
                return 0;
            case 2:
                return 3;
            case 3:
                return 6;
            case 4:
                return 12;
            case 5:
                return 24;
            default:
                return 48; //more than 5 colors?
        }
    }

    public int GroupBonus(int groupLength){
        /**Returns the group bonus for the given group length.!--*/
        int effectiveLength = groupLength - minChainSize;
        if (effectiveLength <= 0){
            return 0;
        } else if (effectiveLength <= 6){
            return effectiveLength + 1;
        } else {
            return 10;
        }
    }

    public static GameController GetController(){
        return GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }
}
