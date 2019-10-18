using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace QuickControlMenu
{
    public class OptionKey : Option
    {
        public string action;
        public bool joystick;
        public bool mouse;
        public string instructions;
        public GameObject titre;
        public float distanceTitre = 30;
        bool changedThisFrame = false;

        // Use this for initialization
        void Start()
        {
            GetComponent<Text>().text = PlayerPrefs.GetString(action, "<None>");
        }
        override public void Select()
        {
            selected = true;
        }
        // Update is called once per frame
        void Update()
        {
            changedThisFrame = false;
            Vector2 pos = transform.position;

            if (titre != null)
            {
                titre.transform.position = new Vector2(pos.x, pos.y + distanceTitre);
            }
            if (enabled)
            {
                GetComponent<Text>().color = new Color(enabledColor.r, enabledColor.g, enabledColor.b, GetComponent<Text>().color.a);
            }
            else
            {
                GetComponent<Text>().color = new Color(disabledColor.r, disabledColor.g, disabledColor.b, GetComponent<Text>().color.a);
            }

            if (selected)
            {
                Informations.SetInfo(instructions);
                GetComponent<Text>().text = "(Enter key)";
                foreach (KeyCode v in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(v) && !Input.GetButton("Start") && !(Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Return)))
                    {
                        GetComponent<Text>().text = v.ToString();
                        PlayerPrefs.SetString(action, v.ToString());
                        Controls.LoadControls();
                        selected = false;
                        Informations.SetInfo(instructions);
                        changedThisFrame = true;
                    }
                }
            }
            if (selected)
            {
                string axis = Controls.GetAxis();
                if (axis != null)
                {
                    GetComponent<Text>().text = axis;
                    PlayerPrefs.SetString(action, axis);
                    selected = false;
                }
                Informations.SetInfo(instructions);
            }

        }
        private void LateUpdate()
        {
            if (Input.GetKey(KeyCode.Escape) && selected)
            {
                Informations.SetInfo(information);
                selected = false;
                GetComponent<Text>().text = PlayerPrefs.GetString(action, "<None>");
            }
        }
    }
}