using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuyoBlock : MonoBehaviour
{
    public int color;
    public Vector2Int position;
    
    void Start(){

    }

    void Update(){

    }

    public void Move(Vector2Int movement){
        /**Moves the block's sprite along the grid by movement (measured in spaces).!--*/
        position += movement;
        transform.Translate(Vector2.Scale(movement, GameController.GetController().spaceDimensions), Space.Self);
    }

    public void SetColor(int newColor){
        this.color = newColor;
        gameObject.GetComponent<SpriteRenderer>().color = GameController.GetController().GetColor(newColor);
    }
}
