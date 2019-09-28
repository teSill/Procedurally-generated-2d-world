using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour {

    public static IEnumerator CallMethodAfterTime(Action onComplete, float waitTime) {
        yield return new WaitForSeconds(waitTime);
        onComplete();
    }

}
