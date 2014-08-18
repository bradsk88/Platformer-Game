using UnityEngine;
using System.Collections;

public class FistState :   AbstractDamagingObject
{ 

		private Dir dir = Dir.LEFT;
		int firingFramesLeft = 0;


		public FistState () : base(10, 100)
		{
		}

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
		}


		public void activate (Dir dir, int numFramesActive)
		{
				if (!isDamaging ()) {
						this.firingFramesLeft = numFramesActive;
				}
				this.dir = dir;
		}

		protected override Dir getPushDir ()
		{
				return this.dir;
		}

		protected override void ChildFixedUpdate ()
		{
				if (firingFramesLeft > 0) {
						firingFramesLeft--;
				}
		}

		protected override LayerMask getLayerMask ()
		{
				return LayerMask.NameToLayer ("Player");
		}

		protected override bool isDamaging ()
		{
				return firingFramesLeft > 0;
		}

		public override bool Equals (object obj)
		{
				if (obj == null)
						return false;
				if (ReferenceEquals (this, obj))
						return true;
				if (obj.GetType () != typeof(FistState))
						return false;
				FistState other = (FistState)obj;
				return firingFramesLeft == other.firingFramesLeft;
		}
	

		public override int GetHashCode ()
		{
				unchecked {
						return firingFramesLeft.GetHashCode ();
				}
		}
	
}
