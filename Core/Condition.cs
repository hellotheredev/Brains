using UnityEngine;
using System.Collections;

namespace Brains.Core
{

	public class Condition : ScriptableObject
	{

		public static Condition Create()
		{
			Condition condition = ScriptableObject.CreateInstance<Condition>();
			condition.hideFlags = HideFlags.HideInHierarchy;

			return condition;
		}

		public Parameter parameter;

		public bool boolValue;
		public int intValue;
		public float floatValue;
		public string stringValue;

		public SizeCondition sizeCondition;
		public EqualityCondition equalityCondition;

		public enum SizeCondition{ Equal, Greater, Less };
		public enum EqualityCondition{ Equal, NotEqual };

	}
		
}