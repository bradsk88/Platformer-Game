using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AbstractDamagingObject : MonoBehaviour
{

		private static readonly float RADIUS = 0.5f;

		private readonly float amount;
		private readonly float forceOfHit;

		protected AbstractDamagingObject (float amount, float forceOfHit)
		{
				this.amount = amount;
				this.forceOfHit = forceOfHit;
		}

		void doHit (MonoBehaviour m)
		{
				if (m == null) {
						return;
				}
				if (m is Forcible) {
						Dir dir = getPushDir ();
						Debug.Log ("Attacking " + dir);
						((Forcible)m).beKnockedBack (dir, 100);
				}
		}

		protected abstract Dir getPushDir ();

		void hitAllColliding (Collider2D[] collided)
		{
				if (collided.Length > 0) {
						foreach (Collider2D m in collided) {
								doHit (m.GetComponent<MonoBehaviour> ());
						}
						return;
				}
		}

		void FixedUpdate ()
		{
				Collider2D[] collided = Physics2D.OverlapCircleAll (this.transform.position, RADIUS, ~ (1 << getLayerMask ()), 0f, 0f);
				if (isDamaging ()) {
						hitAllColliding (collided);
				}
				ChildFixedUpdate ();
		}

		protected abstract LayerMask getLayerMask ();
		protected abstract void ChildFixedUpdate ();
	
		// Use this for initialization
		void Start ()
		{
				//do nothing
		}
	
		// Update is called once per frame
		void Update ()
		{
				//do nothing
		}

		protected abstract bool isDamaging ();

		public override bool Equals (object obj)
		{
				if (obj == null)
						return false;
				if (ReferenceEquals (this, obj))
						return true;
				if (obj.GetType () != typeof(AbstractDamagingObject))
						return false;
				AbstractDamagingObject other = (AbstractDamagingObject)obj;
				return amount == other.amount && forceOfHit == other.forceOfHit;
		}
	

		public override int GetHashCode ()
		{
				unchecked {
						return amount.GetHashCode () ^ forceOfHit.GetHashCode ();
				}
		}
	
	

}
