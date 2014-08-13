using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Brains.Core;

namespace Brains
{

	public class Brain : ScriptableObject
	{

		public void CreateRuntimeInstances()
		{
			Dictionary<Condition, Condition> instanceToRuntimeConditions = CreateRuntimeInstanceDictionaryFromOriginals<Condition>(conditions);
			ReplaceOriginals(conditions, instanceToRuntimeConditions);

			Dictionary<Connection, Connection> instanceToRuntimeConnections = CreateRuntimeInstanceDictionaryFromOriginals<Connection>(connections);
			ReplaceOriginals(connections, instanceToRuntimeConnections);

			Dictionary<Node, Node> instanceToRuntimeNodes = CreateRuntimeInstanceDictionaryFromOriginals<Node>(nodes);
			ReplaceOriginals(nodes, instanceToRuntimeNodes);

			Dictionary<Parameter, Parameter> instanceToRuntimeParameters = CreateRuntimeInstanceDictionaryFromOriginals<Parameter>(parameters);
			ReplaceOriginals(parameters, instanceToRuntimeParameters);

			foreach(Condition condition in conditions)
			{
				condition.parameter = ReplaceOriginal<Parameter>(condition.parameter, instanceToRuntimeParameters);
			}

			foreach(Connection connection in connections)
			{
				connection.from = ReplaceOriginal(connection.from, instanceToRuntimeNodes);
				connection.to = ReplaceOriginal(connection.to, instanceToRuntimeNodes);

				ReplaceOriginals(connection.conditions, instanceToRuntimeConditions);
			}

			foreach(Node node in nodes)
			{
				ReplaceOriginals(node.connections, instanceToRuntimeConnections);
			}

			foreach(Parameter parameter in parameters)
			{
				parameter.runtimeHashCode = parameter.GetHashCode();
			}

			defaultNode = ReplaceOriginal<Node>(defaultNode, instanceToRuntimeNodes);
			anyNode = ReplaceOriginal<Node>(anyNode, instanceToRuntimeNodes);
		}

		Dictionary<T, T> CreateRuntimeInstanceDictionaryFromOriginals<T>(List<T> originalList) where T : ScriptableObject
		{
			Dictionary<T, T> originalToRuntimeInstance = new Dictionary<T, T>();
			foreach(T original in originalList)
			{
				T clone = ScriptableObject.Instantiate(original) as T;
				clone.name = original.name;

				originalToRuntimeInstance.Add(original, clone);
			}
			return originalToRuntimeInstance;
		}

		T ReplaceOriginal<T>(T original, Dictionary<T, T> runtimeInstanceDictionary) where T : ScriptableObject
		{
			if(runtimeInstanceDictionary.ContainsKey(original))
			{
				return runtimeInstanceDictionary[original];
			}
			else
			{
				return original;
			}
		}

		void ReplaceOriginals<T>(List<T> originalList, Dictionary<T, T> runtimeInstanceDictionary) where T : ScriptableObject
		{
			for(int i = 0; i < originalList.Count; i++)
			{
				originalList[i] = ReplaceOriginal(originalList[i], runtimeInstanceDictionary);
			}
		}



		public void Clear()
		{
			if(nodes != null) while(nodes.Count > 0) RemoveNode(nodes[0]);
			if(connections != null) while(connections.Count > 0) RemoveConnection(connections[0]);
			if(parameters != null) while(parameters.Count > 0) RemoveParameter(parameters[0]);
		}



		  /////////////
		 /// NODES ///
		/////////////

		public Node CreateNode()
		{
			Node node = Node.Create();
			node.name = string.Format("Node {0}", nodes.Count);

			nodes.Add(node);

			if(nodes.Count == 1)
				defaultNode = node;

			return node;
		}

		public void RemoveNode(Node node)
		{
			if(defaultNode == node && nodes.Count > 1)
			{
				defaultNode = nodes[1];
			}

			List<Connection> connectionsToRemove = new List<Connection>();
			foreach(Connection connection in connections)
			{
				if((connection.from == node) || (connection.to == node))
					connectionsToRemove.Add(connection);
			}
			foreach(Connection connectionToRemove in connectionsToRemove)
				RemoveConnection(connectionToRemove);

			nodes.Remove(node);

			DestroyImmediate(node, true);
		}



		  ///////////////////
		 /// CONNECTIONS ///

		public bool HasNode(string name)
		{
			for(int i = 0; i < nodes.Count; i++)
			{
				Node node = nodes[i];

				if(string.Compare(node.name, name) == 0) return true;
			}

			return false;
		}
		///////////////////

		public Connection CreateConnection(Node from, Node to)
		{
			Connection connection = Connection.Create();
			connection.from = from;
			connection.to = to;
			connection.name = string.Format("{0} > {1}", from.name, to.name);

			connections.Add(connection);

			from.connections.Add(connection);

			return connection;
		}

		public void RemoveConnection(Connection connection)
		{
			connection.from.connections.Remove(connection);
			connections.Remove(connection);

			DestroyImmediate(connection, true);
		}



		  //////////////////
		 /// CONDITIONS ///
		//////////////////

		public Condition CreateCondition(Parameter parameter)
		{
			Condition condition = Condition.Create();
			condition.parameter = parameter;
			condition.name = string.Format("{0} condition", parameter.name);

			conditions.Add(condition);

			return condition;
		}

		public void RemoveCondition(Condition condition)
		{
			conditions.Remove(condition);

			DestroyImmediate(condition, true);
		}



		  //////////////////
		 /// PARAMETERS ///
		//////////////////

		public Parameter AddParameter(string name, ParameterType type)
		{
			name = name.Trim();

			if(HasParameter(name))
			{
				Debug.LogError(string.Format("Brain already has a parameter with the name {0}", name));
				return null;
			}

			if(System.String.IsNullOrEmpty(name))
			{
				Debug.LogError("Parameter name can not be empty.");
				return null;
			}

			Parameter parameter = Parameter.Create();
			parameter.name = name;
			parameter.type = type;

			parameters.Add(parameter);

			return parameter;
		}

		public void RemoveParameter(Parameter parameter)
		{
			parameters.Remove(parameter);

			DestroyImmediate(parameter, true);
		}

		public bool HasParameter(string name)
		{
			for(int i = 0; i < parameters.Count; i++)
			{
				Parameter parameter = parameters[i];
				
				if(string.Compare(parameter.name, name) == 0) return true;
			}

			return false;
		}

		public Parameter GetParameter(string name)
		{
			for(int i = 0; i < parameters.Count; i++)
			{
				Parameter parameter = parameters[i];
				
				if(string.Compare(parameter.name, name) == 0) return parameter;
			}

			Debug.LogError(string.Format("Brain does not contain parameter \"{0}\"", name));
			return null;
		}

		public Parameter GetParameter(int runtimeHashCode)
		{
			for(int i = 0; i < parameters.Count; i++)
			{
				Parameter parameter = parameters[i];
				
				if(parameter.runtimeHashCode == runtimeHashCode) return parameter;
			}

			Debug.LogError(string.Format("Brain does not contain a parameter with the hashcode \"{0}\"", runtimeHashCode));
			return null;
		}

		public int GetParameterHashCode(string name)
		{
			Parameter parameter = GetParameter(name);

			if(parameter != null) return parameter.runtimeHashCode;

			return 0;
		}



		  ////////////////////////
		 /// PARAMETER VALUES ///
		////////////////////////

		public bool GetBool(string name)
		{
			Parameter parameter = GetParameter(name);

			if(parameter == null || parameter.type != ParameterType.Bool)
			{
				Debug.LogError(string.Format("Brain does not contain the bool parameter {0}.", name));

				return false;
			}

			return parameter.boolValue;
		}

		public bool GetBool(int runtimeHashCode)
		{
			Parameter parameter = GetParameter(runtimeHashCode);

			if(parameter == null || parameter.type != ParameterType.Bool)
			{
				Debug.LogError("Bool parameter does not exist in brain.");

				return false;
			}

			return parameter.boolValue;
		}

		public void SetBool(string name, bool value)
		{
			Parameter parameter = GetParameter(name);

			if(parameter == null || parameter.type != ParameterType.Bool)
			{
				Debug.LogError(string.Format("Brain does not contain the bool parameter {0}.", name));
				return;
			}

			parameter.boolValue = value;
		}

		public void SetBool(int runtimeHashCode, bool value)
		{
			Parameter parameter = GetParameter(runtimeHashCode);

			if(parameter == null || parameter.type != ParameterType.Bool)
			{
				Debug.LogError("Bool parameter does not exist in brain.");
				return;
			}

			parameter.boolValue = value;
		}

		public int GetInt(string name)
		{
			Parameter parameter = GetParameter(name);

			if(parameter == null || parameter.type != ParameterType.Int)
			{
				Debug.LogError(string.Format("Brain does not contain the int parameter {0}.", name));

				return 0;
			}

			return parameter.intValue;
		}

		public int GetInt(int runtimeHashCode)
		{
			Parameter parameter = GetParameter(runtimeHashCode);

			if(parameter == null || parameter.type != ParameterType.Int)
			{
				Debug.LogError("Int parameter does not exist in brain.");

				return 0;
			}

			return parameter.intValue;
		}

		public void SetInt(string name, int value)
		{
			Parameter parameter = GetParameter(name);

			if(parameter == null || parameter.type != ParameterType.Int)
			{
				Debug.LogError(string.Format("Brain does not contain the int parameter {0}.", name));
				return;
			}

			parameter.intValue = value;
		}

		public void SetInt(int runtimeHashCode, int value)
		{
			Parameter parameter = GetParameter(runtimeHashCode);

			if(parameter == null || parameter.type != ParameterType.Int)
			{
				Debug.LogError("Int parameter does not exist in brain.");
				return;
			}

			parameter.intValue = value;
		}

		public float GetFloat(string name)
		{
			Parameter parameter = GetParameter(name);

			if(parameter == null || parameter.type != ParameterType.Float)
			{
				Debug.LogError(string.Format("Brain does not contain the float parameter {0}.", name));

				return 0;
			}

			return parameter.floatValue;
		}

		public float GetFloat(int runtimeHashCode)
		{
			Parameter parameter = GetParameter(runtimeHashCode);

			if(parameter == null || parameter.type != ParameterType.Float)
			{
				Debug.LogError("Float parameter does not exist in brain.");

				return 0;
			}

			return parameter.floatValue;
		}

		public void SetFloat(string name, float value)
		{
			Parameter parameter = GetParameter(name);

			if(parameter == null || parameter.type != ParameterType.Float)
			{
				Debug.LogError(string.Format("Brain does not contain the float parameter {0}.", name));
				return;
			}

			parameter.floatValue = value;
		}

		public void SetFloat(int runtimeHashCode, float value)
		{
			Parameter parameter = GetParameter(runtimeHashCode);

			if(parameter == null || parameter.type != ParameterType.Float)
			{
				Debug.LogError("Float parameter does not exist in brain.");
				return;
			}

			parameter.floatValue = value;
		}

		public string GetString(string name)
		{
			Parameter parameter = GetParameter(name);

			if(parameter == null || parameter.type != ParameterType.String)
			{
				Debug.LogError(string.Format("Brain does not contain the string parameter {0}.", name));
				return "";
			}

			return parameter.stringValue;
		}

		public string GetString(int runtimeHashCode)
		{
			Parameter parameter = GetParameter(runtimeHashCode);

			if(parameter == null || parameter.type != ParameterType.String)
			{
				Debug.LogError("String parameter does not exist in brain.");
				return "";
			}

			return parameter.stringValue;
		}

		public void SetInt(string name, string value)
		{
			Parameter parameter = GetParameter(name);

			if(parameter == null || parameter.type != ParameterType.String)
			{
				Debug.LogError(string.Format("Brain does not contain the string parameter {0}.", name));
				return;
			}

			parameter.stringValue = value;
		}

		public void SetInt(int runtimeHashCode, string value)
		{
			Parameter parameter = GetParameter(runtimeHashCode);

			if(parameter == null || parameter.type != ParameterType.String)
			{
				Debug.LogError("String parameter does not exist in brain.");
				return;
			}

			parameter.stringValue = value;
		}

		public Node defaultNode;
		public Node anyNode;

		public List<Node> nodes;
		public List<Connection> connections;
		public List<Condition> conditions;
		public List<Parameter> parameters;

	}

}