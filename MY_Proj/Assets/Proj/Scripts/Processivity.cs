using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Processivity : MonoBehaviour
{
    public static int processivity = 0;

    void Start() {
        processivity = 0;
    }

    public int GetProcessivity() {
        return processivity;
    }

    public void SetProcessivity(int value) {
        processivity = value;
        Debug.Log("Processivity : " + processivity);
    }
}
