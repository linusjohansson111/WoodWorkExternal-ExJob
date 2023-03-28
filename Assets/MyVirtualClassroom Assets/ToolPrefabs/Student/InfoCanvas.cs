using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoCanvas : MonoBehaviour
{
    public static InfoCanvas Ins 
    {
        get { return instance; }
    }
    private static InfoCanvas instance;

    [SerializeField]
    private TextMeshProUGUI DisplayAboveText;

    [SerializeField]
    private TextMeshProUGUI DisplayUnderText;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void DisplayAboveObjectInfo(string aTextToDisplay)
    {
        DisplayAboveText.text = aTextToDisplay;
    }

    public void DisplayUnderObjectInfo(string aTextToDisplay)
    {
        DisplayUnderText.text = aTextToDisplay;
    }
}
