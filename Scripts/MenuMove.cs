using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuickControlMenu
{
    public class MenuMove : Menu
    {

        int cursor;
        GameObject[] optionsObj;
        float sx;
        float sy;
        public GameObject axeX;
        public GameObject axeY;
        public GameObject axeXm;
        public GameObject axeYm;

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
                }
                obj.transform.SetParent(transform);
                obj.SetActive(true);
            }

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
                        cursor = Next(cursor);//cursor = (cursor + 1) % options.Length;
                        selected.transform.position = new Vector2(sx, sy - sep * cursor);
                        Sounds.Play("sound_menu_change");
                    }
                    if (Controls.UpMenu())
                    {
                        cursor = Last(cursor);//cursor = (cursor + options.Length - 1) % options.Length;
                        selected.transform.position = new Vector2(sx, sy - sep * cursor);
                        Sounds.Play("sound_menu_change");
                    }
                }
                if (horizontal)
                {
                    if (Controls.RightMenu())
                    {
                        cursor = Next(cursor);//cursor = (cursor + 1) % options.Length;
                        selected.transform.position = new Vector2(sx + sep * cursor, sy);
                        Sounds.Play("sound_menu_change");

                    }
                    if (Controls.LeftMenu())
                    {
                        cursor = Last(cursor);//cursor = (cursor + options.Length - 1) % options.Length;
                        selected.transform.position = new Vector2(sx + sep * cursor, sy);
                        Sounds.Play("sound_menu_change");
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
                if (Controls.LastMenu() && !occupied)
                {
                    Sounds.Play("sound_menu_cancel");
                    if (parent != null)
                    {
                        Disappear(false);
                        parent.GetComponent<Menu>().Appear();
                    }

                }
                if (Controls.Select())
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
            options[cursor].hovering = true;
            Informations.SetInfo(options[cursor].information);
            selected.transform.SetParent(optionsObj[cursor].transform);

            HandleActivation();


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

            if (Mathf.Abs(move) < Mathf.Abs(target))
            {
                float dx;
                if (target < 0)
                {
                    dx = -Mathf.Min(speed, Mathf.Abs(move - target));
                    move -= dx;
                }
                else
                {
                    dx = Mathf.Min(speed, Mathf.Abs(move - target));
                    move += dx;
                }
                Vector2 pos = transform.position;
                transform.position = new Vector2(pos.x + dx, pos.y);
            }

        }

        public void HandleActivation()
        {
            if (Controls.AxisAvailableActive(PlayerPrefs.GetString(axeX.GetComponent<OptionKey>().action)))
            {
                axeXm.GetComponent<OptionKey>().enabled = false;
            }
            else
            {
                axeXm.GetComponent<OptionKey>().enabled = true;
            }
            if (Controls.AxisAvailableActive(PlayerPrefs.GetString(axeY.GetComponent<OptionKey>().action)))
            {
                axeYm.GetComponent<OptionKey>().enabled = false;
            }
            else
            {
                axeYm.GetComponent<OptionKey>().enabled = true;
            }

        }
    }
}