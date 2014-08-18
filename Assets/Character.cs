using UnityEngine;
using System.Collections;

/**
 * A Character is an object which has a position, speed and direction.
 */
public interface Character
{
		Vector2 getCenter ();

		float getSpeedX ();
		float getSpeedY ();

		bool isRightFacing ();
}
