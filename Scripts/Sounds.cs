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
        public static AudioSource Pause(string son)
        {
            GameObject g = GameObject.Find(son);
            if (g != null && g.GetComponent<AudioSource>() != null)
            {
                g.GetComponent<AudioSource>().Pause();
            }
            else
            {
                g = GameObject.Find("sound_" + son);
                g.GetComponent<AudioSource>().Pause();
            }
            return g.GetComponent<AudioSource>();
        }

        public static AudioSource Stop(string son)
        {
            GameObject g = GameObject.Find(son);
            if (g != null && g.GetComponent<AudioSource>() != null)
            {
                g.GetComponent<AudioSource>().Stop();
            }
            else
            {
                g = GameObject.Find("sound_" + son);
                g.GetComponent<AudioSource>().Stop();
            }
            return g.GetComponent<AudioSource>();
        }

        public static AudioSource PlayPause(string son)
        {
            GameObject g = GameObject.Find(son);

            if (g == null || g.GetComponent<AudioSource>() == null)
            {
                g = GameObject.Find("sound_" + son);
            }
            if (g.GetComponent<AudioSource>().isPlaying)
            {
                g.GetComponent<AudioSource>().Pause();
            }
            else
            {
                g.GetComponent<AudioSource>().Play();
            }
            return g.GetComponent<AudioSource>();
        }

        public static AudioSource Get(string son)
        {
            GameObject g = GameObject.Find(son);

            if (g == null || g.GetComponent<AudioSource>() == null)
            {
                g = GameObject.Find("sound_" + son);
            }
            return g.GetComponent<AudioSource>();
        }

        public static AudioSource PutVolume(string son, float volume)
        {
            GameObject g = GameObject.Find(son);

            if (g == null || g.GetComponent<AudioSource>() == null)
            {
                g = GameObject.Find("sound_" + son);
            }
            g.GetComponent<AudioSource>().volume = volume;
            return g.GetComponent<AudioSource>();
        }

        public static float GetVolume(string son)
        {
            GameObject g = GameObject.Find(son);

            if (g == null || g.GetComponent<AudioSource>() == null)
            {
                g = GameObject.Find("sound_" + son);
            }

            return g.GetComponent<AudioSource>().volume;
        }
    }
}