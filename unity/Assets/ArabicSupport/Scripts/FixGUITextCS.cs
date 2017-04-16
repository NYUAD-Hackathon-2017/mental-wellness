using UnityEngine;
using System.Collections;
using ArabicSupport;
using UnityEngine.UI;

public class FixGUITextCS : MonoBehaviour {
	
	public string text;
	public bool tashkeel = true;
	public bool hinduNumbers = true;
	private Text tex;
	// Use this for initialization
	void Start () {
		tex = gameObject.GetComponent<Text>();
		tex.text = ArabicFixer.Fix (text, tashkeel, hinduNumbers);
	}
	public void OnChanged(){
		text = tex.text;
		tex.text = ArabicFixer.Fix (text, tashkeel, hinduNumbers);
		print ("yo");
	}
	// Update is called once per frame
	void Update () {
	
	}
}
