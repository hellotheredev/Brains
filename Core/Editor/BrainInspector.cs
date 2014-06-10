using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Brains;

namespace Brains.Core
{
	[CustomEditor(typeof(Brain))]
	public class BrainInspector : Editor
	{

		override public void OnInspectorGUI()
		{
			brain = target as Brain;

			Initialize();

			if(GUILayout.Button("Edit Brain"))
			{
				BrainWindow.ShowWindow(brain);
			}

			DrawParameters();

			if((BrainWindow.window != null))
			{
				if(BrainWindow.window.selectedNode != null)
					DrawNodeInspector(BrainWindow.window.selectedNode);

				if(BrainWindow.window.selectedConnection != null)
					DrawConnectionInspector(BrainWindow.window.selectedConnection);
			}

			if(GUILayout.Button("Clear"))
			{
				brain.Clear();
				RefreshAsset();
			}
		}

		void Initialize()
		{
			if(brain.parameters == null) brain.parameters = new List<Parameter>();

			if(brain.nodes == null) brain.nodes = new List<Node>();
			if(brain.nodes.Count == 0)
			{
				brain.anyNode = brain.CreateNode();
				brain.anyNode.name = "Ω";
				AddToBrainAsset(brain.anyNode);
				
				brain.defaultNode = brain.CreateNode();
				brain.defaultNode.position.x += 256;
				AddToBrainAsset(brain.defaultNode);

				RefreshAsset();
			}
		}

		void RefreshAsset()
		{
			EditorUtility.SetDirty(brain);

			if(BrainWindow.window != null) BrainWindow.window.Repaint();

			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(brain));

			Repaint();
		}

		void AddToBrainAsset(Object o)
		{
			string brainPath = AssetDatabase.GetAssetPath(brain);

			AssetDatabase.AddObjectToAsset(o, brainPath);
		}

		public void DrawParameters()
		{
			if(!(showParametersInspector = EditorGUILayout.Foldout(showParametersInspector, "Parameters")))
				return;

			Parameter removeParameter = null;

			foreach(Parameter parameter in brain.parameters)
			{
				GUILayout.BeginHorizontal();

				string parameterFieldLabel = string.Format("{0} [{1}]", parameter.name, parameter.type.ToString());

				switch(parameter.type)
				{
					case ParameterType.Bool:
						parameter.boolValue = EditorGUILayout.Toggle(parameterFieldLabel, parameter.boolValue);
						break;

					case ParameterType.Int:
						parameter.intValue = EditorGUILayout.IntField(parameterFieldLabel, parameter.intValue);
						break;

					case ParameterType.Float:
						parameter.floatValue = EditorGUILayout.FloatField(parameterFieldLabel, parameter.floatValue);
						break;

					case ParameterType.String:
						parameter.stringValue = EditorGUILayout.TextField(parameterFieldLabel, parameter.stringValue);
						break;

					case ParameterType.Random:
						GUILayout.Label(parameterFieldLabel);
						break;

					default:
						break;
				}

				if(GUILayout.Button("Remove", new GUILayoutOption[]{GUILayout.MaxWidth(80)}))
				{
					removeParameter = parameter;
				}

				GUILayout.EndHorizontal();
			}

			if(removeParameter != null)
			{
				brain.RemoveParameter(removeParameter);
			}

			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();

			string parameterTypeID = "KnodWindowParameterType";
			ParameterType currentParameterType = (ParameterType)EditorPrefs.GetInt(parameterTypeID);
			ParameterType selectedParameterType = (ParameterType)EditorGUILayout.EnumPopup("Type to add", currentParameterType);
			EditorPrefs.SetInt(parameterTypeID, (int)selectedParameterType);

			string parameterNameID = "KnodWindowParameterName";
			string currentParameterName = EditorPrefs.GetString(parameterNameID);
			string selectedParameterName = EditorGUILayout.TextField("Parameter name", currentParameterName);
			EditorPrefs.SetString(parameterNameID, selectedParameterName);

			GUILayout.EndVertical();

			if(GUILayout.Button(string.Format("Add {0}", selectedParameterType.ToString().ToLower()), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(34), GUILayout.MaxWidth(80)))
			{
				Parameter parameter = brain.AddParameter(selectedParameterName, selectedParameterType);

				AddToBrainAsset(parameter);

				EditorPrefs.DeleteKey(parameterNameID);
			}

			GUILayout.EndHorizontal();
		}

		public void DrawNodeInspector(Node node)
		{
			Color originalColor = GUI.color;

			GUI.color = Color.green;

			if(!(showNodeInspector = EditorGUILayout.Foldout(showNodeInspector, string.Format("Node [{0}]", node.name))))
			{
				GUI.color = originalColor;			
				return;
			}

			if(node == brain.anyNode)
			{
				GUILayout.Label("The Ω node has no settings.");
			}
			else
			{
				node.name = EditorGUILayout.TextField("Name", node.name);
				if(GUI.changed)
				{
					RefreshAsset();
				}

				if(GUILayout.Button("Make Default"))
				{
					brain.defaultNode = node;
					RefreshAsset();
				}

				if(GUILayout.Button("Remove Node"))
				{
					brain.RemoveNode(node);
					BrainWindow.window.selectedNode = null;
					BrainWindow.window.selectedConnection = null;
					RefreshAsset();
				}
			}

			GUI.color = originalColor;			
		}

		public void DrawConnectionInspector(Connection connection)
		{
			Color originalColor = GUI.color;

			GUI.color = Color.cyan;

			Node nodeA = connection.from;
			Node nodeB = connection.to;

			if((nodeA == null || nodeB == null) || !(showConnectionInspector = EditorGUILayout.Foldout(showConnectionInspector, string.Format("Connection [{0} > {1}]", nodeA.name, nodeB.name))))
			{
				GUI.color = originalColor;
				return;
			}

			if(GUILayout.Button("Remove Connection"))
			{
				brain.RemoveConnection(connection);
				BrainWindow.window.selectedConnection = null;
				RefreshAsset();
			}

			if((showConditionInspector = EditorGUILayout.Foldout(showConditionInspector, "Conditions")))
			{
				// LIST CONDITIONALS

				Condition removeConditional = null;

				foreach(Condition condition in connection.conditions)
				{
					Parameter parameter = condition.parameter;

					EditorGUILayout.BeginHorizontal();

					switch(parameter.type)
					{
						case ParameterType.Bool:
							condition.equalityCondition = (Condition.EqualityCondition) EditorGUILayout.EnumPopup(condition.equalityCondition, GUILayout.ExpandWidth(true));
							condition.boolValue = GUILayout.Toggle(condition.boolValue, string.Format("{0}", parameter.name));
							break;

						case ParameterType.Int:
							condition.sizeCondition = (Condition.SizeCondition) EditorGUILayout.EnumPopup(condition.sizeCondition, GUILayout.ExpandWidth(true));
							condition.intValue = EditorGUILayout.IntField(string.Format("{0}", parameter.name), condition.intValue);
							break;

						case ParameterType.Float:
							condition.sizeCondition = (Condition.SizeCondition) EditorGUILayout.EnumPopup(condition.sizeCondition, GUILayout.ExpandWidth(true));
							condition.floatValue = EditorGUILayout.FloatField(string.Format("{0}", parameter.name), condition.floatValue);
							break;

						case ParameterType.String:
							condition.equalityCondition = (Condition.EqualityCondition) EditorGUILayout.EnumPopup(condition.equalityCondition, GUILayout.ExpandWidth(true));
							condition.stringValue = EditorGUILayout.TextField(string.Format("{0}", parameter.name), condition.stringValue);
							break;

						case ParameterType.Random:
							condition.sizeCondition = (Condition.SizeCondition) EditorGUILayout.EnumPopup(condition.sizeCondition, GUILayout.ExpandWidth(true));
							condition.floatValue = EditorGUILayout.FloatField(string.Format("{0} (Random 0..1)", parameter.name), condition.floatValue);
							break;

						default:
							break;
					}

					if(GUILayout.Button("Remove", GUILayout.MaxWidth(80)))
					{
						removeConditional = condition;
					}

					EditorGUILayout.EndHorizontal();
				}

				if(removeConditional != null)
				{
					connection.conditions.Remove(removeConditional);

					brain.RemoveCondition(removeConditional);
				}

				EditorGUILayout.Space();


				// ADD CONDITIONAL

				if(brain.parameters.Count > 0)
				{
					if(selectedParameterIndex >= brain.parameters.Count)
						selectedParameterIndex = 0;

					string[] parameterNames = new string[brain.parameters.Count];
					for(int i = 0; i < brain.parameters.Count; i++)
					{
						parameterNames[i] = brain.parameters[i].name;
					}

					selectedParameterIndex = EditorGUILayout.Popup(selectedParameterIndex, parameterNames);

					Parameter selectedParameter = brain.GetParameter(parameterNames[selectedParameterIndex]);

					if(GUILayout.Button(string.Format("Add new {0} condition", selectedParameter.name.ToLower())))
					{
						if(connection.HasConditionForParameter(selectedParameter))
						{
							Debug.LogError("Cannot add the same condition twice.");
						}
						else
						{
							Condition condition = brain.CreateCondition(selectedParameter);

							connection.conditions.Add(condition);

							RefreshAsset();

							AddToBrainAsset(condition);
						}
					}
				}


				EditorGUILayout.Space();

			}

	





			GUI.color = originalColor;
		}

		private bool showParametersInspector
		{
			get{ return EditorPrefs.GetBool("ShowParametersInspector", false); }
			set{ EditorPrefs.SetBool("ShowParametersInspector", value); }
		}

		private bool showNodeInspector
		{
			get{ return EditorPrefs.GetBool("ShowNodeInspector", false); }
			set{ EditorPrefs.SetBool("ShowNodeInspector", value); }
		}

		private bool showConditionInspector
		{
			get{ return EditorPrefs.GetBool("ShowConditionInspector", false); }
			set{ EditorPrefs.SetBool("ShowConditionInspector", value); }
		}

		private bool showConnectionInspector
		{
			get{ return EditorPrefs.GetBool("ShowConnectionInspector", false); }
			set{ EditorPrefs.SetBool("ShowConnectionInspector", value); }
		}

		private int selectedParameterIndex
		{
			get{ return EditorPrefs.GetInt("SelectedParameterIndex", 0); }
			set{ EditorPrefs.SetInt("SelectedParameterIndex", value); }
		}

		private Brain brain;


	}
}

