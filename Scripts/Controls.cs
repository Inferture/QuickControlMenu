using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



namespace QuickControlMenu
{
    public class Controls : MonoBehaviour
    {

        public static int numAlternates = 4;
        public static float threshold;
        public const string PREFIXE_ACTION = "";//Préfixe des actions dans les PlayerPrefs

        public static List<Action> actions = new List<Action>();
        public static Dictionary<string, Action> dictActions = new Dictionary<string, Action>();


        public static void Initialize()
        {
            TextAsset[] files = Resources.LoadAll<TextAsset>("CustomControls/XML");
            for (int i = 0; i < files.Length; i++)
            {

                Action a = XmlSerialization.ReadFromXmlResource<Action>(files[i]);
                actions.Add(a);
            }
            foreach (Action a in actions)
            {
                dictActions.Add(a.id, a);
            }
            LoadControls();
        }



        public static void LoadControls()
        {
            foreach (Action a in actions)
            {
                int i = 0;
                while (i < numAlternates && (i < a.keys.Count && PlayerPrefs.GetString(PREFIXE_ACTION + a.id + "-" + i, a.keys[i].key) != null) || (PlayerPrefs.GetString(PREFIXE_ACTION + a.id + "-" + i, "") != "" && i < 99))
                {
                    if (i < a.keys.Count)
                    {
                        Action.InputType type = (Action.InputType)PlayerPrefs.GetInt(PREFIXE_ACTION + a.id + "-" + i + "-" + "type", (int)a.keys[i].inputType);
                        a.keys[i] = new ActionKey(type, PlayerPrefs.GetString(PREFIXE_ACTION + a.id + "-" + i, a.keys[i].key));
                        i++;
                    }
                    else
                    {
                        Action.InputType type = (Action.InputType)PlayerPrefs.GetInt(PREFIXE_ACTION + a.id + "-" + i + "-" + "type");
                        a.keys.Add(new ActionKey(type, PlayerPrefs.GetString(PREFIXE_ACTION + a.id + "-" + i)));
                        i++;
                    }
                }
            }
        }

        private void LateUpdate()
        {
            foreach (Action a in actions)
            {
                bool active = false;
                foreach (ActionKey ak in a.keys)
                {
                    if (ak.inputType == Action.InputType.Axis && Mathf.Abs(Input.GetAxis(ak.key)) > threshold)
                    {
                        active = true;
                    }
                }
                a.lastFrameActiveAxis = active;
            }
        }

        /// <summary>
        /// Not implemented yet.
        /// </summary>
        public static void Default()
        {
            LoadControls();
        }

        
        public static bool UpMenu()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                return true;
            }
            if (!Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (!Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.Return))
                {
                    foreach (Action a in actions)
                    {
                        if (a.menuAction == MenuAction.Up && GetActionAxisOrButtonValueDown(a.id) > 0 || a.menuAction == MenuAction.Down && GetActionAxisOrButtonValueDown(a.id) < 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool DownMenu()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                return true;
            }
            if (!Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (!Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.Return))
                {
                    foreach (Action a in actions)
                    {
                        if (a.menuAction == MenuAction.Down && GetActionAxisOrButtonValueDown(a.id) > 0 || a.menuAction == MenuAction.Up && GetActionAxisOrButtonValueDown(a.id) < 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool LeftMenu()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                return true;
            }
            if (!Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (!Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.Return))
                {
                    foreach (Action a in actions)
                    {
                        if ((a.menuAction == MenuAction.Left && GetActionAxisOrButtonValueDown(a.id) > 0) || (a.menuAction == MenuAction.Right && GetActionAxisOrButtonValueDown(a.id) < 0))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static bool RightMenu()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                return true;
            }
            if (!Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (!Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.Return))
                {
                    foreach (Action a in actions)
                    {
                        if (a.menuAction == MenuAction.Right && GetActionAxisOrButtonValueDown(a.id) > 0 || a.menuAction == MenuAction.Left && GetActionAxisOrButtonValueDown(a.id) < 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool SelectMenu()
        {
            if (CancelMenu())
            {
                return false;
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                return true;
            }
            if (!Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (!Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.Escape))
                {
                    foreach (Action a in actions)
                    {
                        if (a.menuAction == MenuAction.Select && GetActionDown(a.id))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static bool CancelMenu()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                return true;
            }
            if (!Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (!Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.Return))
                {
                    foreach (Action a in actions)
                    {
                        if (a.menuAction == MenuAction.Cancel && GetActionDown(a.id))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        public static string GetAxisString(int axis)
        {
            int devAxis;
            int numJoy;
            if(axis<=0)
            {
                return null;
            }
            if (axis <= 2)
            {
                devAxis = axis;
                numJoy = 0;//Mouse
            }
            else
            {
                devAxis = (axis - 3) % 28+1;
                numJoy = (axis - 3) / 28 + 1;
            }
            if(axis>2+16*28)
            {
                return GetAxisString(axis - (16 * 28)) + "-";
            }

            return GetAxisString((AxisDevice)numJoy, devAxis);
        }
        static string GetAxisString(AxisDevice device, int realAxis)
        {
            int deviceNum = (int)device;
            int axis = (int)realAxis + 0;
            if (deviceNum == 0)//Mouse
            {
                //return GetAxisValue(realAxis);
                if (axis == 1)
                {
                    return "Mouse ScrollWheel Up";
                }
                else
                {
                    return "Mouse ScrollWheel Down";
                }
            }
            else//Joystick 1 to 16
            {
                if (axis == 1)
                {
                    return "Joystick " + deviceNum + " Axis X";
                }
                if (axis == 2)
                {
                    return "Joystick " + deviceNum + " Axis Y";
                }
                else
                {
                    return "Joystick " + deviceNum + " Axis " + axis;
                }
                //return GetAxisValue(2 + (deviceNum - 1) * 28 + realAxis);
            }
        }
        public static string GetAxis()
        {
            float m = 0;
            int axis = 0;
            float a;
            for (int i = 1; i <= 2+28*16 * 2 ; i++)
            {
                a = GetAxisValue(i);
                if (a > m && a > threshold)
                {
                    m = a;
                    axis = i;
                }
            }
            return GetAxisString(axis);

        }
        static float GetAxisValue(int realAxis)
        {
            return Input.GetAxis(GetAxisString(realAxis));
        }


        public static bool AxisAvailableActive(string s)
        {
            try
            {
                Input.GetAxis(s);
            }
            catch
            {
                return false;
            }
            return Input.GetAxis(s) > 0;
        }
        public static bool AxisAvailable(string s)
        {
            try
            {
                Input.GetAxis(s);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool KeyAvailable(string s)
        {
            try
            {
                Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), s));
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool ButtonAvailable(string s)
        {
            try
            {
                Input.GetButton(s);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool IsPressed(string s)
        {
            if (ButtonAvailable(s) && Input.GetButton(s))
            {
                return Input.GetButton(s);
            }
            else if (AxisAvailableActive(s))
            {
                return true;
            }
            else if (KeyAvailable(s))
            {
                return Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), s));
            }
            return false;
        }

        public static bool IsPressedDown(string s)
        {
            if (ButtonAvailable(s) && Input.GetButton(s))
            {
                return Input.GetButtonDown(s);
            }
            else if (KeyAvailable(s))
            {
                return Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), s));
            }
            else if (AxisAvailableActive(s))//Not good at all but hey 
            {
                return true;
            }
            return false;
        }

        public static bool GetAction(string id)
        {
            Action action = dictActions[id];

            foreach (ActionKey ak in action.keys)
            {
                if (ak.inputType == Action.InputType.Axis && AxisAvailableActive(ak.key))
                {
                    return true;
                }
                else if (ak.inputType == Action.InputType.Button && Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), ak.key)))
                {
                    return true;
                }
                else if (ak.inputType == Action.InputType.Key && Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), ak.key)))
                {
                    return true;
                }
            }

            return false;
        }

        public static float GetActionAxisValue(string id)
        {
            Action action = dictActions[id];
            float maxValue = 0;//can be negative, max in absolute
            foreach (ActionKey ak in action.keys)
            {
                if (ak.inputType == Action.InputType.Axis && Mathf.Abs(Input.GetAxis(ak.key)) > Mathf.Abs(maxValue))
                {
                    maxValue = Input.GetAxis(ak.key);
                }
            }
            return maxValue;
            //Raise an exception if there is no axis found?

        }


        public static float GetActionAxisOrButtonValue(string id, float buttonActiveValue = 1, float buttonInactiveValue = 0)
        {
            Action action = dictActions[id];
            float maxValue = 0;
            bool activeChecked = false;
            foreach (ActionKey ak in action.keys)
            {
                if (ak.inputType == Action.InputType.Axis && Mathf.Abs(Input.GetAxis(ak.key)) > Mathf.Abs(maxValue))
                {
                    maxValue = Input.GetAxis(ak.key);
                    activeChecked = true;
                }
                else if ((ak.inputType == Action.InputType.Key || ak.inputType == Action.InputType.Button) && Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), ak.key)) && Mathf.Abs(buttonActiveValue) >= Mathf.Abs(maxValue))
                {
                    maxValue = buttonActiveValue;
                    activeChecked = true;
                }
            }
            if (!activeChecked)
            {
                maxValue = buttonInactiveValue;
            }
            return maxValue;

        }

        public static float GetActionAxisOrButtonValueDown(string id, float buttonActiveValue = 1, float buttonInactiveValue = 0)
        {
            Action action = dictActions[id];
            float maxValue = 0;
            bool activeChecked = false;
            foreach (ActionKey ak in action.keys)
            {
                if (ak.inputType == Action.InputType.Axis && !action.lastFrameActiveAxis && (Mathf.Abs(Input.GetAxis(ak.key)) > Mathf.Max(Mathf.Abs(maxValue), threshold)))
                {
                    maxValue = Input.GetAxis(ak.key);
                    activeChecked = true;
                }
                else if (ak.inputType == Action.InputType.Key || ak.inputType == Action.InputType.Button)
                {
                    if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), ak.key)) && (Mathf.Abs(buttonActiveValue) >= Mathf.Abs(maxValue)))
                    {
                        maxValue = buttonActiveValue;
                        activeChecked = true;
                    }
                }
            }

            if (!activeChecked)
            {
                maxValue = buttonInactiveValue;
            }
            return maxValue;
        }


        public static bool GetActionDown(string id)
        {
            if (!dictActions.ContainsKey(id))
            {
                return false;
            }
            Action action = dictActions[id];

            foreach (ActionKey ak in action.keys)
            {
                if (ak.inputType == Action.InputType.Axis && !action.lastFrameActiveAxis && AxisAvailableActive(ak.key))
                {
                    return true;
                }
                if (ak.inputType == Action.InputType.Axis && !AxisAvailableActive(ak.key))
                {
                }
                else if (ak.inputType == Action.InputType.Key || ak.inputType == Action.InputType.Button)//)
                {
                    if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), ak.key)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}