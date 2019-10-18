using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace QuickControlMenu
{
    public class Selected : MonoBehaviour
    {

        public float speed;
        public bool second;
        // Use this for initialization
        void Start()
        {
            GetComponent<Image>().SetNativeSize();
            if (second)
            {
                transform.localPosition = new Vector2(2 * GetComponent<RectTransform>().rect.width, 0);
            }
            else
            {
                transform.localPosition = new Vector2(0, 0);
            }
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 pos = transform.localPosition;
            transform.localPosition = new Vector2(pos.x - speed, pos.y);
            if (GetComponent<RectTransform>().localPosition.x < -2 * GetComponent<RectTransform>().rect.width)
            {
                transform.localPosition = new Vector2(2 * GetComponent<RectTransform>().rect.width - speed * Time.deltaTime * 60, pos.y);
            }
        }
    }
}