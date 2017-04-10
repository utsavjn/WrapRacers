using System;
using UnityEngine;

namespace Xengine
{
	namespace UnityExtensions
	{
		public static class AnimatorExtension
		{
			public static void Animate(this Animator animator, string animation, bool exclusive = true)
			{
				if (exclusive) animator.DisableOtherAnimations(animation);

				if (animator.GetBool(animation)) return;
				animator.SetBool(animation, true);
			}

			private static void DisableOtherAnimations(this Animator animator, string animation)
			{
				foreach (AnimatorControllerParameter parameter in animator.parameters)
				{
					if (parameter.name != animation)
					{
						animator.SetBool(parameter.name, false);
					}
				}
			}
		}
	}
}
