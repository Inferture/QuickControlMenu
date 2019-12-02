using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickControlMenu
{
    public class Initialisation : MonoBehaviour
    {
        public static bool initialized = false;
        // Use this for initialization
        [RuntimeInitializeOnLoadMethod()]
        public static void Init()
        {
            if (!initialized)
            {
                initialized = true;
                Controls.Initialize();
            }
        }
    }
}
