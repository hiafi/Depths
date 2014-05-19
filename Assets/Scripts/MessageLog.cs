using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MessageLog
{
	private static List<string> messages;
	private static GUIText text_obj;
	private static int displayed_lines = 4;
	private static int max_lines = 20;

	public MessageLog ()
	{
	}

	public static void Init()
	{
		MessageLog.messages = new List<string>();
		GameObject loaded = Resources.Load<GameObject>("Prefabs/MessageLog");
		GameObject temp  = (GameObject)GameObject.Instantiate(loaded);
		text_obj = (GUIText)temp.GetComponent("GUIText");
		text_obj.text = "";
	}

	public static void Update()
	{

	}

	public static void AddMessage(string message)
	{
		StringBuilder sb = new StringBuilder();
		int char_count = 0;
		int width = 50;
		foreach (char c in message)
		{
			sb.Append(c);
			char_count += 1;
			if (char_count >= width && c == ' ')
			{
				sb.Append('\n');
				char_count = 0;
			}
		}
		if (messages.Count >= MessageLog.max_lines)
		{
			messages.RemoveAt(0);
		}
		messages.Add(sb.ToString());
		UpdateMessages();
	}

	public static void UpdateMessages()
	{
		StringBuilder sb = new StringBuilder();
		for (int i = MessageLog.messages.Count - 1; i >= Mathf.Max(0, MessageLog.messages.Count - displayed_lines); i--)
		{
			sb.Append(MessageLog.messages[i] + "\n\n");
		}
		text_obj.text = sb.ToString();
	}
}


