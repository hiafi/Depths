using System.Collections.Generic;

namespace MessageHandle
{
	public class MessageHandler
	{
		private static int max_messages = 5;
		private static List<Message> messages;

		public MessageHandler ()
		{

		}

		public static void Init()
		{
			if (MessageHandler.messages == null)
			{
				messages = new List<Message>();
			}
		}

		public static void AddMessage(Message msg)
		{
			if (MessageHandler.messages.Count >= MessageHandler.max_messages)
			{
				MessageHandler.messages.RemoveAt(0);
			}
			MessageHandler.messages.Add(msg);
		}

		public static bool HasMessages()
		{
			if (MessageHandler.messages.Count > 0)
			{
				return true;
			}
			return false;
		}

		public static Message GetMessage()
		{
			Message msg = null;
			if (MessageHandler.messages.Count > 0)
			{
				msg = MessageHandler.messages[0];
				MessageHandler.messages.RemoveAt(0);

			}
			return msg;
		}

		public static void ClearMessages()
		{
			MessageHandler.messages.Clear();
		}
	}

	public class Message
	{
		public string type;
		public int index;

		public Message(string ty, int ind)
		{
			type = ty;
			index = ind;
		}
	}
}

