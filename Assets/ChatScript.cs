using UnityEngine;
using System.Collections;
using Mirror;
using UnityEngine.UI;
 
public class ChatScript : NetworkBehaviour
{
	Text TxtTexto;
	InputField inputField;
	private bool isChatActive=false;
	private GameObject chatGameObject;
	public float secondsDisplayed = 2f;
	void Start () 
	{
		TxtTexto = GameObject.Find ("TxtTexto").GetComponent < Text>();
		inputField = GameObject.Find ("input").GetComponent<InputField> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//TODO AVOID THIS
		if (inputField==null)
		{
			TxtTexto = GameObject.Find ("TxtTexto").GetComponent < Text>();
			inputField = GameObject.Find ("input").GetComponent<InputField> ();
			chatGameObject = inputField.transform.parent.gameObject;
		}
		if (!isLocalPlayer)
			return;


		if(Input.GetKeyDown(KeyCode.Return))
		{
			if(inputField.text != "")
			{
				string message = inputField.text;
				inputField.text = "";

				CmdSendText (message);
			}
		}
	}

	[Command]
	void CmdSendText(string message)
	{
		RpcReceiveText (message);

	}

	[ClientRpc]
	public void RpcReceiveText(string message)
	{
		TxtTexto.text += ">>" + message + "\n";
		isChatActive = true;
		chatGameObject.SetActive(true);
		StartCoroutine(nameof(ChatTimer));
	}

	public IEnumerator ChatTimer()
	{
		yield return new WaitForSeconds(secondsDisplayed);
		if (inputField.isFocused|| inputField.text.Length!=0)
		{
			yield return null;
		}
		isChatActive = false;
		chatGameObject.SetActive(false);
		}

}
