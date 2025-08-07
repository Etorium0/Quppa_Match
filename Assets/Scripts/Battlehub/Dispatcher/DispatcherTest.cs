using System;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.Dispatcher
{
	public class DispatcherTest : MonoBehaviour
	{
		[SerializeField]
		private Text Output;

		private void Start()
		{
			for (int i = 0; i < 10; i++)
			{
				ThreadFunction(i);
			}
		}

		private void ThreadFunction(object param)
		{
			Dispatcher.Current.BeginInvoke(delegate
			{
				Output.text += param;
				Output.text += Environment.NewLine;
			});
		}
	}
}
