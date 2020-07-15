using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float speed;
    [SerializeField] private Transform textParent;

    private void OnEnable()
    {
        StartFloat(5.ToString());
    }
    public void StartFloat(string value)
    {
        StartCoroutine(FloatText(value));
    }
    private IEnumerator FloatText(string value)
    {
        Color tcolor = text.color;
        text.text = value;
        float t = -0.25f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            textParent.LookAt(Camera.main.transform.position);
            //Vector3 tp = textParent.position;
            textParent.localPosition = new Vector3(0, Mathf.Lerp(0, 1.35f, t), 0);
            text.color = new Color(tcolor.r, tcolor.g, tcolor.b, Mathf.Lerp(1, 0, t));
            yield return null;
        }
        //Destroy(gameObject);
    }
}
