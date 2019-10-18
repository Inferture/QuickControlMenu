using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace QuickControlMenu
{
    public class Informations : MonoBehaviour
    {
        public static GameObject infos;

        // Use this for initialization
        void Start()
        {
            infos = gameObject;
        }

        public static void SetInfo(string information)
        {
            infos.GetComponent<Text>().text = information;
        }
    }
}