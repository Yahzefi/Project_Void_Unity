using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject PlayerMenu; // main/container menu

    public GameObject SubMenuPrefab; // submenu prefab
    public GameObject MenuItemsContainerPrefab; // items container prefab
    public MenuItem[] MenuItems; // item prefab array/collection

    private GameObject PlayerSubMenu; // local ref to submenu (child of "PlayerMenu")
    private GameObject SubMenuContents; // local ref to "PlayerSubMenu" child (built into "SubMenuPrefab")
    private GameObject PlayerItems; // local ref to inventory items container (child of "SubMenuContents")
    private GameObject ItemContainer; // local ref used for each item/child within "PlayerItems"

    private ColorBlock btnColors; // variable for updating various colors via script

    private bool subMenuIsInstantiated; // used to determine how/when to add/remove child components from submenu

    // Start is called before the first frame update
    void Start()
    {
        PlayerMenu.SetActive(false); // on start: set menu objects to inactive
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) ToggleMenu(); // if TAB is pressed: open main/container menu
    }

    private void ToggleMenu ()
    {

        EventSystem.current.SetSelectedGameObject(null); // resets btn selection status
        // if menu is toggled and a submenu is currently open: destroy it & update corresponding status bool
        if (PlayerSubMenu != null)
        {
            Destroy(PlayerSubMenu);
            subMenuIsInstantiated = false;
        }
        PlayerMenu.SetActive(!PlayerMenu.activeInHierarchy); // toggle menu active status
    }

    public void ToggleSubMenu (Button btn)
    {
        if (subMenuIsInstantiated)
        {
            // if the previously instantiated submenu matches the current selection:
                // -> [1] reset the selection status 
                // -> [2] destroy exisiting submenu & update corresponding status bool 
                // -> [3] return (avoid new submenu instantiation
            if (PlayerSubMenu.name == btn.name)
            {
                EventSystem.current.SetSelectedGameObject(null);
                Destroy(PlayerSubMenu);
                subMenuIsInstantiated = false;
                return;
            }
            // otherwise: still destroy existing submenu & update corresponding status bool -> continue (instantiate new submenu)
            Destroy(PlayerSubMenu);
            subMenuIsInstantiated = false;
        }

        Instantiate(SubMenuPrefab, this.transform); // instantiate new submenu as child to PlayerMenu gameObject
        PlayerSubMenu = this.transform.Find("PlayerSubMenu(Clone)").gameObject; // set to newly instantiated submenu
        PlayerSubMenu.name = btn.name; // change default "clone" name to match selected button's name

        subMenuIsInstantiated = true; // update corresponding status bool
        
        SubMenuContents = PlayerSubMenu.transform.GetChild(0).gameObject;

        switch (PlayerSubMenu.name)
        {
            case "Inventory":
                // if the submenu requested is "Inventory":
                    // -> [1] instantiate the items container as a child of the "SubMenuContents" gameObject
                    /* -> [2] set "PlayerItems" to the newly instantiated gameObject (items container clone) & update its data 
                    (name, color, etc.) */
                Instantiate(MenuItemsContainerPrefab, SubMenuContents.transform);
                PlayerItems = SubMenuContents.transform.GetChild(0).gameObject;
                PlayerItems.name = "PlayerItems";

                SubMenuContents.GetComponent<Image>().color = Color.red;
                
                // for each of the items ("MenuItems" component on "PlayerMenu"(this)):
                    // -> [1] instantiate an item container as a child of the "PlayerItems" (items container) gameObject
                    /* -> [2] set "ItemContainer" to the newly instantiated gameObject (item container clone) & update its name 
                    [[NOTE]]: {item.number}'s starting value == 1 :: get the child of {item.number - 1} to match with correct index */
                    /* -> [3] instantiate the "itemName" & "itemCount" gameObjects as children of the "ItemContainer" gameObject 
                    & update both of their names */
                    // -> [4]
                foreach (MenuItem item in MenuItems)
                {
                    Instantiate(item.itemContainer, PlayerItems.transform);
                    ItemContainer = PlayerItems.transform.GetChild(item.number - 1).gameObject;
                    ItemContainer.name = item.itemContainer.name;
                    Instantiate(item.itemName, ItemContainer.transform);
                    Instantiate(item.itemCount, ItemContainer.transform);
                    ItemContainer.transform.GetChild(0).gameObject.name = item.itemName.name;
                    ItemContainer.transform.GetChild(1).gameObject.name = item.itemCount.name;

                    // check active bool and set active status accordingly
                    if (item.slotIsInactive) ItemContainer.SetActive(false);
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
