using UnityEngine;
using System.Collections;

/**
 * This class handles collisions with adjacent objects and the ground.  Use it
 * as a delegate for another entity object and call FixedUpdate on each frame.
 * Then, whenever that entity is to be moved, pass the amount of movement desired
 * into getValidMovementX and/or getValidMovementY.
 */
public class Collisions
{

		//fall constants
		private static readonly float fallFactor = -0.5f;
		private static readonly float gravity = -0.06f;
		private static readonly float delta = 0.1f;

		private float skin = 0.005f;
		private int layerMask = -1;

		private RayCastResult rX;
		private RayCastResult rUp;
		private RayCastResult rDown;

		private bool leftBlocked, rightBlocked;

		private readonly float height;
		private readonly float width;

		public Collisions (float height, float width)
		{
				this.height = height;
				this.width = width;
		}

		public bool isGrounded ()
		{
				if (rDown == null) {
						return true;
				}
				return rDown.isHit ();
		}

		public bool isBlocked (Dir dir)
		{
				if (dir == Dir.LEFT) {
						return leftBlocked;
				}
				if (dir == Dir.RIGHT) {
						return rightBlocked;
				}
				throw new UnityException ("Unexpected dir: " + dir);
		}

		public bool isLeftOrRightBlocked ()
		{
				return isBlocked (Dir.LEFT) || isBlocked (Dir.RIGHT);
		}

		public float getValidMovementX (float orSmallerInput)
		{
				if (rX == null) {
						return orSmallerInput;
				} 
				return getSmallest (rX.getAllowableSpeed (), orSmallerInput);
		} 
	
		public float getValidMovementY (float orSmallerInput)
		{
				if (orSmallerInput > 0) {
						if (rUp == null) {
								Debug.Log ("rUp is null");
								return orSmallerInput;
						}
						if (rUp.isHit ()) {
								return  rUp.getAllowableSpeed ();
						}
						return orSmallerInput;
				}  
				if (rDown == null) {
						Debug.Log ("rDown is null");
						return orSmallerInput;
				}
				if (rDown.isHit ()) {
						return  rDown.getAllowableSpeed ();
				}
				return orSmallerInput;
		}

		private float getSmallest (float speed1, float speed2)
		{
				if (Mathf.Abs (speed1) < Mathf.Abs (speed2)) {
						return speed1;
				}
				return speed2;	
		}

		public void FixedUpdate (SpeedProvider chrc)
		{
				if (layerMask < 0) {
						layerMask = 1 << LayerMask.NameToLayer ("Ground");
						if (layerMask < 0) {
								return;
						}
				}
				Vector2 c = chrc.getCenter ();
				float speedX = chrc.getSpeedX ();
				float speedY = chrc.getSpeedY ();
				Vector2 normalX = new Vector2 (Mathf.Sign (speedX), 0); 
				Vector2 normalY = new Vector2 (0, Mathf.Sign (speedY)); 

				if (chrc.isRightFacing ()) {
						rUp = doGetValidMovement (normalY, speedY, getTopLeftPlusDelta (c), getTopMiddle (c), getTopRightMinusDelta (c));
						rDown = doGetValidMovement (normalY, speedY, getBotLeftPlusDelta (c), getBotMiddle (c), getBotRightMinusDelta (c));
				} else {
						rUp = doGetValidMovement (normalY, speedY, getTopRightMinusDelta (c), getTopMiddle (c), getTopLeftPlusDelta (c));
						rDown = doGetValidMovement (normalY, speedY, getBotRightMinusDelta (c), getBotMiddle (c), getBotLeftPlusDelta (c));
				}
				if (speedX > 0) {
						if (isGrounded ()) {
								rX = doGetValidMovement (normalX, speedX, getTopRight (c), getRight (c), getLowRight (c));
						} else {
								rX = doGetValidMovement (normalX, speedX, getTopRight (c), getRight (c), getBotRight (c));
						}
						if (rX.isHit ()) {
								rightBlocked = true;
						} else {
								rightBlocked = false;
						}
				} else {
						if (isGrounded ()) {
								rX = doGetValidMovement (normalX, speedX, getTopLeft (c), getLeft (c), getLowLeft (c));
						} else {
								rX = doGetValidMovement (normalX, speedX, getTopLeft (c), getLeft (c), getBotLeft (c));
						}
						if (rX.isHit ()) {
								leftBlocked = true;
						} else {
								leftBlocked = false;
						}
				}
		}




		RayCastResult doGetValidMovement (Vector2 normal, float speed, params Vector2[] origins)
		{
				float dir = Mathf.Sign (speed); 

				foreach (Vector2 v in origins) {
						RayCastResult r1 = doTryRaycast (v, speed, dir, normal);
						if (r1.isHit ()) {
								return r1;
						}
				}
				return RayCastResult.miss (speed);
		}


		private RayCastResult doTryRaycast (Vector2 origin, float speed, float dir, Vector2 normal)
		{
				RaycastHit2D hit = Physics2D.Raycast (origin, normal, Mathf.Abs (speed), layerMask);
				if (hit) {
						Debug.DrawRay (origin, normal * speed, Color.yellow);
						// Get Distance between entity and ground
						float distance = hit.fraction * Mathf.Abs (speed);
						// Stop entity's downward movement after coming within skin width of a boxCollider
						if (distance > skin) {
								return RayCastResult.hit ((distance * dir) + skin);
						}
						return RayCastResult.hit (0);
				}
				Debug.DrawRay (origin, normal);
				return RayCastResult.miss (speed);
		}

		private class RayCastResult
		{

				private readonly bool isHit1;
				private readonly float allowableSpeed;

				private RayCastResult (bool isHit, float speed)
				{
						this.isHit1 = isHit;
						this.allowableSpeed = speed;
				}

				public static RayCastResult hit (float allowableSpeed)
				{
						return new RayCastResult (true, allowableSpeed);
				}
				public static RayCastResult miss (float allowableSpeed)
				{
						return new RayCastResult (false, allowableSpeed);
				}

				public bool isHit ()
				{
						return isHit1;
				}

				public float getAllowableSpeed ()
				{
						return allowableSpeed;
				}

		}

		public float applyGravityDisregardGround (float inFloat)
		{
				float outFloat = inFloat;
				outFloat += gravity;
				if (outFloat < fallFactor) {
						return fallFactor;
				}
				return outFloat;
		}

		public float applyGravity (float inFloat)
		{
				return applyGravityDisregardGround (inFloat);
		}


		public void beKnockedBack (SpeedProvider chrc, Dir dir, int amount)
		{ 
				if (dir == Dir.LEFT) {
						if (leftBlocked) {
								return;
						}
						chrc.setSpeedX (-amount / 100f);
				}
				if (dir == Dir.RIGHT) {
						if (rightBlocked) {
								return;
						}

						chrc.setSpeedX (amount / 100f);
				}
		}

		Vector2 getTopRight (Vector2 c)
		{
				float x = c.x + width;
				float y = c.y + height;
				return new Vector2 (x, y);
		}
	
		Vector2 getTopLeft (Vector2 c)
		{
				float x = c.x - width;
				float y = c.y + height;
				return new Vector2 (x, y);
		}
	
		Vector2 getRight (Vector2 c)
		{
				float x = c.x + width;
				float y = c.y;
				return new Vector2 (x, y);
		}
	
		Vector2 getLeft (Vector2 c)
		{
				float x = c.x - width;
				float y = c.y;
				return new Vector2 (x, y);
		}
	
		Vector2 getBotRight (Vector2 c)
		{
				float x = c.x + width;
				float y = c.y - height;
				return new Vector2 (x, y);
		}

		Vector2 getBotRightMinusDelta (Vector2 c)
		{
				float x = (c.x + width) - delta;
				float y = c.y - height;
				return new Vector2 (x, y);
		}

		Vector2 getBotLeftPlusDelta (Vector2 c)
		{
				float x = (c.x - width) + delta;
				float y = c.y - height;
				return new Vector2 (x, y);
		}

		Vector2 getTopRightMinusDelta (Vector2 c)
		{
				float x = (c.x + width) - delta;
				float y = c.y + height;
				return new Vector2 (x, y);
		}
	
		Vector2 getTopLeftPlusDelta (Vector2 c)
		{
				float x = (c.x - width) + delta;
				float y = c.y + height;
				return new Vector2 (x, y);
		}
		
		Vector2 getBotLeft (Vector2 c)
		{
				float x = c.x - width;
				float y = c.y - height;
				return new Vector2 (x, y);
		}

		Vector2 getLowRight (Vector2 c)
		{
				float x = c.x + width;
				float y = (c.y - height) + delta;
				return new Vector2 (x, y);
		}

		Vector2 getLowLeft (Vector2 c)
		{
				float x = c.x - width;
				float y = (c.y - height) + delta;
				return new Vector2 (x, y);
		}

		Vector2 getBotMiddle (Vector2 c)
		{
				float x = c.x;
				float y = c.y - height;
				return new Vector2 (x, y);
		}

		Vector2 getTopMiddle (Vector2 c)
		{
				float x = c.x;
				float y = c.y + height;
				return new Vector2 (x, y);
		}


		public override bool Equals (object obj)
		{
				if (obj == null)
						return false;
				if (ReferenceEquals (this, obj))
						return true;
				if (obj.GetType () != typeof(Collisions))
						return false;
				Collisions other = (Collisions)obj;
				return layerMask == other.layerMask && rX == other.rX && rUp == other.rUp && rDown == other.rDown && leftBlocked == other.leftBlocked && rightBlocked == other.rightBlocked && height == other.height && width == other.width;
		}
	

		public override int GetHashCode ()
		{
				unchecked {
						return layerMask.GetHashCode () ^ (rX != null ? rX.GetHashCode () : 0) ^ (rUp != null ? rUp.GetHashCode () : 0) ^ (rDown != null ? rDown.GetHashCode () : 0) ^ leftBlocked.GetHashCode () ^ rightBlocked.GetHashCode () ^ height.GetHashCode () ^ width.GetHashCode ();
				}
		}
	
	
	
}
