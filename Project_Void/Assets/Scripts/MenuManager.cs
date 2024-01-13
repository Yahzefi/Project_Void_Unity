using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject PlayerMenu; // main/container menu
    public GameObject SubMenuPrefab;

    private GameObject PlayerSubMenu;
    private GameObject SubMenuContents;

    private ColorBlock btnColors; // variable for updating various colors via script

    private bool subMenuIsInstantiated;

    // Start is called before the first frame update
    void Start()
    {
        // on start: set menu objects to inactive
        PlayerMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) ToggleMenu(); // if TAB is pressed: open main/container menu
    }

    private void ToggleMenu ()
    {

        EventSystem.current.SetSelectedGameObject(null); // resets btn selection status

        PlayerMenu.SetActive(!PlayerMenu.activeInHierarchy);
    }

    public void ToggleSubMenu (Button btn)
    {
        if (subMenuIsInstantiated)
        {
            // if the previously instantiated submenu matches the current selection:
                // -> reset the selection status 
                // -> destroy exisiting submenu & update corresponding status bool 
                // -> return (avoid new submenu instantiation
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

        SubMenuContents = PlayerSubMenu.transform.GetChild(0).gameObject; // set to submenu's child gameObject

        switch (PlayerSubMenu.name)
        {
            case "Inventory":
                // instantiate contents material as child to SubMenuContents gameObject
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
