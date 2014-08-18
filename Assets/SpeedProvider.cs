using UnityEngine;
using System.Collections;

public interface SpeedProvider
{
		Vector2 getCenter ();
		float getSpeedX ();
		void setSpeedX (float f);

		float getSpeedY ();
		bool isRightFacing ();
}

