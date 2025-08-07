using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Battlehub.Dispatcher
{
	public class Dispatcher : MonoBehaviour
	{
		private Dispatcher m_current;

		private int m_lock;

		private bool m_run;

		private Queue<Action> m_wait;

		public static Dispatcher Current
		{
			get;
			private set;
		}

		public void BeginInvoke(Action action)
		{
			while (Interlocked.Exchange(ref m_lock, 1) != 0)
			{
			}
			m_wait.Enqueue(action);
			m_run = true;
			Interlocked.Exchange(ref m_lock, 0);
		}

		private void Awake()
		{
			if (Current != null)
			{
				UnityEngine.Object.Destroy(Current);
			}
			Current = this;
			m_wait = new Queue<Action>();
		}

		private void Update()
		{
			if (!m_run)
			{
				return;
			}
			Queue<Action> queue = null;
			if (Interlocked.Exchange(ref m_lock, 1) == 0)
			{
				queue = new Queue<Action>(m_wait.Count);
				while (m_wait.Count != 0)
				{
					Action item = m_wait.Dequeue();
					queue.Enqueue(item);
				}
				m_run = false;
				Interlocked.Exchange(ref m_lock, 0);
			}
			if (queue != null)
			{
				while (queue.Count != 0)
				{
					Action action = queue.Dequeue();
					action();
				}
			}
		}
	}
}
