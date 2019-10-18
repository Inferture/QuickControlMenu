using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuickControlMenu
{
    public class General : MonoBehaviour
    {
        public static bool Contains<T>(T[] list, T value)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Equals(value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}