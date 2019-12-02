using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuickControlMenu
{
    public class Option : MonoBehaviour
    {

        public GameObject parentMenu;
        public GameObject menu;
        public string information;
        public bool selected;
        public bool hovering;
        public bool isEnabled = true;
        public GameObject selectedObject;
        public Color enabledColor = new Color(1, 1, 1);
        public Color disabledColor = new Color(0.5f, 0.5f, 0.5f);

        // Use this for initialization
        void Start()
        {
            if (parentMenu == null && transform.parent != null && transform.parent.GetComponent<Menu>() != null)
            {
                parentMenu = transform.parent.gameObject;
            }
            if (menu == null)
            {
                menu = transform.parent.gameObject;
            }
        }

        virtual public void Select()
        {
            if (menu != null)
            {
                parentMenu.GetComponent<Menu>().Disappear(true);
                menu.GetComponent<Menu>().Appear();
            }
        }
        virtual public void Hover()
        {
            hovering = true;
        }
        // Update is called once per frame
        void Update()
        {
            if (isEnabled)
            {
                GetComponent<Text>().color = new Color(enabledColor.r, enabledColor.g, enabledColor.b, GetComponent<Text>().color.a);
            }
            else
            {
                GetComponent<Text>().color = new Color(disabledColor.r, disabledColor.g, disabledColor.b, GetComponent<Text>().color.a);
            }
        }
    }
}