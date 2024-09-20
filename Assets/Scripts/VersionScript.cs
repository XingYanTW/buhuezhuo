using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionScript : MonoBehaviour
{

    public TextMeshProUGUI versionText;
    // Start is called before the first frame update
    void Start()
    {
        string version = Application.version;
        versionText.text = "v."+version;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
