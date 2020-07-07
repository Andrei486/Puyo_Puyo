using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities{
    
    public static class Extensions
    {
        public static T Next<T>(this T src) where T : struct
		{
			if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

			T[] Arr = (T[])Enum.GetValues(src.GetType());
			int j = Array.IndexOf<T>(Arr, src) + 1;
			return (Arr.Length==j) ? Arr[0] : Arr[j];            
		}

        public static T Previous<T>(this T src) where T : struct
		{
			if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

			T[] Arr = (T[])Enum.GetValues(src.GetType());
			int j = Array.IndexOf<T>(Arr, src) - 1;
			return (-1==j) ? Arr[Arr.Length - 1] : Arr[j];            
		}

        public static Vector2Int Rotate(this Vector2Int v2int, float angle)
		{
			/**Rotates the vector by angle degrees COUNTERCLOCKWISE, returning a copy.!--*/
            Vector2 v2 = (Vector2) v2int;
			angle *= Mathf.Deg2Rad;
			float x = Mathf.Cos(angle) * v2.x - Mathf.Sin(angle) * v2.y;
			float y = Mathf.Sin(angle) * v2.x + Mathf.Cos(angle) * v2.y;
			return Vector2Int.RoundToInt(new Vector2(x, y));
		}
        public static Vector2 Rotate(this Vector2 v2, float angle)
		{
			/**Rotates the vector by angle degrees COUNTERCLOCKWISE, returning a copy.!--*/
			angle *= Mathf.Deg2Rad;
			float x = Mathf.Cos(angle) * v2.x - Mathf.Sin(angle) * v2.y;
			float y = Mathf.Sin(angle) * v2.x + Mathf.Cos(angle) * v2.y;
			return new Vector2(x, y);
		}

		public static Vector2Int RotateAround(this Vector2Int position, Vector2 pivot, float angle){
			/**Rotates position around pivot counterclockwise by angle degrees, returning a copy.!--*/
			Vector2 relativePosition = position - pivot;
			return Vector2Int.RoundToInt(relativePosition.Rotate(angle) + pivot);
		}
    }
    public class Block
    {
        Color color;
        public Vector2Int position;
        public GameObject sprite;
        
		public Block(Color color, Vector2Int relativePosition, GameObject sprite){
			this.color = color;
			this.position = relativePosition;
			this.sprite = sprite;
			SetColor();
		}

		public void Move(Vector2Int movement){
			/**Moves the block's sprite along the grid by movement (measured in spaces).!--*/
			position += movement;
		}

		public void SetColor(){
			sprite.GetComponent<SpriteRenderer>().color = color;
		}
    }

    [Serializable]
    public enum Direction
    {
        /**Use Next() to go clockwise, and Previous to go counterclockwise.!-- */
        NORTH=0, EAST=1, SOUTH=2, WEST=3
    }

}