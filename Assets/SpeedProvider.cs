using UnityEngine;
using System.Collections;

/**
 * An object which has a speed which can be set.
 */
public interface SpeedProvider
{
		Vector2 getCenter ();
		float getSpeedX ();
		void setSpeedX (float f);

		float getSpeedY ();
		bool isRightFacing ();
}

