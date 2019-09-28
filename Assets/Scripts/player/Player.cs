using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float Health { get; private set; }
    
    public static Player Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }
}
