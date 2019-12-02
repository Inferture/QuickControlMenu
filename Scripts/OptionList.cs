using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuickControlMenu
{
    public class OptionList : Option
    {

        public string action;
        public string[] choix;
        public int numChoix = 0;

        // Use this for initialization
        void Start()
        {

        }
        override public void Select()
        {
            numChoix = (numChoix + 1) % choix.Length;
            Informations.SetInfo(information + "(currently " + choix[numChoix] + ")");
            selected = false;
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
            if (hovering)
            {
                Informations.SetInfo(information + "(currently " + choix[numChoix] + ")");
            }
            GetComponent<Text>().text = choix[numChoix];
            PlayerPrefs.SetString(action, choix[numChoix]);
        }
    }
}