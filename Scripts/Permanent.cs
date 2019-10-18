using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickControlMenu
{
    public class Permanent : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            DontDestroyOnLoad(this);
        }
    }
}