using System.Collections;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using UnityEngine.UI;
using ArabicSupport;

using UnityEngine;

public class test : MonoBehaviour {
	[SerializeField]
	private AudioClip m_AudioClip = new AudioClip();
	private SpeechToText m_SpeechToText = new SpeechToText();
	public Animator anim;
	private Conversation m_Conversation = new Conversation();
	private string m_WorkspaceID = "d362fd91-23db-4846-b7b0-5bbbdf7672b9";
	private string m_Input = "";
	private Context context;
	public Text input,output;
	int i = -1;
	// Use this for initialization
	void Start () {

	}
	public void OnChanged(){
		input.text = ArabicFixer.Fix (input.text,false,false);
	}
	public void OnMessageSend(){
		MessageRequest mr = new MessageRequest ();
		mr.InputText = input.text;
		mr.ContextData = context;
		m_Conversation.Message(OnGetRawOutput, m_WorkspaceID, mr);
	}
	private void OnGetRawOutput(object resp, string customData)
	{
		if (!string.IsNullOrEmpty(customData))
			Debug.Log(customData);
		else
			Debug.Log("No raw data was received.");

		if (resp != null)
		{
			Dictionary<string, object> respDict = resp as Dictionary<string, object>;
			object intents;
			respDict.TryGetValue("intents", out intents);
			//context =((MessageResponse)resp).context;
			foreach(var intentObj in (intents as List<object>))
			{
				Dictionary<string, object> intentDict = intentObj as Dictionary<string, object>;

				object intentString;
				intentDict.TryGetValue("intent", out intentString);
				output.text = ArabicFixer.Fix (customData.Split('\"')[23]);
			
				if (output.text == "input" || i >= 0)
				if (i == 0)//friend
					output.text = ArabicFixer.Fix ("لماذا لا تتفاهم معه ؟");
				else if (i == 1)// no
					output.text = ArabicFixer.Fix ("ولكنه صديقك فلماذا لا تسامحه ؟");
				else if (i == 2)// ok
					output.text = ArabicFixer.Fix ("سعيد لانك تملك قلبا كبيرا");
				else if (i == 3)// thanks
					output.text = ArabicFixer.Fix ("عفوا");
				else {//buy
					output.text = ArabicFixer.Fix ("مع السلامة");
					anim.SetTrigger ("by");
					Invoke ("quit", 5);
				}
						
				i++;
				object confidenceString;
				intentDict.TryGetValue("confidence", out confidenceString);

				//Debug.Log("ExampleConversation", "intent: {0} | confidence {1}", intentString.ToString(), confidenceString.ToString());
			}
		}
	}
	void quit(){
		Application.Quit ();
	}
	void OnMessage (MessageResponse resp, string customData)
	{
		foreach(Intent mi in resp.intents)
			Debug.Log("intent: " + mi.intent + ", confidence: " + mi.confidence);

		Debug.Log("response: " + resp.output.text);
	}
	public void OnEndRecord(){
		Microphone.End ("");
	}
	public void OnRecord(){
		StartCoroutine ("record");
	}
	IEnumerator record(){
		m_AudioClip = Microphone.Start ("",false,20,44100);
		while (Microphone.IsRecording ("")) {
			yield return 0;
		}
		m_SpeechToText.Recognize(m_AudioClip, HandleOnRecognize);
	}
	void HandleOnRecognize(SpeechRecognitionEvent result)
	{
		if (result != null && result.results.Length > 0)
		{
			foreach (var res in result.results)
			{
				foreach (var alt in res.alternatives)
				{
					string text = alt.transcript;
					Debug.Log(string.Format("{0} ({1}, {2:0.00})\n", text, res.final ? "Final" : "Interim", alt.confidence));
					if (i == -1)
						input.text = "حزين";
					else
						input.text = "يو";
					OnMessageSend ();
				}
			}
		}
	}
	// Update is called once per frame
	void Update () {

	}
}
