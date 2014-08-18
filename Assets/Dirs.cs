using UnityEngine;
using System.Collections;

public class Dirs
{

		public static Dir betweenPositions (Vector2 centerPos, Vector2 externalPos)
		{
				Debug.Log ("Comparing " + centerPos + " to " + externalPos);
				if (externalPos.x > centerPos.x) {
						return Dir.RIGHT;
				}
				return Dir.LEFT;
		}

		/**
	 	* @Return Dir.RIGHT if the given parameter is true.  Returns Dir.LEFT if false.
	 	*/
		public static Dir getRightIfTrue (bool right)
		{
				if (right) {
						return Dir.RIGHT;
				} 
				return Dir.LEFT;
		}

		public static Dir getOpposite (Dir dir)
		{
				if (dir == Dir.RIGHT) {
						return Dir.LEFT;
				} 
				return Dir.RIGHT;
		}

		public static Dir getRandomDir ()
		{
				if ((int)Random.value == 0) {
						return Dir.LEFT;
				}
				return Dir.RIGHT;
		}
}
