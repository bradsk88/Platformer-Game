using UnityEngine;
using System.Collections;

/**
 * An object which may be present or absent.
 */
public class Optional<T> where T : struct
{

		private readonly T? obj;
		private readonly bool present;

		private Optional (T? obj, bool present)
		{
				this.obj = obj;
				this.present = present;
		}

		public bool isPresent ()
		{
				return present;
		}

		public T get ()
		{
				if (present) {
						return (T)obj;
				}
				Debug.Log ("NOOO");
				throw new UnassignedReferenceException ();
		}

		public static Optional<T> of (T objIn)
		{
				return  new Optional<T> (objIn, true);
		}

		public static Optional<T> absent ()
		{
				return new Optional<T> (null, false);
		}
	
}
