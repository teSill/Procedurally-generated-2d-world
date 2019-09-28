using UnityEngine;
using System.Collections;

namespace Pathfinding {
	/// <summary>
	/// Sets the destination of an AI to the position of a specified object.
	/// This component should be attached to a GameObject together with a movement script such as AIPath, RichAI or AILerp.
	/// This component will then make the AI move towards the <see cref="target"/> set on this component.
	///
	/// See: <see cref="Pathfinding.IAstarAI.destination"/>
	///
	/// [Open online documentation to see images]
	/// </summary>
	public class AIDestinationSetter : VersionedMonoBehaviour {
        /// <summary>The object that the AI should move to</summary>
        private Vector3 target;
		public IAstarAI ai;

        private float _moveRadius = 4;

        void Start () {
            target = transform.position;
            ai = GetComponent<IAstarAI>();
            StartCoroutine(GetDestination());
        }

        Vector3 PickRandomPoint () {
            var point = Random.insideUnitSphere * _moveRadius;
            //point.y = transform.position.y;
           // point += ai.position;
            return point;
        }

        private IEnumerator GetDestination() {
            yield return new WaitForSeconds(Random.Range(2, 5));
            while(true) {
                if (!ai.pathPending && (ai.reachedDestination || !ai.hasPath)) {
                    ai.destination = PickRandomPoint();
                    Debug.Log(transform.position +  " --- " + ai.destination);
                    ai.SearchPath();
                }
                yield return new WaitForSeconds(Random.Range(1, 7.5f));
            }
        }
	}
}
