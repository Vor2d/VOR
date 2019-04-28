using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {

    DynamicAcuityController Dc;
	// Use this for initialization
	void Start () {
        Dc = GameObject.Find("OptotypeE").GetComponent<DynamicAcuityController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void StartingScene()
    {
        Dc.logger();
        SceneManager.LoadScene("StartingScene");
    }

    public void Recalibrate()
    {
        Dc.recalibrate();
    }

    public void LogResult()
    {
        Dc.logger();
    }
}
