using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickControlMenu
{
    public class Initialisation : MonoBehaviour
    {
        public static bool initialized = false;
        // Use this for initialization
        public static void Start()
        {
            if (!initialized)
            {
                initialized = true;
                Controls.Initialize();
            }
        }
    }
}
