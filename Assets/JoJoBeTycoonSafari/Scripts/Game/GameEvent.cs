using System.Collections;
using System.Collections.Generic;
using UFrame.Common;
using UnityEngine;

namespace UFrame
{
	public class GameEventManager : Singleton<GameEventManager>, ISingleton
	{
		public void Init()
		{
		}

		public static void SendEvent(string eventName)
		{
			GameEventManager.GetInstance().SendEventImp(eventName);
		}

		public void SendEventImp(string eventName)
		{

		}
	}

}
