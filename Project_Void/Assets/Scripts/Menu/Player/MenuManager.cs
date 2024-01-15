using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public PlayerMenu PlayerMenu; // gameObject/UI prefabs
    public MenuItem[] MenuItems; // item prefab array/collection

    // PlayerMenu (Prefabs)
    private GameObject playerMenuContainer; // prefab for main/primary container
    private GameObject playerMenuBox; // prefab container that holds menu buttons
    private GameObject playerSubMenuContainer; // prefab container for submenu
    private GameObject playerItemsContainer; // prefab container for inventory submenu items
    // PlayerMenu (Instantiated Objects)
    private GameObject menuContainer; // local ref for main/primary container (child of "this")
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
        //Debug.Log(playerMenuContainer.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) ToggleMenu(); // if TAB is pressed: open main/container menu
    }

    private void ToggleMenu ()
    {
        playerMenuContainer = PlayerMenu.PlayerMenuContainer;
        playerMenuBox = PlayerMenu.PlayerMenuBox;
        playerSubMenuContainer = PlayerMenu.PlayerSubMenuContainer;
        playerItemsContainer = PlayerMenu.PlayerItemsContainer;

        if (!PlayerMenu.menuIsInstantiated)
        {
            Instantiate(playerMenuContainer, this.transform);
            menuContainer = this.transform.Find($"{playerMenuContainer.name}(Clone)").gameObject;
            menuContainer.name = playerMenuContainer.name;
            Instantiate(playerMenuBox, menuContainer.transform);
            menuBox = menuContainer.transform.Find($"{playerMenuBox.name}(Clone)").gameObject;
            menuBox.name = playerMenuBox.name;

        }
        

        EventSystem.current.SetSelectedGameObject(null); // resets btn selection status
        // if menu is toggled and a submenu is currently open: destroy it & update corresponding status bool
        if (subMenuContainer != null)
        {
            Destroy(subMenuContainer);
            PlayerMenu.subMenuIsInstantiated = false;
        }
        menuContainer.SetActive(!menuContainer.activeInHierarchy); // toggle menu active status
    }

    public void ToggleSubMenu (Button btn)
    {
        if (PlayerMenu.subMenuIsInstantiated)
        {
            // if the previously instantiated submenu matches the current selection:
                // -> [1] reset the selection status 
                // -> [2] destroy exisiting submenu & update corresponding status bool 
                // -> [3] return (avoid new submenu instantiation
            if (subMenuContainer.name == btn.name)
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

        Instantiate(playerSubMenuContainer, menuContainer.transform); // instantiate new submenu as child to "menuContainer" gameObject
        subMenuContainer = menuContainer.transform.Find("PlayerSubMenu(Clone)").gameObject; // set to newly instantiated submenu
        subMenuContainer.name = btn.name; // change default "clone" name to match selected button's name

        PlayerMenu.subMenuIsInstantiated = true; // update corresponding status bool
        
        subMenuContents = subMenuContainer.transform.GetChild(0).gameObject;

        switch (subMenuContainer.name)
        {
            case "Inventory":
                // if the submenu requested is "Inventory":
                    // -> [1] instantiate the items container as a child of the "subMenuContents" gameObject
                    /* -> [2] set "PlayerItems" to the newly instantiated gameObject (items container clone) & update its data 
                    (name, color, etc.) */
                Instantiate(playerItemsContainer, subMenuContents.transform);
                playerItems = subMenuContents.transform.GetChild(0).gameObject;
                playerItems.name = "PlayerItems";

                subMenuContents.GetComponent<Image>().color = Color.red;
                
                // for each of the items ("MenuItems" component on "this"):
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
