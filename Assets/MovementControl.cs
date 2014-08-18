using UnityEngine;
using System.Collections;

public class MovementControl : SpeedProvider, Forcible
{
		//walking
		private readonly float airSpeed;	
		private readonly float speedFactor = 0.1f;
		private readonly float reverseAccel = 0.01f;
		private readonly float stopFactor = 0.5f;
		private readonly float minMovement = 0.01f; 
		private float speedX = 0;
		private float speedY = 0;
		private Dir dir = Dir.LEFT;

		//Colliding
		private readonly Collisions collDel;
		private Vector2 center;
		private bool dead = false;

		//jumping
		private readonly float jumpFactor = 0.7f;

		float MAX_FALL_ALLOWING_DOUBLE_JUMP = -0.5f;

		private bool jump = false;
		private bool doubleJumped = false;

		public MovementControl (float height, float width, float speed)
		{
				collDel = new Collisions (height, width);
				this.speedFactor = speed;
				this.airSpeed = speed / 3f;
		}

		public void die ()
		{
				dead = true;
		}

		void deadFixedUpdate ()
		{
				speedY = -0.5f;
				speedX = 0;
				return;
		}

		void aliveFixedUpdate ()
		{
				collDel.FixedUpdate (this);
				if (!jump) {
						speedY = collDel.applyGravity (speedY);
				}
				if (collDel.isGrounded ()) {
						doubleJumped = false;
				}
				tryJump ();
				speedX = collDel.getValidMovementX (speedX);
				speedY = collDel.getValidMovementY (speedY);
		}

		public void FixedUpdate (Character c)
		{
				center = c.getCenter ();
				if (dead) {
						deadFixedUpdate ();
						return;
				}
				aliveFixedUpdate ();
				return;
		}

		public Vector3 getPosition ()
		{
				return new Vector2 (center.x + speedX, center.y + speedY);
		}

		private void tryJump ()
		{
		
				if (jump) {
						Debug.Log ("Will jump");
						if (speedY > MAX_FALL_ALLOWING_DOUBLE_JUMP) {	
								if (doubleJumped) {
										jump = false;
										return;
								}
								speedY = jumpFactor;
								doubleJumped = true;
						}
						if (collDel.isGrounded ()) {
								speedY = jumpFactor;
								doubleJumped = false;
						}
						jump = false; 
						return;
				}
		}
	 
		public void beKnockedBack (Dir dir, int amount)
		{
				collDel.beKnockedBack (this, dir, amount);
		}

		public void requestIdle ()
		{ 
				if (collDel.isGrounded ()) {
						slowDownX (); 
						return;
				}
		}
	
		private void slowDownX ()
		{
				if (Mathf.Abs (speedX) > minMovement) {
						speedX = speedX * stopFactor;
						return;
				} 
				speedX = 0;
		}

		private bool canMoveLeftRight ()
		{
				if (collDel.isGrounded ()) {
						return true;
				}
				//TODO: Should skywalk be allowed?
				return true;
		}
		public void requestJump (Character c)
		{
				if (canJump ()) {
						Debug.Log ("Requested jump");
						jump = true;
						return;
				}
				Debug.Log ("Cannot jump");
		}
	
		public void requestMove (Character c, Dir dir)
		{
				this.dir = dir;
				if (!canMoveLeftRight ()) {
						return;
				}

				if (dir == Dir.LEFT) {
						float goalSpeedL = Mathf.Min (speedX, -speedFactor);
//						if (isGrounded ()) {
//						speedX = goalSpeedL;
//						return;
//						}
						if (speedX > goalSpeedL) {
								speedX -= reverseAccel;
								return;
						}
						speedX = goalSpeedL;
						return;
				}
				float goalSpeedR = Mathf.Max (speedX, speedFactor);
//				if (isGrounded ()) {
//				speedX = goalSpeedR;
//				return;
//				}
				if (speedX < goalSpeedR) {
						speedX += reverseAccel;
						return;
				}
				speedX = goalSpeedR;
				return;
		}

		private bool canJump ()
		{
				return collDel.isGrounded () || !doubleJumped;
		}

		public Vector2 getCenter ()
		{
				return center;
		}
	
		public float getSpeedX ()
		{
				return speedX;
		}
	
		public void setSpeedX (float f)
		{
				speedX = f;
		}
	
		public float getSpeedY ()
		{
				return speedY;
		}

		public bool isGrounded ()
		{
				return collDel.isGrounded ();
		}	
	 
	
		public bool isRightFacing ()
		{
				return dir == Dir.RIGHT;
		}

		public bool isLeftOrRightBlocked ()
		{
				return isBlocked (Dir.LEFT) || isBlocked (Dir.RIGHT);
		}

		public bool isBlocked (Dir dir)
		{
				return collDel.isBlocked (dir);
		}
}
