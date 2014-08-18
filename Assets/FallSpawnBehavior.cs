using UnityEngine;
using System.Collections;

/**
 * A FallSpawnBehavior is behavior for an object which falls at a constant speed until it comes in contact with the ground.
 */
public class FallSpawnBehavior : AbstractDamagingObject, Character
{

		MovementControl moveDel = new MovementControl (0.5f, 0.5f, 0f);
		private bool dead = false;

		public FallSpawnBehavior () : base(100, 10)
		{
		}


		// Use this for initialization
		void Start ()
		{
				gameObject.renderer.material.color = Color.red;
				gameObject.layer = LayerMask.NameToLayer ("Hazards");
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (moveDel.isGrounded ()) {
						dead = true;
						gameObject.layer = LayerMask.NameToLayer ("Ground");
				}
		}
		void FixedUpdate ()
		{
				if (dead) {
						return;
				}
				moveDel.FixedUpdate (this);
				this.transform.position = moveDel.getPosition ();
		}

		public Vector2 getCenter ()
		{
				return transform.position;
		}

		public float getSpeedX ()
		{
				return moveDel.getSpeedX ();
		}

		public float getSpeedY ()
		{
				return moveDel.getSpeedY ();
		}

		public bool isRightFacing ()
		{
				return moveDel.isRightFacing ();
		}
		protected override Dir getPushDir ()
		{
				return Dirs.getRandomDir ();
		}

		protected override bool isDamaging ()
		{
				return !moveDel.isGrounded ();
		}
		protected override LayerMask getLayerMask ()
		{
				return LayerMask.NameToLayer ("Hazards");
		}

		protected override void ChildFixedUpdate ()
		{
				//do nothing
		}

}
