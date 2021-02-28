using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    // All Units that can be selected
    public SelectionSprite[] allObjects;

    // Start is called before the first frame update
    void Awake()
    {
        allObjects = FindObjectsOfType<SelectionSprite>();
    }
}
