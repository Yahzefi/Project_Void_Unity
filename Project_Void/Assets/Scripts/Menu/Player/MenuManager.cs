using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public PlayerMenu PlayerMenu; // gameObject/UI prefabs
    public MenuItem[] MenuItems; // item prefab array/collection

    // PlayerMenu (Instantiated Objects)
    private GameObject menuContainer; // local ref for main/primary container (child of "GameUI")
    private GameObject menuBox; // local ref for container that holds menu buttons (child of "menuContainer")
    private GameObject subMenuContainer; // local ref for submenu container (child of "menuContainer")
    private GameObject subMenuContents; // local ref to submenu contents (child of "subMenuContainer")
    private GameObject playerItems; // local ref for menu items container (child of "subMenuContents")

    // MenuItems
    private GameObject itemContainer; // local ref used for each item/child within "playerItems"

    private ColorBlock btnColors; // variable for updating various colors via script

    // Start is called before the first frame update
    void Start()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) ToggleMenu(); // if TAB is pressed: open main/container menu
    }

    private void ToggleMenu ()
    {
        GameObject GameUI = GameObject.Find("GameUI"); // define canvas/UI gameObject

        // if the menu container hasn't yet been instantiated:
        // -> [1] instantiate it as a child of "GameUI" | def local ref as newly instantiated object | rename menu container
        // -> [2] instantiate menu box (holds buttons) as child of "menuContainer" | def local ref | rename menu box
        // -> [3] update "menuIsInstantiated" status bool
        if (!PlayerMenu.menuIsInstantiated)
        {
            Instantiate(PlayerMenu.PlayerMenuContainer, GameUI.transform);
            menuContainer = GameUI.transform.Find($"{PlayerMenu.PlayerMenuContainer.name}(Clone)").gameObject;
            menuContainer.name = PlayerMenu.PlayerMenuContainer.name;
            Instantiate(PlayerMenu.PlayerMenuBox, menuContainer.transform);
            menuBox = menuContainer.transform.Find($"{PlayerMenu.PlayerMenuBox.name}(Clone)").gameObject;
            menuBox.name = PlayerMenu.PlayerMenuBox.name;

            PlayerMenu.menuIsInstantiated = true;
        }
        // if the menu container is already instantiated:
        // -> [1] redefine menu container local ref (after renaming)
        // -> [2] if the submenu is also already instantiated: def local ref "subMenuContainer"
            // -> [2a] if the submenu container isn't null: remove the submenu container & update the "subMenuIsInstantiated" status bool
        // -> [3] remove the menu container & update the "menuIsInstantiated" status bool
        else
        {
            menuContainer = GameUI.transform.Find($"{PlayerMenu.PlayerMenuContainer.name}").gameObject;
            if (PlayerMenu.subMenuIsInstantiated) subMenuContainer = menuContainer.transform.Find($"{PlayerMenu.PlayerSubMenuContainer.name}").gameObject;
            if (subMenuContainer != null)
            {
                Destroy(subMenuContainer);
                PlayerMenu.subMenuIsInstantiated = false;
            }
            Destroy(menuContainer);
            PlayerMenu.menuIsInstantiated = false;
        }
        EventSystem.current.SetSelectedGameObject(null); // resets btn selection status      
    }

    public void ToggleSubMenu (Button btn)
    {
        GameObject GameUI = GameObject.Find("GameUI"); // def canvas/UI gameObject
        menuContainer = GameUI.transform.Find($"{PlayerMenu.PlayerMenuContainer.name}").gameObject; // def local ref (child of "GameUI")

        if (PlayerMenu.subMenuIsInstantiated)
        {
            // redefine refs to submenu gameObjects (after they've been renamed)
            subMenuContainer = menuContainer.transform.Find($"{PlayerMenu.PlayerSubMenuContainer.name}").gameObject;
            subMenuContents = subMenuContainer.transform.GetChild(0).gameObject;
            // if the previously instantiated submenu matches the current selection:
                // -> [1] reset the selection status 
                // -> [2] destroy exisiting submenu & update corresponding status bool 
                // -> [3] return (avoid new submenu instantiation)
            if (subMenuContents.name == btn.name)
            {
                EventSystem.current.SetSelectedGameObject(null);
                Destroy(subMenuContainer);
                PlayerMenu.subMenuIsInstantiated = false;
                return;
            }
            // otherwise: still destroy existing submenu & update corresponding status bool -> continue (instantiate new submenu)
            Destroy(subMenuContainer);
            PlayerMenu.subMenuIsInstantiated = false;
        }

        Instantiate(PlayerMenu.PlayerSubMenuContainer, menuContainer.transform); // instantiate new submenu as child to "menuContainer" gameObject
        subMenuContainer = menuContainer.transform.Find($"{PlayerMenu.PlayerSubMenuContainer.name}(Clone)").gameObject; // set to newly instantiated submenu
        subMenuContainer.name = PlayerMenu.PlayerSubMenuContainer.name;

        PlayerMenu.subMenuIsInstantiated = true; // update corresponding status bool
        
        subMenuContents = subMenuContainer.transform.GetChild(0).gameObject;
        subMenuContents.name = btn.name; // rename submenu contents for future ref (above)

        switch (btn.name)
        {
            case "Inventory":
                // if the submenu requested is "Inventory":
                    // -> [1] instantiate the items container as a child of the "subMenuContents" gameObject
                    /* -> [2] set "PlayerItems" to the newly instantiated gameObject (items container clone) & update its data 
                    (name, color, etc.) */
                Instantiate(PlayerMenu.PlayerItemsContainer, subMenuContents.transform);
                playerItems = subMenuContents.transform.GetChild(0).gameObject;
                playerItems.name = "PlayerItems";

                subMenuContents.GetComponent<Image>().color = Color.red;
                
                // for each of the items ("MenuItems" component on "GameUI"):
                    // -> [1] instantiate an item container as a child of the "playerItems" (items container) gameObject
                    /* -> [2] set "itemContainer" to the newly instantiated gameObject (item container clone) & update its name 
                    [[NOTE]]: {item.number}'s starting value == 1 :: get the child of {item.number - 1} to match with correct index */
                    /* -> [3] instantiate the "itemName" & "itemCount" gameObjects as children of the "itemContainer" gameObject 
                    & update both of their names */
                    // -> [4]
                foreach (MenuItem item in MenuItems)
                {
                    Instantiate(item.itemContainer, playerItems.transform);
                    itemContainer = playerItems.transform.GetChild(item.number - 1).gameObject;
                    itemContainer.name = item.itemContainer.name;
                    Instantiate(item.itemName, itemContainer.transform);
                    Instantiate(item.itemCount, itemContainer.transform);
                    itemContainer.transform.GetChild(0).gameObject.name = item.itemName.name;
                    itemContainer.transform.GetChild(1).gameObject.name = item.itemCount.name;

                    // check active bool and set active status accordingly
                    if (item.slotIsInactive) itemContainer.SetActive(false);
                }

                break;
            case "Profile":
                // instantiate contents material as child to SubMenuContents gameObject
                break;
            case "Save":
                // instantiate contents material as child to SubMenuContents gameObject
                break;
            default:
                break;
        }

        // button color scripting
        btnColors = btn.colors;
        btnColors.selectedColor = new Color32(0, 0, 175, 255);
        btn.colors = btnColors;

    }

    //

}
