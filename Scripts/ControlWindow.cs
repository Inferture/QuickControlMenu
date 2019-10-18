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
    public enum JoystickButton
    {
        Joystick1Button0,
        Joystick1Button1,
        Joystick1Button2,
        Joystick1Button3,
        Joystick1Button4,
        Joystick1Button5,
        Joystick1Button6,
        Joystick1Button7,
        Joystick1Button8,
        Joystick1Button9,
        Joystick1Button10,
        Joystick1Button11,
        Joystick1Button12,
        Joystick1Button13,
        Joystick1Button14,
        Joystick1Button15,
        Joystick1Button16,
        Joystick1Button17,
        Joystick1Button18,
        Joystick1Button19,
        Joystick1Button20,
        Joystick1Button21,
        Joystick1Button22,
        Joystick1Button23,
        Joystick1Button24,
        Joystick1Button25,
        Joystick1Button26,
        Joystick1Button27,
        Joystick1Button28,
    }
    public enum Axis
    {
        AxisXJoystick1,
        AxisYJoystick1,
        Axis3Joystick1,
        Axis4Joystick1,
        Axis5Joystick1,
        Axis6Joystick1,
        Axis7Joystick1,
        Axis8Joystick1,
        Axis9Joystick1,
        Axis10Joystick1,
        Axis11Joystick1,
        Axis12Joystick1,
        Axis13Joystick1,
        Axis14Joystick1,
        Axis15Joystick1,
        Axis16Joystick1,
        Axis17Joystick1,
        Axis18Joystick1,
        Axis19Joystick1,
        Axis20Joystick1,
        Axis21Joystick1,
        Axis22Joystick1,
        Axis23Joystick1,
        Axis24Joystick1,
        Axis25Joystick1,
        Axis26Joystick1,
        Axis27Joystick1,
        Axis28Joystick1,
        MouseScrollwheelUp,
        MouseScrollwheelDown,
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
                                                            if (a.keyAble)
                                                            {
                                                                foreach (KeyCode v in Enum.GetValues(typeof(KeyCode)))
                                                                {
                                                                    if (Input.GetKeyDown(v) && !(Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Return)))
                                                                    {
                                                                        a.keys[k] = new ActionKey(a.keys[k].inputType, v.ToString());
                                                                        waitingAction = -1;
                                                                        waitingCategory = -1;
                                                                        waitingKeynumber = -1;
                                                                    }
                                                                }
                                                            }
                                                            if (a.axisAble)
                                                            {
                                                                string axis = Controls.GetAxis();
                                                                if (axis != null)
                                                                {
                                                                    a.keys[k] = new ActionKey(a.keys[k].inputType, axis);
                                                                    waitingAction = -1;
                                                                    waitingCategory = -1;
                                                                    waitingKeynumber = -1;
                                                                }
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
                                                                if (e.button == 0)
                                                                {
                                                                    a.keys[k] = new ActionKey(a.keys[k].inputType, "Mouse0");
                                                                    waitingAction = -1;
                                                                    waitingCategory = -1;
                                                                    waitingKeynumber = -1;
                                                                }
                                                                if (e.button == 1)
                                                                {
                                                                    a.keys[k] = new ActionKey(a.keys[k].inputType, "Mouse1");
                                                                    waitingAction = -1;
                                                                    waitingCategory = -1;
                                                                    waitingKeynumber = -1;
                                                                }
                                                                if (e.button == 2)
                                                                {
                                                                    a.keys[k] = new ActionKey(a.keys[k].inputType, "Mouse2");
                                                                    waitingAction = -1;
                                                                    waitingCategory = -1;
                                                                    waitingKeynumber = -1;
                                                                }
                                                            }
                                                            if (waitingAction < 0)
                                                            {
                                                                GUI.UnfocusWindow();
                                                            }
                                                        }

                                                    }
                                                    else if (a.keys[k].inputType == Action.InputType.Axis)
                                                    {
                                                        a.keys[k] = new ActionKey(a.keys[k].inputType, GetAxisValue((Axis)EditorGUILayout.EnumPopup("Axis:", GetValueAxis(a.keys[k].key))));
                                                    }
                                                    else if (a.keys[k].inputType == Action.InputType.Button)
                                                    {
                                                        a.keys[k] = new ActionKey(a.keys[k].inputType, GetButtonValue((JoystickButton)EditorGUILayout.EnumPopup("Button:", GetValueButton(a.keys[k].key))));
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




        static string GetButtonValue(JoystickButton realButton)
        {
            int button = (int)realButton;
            return "Joystick1Button" + button.ToString();
        }
        static JoystickButton GetValueButton(string button)
        {
            foreach (JoystickButton realButton in Enum.GetValues(typeof(JoystickButton)))
            {
                if (GetButtonValue(realButton).Equals(button))
                {
                    return realButton;
                }
            }
            return JoystickButton.Joystick1Button0;
        }
        static string GetAxisValue(Axis realAxis)
        {
            int axis = (int)realAxis + 1;
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
        static Axis GetValueAxis(string axis)
        {
            foreach (Axis realAxis in Enum.GetValues(typeof(Axis)))
            {
                if (GetAxisValue(realAxis).Equals(axis))
                {
                    return realAxis;
                }
            }
            return Axis.AxisXJoystick1;
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
        }
        //[WrapperlessIcall]
        /* [MethodImpl(MethodImplOptions.InternalCall)]
         public static extern bool GetButtonn(string buttonName);*/


        //Write the code automatically gud ?

        /*
         mettre le tout dans un dll or something
         */
    }
#endif
}