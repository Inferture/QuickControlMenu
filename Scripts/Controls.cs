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
        public static string up;
        public static string down;
        public static string left;
        public static string right;

        public static string dash;
        public static string magic;

        public static string select;
        public static string last;

        public static string option1;
        public static string option2;
        public static string option3;
        public static string option4;


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


        // Use this for initialization
        private void Start()
        {
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

        public static void Default()
        {
            PlayerPrefs.SetString("up", "UpArrow");
            PlayerPrefs.SetString("down", "DownArrow");
            PlayerPrefs.SetString("left", "LeftArrow");
            PlayerPrefs.SetString("right", "RightArrow");

            PlayerPrefs.SetString("magic", "W");
            PlayerPrefs.SetString("dash", "Space");

            PlayerPrefs.SetString("select", "Return");
            PlayerPrefs.SetString("last", "Escape");

            PlayerPrefs.SetString("option1", "Alpha1");
            PlayerPrefs.SetString("option2", "Alpha2");
            PlayerPrefs.SetString("option3", "Alpha3");
            PlayerPrefs.SetString("option4", "Alpha4");
            LoadControls();
        }

        public static float GetHDirection()
        {
            float vx = 0;
            if (Input.GetKey(KeyCode.RightArrow))
            {
                vx += 1;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                vx -= 1;
            }
            return vx;
        }

        public static bool GetJump()
        {
            return Input.GetKey(KeyCode.Space);
        }

        public static Vector2 GetMovement()
        {
            float vx = 0;
            float vy = 0;

            vx = Mathf.Max(GetActionAxisOrButtonValue("right"), 0) - Mathf.Max(GetActionAxisOrButtonValue("left"), 0);
            vy = Mathf.Max(GetActionAxisOrButtonValue("up"), 0) - Mathf.Max(GetActionAxisOrButtonValue("down"), 0);

            Vector2 v = new Vector2(vx, vy);
            if (v != Vector2.zero)
            {
                v = v.normalized;
            }
            return v;
        }


        public static bool GetDash()
        {
            return IsPressed(dash);
        }
        public static bool GetHit()
        {
            return Input.GetKeyDown(KeyCode.X);
        }
        public static bool GetShoot()
        {
            return IsPressed(magic);
        }
        public static bool GetSelect()
        {
            return IsPressedDown(select);
        }
        public static bool GetLast()
        {
            return IsPressedDown(last);
        }

        public static int GetOption()
        {
            if (GetAction("option1"))
            {
                return 1;
            }
            if (GetAction("option2"))
            {
                return 2;
            }
            if (GetAction("option3"))
            {
                return 3;
            }
            if (GetAction("option4"))
            {
                return 4;
            }
            return -1; //-1: pas encore choisi, 0: pas répondu au final
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

        public static bool Select()
        {
            if (LastMenu())
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
        public static bool LastMenu()
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

        public static string GetAxis()
        {

            float m = 0;
            int axis = 0;
            float a;
            for (int i = 1; i <= 30; i++)
            {
                a = GetAxisValue(i);
                if (a > m && a > threshold)
                {
                    m = a;
                    axis = i;
                }
            }
            if (axis == 0)
            {
                return null;
            }
            if (axis == 1)
            {
                return "Joystick 1 Axis X";
            }
            if (axis == 2)
            {
                return "Joystick 1 Axis Y";
            }
            if (axis == 29)
            {
                return "Mouse ScrollWheel Up";
            }
            if (axis == 30)
            {
                return "Mouse ScrollWheel Down";
            }
            else
            {
                return "Joystick 1 Axis " + axis.ToString();
            }
        }

        static float GetAxisValue(int axis)
        {
            if (axis == 1)
            {
                return Input.GetAxis("Joystick 1 Axis X");
            }
            if (axis == 2)
            {
                return Input.GetAxis("Joystick 1 Axis Y");
            }
            if (axis == 29)
            {
                return Input.GetAxis("Mouse ScrollWheel Up");
            }
            if (axis == 30)
            {
                return Input.GetAxis("Mouse ScrollWheel Down");
            }
            else
            {
                return Input.GetAxis("Joystick 1 Axis " + axis.ToString());
            }

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
            return Input.GetAxis(s) != 0;
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