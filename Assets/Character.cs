using UnityEngine;
using System.Collections;

public interface Character
{
		Vector2 getCenter ();

		float getSpeedX ();
		float getSpeedY ();

		bool isRightFacing ();
}
