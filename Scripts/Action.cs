using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

namespace QuickControlMenu
{
    public enum MenuAction
    {
        None,
        Up,
        Down,
        Left,
        Right,
        Select,
        Cancel
    }

    public struct CategoryAction
    {

        public string name;
        public string description;
        public CategoryAction(Category category)
        {
            name = category.name;
            description = category.description;
        }
        public CategoryAction(string name, string description = "")
        {
            this.name = name;
            this.description = description;
        }
    }

    public struct ActionKey
    {

        public Action.InputType inputType;
        public string key;
        public ActionKey(Action.InputType type, string key)
        {
            inputType = type;
            this.key = key;
        }
    }

    [Serializable]
    public class Action
    {

        [Serializable]
        public enum InputType
        {
            Key,
            Button,
            Axis,
        }
        public List<ActionKey> keys;
        public string id;

        public string name;//verbose name ex: Saut
        public string information;//What that key is for


        public CategoryAction category;
        public bool axisAble;
        //public bool buttonAble;//unusable now, equivalent to keyable
        public bool keyAble;


        public bool lastFrameActiveAxis; //Pour vérifier si l'axe est passé de passif à actif à cette frame

        public MenuAction menuAction;
        public Action()
        {
            keys = new List<ActionKey>();
        }

        public Action(string id)
        {
            this.id = id;
            this.axisAble = true;
            this.keyAble = true;
            keys = new List<ActionKey>();
        }
        public Action(string id, bool axisAble)
        {
            this.id = id;
            this.axisAble = axisAble;
            this.keyAble = true;
            keys = new List<ActionKey>();
        }
        public Action(string id, bool axisAble, bool keyAble)
        {
            this.id = id;
            this.axisAble = axisAble;
            this.keyAble = keyAble;
            keys = new List<ActionKey>();
        }

        public Action(string id, InputType type, string key)
        {
            this.id = id;
            this.axisAble = true;
            this.keyAble = true;
            this.keys = new List<ActionKey> { new ActionKey(type, key) };

        }
        public Action(string id, InputType type, string key, bool axisAble)
        {
            this.id = id;
            this.axisAble = axisAble;
            this.keyAble = true;

            this.keys = new List<ActionKey> { new ActionKey(type, key) };//new ActionKey(type, key);
        }
        public Action(string id, InputType type, string key, bool axisAble, bool keyAble)
        {
            this.id = id;
            this.axisAble = axisAble;
            this.keyAble = keyAble;
            this.keys = new List<ActionKey> { new ActionKey(type, key) };
        }
        public Action(string id, InputType type, string key, CategoryAction category, string name)
        {
            this.axisAble = true;
            this.keyAble = true;
            this.id = id;
            this.category = category;
            this.name = name;
            this.keys = new List<ActionKey> { new ActionKey(type, key) };
        }

        public void AddKey(ActionKey ak)
        {
            keys.Add(ak);
        }
        public void AddKey(InputType type, string key)
        {
            keys.Add(new ActionKey(type, key));
        }
        new public string ToString()
        {
            if (this == null)
            {
                if (id != null)
                {
                    return "nullAction" + id;
                }
                return "nullAction";
            }
            else if (keys.Count == 0)
            {
                return "Action: id=" + id + " keyless";
            }
            else
            {
                string s = "Action: id=" + id + " keys: ";
                foreach (ActionKey ak in keys)
                {
                    s += ak.key + "/";
                }
                return s;
            }

        }
    }
}

