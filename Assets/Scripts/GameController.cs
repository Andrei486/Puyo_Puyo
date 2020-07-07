using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class GameController : MonoBehaviour
{
    [SerializeField] //show colorMappings in the editor for easier setup
    private Color[] colors = new Color[4];
    // Dimensions as (width, height)
    [SerializeField]
    public Vector2Int boardDimensions; //(x, y) size of grid in spaces.
    public Vector2 spaceDimensions; //(x, y) size of one space in world space.
    System.Random rng;
    void Start()
    {
        rng = new System.Random();
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

    public Color GetRandomColor(){
        /**Returns a random color from the color map.!--*/
        return GetColor(rng.Next(colors.Length));
    }

    public static GameController GetController(){
        return GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }
}
