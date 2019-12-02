using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace QuickControlMenu
{
    public class Menu : MonoBehaviour
    {

        public GameObject selected;
        public GameObject option;
        public GameObject parent;
        public GameObject explanations;
        //public string[] options;

        public float sep;
        public bool horizontal;
        int cursor;
        public bool active;
        GameObject[] optionsObj;
        public Option[] options;
        float sx;
        float sy;
        public float disappearDistance = 150;
        public float disAlpha = 0.2f;

        public bool under = false;
        public float move = 0;
        public float speed = 10;
        public float target = 0;
        public float alpha = 1;
        public float alphaTarget = 1;
        public float alphaSpeed = 0.04f;
        // Use this for initialization
        void Start()
        {

            Vector2 spos = selected.transform.position;
            sx = spos.x;
            sy = spos.y;

            options = GetComponentsInChildren<Option>();

            optionsObj = new GameObject[options.Length];

            Vector2 pos = transform.position;
            cursor = 0;


            for (int i = 0; i < options.Length; i++)
            {
                optionsObj[i] = options[i].gameObject;
                GameObject obj = optionsObj[i];
                if (!horizontal)
                {
                    obj.transform.position = new Vector2(pos.x, pos.y - i * sep);
                }
                if (horizontal)
                {
                    obj.transform.position = new Vector2(pos.x + i * sep, pos.y);
                    if (obj.GetComponent<OptionAction>() != null && obj.GetComponent<OptionAction>().title != null)
                    {
                        obj.GetComponent<OptionAction>().title.transform.localPosition = new Vector2(obj.transform.localPosition.x, obj.transform.localPosition.y + 50);
                    }
                }
                obj.transform.SetParent(transform);
                obj.SetActive(true);
            }

            selected.transform.SetParent(optionsObj[cursor].transform);
            selected.transform.localPosition = Vector2.zero;
            options[cursor].selectedObject = selected;

        }

        // Update is called once per frame
        void Update()
        {

            if (active)
            {
                if (alpha < alphaTarget)
                {
                    alpha = Mathf.Min(alpha + alphaSpeed, alphaTarget);
                }
                Text[] texts = GetComponentsInChildren<Text>();
                foreach (Text t in texts)
                {
                    t.color = new Color(t.color.r, t.color.g, t.color.b, alpha);
                }
                Image[] images = GetComponentsInChildren<Image>();
                foreach (Image im in images)
                {
                    im.color = new Color(im.color.r, im.color.g, im.color.b, alpha);
                }

                if (!horizontal)
                {
                    if (Controls.DownMenu())
                    {
                        cursor = Next(cursor);
                        selected.transform.position = new Vector2(sx, sy - sep * cursor);
                        Sounds.Play("sound_menu_change");

                        selected.transform.SetParent(optionsObj[cursor].transform);
                        selected.transform.localPosition = Vector2.zero;
                        options[cursor].selectedObject = selected;
                    }
                    if (Controls.UpMenu())
                    {
                        cursor = Last(cursor);
                        selected.transform.position = new Vector2(sx, sy - sep * cursor);
                        Sounds.Play("sound_menu_change");

                        selected.transform.SetParent(optionsObj[cursor].transform);
                        selected.transform.localPosition = Vector2.zero;
                        options[cursor].selectedObject = selected;
                    }
                }
                if (horizontal)
                {
                    if (Controls.RightMenu())
                    {
                        cursor = Next(cursor);
                        selected.transform.position = new Vector2(sx + sep * cursor, sy);
                        Sounds.Play("sound_menu_change");

                        selected.transform.SetParent(optionsObj[cursor].transform);
                        selected.transform.localPosition = Vector2.zero;
                        options[cursor].selectedObject = selected;

                    }
                    if (Controls.LeftMenu())
                    {
                        cursor = Last(cursor);
                        selected.transform.position = new Vector2(sx + sep * cursor, sy);
                        Sounds.Play("sound_menu_change");

                        selected.transform.SetParent(optionsObj[cursor].transform);
                        selected.transform.localPosition = Vector2.zero;
                        options[cursor].selectedObject = selected;
                    }
                }
                bool occupied = false;
                foreach (Option op in options)
                {
                    if (op.selected)
                    {
                        occupied = true;
                    }
                }
                if (Controls.CancelMenu() && !occupied)
                {
                    Sounds.Play("sound_menu_cancel");
                    if (parent != null)
                    {
                        Disappear(false);
                        parent.GetComponent<Menu>().Appear();
                    }
                }
                if (Controls.SelectMenu())
                {
                    if (optionsObj[cursor].GetComponent<Option>().menu != null)
                    {
                        optionsObj[cursor].GetComponent<Option>().menu.GetComponent<Menu>().parent = gameObject;
                    }

                    optionsObj[cursor].GetComponent<Option>().parentMenu = gameObject;
                    optionsObj[cursor].GetComponent<Option>().Select();
                    if (optionsObj[cursor].GetComponent<OptionKey>() != null)
                    {
                        optionsObj[cursor].GetComponent<OptionKey>();
                    }
                    Sounds.Play("sound_menu_select");
                }
            }

            foreach (Option op in options)
            {
                op.hovering = false;
            }
            options[cursor].Hover();
            Informations.SetInfo(options[cursor].information);


            if (!active)
            {

                if (alpha > alphaTarget)
                {
                    alpha = Mathf.Max(alpha - alphaSpeed, alphaTarget);
                }
                else if (alpha == 0)
                {
                    gameObject.SetActive(false);
                }
                Text[] texts = GetComponentsInChildren<Text>();
                foreach (Text t in texts)
                {
                    t.color = new Color(t.color.r, t.color.g, t.color.b, alpha);
                }
                Image[] images = GetComponentsInChildren<Image>();
                foreach (Image im in images)
                {
                    im.color = new Color(im.color.r, im.color.g, im.color.b, alpha);
                }
            }

            if (target != 0)
            {
                float dx;
                if (target < 0)
                {
                    dx = -Mathf.Min(speed, Mathf.Abs(target));
                    target -= dx;
                }
                else
                {
                    dx = Mathf.Min(speed, Mathf.Abs(target));
                    target -= dx;
                }
                Vector2 pos = transform.position;
                transform.position = new Vector2(pos.x + dx, pos.y);
            }
        }


        public int Next(int cur)
        {
            int next = (cur + 1) % options.Length;
            int changes = 1;
            while (!options[next].isEnabled && changes < options.Length)
            {
                next = (next + 1) % options.Length;
                changes++;
            }
            return next;
        }

        public int Last(int cur)
        {
            int next = (cur + options.Length - 1) % options.Length;
            int changes = 1;
            while (!options[next].isEnabled && changes < options.Length)
            {
                next = (next + options.Length - 1) % options.Length;
                changes++;
            }
            return next;
        }
        public void GetBack()
        {
            move = 0;
            target += Mathf.Abs(disappearDistance);
            if (parent != null)
            {
                parent.GetComponent<Menu>().GetBack();
            }
        }
        public void Appear()
        {
            if (explanations != null)
            {
                explanations.SetActive(true);
            }
            active = true;
            alphaTarget = 1;
            if (under)
            {
                target += Mathf.Abs(disappearDistance);
                if (parent != null)
                {
                    parent.GetComponent<Menu>().GetBack();
                }

            }
            move = 0;
            gameObject.SetActive(true);
        }
        public void Disappear(bool stay)
        {
            if (explanations != null)
            {
                explanations.SetActive(false);
            }
            move = 0;
            active = false;
            if (!stay)
            {
                under = false;
                alphaTarget = 0;
            }
            else
            {
                under = true;
                if (parent != null)
                {
                    parent.GetComponent<Menu>().Disappear(true);
                }
                target -= Mathf.Abs(disappearDistance);
                alphaTarget = disAlpha;
            }
        }
    }
}