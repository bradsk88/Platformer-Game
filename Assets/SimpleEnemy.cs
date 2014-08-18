using UnityEngine;
using System.Collections;

/**
 * An entity which wanders the map randomly and jumps over obstacles.
 * 
 * This entity will also chase the player, but this is disabled at the moment.
 */
public class SimpleEnemy : MonoBehaviour, Character, Forcible
{
		
		private static readonly float REACH_DIST = 1f;
		private static readonly int JUMP_COOLDOWN_DURATION = 150;
		private static readonly float BASE_SPEED = 0.04f;
		private static readonly float LARGE_SEARCH_RADIUS = 20f;
		private static readonly float SMALL_SEARCH_RADIUS = 5f;
		private static readonly float MAX_WANDER_JUMP_ATTEMPTS = 1;
		private static readonly float DECISION_CHANCE = 500;

		private float wanderJumpAttempts = 0;
		private int newAction;

		private MovementControl moveDelegate;
		private bool chasing = false;

		private bool alive = true;
		private int health = 100;
		private int jumpCoolDown = 0;

		public SimpleEnemy ()
		{
		}
	
		void Start ()
		{
				float speedTweak = Random.value * 0.01f;
				moveDelegate = new MovementControl (1.1f, 0.6f, BASE_SPEED + speedTweak);
				chooseNewWanderAction ();
		}
		
		void Update ()
		{ 
				if (health < 0 && alive) {
						alive = false;
						moveDelegate.die ();
				}
				if (alive) {
						runLogic ();
				} else {
						removeObjectIfBelowScreen (); 
				} 
		}

		bool tryToJumpOverObstacle ()
		{
				if (jumpCoolDown == 0) {
						moveDelegate.requestJump (this);
						jumpCoolDown = JUMP_COOLDOWN_DURATION + (int)(Random.value * 50);
						return true;
				}
				return false;
		}

		private void updateCounters ()
		{
				if (jumpCoolDown > 0) {
						jumpCoolDown--;
				}
		}

		void FixedUpdate ()
		{
				updateCounters ();

				moveDelegate.FixedUpdate (this);
				this.transform.position = moveDelegate.getPosition ();

		}
		
		private Optional<Dir> getChaseDirection (float searchRadius)
		{
				Collider2D[] collided = Physics2D.OverlapCircleAll (this.transform.position, searchRadius, 1 << LayerMask.NameToLayer ("Player"), 0f, 0f);
				if (collided.Length > 0) {
						chasing = true;
						getDirectionFromFirstCollided (collided);
				}
				return Optional<Dir>.absent ();
		}

		Optional<Dir> getDirectionFromFirstCollided (Collider2D[] collided)
		{
				float collidedX = collided [0].transform.position.x;
				float thisX = this.transform.position.x;
				float distance = Mathf.Max (collidedX, thisX) - Mathf.Min (collidedX, thisX);
				if (distance < REACH_DIST) {
						return Optional<Dir>.absent ();
				}
				if (collidedX > thisX) {
						return Optional<Dir>.of (Dir.RIGHT);
				} else {
						return Optional<Dir>.of (Dir.LEFT);
				}
		}



		void removeObjectIfBelowScreen ()
		{
				if (this.transform.position.y < -10) {
						Destroy (this.gameObject);
				} 
		}

		void runLogic ()
		{
//				if (chasing) {
//						var searchRadius = LARGE_SEARCH_RADIUS;
//						runChaseLogic (searchRadius);
//						return;
//				}
				doWanderLogic (SMALL_SEARCH_RADIUS);
		}

	
		void runChaseLogic (float searchRadius)
		{
				Optional<Dir> dir = getChaseDirection (searchRadius);
				if (dir.isPresent ()) {
						moveDelegate.requestMove (this, dir.get ());
				} else {
						moveDelegate.requestIdle ();
				}
				if (moveDelegate.isLeftOrRightBlocked ()) {
						tryToJumpOverObstacle ();
				}
		}

		void doWanderLogic (float sMALL_SEARCH_RADIUS)
		{
				if ((int)(Random.value * DECISION_CHANCE) == 0) {
						chooseNewWanderAction ();
				}
				doCurrentWanderAction ();
				//continue current wander logic.
		}

		void chooseNewWanderAction ()
		{
				Debug.Log ("choosing new action");
				newAction = (int)(Random.value * 3);
				Debug.Log ("Current action is " + newAction);
		}

		void chooseNewWanderActionNot (int i)
		{
				chooseNewWanderAction ();
				if (newAction == i) {
						chooseNewWanderActionNot (i);
				}
		}

		void doCurrentWanderAction ()
		{
				switch (newAction) {
				case 0:
						doWander (Dir.LEFT);
						return;
				case 1:
						doWander (Dir.RIGHT);
						return;
				case 2:
						moveDelegate.requestIdle ();
						return;
				}
				throw new UnityException ("Unknown wander action id " + newAction);
		}

		void doWander (Dir dir)
		{
				moveDelegate.requestMove (this, dir);
				if (moveDelegate.isBlocked (dir)) {
						if (wanderJumpAttempts > MAX_WANDER_JUMP_ATTEMPTS) {
								wanderJumpAttempts = 0;
								chooseNewWanderActionNot (newAction);
								return;
						}
						bool jumped = tryToJumpOverObstacle ();
						if (jumped) {
								wanderJumpAttempts += 1;
						}
						return;
				}
				wanderJumpAttempts = 0;
		}

	#region Character implementation

		public Vector2 getCenter ()
		{
				//Debug.Log ("Got center " + transform.position);
				return transform.position;
		}

		public float getSpeedX ()
		{
				return moveDelegate.getSpeedX ();
		}


		public float getSpeedY ()
		{
				return moveDelegate.getSpeedY ();
		}

		public bool isRightFacing ()
		{
				return moveDelegate.isRightFacing ();
		}

	#endregion

	#region Forcible implementation

		public void beKnockedBack (Dir dir, int amount)
		{
				this.health -= 10;
				moveDelegate.beKnockedBack (dir, amount);
		}

	#endregion

	

}
