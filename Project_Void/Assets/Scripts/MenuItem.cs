using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MenuItem", menuName = "Menu Item", order = 1)]
public class MenuItem : ScriptableObject
{
    public int number;
    public GameObject itemContainer;
    public GameObject itemName;
    public GameObject itemCount;

    public bool slotIsInactive;
}
