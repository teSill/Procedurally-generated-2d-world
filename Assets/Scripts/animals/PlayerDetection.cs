using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerDetection : MonoBehaviour {
    
    private Animal _animal;

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public const float DETECTION_DELAY = 0.2f;

    private void Start() {
        _animal = transform.parent.GetComponent<Animal>();
    }

    public IEnumerator LookForPlayer(float delay) {
        while (true) {
            yield return new WaitForSeconds(delay);
            if (CanSeePlayer()) {
                BecomeSuspicious();
            }
        }
    }

    public bool CanSeePlayer() {
        if (Vector3.Distance(Player.Instance.transform.position, transform.position) > viewRadius) {
            return false;
        }

        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), viewRadius, targetMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++) {
            Transform target = targetsInViewRadius[i].transform;
            Vector2 dirToTarget = (target.position - transform.position).normalized;
            if (Vector2.Angle(transform.up, dirToTarget) < viewAngle / 2) {
                float distToTarget = Vector2.Distance(transform.position, target.position);
                if (!Physics2D.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask)) {
                    return true;
                }
            }
        }
        return false;
    }

    private void BecomeSuspicious() {
        if (_animal.State == States.Wander) {
            _animal.State = States.Alert;
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
        if (!angleIsGlobal) {
            angleInDegrees += transform.eulerAngles.z;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }
}