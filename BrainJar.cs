using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Brains.Core;

namespace Brains
{

	public class BrainJar : MonoBehaviour
	{

		void Awake()
		{
			brain = GameObject.Instantiate(brain) as Brain;
			brain.CreateRuntimeInstances();

			GetSensors();
			GetNodeBehaviours();
		}

		void Start()
		{
			Reset();

			isInitialized = true;
		}

		void GetSensors()
		{
			sensors = new List<Sensor>();
			sensors.AddRange(transform.GetComponentsInChildren<Sensor>(true) as Sensor[]);
		}

		public void Reset()
		{
			for(int i = 0; i < sensors.Count; i++)
				sensors[i].Reset();

			ActivateNode(brain.defaultNode);
		}

		void GetNodeBehaviours()
		{
			BrainState[] statesInChildren = transform.GetComponentsInChildren<BrainState>(true) as BrainState[];

			foreach(Node node in brain.nodes)
			{
				node.states = new List<BrainState>();

				foreach(BrainState state in statesInChildren)
				{
					foreach(string nodeName in state.activationSettings.nodes)
					{
						if(	((node.name == nodeName) && !state.activationSettings.inverted) ||
							((node.name != nodeName) && state.activationSettings.inverted) )
						{
							node.states.Add(state);
						}
					}

					state.enabled = false;
				}
			}
		}

		void LateUpdate()
		{
			if(!isInitialized) return;

			if(brain == null)
			{
				Debug.LogError("Brain is missing.");
				return;
			}

			CheckConditions(brain.anyNode);
			CheckConditions(activeNode);
		}

		void CheckConditions(Node node)
		{
			for(int connectionIndex = 0; connectionIndex < node.connections.Count; connectionIndex++)
			{
				Connection connection = node.connections[connectionIndex];

				int fulfilledConditions = 0;

				for(int conditionIndex = 0; conditionIndex < connection.conditions.Count; conditionIndex++)
				{
					Condition condition = connection.conditions[conditionIndex];

					Parameter parameter = condition.parameter;

					switch(parameter.type)
					{
						case ParameterType.Bool:
							bool boolA = parameter.boolValue;
							bool boolB = condition.boolValue;

							if(	(boolA == boolB) && (condition.equalityCondition == Condition.EqualityCondition.Equal) ||
								(boolA != boolB) && (condition.equalityCondition == Condition.EqualityCondition.NotEqual))
							{
								fulfilledConditions++;
							}
							break;
							
						case ParameterType.Int:
							int intA = parameter.intValue;
							int intB = condition.intValue;

							if(	(intA == intB) && (condition.sizeCondition == Condition.SizeCondition.Equal) ||
								(intA > intB) && (condition.sizeCondition == Condition.SizeCondition.Greater) ||
								(intA < intB) && (condition.sizeCondition == Condition.SizeCondition.Less))
							{
								fulfilledConditions++;
							}
							break;
							
						case ParameterType.Float:
							float floatA = parameter.floatValue;
							float floatB = condition.floatValue;

							if(	(floatA == floatB) && (condition.sizeCondition == Condition.SizeCondition.Equal) ||
								(floatA > floatB) && (condition.sizeCondition == Condition.SizeCondition.Greater) ||
								(floatA < floatB) && (condition.sizeCondition == Condition.SizeCondition.Less))
							{
								fulfilledConditions++;
							}
							break;
							
						case ParameterType.String:
							string stringA = parameter.stringValue;
							string stringB = condition.stringValue;

							if(	(stringA == stringB) && (condition.equalityCondition == Condition.EqualityCondition.Equal) ||
								(stringA != stringB) && (condition.equalityCondition == Condition.EqualityCondition.NotEqual))
							{
								fulfilledConditions++;
							}
							break;

						case ParameterType.Random:
							float rFloatA = Random.value;
							float rFloatB = condition.floatValue;

							if(	(rFloatA == rFloatB) && (condition.sizeCondition == Condition.SizeCondition.Equal) ||
								(rFloatA > rFloatB) && (condition.sizeCondition == Condition.SizeCondition.Greater) ||
								(rFloatA < rFloatB) && (condition.sizeCondition == Condition.SizeCondition.Less))
							{
								fulfilledConditions++;
							}
							break;
					}
				}

				if(connection.conditions.Count == fulfilledConditions && activeNode != connection.to)
				{
					ActivateNode(connection.to);
				}
			}
		}

		void ActivateNode(Node node)
		{
			Debug.Log("Activating: " + brain.name + "/" + node.name);

			if(activeNode != null)
			{
				foreach(BrainState state in activeNode.states)
				{
					state.OnNodeEnd();
					state.enabled = false;
				}
			}

			activeNode = node;

			foreach(BrainState state in activeNode.states)
			{
				state.enabled = true;
				state.OnNodeBegin();
			}
		}

		public Brain brain;
		public string current { get{ return activeNode != null ? activeNode.name : ""; } }

		private List<Sensor> sensors;
		private Node activeNode = null;
		private bool isInitialized = false;

	}
	
}