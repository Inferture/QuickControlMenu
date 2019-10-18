using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace QuickControlMenu
{
    public class OptionAction : Option
    {
        public Action action;
        public GameObject title;
        public List<GameObject> alternates = new List<GameObject>();
        public int verticalSpace = 50;
        int selectedNum;
        bool endSelect = false;
        //Titre, information

        // Use this for initialization
        void Start()
        {
            if (action.keys.Count == 0)
            {
                GetComponent<Text>().text = "<None>";
            }
            else
            {
                GetComponent<Text>().text = action.keys[0].key;
            }
            title.GetComponent<Text>().text = action.name;
            title.transform.SetAsFirstSibling();
            alternates.Add(gameObject);
            for (int i = 1; i < action.keys.Count; i++)
            {
                GameObject act;
                act = new GameObject();
                act.SetActive(false);
                act.AddComponent<CanvasRenderer>();
                Text textAction = act.AddComponent<Text>();
                act.AddComponent<Mask>();

                textAction.resizeTextForBestFit = true;
                textAction.rectTransform.sizeDelta = new Vector2(176, 32);
                textAction.font = GetComponent<Text>().font;
                textAction.alignment = TextAnchor.MiddleCenter;

                textAction.text = action.keys[i].key;
                act.transform.parent = transform.parent;
                act.transform.position = new Vector2(transform.position.x, transform.position.y - i * verticalSpace);
                alternates.Add(act);
                act.SetActive(true);
            }

            if (alternates.Count < Controls.numAlternates)
            {
                AddEmpty();
            }
        }
        void AddEmpty()
        {
            GameObject act;
            act = new GameObject();
            act.SetActive(false);
            act.AddComponent<CanvasRenderer>();
            Text textAction = act.AddComponent<Text>();
            act.AddComponent<Mask>();

            textAction.resizeTextForBestFit = true;
            textAction.rectTransform.sizeDelta = new Vector2(176, 32);
            textAction.font = GetComponent<Text>().font;
            textAction.alignment = TextAnchor.MiddleCenter;

            textAction.text = "<None>";
            act.transform.parent = transform.parent;
            act.transform.position = new Vector2(transform.position.x, transform.position.y - alternates.Count * verticalSpace);
            alternates.Add(act);
            act.SetActive(true);
        }
        override public void Select()
        {
            selected = true;
        }

        override public void Hover()
        {
            Vector2 localpos = selectedObject.transform.localPosition;

            selectedObject.transform.SetParent(alternates[selectedNum].transform);

            selectedObject.transform.localPosition = localpos;
            hovering = true;

        }
        // Update is called once per frame
        void Update()
        {
            if (hovering && !selected)
            {
                if (Controls.DownMenu())
                {
                    if (alternates.Count > 0)
                    {
                        Sounds.Play("sound_menu_change");
                    }
                    selectedNum = (selectedNum + 1) % (alternates.Count);

                    Vector2 localpos = selectedObject.transform.localPosition;
                    selectedObject.transform.SetParent(alternates[selectedNum].transform);

                    selectedObject.transform.localPosition = localpos;

                }
                if (Controls.UpMenu())
                {
                    if (alternates.Count > 0)
                    {
                        Sounds.Play("sound_menu_change");
                    }
                    selectedNum = (selectedNum - 1 + (alternates.Count)) % (alternates.Count);

                    Vector2 localpos = selectedObject.transform.localPosition;
                    selectedObject.transform.SetParent(alternates[selectedNum].transform);
                    selectedObject.transform.localPosition = localpos;
                }
            }


            if (selected && action.keyAble)
            {

                alternates[selectedNum].GetComponent<Text>().text = "(Enter key)";
                if (!Controls.Select())
                {
                    endSelect = true;
                }
                if (!Controls.Select() || endSelect)
                {

                    foreach (KeyCode v in Enum.GetValues(typeof(KeyCode)))
                    {
                        if (Input.GetKeyDown(v))//
                        {
                            alternates[selectedNum].GetComponent<Text>().text = v.ToString();
                            PlayerPrefs.SetString(Controls.PREFIXE_ACTION + action.id + "-" + selectedNum, v.ToString());
                            PlayerPrefs.SetInt(Controls.PREFIXE_ACTION + action.id + "-" + selectedNum + "-" + "type", (int)Action.InputType.Key);
                            selected = false;

                            if (selectedNum == alternates.Count - 1 && action.keys.Count <= selectedNum)
                            {
                                action.AddKey(new ActionKey());

                            }
                            if (alternates.Count < Controls.numAlternates)
                            {
                                AddEmpty();
                            }
                            Controls.LoadControls();
                            endSelect = false;
                        }
                    }
                }
            }
            if (selected && action.axisAble)
            {
                alternates[selectedNum].GetComponent<Text>().text = "(Enter axis)";
                string axis = Controls.GetAxis();
                if (axis != null)
                {
                    alternates[selectedNum].GetComponent<Text>().text = axis;
                    PlayerPrefs.SetString(Controls.PREFIXE_ACTION + action.id + "-" + selectedNum, axis);
                    PlayerPrefs.SetInt(Controls.PREFIXE_ACTION + action.id + "-" + selectedNum + "-" + "type", (int)Action.InputType.Axis);
                    selected = false;

                    if (selectedNum == alternates.Count - 1 && alternates.Count < Controls.numAlternates)
                    {
                        action.AddKey(new ActionKey(Action.InputType.Axis, axis));
                        AddEmpty();
                    }
                    Controls.LoadControls();
                }
                Informations.SetInfo(action.information);
            }
            if(hovering)
            {
                Informations.SetInfo(action.information);
            }

        }
    }
}