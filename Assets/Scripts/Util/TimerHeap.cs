using MS;
using System;
using System.Diagnostics;

namespace Game.Util
{
	public class TimerHeap
	{
		private static uint m_nNextTimerId;
		private static uint m_unTick;
		private static KeyedPriorityQueue<uint, AbsTimerData, ulong> m_queue;
		private static Stopwatch m_stopWatch;
		private static readonly object m_queueLock = new object();

		private TimerHeap() { }

		static TimerHeap()
		{
			m_queue = new KeyedPriorityQueue<uint, AbsTimerData, ulong>();
			m_stopWatch = new Stopwatch();
		}

		public static uint AddTimer(uint start, int interval, Action handler)
		{
			var p = GetTimerData(new TimerData(), start, interval);
			p.Action = handler;
			return AddTimer(p);
		}

		public static uint AddTimer<T>(uint start, int interval, Action<T> handler, T arg1)
		{
			var p = GetTimerData(new TimerData<T>(), start, interval);
			p.Action = handler;
			p.Arg1 = arg1;
			return AddTimer(p);
		}

		public static uint AddTimer<T, U>(uint start, int interval, Action<T, U> handler, T arg1, U arg2)
		{
			var p = GetTimerData(new TimerData<T, U>(), start, interval);
			p.Action = handler;
			p.Arg1 = arg1;
			p.Arg2 = arg2;
			return AddTimer(p);
		}

		public static uint AddTimer<T, U, V>(uint start, int interval, Action<T, U, V> handler, T arg1, U arg2, V arg3)
		{
			var p = GetTimerData(new TimerData<T, U, V>(), start, interval);
			p.Action = handler;
			p.Arg1 = arg1;
			p.Arg2 = arg2;
			p.Arg3 = arg3;
			return AddTimer(p);
		}

		public static void DelTimer(uint timerId)
		{
			lock (m_queueLock)
				m_queue.Remove(timerId);
		}

		public static void Tick()
		{
			m_unTick += (uint)m_stopWatch.ElapsedMilliseconds;
			m_stopWatch.Reset();
			m_stopWatch.Start();

			while (m_queue.Count != 0)
			{
				AbsTimerData p;
				lock (m_queueLock)
					p = m_queue.Peek();
				if (m_unTick < p.UnNextTick)
				{
					break;
				}
				lock (m_queueLock)
					m_queue.Dequeue();
				if (p.NInterval > 0)
				{
					p.UnNextTick += (ulong)p.NInterval;
					lock (m_queueLock)
						m_queue.Enqueue(p.NTimerId, p, p.UnNextTick);
					p.DoAction();
				}
				else
				{
					p.DoAction();
				}
			}
		}

		public static void Reset()
		{
			m_unTick = 0;
			m_nNextTimerId = 0;
			lock (m_queueLock)
				while (m_queue.Count != 0)
					m_queue.Dequeue();
		}

		private static uint AddTimer(AbsTimerData p)
		{
			lock (m_queueLock)
				m_queue.Enqueue(p.NTimerId, p, p.UnNextTick);
			return p.NTimerId;
		}

		private static T GetTimerData<T>(T p, uint start, int interval) where T : AbsTimerData
		{
			p.NInterval = interval;
			p.NTimerId = ++m_nNextTimerId;
			p.UnNextTick = m_unTick + 1 + start;
			return p;
		}
	}
}