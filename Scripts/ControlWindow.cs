using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;
using System.IO;
using System.Xml.Serialization;

namespace QuickControlMenu
{
    public enum InputTypeEditor
    {
        Key,
        Button,
        Axis
    }
    
    public enum AxisDevice
    {
        Mouse,
        AllJoysticks,
        Joystick1,
        Joystick2,
        Joystick3,
        Joystick4,
        Joystick5,
        Joystick6,
        Joystick7,
        Joystick8,
        Joystick9,
        Joystick10,
        Joystick11,
        Joystick12,
        Joystick13,
        Joystick14,
        Joystick15,
        Joystick16
    }
    public enum ButtonDevice
    {
        AllJoysticks,
        Joystick1,
        Joystick2,
        Joystick3,
        Joystick4,
        Joystick5,
        Joystick6,
        Joystick7,
        Joystick8,
        Joystick9,
        Joystick10,
        Joystick11,
        Joystick12,
        Joystick13,
        Joystick14,
        Joystick15,
        Joystick16
    }

    public enum JoystickButton
    {
        Button0,
        Button1,
        Button2,
        Button3,
        Button4,
        Button5,
        Button6,
        Button7,
        Button8,
        Button9,
        Button10,
        Button11,
        Button12,
        Button13,
        Button14,
        Button15,
        Button16,
        Button17,
        Button18,
        Button19,
        Button20,
        Button21,
        Button22,
        Button23,
        Button24,
        Button25,
        Button26,
        Button27,
        Button28
    }

    public enum MouseAxis
    {
        MouseScrollwheelUp,
        MouseScrollwheelDown,
    }
    public enum JoystickAxis
    {
        AxisX,
        AxisY,
        Axis3,
        Axis4,
        Axis5,
        Axis6,
        Axis7,
        Axis8,
        Axis9,
        Axis10,
        Axis11,
        Axis12,
        Axis13,
        Axis14,
        Axis15,
        Axis16,
        Axis17,
        Axis18,
        Axis19,
        Axis20,
        Axis21,
        Axis22,
        Axis23,
        Axis24,
        Axis25,
        Axis26,
        Axis27,
        Axis28,
    }

    


    [Serializable]
    public class Category
    {
        public string name;
        public string description;
        [XmlIgnore]
        public List<Action> actions;
        public Category()
        {

        }
        public Category(string name, string description)
        {
            this.name = name;
            this.description = description;
            this.actions = new List<Action>();
        }
        public Category(string name, string description, List<Action> actions)
        {
            this.name = name;
            this.description = description;
            this.actions = actions;
        }
        public void SetName(string nom)
        {
            name = nom;
        }
        public void SetDescription(string description)
        {
            this.description = description;
        }
        public void AddAction(Action a)
        {
            actions.Add(a);
        }
        public void RemoveAction(Action a)
        {
            actions.Remove(a);
        }
    }




    ///////////////EditorWindow\\\\\\\\\\\\\\\\\\\\



#if (UNITY_EDITOR)

    [Serializable]
    public class ControlWindow : EditorWindow
    {


        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        public List<Category> categories = new List<Category>();//static stops serialization
        public List<Action> actions = new List<Action>();

        string s;

        int waitingCategory = -1;
        int waitingAction = -1;
        int waitingKeynumber = -1;
        bool execution = false;
        bool error = false;
        List<bool> showActions = new List<bool>();

        Vector2 scrollPos = Vector2.zero;

        public void Restart()//static
        {
            actions = new List<Action>();
            categories = new List<Category>();
            s = "";
            waitingCategory = -1;
            waitingAction = -1;
            scrollPos = Vector2.zero;
        }

        public void LoadActions()//static
        {
            AssetDatabase.Refresh();
            Restart();
            TextAsset[] files = Resources.LoadAll<TextAsset>("CustomControls/XML");
            for (int i = 0; i < files.Length; i++)
            {
                Action a = XmlSerialization.ReadFromXmlResource<Action>(files[i]);
                actions.Add(a);
            }
            for (int i = 0; i < actions.Count; i++)
            {
                bool found = false;
                int j = 0;
                while (j < categories.Count && !found)
                {

                    if (categories[j].name.Equals(actions[i].category.name))
                    {
                        found = true;
                        if (categories[j].actions == null)
                        {
                            categories[j].actions = new List<Action>();
                        }
                        categories[j].AddAction(actions[i]);
                    }
                    j++;
                }
                if (!found)
                {
                    categories.Add(new Category(actions[i].category.name, actions[i].category.description));
                    Category newCat = categories[categories.Count - 1];
                    newCat.actions = new List<Action> { actions[i] };
                }
            }
            for (int i = 0; i < categories.Count; i++)
            {
                showActions.Add(true);
            }
        }

        [MenuItem("Window/QuickControlMenu/KeyMapping")]
        public static void ShowWindow()//static
        {
            EditorWindow.GetWindow(typeof(ControlWindow),false,"Actions list");
        }
        private void OnDisable()
        {
        }
        private void OnEnable()
        {
            LoadActions();
            EditorUtility.SetDirty(this);
        }










        private void OnGUI()
        {

            GUIStyle styleDelete = new GUIStyle(GUI.skin.button);
            styleDelete.normal.textColor = new Color(0.8f, 0.9f, 0.9f);


            if (GUILayout.Button("Reload (unsaved data will be lost) "))
            {
                LoadActions();
            }
            GUI.backgroundColor = new Color(0.4f, 0.75f, 0.4f);
            if (GUILayout.Button("Add Category"))
            {
                categories.Add(new Category("", "", new List<Action>()));
                showActions.Add(true);
            }
            GUI.backgroundColor = Color.white;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);


            using (GUILayout.HorizontalScope hsCategories = new GUILayout.HorizontalScope())
            {
                GUILayout.Space(20f);
                using (GUILayout.VerticalScope vsCategories = new GUILayout.VerticalScope())
                {

                    for (int i = 0; i < categories.Count; i++)
                    {
                        Rect categoryRect = new Rect(0, 0, position.width - 50, 200);


                        EditorGUILayout.LabelField(categories[i].name, EditorStyles.boldLabel);

                        s = EditorGUILayout.TextField("Name", categories[i].name);
                        categories[i].name = s;

                        s = EditorGUILayout.TextField("Description", categories[i].description);
                        categories[i].description = s;

                        showActions[i] = EditorGUILayout.Foldout(showActions[i], "Actions");
                        if (i >= showActions.Count || showActions[i])
                        {

                            using (GUILayout.HorizontalScope hsActions = new GUILayout.HorizontalScope())
                            {
                                GUILayout.Space(20f);

                                using (GUILayout.VerticalScope vsActions = new GUILayout.VerticalScope())
                                {
                                    for (int j = 0; j < categories[i].actions.Count; j++)
                                    {
                                        Action a = categories[i].actions[j];

                                        EditorGUILayout.LabelField(a.name, EditorStyles.boldLabel);

                                        a.id = EditorGUILayout.TextField("Action id", a.id);
                                        a.name = EditorGUILayout.TextField("Action name", a.name);
                                        a.information = EditorGUILayout.TextField("Info about the action", a.information);
                                        a.menuAction = (MenuAction)EditorGUILayout.EnumPopup("Action in the menu:", a.menuAction);
                                        a.keyAble = EditorGUILayout.Toggle("Allow key entries", a.keyAble);
                                        a.axisAble = EditorGUILayout.Toggle("Allow axis entries", a.axisAble);

                                        GUI.backgroundColor = new Color(0.75f, 0.75f, 1f);
                                        int numKeys = EditorGUILayout.IntField("Number of keys", a.keys.Count);

                                        using (GUILayout.HorizontalScope hsKeys = new GUILayout.HorizontalScope())
                                        {
                                            GUILayout.Space(20f);
                                            using (GUILayout.VerticalScope vsKeys = new GUILayout.VerticalScope())
                                            {

                                                GUILayout.Space(10f);
                                                for (int k = 0; k < numKeys; k++)
                                                {
                                                    if (k % 2 == 0)
                                                    {
                                                        GUI.backgroundColor = new Color(0.85f, 0.8f, 1f);
                                                    }
                                                    else
                                                    {
                                                        GUI.backgroundColor = new Color(0.8f, 0.9f, 1f);
                                                    }

                                                    while (a.keys.Count < numKeys)
                                                    {
                                                        a.keys.Add(new ActionKey());
                                                    }
                                                    while (a.keys.Count > numKeys)
                                                    {
                                                        a.keys.RemoveAt(a.keys.Count - 1);
                                                    }
                                                    GUILayout.Box(k.ToString());
                                                    a.keys[k] = new ActionKey((Action.InputType)EditorGUILayout.EnumPopup("Input type:", a.keys[k].inputType), a.keys[k].key);


                                                    if (waitingAction != j || waitingCategory != i || waitingKeynumber != k || a.keys[k].inputType != Action.InputType.Key)
                                                    {
                                                        EditorGUILayout.LabelField("Command:", a.keys[k].key);
                                                    }
                                                    else
                                                    {
                                                        GUI.SetNextControlName("waitInput" + i + "|" + j);
                                                        GUI.FocusControl("waitInput" + i + "|" + j);
                                                        EditorGUILayout.LabelField("Command:", "(Enter Key)");
                                                    }

                                                    if (a.keys[k].inputType == Action.InputType.Key)
                                                    {
                                                        if (GUILayout.Button("Enter Key", GUILayout.Width(100)) || (waitingAction == j && waitingCategory == i && waitingKeynumber == k))
                                                        {
                                                            waitingAction = j;
                                                            waitingCategory = i;
                                                            waitingKeynumber = k;
                                                            if (!a.keyAble && !a.axisAble)
                                                            {
                                                                waitingAction = -1;
                                                                waitingCategory = -1;
                                                                waitingKeynumber = -1;
                                                            }

                                                            Event e = Event.current;
                                                            if (e.type == EventType.KeyDown)
                                                            {
                                                                a.keys[k] = new ActionKey(a.keys[k].inputType, Event.current.keyCode.ToString());
                                                                waitingAction = -1;
                                                                waitingCategory = -1;
                                                                waitingKeynumber = -1;
                                                            }
                                                            if (e.type == EventType.MouseDown)
                                                            {

                                                                a.keys[k] = new ActionKey(a.keys[k].inputType, "Mouse" + e.button);
                                                                waitingAction = -1;
                                                                waitingCategory = -1;
                                                                waitingKeynumber = -1;
                                                            }
                                                            if (waitingAction < 0)
                                                            {
                                                                GUI.UnfocusWindow();
                                                            }
                                                        }

                                                    }
                                                    else if (a.keys[k].inputType == Action.InputType.Axis)
                                                    {
                                                        
                                                        //a.keys[k] = new ActionKey(a.keys[k].inputType, GetAxisValue((Axis)EditorGUILayout.EnumPopup("Axis:", GetValueAxis(a.keys[k].key))));
                                                        //TOMODIFYYY !!!

                                                        AxisDevice device = (AxisDevice)EditorGUILayout.EnumPopup("Device:", GetValueAxisDevice(a.keys[k].key));

                                                        //MouseAxis axisNum;
                                                        int axisNum=0;
                                                        if(device==AxisDevice.Mouse)
                                                        {
                                                            MouseAxis mouseAxis = (MouseAxis)EditorGUILayout.EnumPopup("Axis:", (MouseAxis)GetValueAxis(device,a.keys[k].key));
                                                            axisNum = (int)mouseAxis;
                                                            // a.keys[k] = new ActionKey(a.keys[k].inputType, GetAxisValue((Axis)EditorGUILayout.EnumPopup("Axis:", GetValueAxis(a.keys[k].key))));
                                                            a.keys[k] = new ActionKey(a.keys[k].inputType, GetAxisString(device, axisNum));
                                                        }
                                                        else
                                                        {
                                                            JoystickAxis joystickAxis = (JoystickAxis)EditorGUILayout.EnumPopup("Axis:", (JoystickAxis)GetValueAxis(device, a.keys[k].key));
                                                            axisNum = (int)joystickAxis;
                                                            bool inverted= (a.keys[k].key!=null && a.keys[k].key.EndsWith("-"));
                                                            inverted = EditorGUILayout.Toggle("Invert:",inverted);
                                                            a.keys[k] = new ActionKey(a.keys[k].inputType, GetAxisString(device, axisNum,inverted));
                                                            /*if (inverted)
                                                            {
                                                                a.keys[k] = new ActionKey(a.keys[k].inputType, GetAxisString(device, axisNum) +"-");
                                                            }
                                                            else
                                                            {
                                                                a.keys[k] = new ActionKey(a.keys[k].inputType, GetAxisString(device, axisNum));
                                                            }*/
                                                        }
                                                    }
                                                    else if (a.keys[k].inputType == Action.InputType.Button)
                                                    {
                                                        ButtonDevice device = (ButtonDevice)EditorGUILayout.EnumPopup("Device:", GetValueButtonDevice(a.keys[k].key));
                                                        JoystickButton button = (JoystickButton)EditorGUILayout.EnumPopup("Button:", GetValueButton(device, a.keys[k].key));

                                                        a.keys[k] = new ActionKey(a.keys[k].inputType, GetButtonString(device, button));
                                                        //a.keys[k] = new ActionKey(a.keys[k].inputType, GetButtonString((JoystickButton)EditorGUILayout.EnumPopup("Button:", GetValueButtonDevice(a.keys[k].key))));
                                                    }

                                                    GUILayout.Space(10f);
                                                }
                                            }
                                        }
                                        GUI.backgroundColor = Color.white;
                                        EditorGUILayout.Space();
                                        //Enter the key
                                        GUI.backgroundColor = new Color(0.9f, 0.5f, 0.5f);
                                        if (GUILayout.Button("Delete action " + a.name, styleDelete))
                                        {
                                            categories[i].RemoveAction(a);
                                        }
                                        GUI.backgroundColor = Color.white;
                                        EditorGUILayout.Space();
                                        EditorGUILayout.Space();

                                    }
                                }
                            }

                            GUI.backgroundColor = new Color(0.5f, 0.9f, 0.5f);
                            if (GUILayout.Button("Add action to " + categories[i].name))
                            {
                                categories[i].AddAction(new Action(""));
                            }
                            GUI.backgroundColor = Color.white;
                            EditorGUILayout.Space();

                        }

                        EditorGUILayout.Space();
                        GUI.backgroundColor = new Color(0.75f, 0.35f, 0.35f);
                        if (GUILayout.Button("Delete category " + categories[i].name, styleDelete))
                        {
                            categories.Remove(categories[i]);
                        }
                        GUI.backgroundColor = Color.white;
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();

                    }
                }
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Execute"))
            {
                execution = true;
                error = false;
            }
            if (execution)
            {


                actions = new List<Action>();
                for (int i = 0; i < categories.Count; i++)
                {
                    for (int j = 0; j < categories[i].actions.Count; j++)
                    {
                        categories[i].actions[j].category = new CategoryAction(categories[i].name, categories[i].description);
                        actions.Add(categories[i].actions[j]);
                    }
                }
                bool unicityActions = true;
                List<String> actionsIDs = new List<String>();
                for (int i = 0; i < actions.Count; i++)
                {
                    if (actionsIDs.Contains(actions[i].id))
                    {
                        unicityActions = false;
                        EditorGUILayout.HelpBox("Multiple actions with id: '" + actions[i].id + "'", MessageType.Error);
                        error = true;
                    }
                    if (actions[i].id.Equals(""))
                    {
                        EditorGUILayout.HelpBox("Action without id '" + actions[i].name + "'", MessageType.Error);
                        error = true;
                        unicityActions = false;
                    }
                    actionsIDs.Add(actions[i].id);

                }
                bool unicityCategories = true;
                List<string> categoriesNames = new List<string>();
                if (unicityActions)
                {
                    for (int i = 0; i < categories.Count; i++)
                    {
                        if (categoriesNames.Contains(categories[i].name))
                        {
                            unicityCategories = false;
                            EditorGUILayout.HelpBox("Multiple categories with name: '" + categories[i].name + "'. Please fuse them or change their name(s).", MessageType.Error);
                            error = true;

                        }
                        if (categories[i].name.Equals(""))
                        {
                            EditorGUILayout.HelpBox("Action without id '" + actions[i].name + "'", MessageType.Error);
                            error = true;
                            unicityActions = false;
                        }
                        categoriesNames.Add(categories[i].name);
                    }
                }
                if (error && unicityActions && unicityCategories)
                {
                    EditorGUILayout.HelpBox("Ready for execution (press execute again if you're feeling ready)", MessageType.None);
                }
                if (!error || (unicityActions && !unicityCategories && GUILayout.Button("Fuse categories")))
                {
                    EditorGUILayout.HelpBox("Executing...", MessageType.None);
                    for (int i = 0; i < categories.Count; i++)
                    {
                        categories[i].actions = new List<Action>();
                    }

                    if (!Directory.Exists(Path.Combine(Application.dataPath, "Resources")))
                    {
                        Directory.CreateDirectory(Path.Combine(Application.dataPath, "Resources"));
                    }
                    if (!Directory.Exists(Path.Combine(Application.dataPath, "Resources/CustomControls")))
                    {
                        Directory.CreateDirectory(Path.Combine(Application.dataPath, "Resources/CustomControls"));
                    }
                    if (!Directory.Exists(Path.Combine(Application.dataPath, "Resources/CustomControls/XML")))
                    {
                        Directory.CreateDirectory(Path.Combine(Application.dataPath, "Resources/CustomControls/XML"));
                    }

                    string[] files = Directory.GetFiles(Path.Combine(Application.dataPath, "Resources/CustomControls/XML"), "*.xml");
                    for (int i = 0; i < files.Length; i++)
                    {
                        File.Delete(files[i]);
                    }

                    for (int i = 0; i < actions.Count; i++)
                    {
                        string fileName = actions[i].id;
                        fileName = FormatFileName(fileName);
                        if (File.Exists(Path.Combine(Application.dataPath, "Resources/CustomControls/XML/" + fileName + ".xml")))
                        {
                            string newFileName = fileName + "1";
                            int num = 1;
                            while (File.Exists(Path.Combine(Application.dataPath, "Resources/CustomControls/XML/" + newFileName + ".xml")))
                            {
                                num++;
                                newFileName = fileName + num;
                            }
                            fileName = newFileName;
                        }

                        XmlSerialization.WriteToXmlResource<Action>(Path.Combine("CustomControls/XML/" + fileName + ".xml"), actions[i]);
                        actions[i].category = new CategoryAction(actions[i].category.name, actions[i].category.description);
                    }
                    LoadActions();
                    GenerateKeyStringsFile();
                    execution = false;
                }
            }

            this.Repaint();
        }


        public static string FormatFileName(string s)
        {
            string r = s;
            string forbiddenChars = "<>:?/*|\\\"";
            for (int i = 0; i < forbiddenChars.Length; i++)
            {
                r = r.Replace(forbiddenChars[i], '_');
            }
            return r;
        }

        public static string FormatVariableName(string s)
        {
            string r = s;
            for (int i = 0; i < r.Length; i++)
            {
                if (!Char.IsLetterOrDigit(r[i]))
                {
                    r = r.Replace(r[i], '_');
                }

            }
            return r;
        }
        public static string FormatString(string s)
        {
            string r = @s;
            r = r.Replace(@"\", @"\\");
            r = r.Replace("\"", "\\\"");
            return r;
        }
        public static bool Contains<T>(T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(value))
                {
                    return true;
                }
            }
            return false;
        }




        static string GetButtonString(ButtonDevice device, JoystickButton button)
        {
            int buttonNum = (int)button;
            int numDevice = (int)device;
            if(numDevice >= 1)//Joystick 1 to 16
            {
                return "Joystick" + numDevice + "Button" + buttonNum;
            }
            return "JoystickButton" + buttonNum; //All joysticks
        }

        static JoystickButton GetValueButton(ButtonDevice device, string button)
        {
            foreach (JoystickButton realButton in Enum.GetValues(typeof(JoystickButton)))
            {
                if (GetButtonString(device, realButton).Equals(button))
                {
                    return realButton;
                }
            }
            return JoystickButton.Button0;
        }

        static string GetAxisString(AxisDevice device, int realAxis, bool inverted=false)
        {
            int deviceNum = (int)device;
            int axis = realAxis + 1;
            string sign = "";
            if(inverted)
            {
                sign = "-";
            }
            if(deviceNum==0)//Mouse
            {
                //return GetAxisValue(realAxis);
                if(axis==1)
                {
                    return "Mouse ScrollWheel Up";
                }
                else
                {
                    return "Mouse ScrollWheel Down";
                }
            }
            else if(deviceNum==1)//All joysticks
            {
                if (axis == 1)
                {
                    return "Joystick Axis X" +sign;
                }
                if (axis == 2)
                {
                    return "Joystick Axis Y" + sign;
                }
                else
                {
                    return "Joystick Axis " + axis + sign;
                }
            }
            else//Joystick 1 to 16
            {
                if(axis==1)
                {
                    return "Joystick " + (deviceNum-1) + " Axis X" + sign;
                }
                if(axis==2)
                {
                    return "Joystick " + (deviceNum -1) + " Axis Y" + sign;
                }
                else
                {
                    return "Joystick " + (deviceNum -1) + " Axis " + axis + sign;
                }
            }
            
        }

        static string GetAxisString(int realAxis)
        {
            int axis = (int)realAxis + 1;
            if (axis == 1)
            {
                return "Mouse ScrollWheel Up";
            }
            if (axis == 2)
            {
                return "Mouse ScrollWheel Down";
            }

            int joyAxis = axis - 2 % 28;
            int numJoy = axis - 2 / 28  + 1;

            return GetAxisString((AxisDevice)numJoy, joyAxis);

        }
        /*static Axis GetValueAxis(string axis)
        {
            foreach (Axis realAxis in Enum.GetValues(typeof(Axis)))
            {
                if (GetAxisString(realAxis).Equals(axis))
                {
                    return realAxis;
                }
            }
            return Axis.AxisXJoystick1;
        }*/
        static int GetValueAxis(AxisDevice device, string axis)
        {
            if(device==AxisDevice.Mouse)
            {
                foreach (MouseAxis realAxis in Enum.GetValues(typeof(MouseAxis)))
                {
                    if (GetAxisString(AxisDevice.Mouse, (int)realAxis).Equals(axis))
                    {
                        return (int)realAxis;
                    }
                }
            }
            else
            {
                foreach (JoystickAxis realAxis in Enum.GetValues(typeof(JoystickAxis)))
                {
                    if (GetAxisString(device, (int)realAxis).Equals(axis) || (GetAxisString(device, (int)realAxis) +"-").Equals(axis))
                    {
                        return (int)realAxis;
                    }
                }
            }
            
            return 0;
        }

        static AxisDevice GetValueAxisDevice(string axis)
        {
            foreach (MouseAxis realAxis in Enum.GetValues(typeof(MouseAxis)))
            {
                if (GetAxisString(AxisDevice.Mouse, (int)realAxis).Equals(axis))
                {
                    return AxisDevice.Mouse;
                }
            }
            foreach (AxisDevice device in Enum.GetValues(typeof(AxisDevice)))
            {
                if (device != AxisDevice.Mouse)
                {
                    foreach (JoystickAxis realAxis in Enum.GetValues(typeof(JoystickAxis)))
                    {
                        if (GetAxisString(device, (int)realAxis).Equals(axis) || (GetAxisString(device, (int)realAxis) +"-").Equals(axis))
                        {
                            return device;
                        }
                    }
                }
            }

            return AxisDevice.Joystick1;
        }
        static ButtonDevice GetValueButtonDevice(string axis)
        {
            
            foreach (ButtonDevice device in Enum.GetValues(typeof(ButtonDevice)))
            {
                
                foreach (JoystickButton button in Enum.GetValues(typeof(JoystickButton)))
                {
                    if (GetButtonString(device, button).Equals(axis))
                    {
                        return device;
                    }
                }
                
            }

            return ButtonDevice.Joystick1;
        }

        string GenerateKeyStrings()
        {
            string start = "///Generated automatically\nnamespace QuickControlMenu{\npublic struct KeyStrings{ \n";
            string consts = "";
            List<string> actionsVariables = new List<string>();
            for (int i = 0; i < actions.Count; i++)
            {
                string variableName = FormatVariableName(actions[i].id);
                if (actionsVariables.Contains(variableName))
                {
                    string newName = variableName + "1";
                    int num = 1;
                    while (actionsVariables.Contains(newName))
                    {
                        num++;
                        newName = variableName + num;
                    }
                    variableName = newName;
                }
                consts += "public const string " + "key_" + variableName + "=\"" + FormatString(actions[i].id) + "\";\n";
                actionsVariables.Add(variableName);
            }
            string end = "}\n}\n";
            return start + consts + end;
        }

        void GenerateKeyStringsFile()
        {
            if (!Directory.Exists(Path.Combine(Application.dataPath, "QuickControlMenu")))
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "QuickControlMenu"));
            }
            if (!Directory.Exists(Path.Combine(Application.dataPath, "QuickControlMenu/GeneratedScripts")))
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "QuickControlMenu/GeneratedScripts"));
            }
            TextWriter writer = new StreamWriter(Path.Combine(Application.dataPath, "QuickControlMenu/GeneratedScripts/KeyStrings.cs"));
            writer.Write(GenerateKeyStrings());
            writer.Close();
            AssetDatabase.Refresh();
        }
        //[WrapperlessIcall]
        /* [MethodImpl(MethodImplOptions.InternalCall)]
         public static extern bool GetButtonn(string buttonName);*/


        //Write the code automatically gud ?

        /*
         mettre le tout dans un dll or something
         */

        /*
         Séparer les axes en Mouse/Joystick1/Joystick2...
         Enum pour choisir le joystick/mouse
         Choisir ensuite parmi les différents enum
         */
    }
#endif
}