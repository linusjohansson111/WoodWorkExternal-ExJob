using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoCanvas : MonoBehaviour
{
    public Transform SubstanceToCheck;

    private List<TMPro.TextMeshProUGUI> Texts = new List<TMPro.TextMeshProUGUI>();

    TextMeshPro myText;
    // Start is called before the first frame update

    private void Awake()
    {
        //Substance.OnDisplayTransformInfo += DisplayObjectInfo;
    }

    private void OnDestroy()
    {
        //Substance.OnDisplayTransformInfo -= DisplayObjectInfo;
    }

    void Start()
    {

        TMPro.TextMeshProUGUI[] text = GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        if (text != null)
            Texts = text.ToList<TMPro.TextMeshProUGUI>().ToList();
        return;
    }

    // Update is called once per frame
    void Update()
    {
        DisplayObjectInfo();
    }

    private void DisplayObjectInfo()
    {
        Texts[0].text = "Pos: " + SubstanceToCheck.position.ToString();
        Texts[1].text = "Rot: " + SubstanceToCheck.rotation.eulerAngles.ToString();
        Texts[2].text = "Scl: " + SubstanceToCheck.localScale.ToString();
    }
}
