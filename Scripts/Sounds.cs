using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickControlMenu
{
    public class Sounds
    {
        public static AudioSource Play(string son)
        {
            GameObject g = GameObject.Find(son);
            if (g != null && g.GetComponent<AudioSource>() != null)
            {
                g.GetComponent<AudioSource>().Play();
            }
            else
            {
                g = GameObject.Find("sound_" + son);
                g.GetComponent<AudioSource>().Play();
            }
            return g.GetComponent<AudioSource>();
        }
    }
}