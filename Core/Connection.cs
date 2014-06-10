using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Brains.Core
{

	public class Connection : ScriptableObject
	{

		public static Connection Create()
		{
			Connection connection = ScriptableObject.CreateInstance<Connection>();
			connection.hideFlags = HideFlags.HideInHierarchy;
			connection.conditions = new List<Condition>();

			return connection;
		}

		public bool HasConditionForParameter(Parameter parameter)
		{
			foreach(Condition condition in conditions)
			{
				if(condition.parameter == parameter) return true;
			}

			return false;
		}

		public Node from;
		public Node to;
		
		public List<Condition> conditions;

	}
		
}