using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMenu", menuName = "Player Menu", order = 3)]
public class PlayerMenu : ScriptableObject
{
    public GameObject PlayerMenuContainer;
    public GameObject PlayerMenuBox;
    public GameObject PlayerSubMenuContainer;
    public GameObject PlayerItemsContainer;

    public bool menuIsInstantiated; 
    public bool subMenuIsInstantiated;
}
