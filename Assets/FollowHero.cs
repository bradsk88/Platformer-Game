using UnityEngine;
using System.Collections;

public class FollowHero : MonoBehaviour
{
		public GameObject marker;
		public GameObject hero;
		private bool locking = false;
		private bool locked = false;
		private readonly float gravyZoneWidth = 2f;
		private float toAdd = 0;

		// Use this for initialization
		void Start ()
		{
				this.transform.position = new Vector3 (hero.transform.position.x + toAdd, this.transform.position.y, this.transform.position.z);
		}
	
		// Update is called once per frame
		void Update ()
		{
//				marker.transform.position = new Vector2 (transform.position.x, transform.position.y);
				HeroInput input = hero.GetComponent<HeroInput> ();
				if (input != null) {
						//returnToCenter (input);
				}
				if (locking || locked) {
						this.transform.position = new Vector3 (hero.transform.position.x + toAdd, this.transform.position.y, this.transform.position.z);
				}
				if (input != null) {
						if (input.getSpeedX () > 0) {
								if (toAdd > -gravyZoneWidth) {
										this.locking = true;
										this.locked = false;
										toAdd -= input.getSpeedX ();
								} else {
										this.locking = false;
										this.locked = true;
								}
						} 
						if (input.getSpeedX () < 0) {
								if (toAdd < gravyZoneWidth) {
										this.locking = true;
										this.locked = false;
										toAdd -= input.getSpeedX ();
								} else {
										this.locking = false;
										this.locked = true;
								}
						}
				}

		}

		void returnToCenter (HeroInput input)
		{
				if (locking && toAdd < 0.05f && toAdd > -0.05f) {
						locking = false;
						locked = false;
						return;
				}
				if (toAdd > 0) {
						toAdd -= 0.05f;
				}
				if (toAdd < 0) {
						toAdd += 0.05f;
				}
				
		}

		private bool movedOutOfGravyZone ()
		{
				float distance = Mathf.Abs (Mathf.Max (this.transform.position.x, hero.transform.position.x) - Mathf.Min (this.transform.position.x, hero.transform.position.x));
				if (distance > gravyZoneWidth) {
						return true;
				}
				return false;
		}
}
