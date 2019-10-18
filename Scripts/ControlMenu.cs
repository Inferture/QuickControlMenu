using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace QuickControlMenu
{
    public class ControlMenu : MonoBehaviour
    {

        List<Action> actions = new List<Action>();
        List<Category> categories = new List<Category>();
        Dictionary<string, List<Action>> actionByCategory = new Dictionary<string, List<Action>>();

        public Option ControlOption;

        public Font font;
        public Sprite selectedImage;
        public int sep;
        public int disappearDistance = 200;
        public float selectedSpeed = 1f;

        public float disAlpha = 0.2f;
        public float speed = 10;
        public float alphaSpeed = 0.04f;

        public Color actionTitleColor;

        public void OnEnable()
        {
            Initialisation.Start();
            actions = Controls.actions;
            foreach (Action a in actions)
            {
                try
                {
                    actionByCategory[a.category.name].Add(a);
                }
                catch (KeyNotFoundException e)
                {
                    List<Action> l = new List<Action> { a };
                    actionByCategory.Add(a.category.name, l);
                    categories.Add(new Category(a.category.name, a.category.description));
                }
            }

            GameObject mainControlMenu = CreateMenu();
            mainControlMenu.transform.SetParent(transform.parent);
            mainControlMenu.transform.localPosition = new Vector2(0, 0);
            mainControlMenu.name = "Menu Control";

            ControlOption.menu = mainControlMenu;
            GameObject selected = mainControlMenu.GetComponent<Menu>().selected;
            for (int k = 0; k < categories.Count; k++)
            {

                Category c = categories[k];

                GameObject catMenu = CreateMenuCategory(actionByCategory[c.name]);

                catMenu.name = "Menu " + actionByCategory[c.name][0].category;

                GameObject catOption = CreateOptionCategory(actionByCategory[c.name], catMenu);
                catOption.transform.SetParent(mainControlMenu.transform);
                catOption.name = "Option " + actionByCategory[c.name][0].category;


                catMenu.transform.SetParent(transform.parent);
                catMenu.transform.localPosition = k * sep * Vector2.down;


                if (selected.transform.parent == null)
                {
                    selected.transform.parent = catMenu.transform;
                    selected.transform.localPosition = Vector2.zero;
                }
            }
            gameObject.AddComponent<Controls>();
        }


        GameObject CreateMenu()
        {
            GameObject m;
            m = new GameObject();
            m.SetActive(false);
            m.AddComponent<RectTransform>();
            m.transform.SetParent(transform);
            Menu menu = m.AddComponent<Menu>();

            //Selected
            GameObject selected = new GameObject();
            GameObject selected1 = new GameObject();
            GameObject selected2 = new GameObject();

            selected.AddComponent<RectTransform>();
            selected1.AddComponent<RectTransform>();
            selected2.AddComponent<RectTransform>();

            Image imSel1 = selected1.AddComponent<Image>();
            imSel1.sprite = selectedImage;
            imSel1.SetNativeSize();


            selected1.transform.SetParent(selected.transform);
            Selected sel1 = selected1.AddComponent<Selected>();
            sel1.speed = selectedSpeed;

            Image imSel2 = selected2.AddComponent<Image>();
            imSel2.sprite = selectedImage;
            imSel2.SetNativeSize();

            selected2.transform.SetParent(selected.transform);
            Selected sel2 = selected2.AddComponent<Selected>();
            sel2.speed = selectedSpeed;
            sel2.second = true;
            menu.selected = selected;
            

            selected.name = "Selected";
            selected1.name = "Selected1";
            selected2.name = "Selected2";

            selected.transform.localPosition = Vector2.zero;
            selected1.transform.localPosition = Vector2.zero;
            selected2.transform.localPosition = Vector2.zero;
            selected1.transform.localScale = new Vector2(2, 2);
            selected2.transform.localScale = new Vector2(2, 2);

            //\\

            //Variables in menu
            menu.sep = sep;
            menu.disappearDistance = disappearDistance;
            menu.alpha = 1;
            menu.alphaTarget = 1;
            menu.alphaSpeed = alphaSpeed;
            menu.speed = speed;
            menu.disAlpha = disAlpha;
            //\\


            return m;
        }
        GameObject CreateMenuCategory(List<Action> l)
        {
            GameObject m;
            m = CreateMenu();
            Menu menu = m.GetComponent<Menu>();
            menu.sep = 200;
            menu.horizontal = true;

            foreach (Action a in l)
            {
                GameObject menuAction = CreateOptionAction(a);
                menuAction.transform.SetParent(m.transform);

                GameObject title = menuAction.GetComponent<OptionAction>().title;
                title.transform.parent = menuAction.transform.parent;
                title.transform.localPosition = new Vector2(menuAction.transform.localPosition.x, 50);
            }

            m.transform.localPosition = Vector2.zero;

            return m;
        }
        GameObject CreateOptionCategory(List<Action> l, GameObject menu)
        {
            GameObject m;
            m = new GameObject();
            m.AddComponent<CanvasRenderer>();
            Text txt = m.AddComponent<Text>();
            m.AddComponent<Mask>();
            txt.text = l[0].category.name;

            txt.resizeTextForBestFit = true;
            txt.rectTransform.sizeDelta = new Vector2(176, 32);
            txt.font = font;
            txt.alignment = TextAnchor.MiddleCenter;

            m.transform.localPosition = Vector2.zero;

            //Option
            Option opt = m.AddComponent<Option>();
            opt.information = l[0].category.description;
            opt.enabled = true;
            opt.disabledColor = new Color(0.5f, 0.5f, 0.5f);
            opt.enabledColor = new Color(1, 1, 1);
            opt.menu = menu;

            return m;
        }

        GameObject CreateOptionAction(Action a)
        {
            GameObject act;
            act = new GameObject();
            act.AddComponent<CanvasRenderer>();
            Text textAction = act.AddComponent<Text>();
            act.AddComponent<Mask>();

            textAction.resizeTextForBestFit = true;
            textAction.rectTransform.sizeDelta = new Vector2(176, 32);
            textAction.font = font;
            textAction.alignment = TextAnchor.MiddleCenter;


            OptionAction oa = act.AddComponent<OptionAction>();
            oa.action = a;
            oa.verticalSpace = 50;
            GameObject title = new GameObject();
            title.AddComponent<CanvasRenderer>();
            Text txtTitle = title.AddComponent<Text>();
            txtTitle.color = actionTitleColor;
            txtTitle.text = a.name;

            txtTitle.resizeTextForBestFit = true;
            txtTitle.rectTransform.sizeDelta = new Vector2(176, 32);
            txtTitle.font = font;
            txtTitle.alignment = TextAnchor.MiddleCenter;

            oa.title = title;

            return act;
        }
    }
}