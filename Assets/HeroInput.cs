using UnityEngine;
using System.Collections;

public class HeroInput : MonoBehaviour, Character, Forcible
{
		private readonly float minActivation = 0.15f;

		//fighting vars
		private bool punch;
		private float fistDist = 0.5f;

		//collision checking
		private FistState fist;

 	
		private readonly MovementControl movementDelegate = new MovementControl (0.6f, 0.6f, 0.3f);

		public HeroInput ()
		{
		}

		void Start ()
		{
				this.fist = GameObject.Find ("Fist").GetComponent<FistState> ();
		}
	
		void Update ()
		{
				alignFist ();
				if (Input.GetKeyUp (KeyCode.W)) {
						movementDelegate.requestJump (this); 
				}

				if (Input.GetKeyUp (KeyCode.Space)) {
						this.fist.activate (Dirs.getRightIfTrue (isRightFacing ()), 1);
				}
				handleXInput (); 
		}

		private void alignFist ()
		{
				if (isRightFacing ()) {
						this.fist.transform.position = new Vector2 (this.transform.position.x + fistDist, this.transform.position.y);
				} else {
						this.fist.transform.position = new Vector2 (this.transform.position.x - fistDist, this.transform.position.y);
				}
		}

		void FixedUpdate ()
		{
				movementDelegate.FixedUpdate (this);
				this.transform.position = movementDelegate.getPosition ();
		}	 

		private Dir getDirection (float xInput)
		{
				if (xInput < 0) {
						return Dir.LEFT;
				}
				return Dir.RIGHT;

		} 

		private void handleXInput ()
		{

				float xInput = Input.GetAxis ("Horizontal");


				if (Mathf.Abs (xInput) < minActivation) {
						movementDelegate.requestIdle ();
						return;
				}
				movementDelegate.requestMove (this, getDirection (xInput));

		}

	#region Character implementation
	
		public Vector2 getCenter ()
		{
				return transform.position;
		}

		public float getSpeedX ()
		{
				return movementDelegate.getSpeedX ();
		}
 
	
		public float getSpeedY ()
		{
				return movementDelegate.getSpeedY ();
		}


		public bool isRightFacing ()
		{
				return movementDelegate.isRightFacing ();
		}
	#endregion

	#region Forcible implementation

		public void beKnockedBack (Dir dir, int amount)
		{
				movementDelegate.beKnockedBack (dir, amount);
		}

	#endregion
}
