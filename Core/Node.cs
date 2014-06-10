using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Brains.Core
{

	public class Node : ScriptableObject
	{

		public static Node Create()
		{
			Node node = ScriptableObject.CreateInstance<Node>();
			node.hideFlags = HideFlags.HideInHierarchy;
			node.position = new Rect(0, 0, 128, 64);
			node.connections = new List<Connection>();

			return node;
		}

		public bool HasConnectionTo(Node node)
		{
			foreach(Connection connection in connections)
			{
				if(connection.to == node) return true;
			}

			return false;
		}

		public Rect position;
		public List<Connection> connections;
		public List<BrainState> states;

	}

}