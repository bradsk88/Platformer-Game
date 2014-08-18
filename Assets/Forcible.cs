using UnityEngine;
using System.Collections;

/**
 * An object which may be forced in a direction.
 */
public interface Forcible
{
		void beKnockedBack (Dir dir, int amount);
}
