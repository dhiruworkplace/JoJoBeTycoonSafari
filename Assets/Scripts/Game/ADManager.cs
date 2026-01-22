using System.Collections;
using System.Collections.Generic;
using UFrame.Common;
using UnityEngine;

namespace UFrame
{
	public class ADManager : Singleton<ADManager>, ISingleton
	{
		public void Init()
		{

		}

		public static void ShowRewardedAD(System.Action<bool> cb)
		{
			ADManager.GetInstance().ShowRewardADImp(cb);
		}

		public void ShowRewardADImp(System.Action<bool> cb)
		{
			PromptText.CreatePromptTextPure("Coming soon\r\nWe will provide this feature in the next release");
		}
	}

}
