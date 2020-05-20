using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    private static UserInterface _instance { get; set; }
    public static UserInterface Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UserInterface>();
            }

            return _instance;
        }
    }

    public GameObject crossHair;
    public Text targetText;
}
