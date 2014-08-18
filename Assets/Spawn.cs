using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour
{

		public GameObject fallingRock;
		public float chance;

		private readonly float spawnMaxY = 15f;
		private readonly float spawnMinY = 1f;
		private readonly float spawnMaxX = 50f;
		private readonly float spawnMinX = -50f;

		// Use this for initialization
		void Start ()
		{
		}

		private void doSpawn (float x, float y)
		{
				Vector3 pos = new Vector3 (x, y, 0);
				Quaternion rot = Quaternion.identity;
				Instantiate (fallingRock, pos, rot);
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (willSpawn ()) {
						float x = getRandomInRange (spawnMinX, spawnMaxX);
						float y = getRandomInRange (spawnMinY, spawnMaxY);
						doSpawn (x, y);
				}
		}

		private float getRandomInRange (float low, float high)
		{
				float width = high - low;
				float r = Random.value;
				return (width * r) + low;
		}

		private bool willSpawn ()
		{
				float r = Random.value;
				int intR = (int)(r * chance);
				if (intR == 1) {
						return true;
				}
				return false;
		}
}
