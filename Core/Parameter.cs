using UnityEngine;
using System.Collections;

namespace Brains.Core
{

	public class Parameter : ScriptableObject
	{

		public static Parameter Create()
		{
			Parameter parameter = ScriptableObject.CreateInstance<Parameter>();
			parameter.hideFlags = HideFlags.HideInHierarchy;

			return parameter;
		}

		public ParameterType type;

		public bool boolValue;
		public int intValue;
		public float floatValue;
		public string stringValue;

		public int runtimeHashCode;
		
	}
	
}