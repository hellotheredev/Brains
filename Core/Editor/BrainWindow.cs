using UnityEditor;
using UnityEngine;
using System.Collections;

using Brains;

namespace Brains.Core
{

	public class BrainWindow : EditorWindow
	{

		[MenuItem("Brains/Edit Brain")]
		public static void ShowWindow()
		{
			ShowWindow(null);
		}

		public static void ShowWindow(Brain brain)
		{
			window = EditorWindow.GetWindow(typeof(BrainWindow)) as BrainWindow;
			window.title = "Brain";
			window.scrollPosition = Vector2.zero;
			window.brain = brain;

			window.CenterOnNodes();
		}

		void OnFocus()
		{
			dragNode = null;
			linkNode = null;
			selectedConnection = null;
		}

		void Save()
		{
			EditorUtility.SetDirty(brain);

			Repaint();
		}

		void AddToBrainAsset(Object o)
		{
			string brainPath = AssetDatabase.GetAssetPath(brain);

			AssetDatabase.AddObjectToAsset(o, brainPath);
			AssetDatabase.ImportAsset(brainPath);
		}

		void OnGUI()
		{
			window = this;

			if(brain == null) brain = Selection.activeObject as Brain;
			if(brain == null)
			{
				GUILayout.Label("Select a brain.");
				return;
			}

			GUILayout.Label(scrollPosition.ToString());

			DrawBrain();

			WindowEvents();
		}

		void WindowEvents()
		{
			if((Event.current.button == 0) && (Event.current.type == EventType.MouseDrag))
			{
				scrollPosition += Event.current.delta;
				Repaint();
			}

			if((Event.current.button == 0) && (Event.current.type == EventType.MouseDown))
			{
				Selection.activeObject = brain;

				selectedNode = null;
				selectedConnection = null;
				linkNode = null;
				GUI.FocusControl("");
				Save();
			}

			if((Event.current.button == 1) && (Event.current.type == EventType.MouseDown))
			{
				Node node = brain.CreateNode();
				node.position.x = Event.current.mousePosition.x - node.position.width / 2f - scrollPosition.x;
				node.position.y = Event.current.mousePosition.y - node.position.height / 2f - scrollPosition.y;
				selectedNode = node;

				Save();

				AddToBrainAsset(node);
			}
		}

		void CenterOnNodes()
		{
			if(brain == null || brain.nodes == null)
				return;

			Rect bounds = new Rect();
			bounds.xMin = bounds.yMin = Mathf.Infinity;
			bounds.xMax = bounds.yMax = -Mathf.Infinity;

			foreach(Node node in brain.nodes)
			{
				bounds.xMin = Mathf.Min(bounds.xMin, node.position.xMin);
				bounds.yMin = Mathf.Min(bounds.yMin, node.position.yMin);
				bounds.xMax = Mathf.Max(bounds.xMax, node.position.xMax);
				bounds.yMax = Mathf.Max(bounds.yMax, node.position.yMax);
			}

			Vector2 center = new Vector2(position.width / 2f, position.height / 2f);

			scrollPosition = - bounds.center + center;
		}

		void DrawBrain()
		{
			foreach(Node node in brain.nodes) DrawNodeConnections(node);
			foreach(Node node in brain.nodes) DrawNode(node);
		}

		void DrawNode(Node node)
		{
			Color originalColor = GUI.color;

			if(brain.defaultNode == node)
				GUI.color = Color.yellow;

			if(brain.anyNode == node)
				GUI.color = Color.magenta;

			if(selectedNode == node)
				GUI.color = Color.green;

			Vector2 originalPosition = new Vector2(node.position.x, node.position.y);

			node.position.x += scrollPosition.x;
			node.position.y += scrollPosition.y;

			if(selectedNode == node)
			{
				Rect positionLabelRect = new Rect(node.position);
				positionLabelRect.y += node.position.height;
				GUI.Label(positionLabelRect, originalPosition.ToString());
			}

			GUILayout.BeginArea(node.position);
			GUI.Box(new Rect(0, 0, node.position.width, node.position.height), "");
			GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
			labelStyle.fontSize = brain.anyNode == node ? 48 : 12;
			labelStyle.fontStyle = FontStyle.Bold;
			labelStyle.alignment = TextAnchor.MiddleCenter;
			labelStyle.wordWrap = true;
			GUI.Label(new Rect(0, 0, node.position.width, node.position.height), node.name, labelStyle);
			GUILayout.EndArea();

			if(Event.current.button == 0)
			{
				switch(Event.current.type)
				{
					case EventType.MouseDown:
						if(node.position.Contains(Event.current.mousePosition))
						{
							if(linkNode != null && linkNode != node)
							{
								if(node == brain.anyNode)
								{
									ShowNotification(new GUIContent("Can not connect to the Ω node."));
								}
								else if(linkNode.HasConnectionTo(node))
								{
									ShowNotification(new GUIContent("Can not connect to the same node twice."));
								}
								else
								{
									selectedConnection = brain.CreateConnection(linkNode, node);
									AddToBrainAsset(selectedConnection);
								}

								linkNode = null;
							}

							dragNode = node;
							selectedNode = node;
							Event.current.Use();
							Save();
						}
						break;

					case EventType.MouseDrag:
						if(dragNode == node)
						{
							originalPosition.x += Event.current.delta.x;
							originalPosition.y += Event.current.delta.y;
							Event.current.Use();
							Repaint();
						}
						break;

					case EventType.MouseUp:
						if(dragNode == node)
						{
							dragNode = null;
							int snap = 64;
							originalPosition.x = Mathf.RoundToInt(originalPosition.x / snap) * snap;
							originalPosition.y = Mathf.RoundToInt(originalPosition.y / snap) * snap;
							Event.current.Use();
							Repaint();
						}
						break;
				}
			}

			node.position.x = originalPosition.x;
			node.position.y = originalPosition.y;



			GUI.color = originalColor;
		}

		void DrawNodeConnections(Node node)
		{
			int connectionCounter = 0;

			Vector3 startPos;
			Vector3 endPos;
			Vector3 startTan;
			Vector3 endTan;

			Rect fromNodeRect = node.position;
			Vector3 connectFromPos = new Vector3(fromNodeRect.x + fromNodeRect.width, fromNodeRect.y, 0) + (Vector3) scrollPosition;

			Vector3 buttonSize = new Vector3(80, 20, 0);
			Vector3 buttonRelativePosition = Vector3.zero;
			Rect buttonRect;

			foreach(Connection connection in node.connections)
			{
				Color originalColor = GUI.color;

				Rect toNodeRect = connection.to.position;

				Color linkColor;

				if(selectedConnection == connection)
				{
					linkColor = Color.cyan;
				}
				else if((selectedNode != null) && (selectedNode == connection.from || selectedNode == connection.to))
				{
					linkColor = Color.yellow;
				}
				else if(selectedNode == null)
				{
					linkColor = new Color(1, 1, 1, 0.5f);
				}
				else
				{
					linkColor = new Color(1, 1, 1, 0.1f);
				}

				GUI.color = linkColor;

				buttonRelativePosition = new Vector3(connectFromPos.x + 10, connectFromPos.y + connectionCounter * (buttonSize.y + 2), 0);
				buttonRect = new Rect(buttonRelativePosition.x, buttonRelativePosition.y, buttonSize.x, buttonSize.y);

				if(GUI.Button(buttonRect, connection.to.name))
				{
					selectedConnection = connection;
					Save();
				}

				linkColor.a = 0.3f;

				startPos = connectFromPos + new Vector3(0, 10, 0);
				endPos = new Vector3(buttonRect.x, buttonRect.y + buttonRect.height / 2f, 0);
				startTan = startPos + Vector3.right * 10;
				endTan = endPos + Vector3.left * 10;

				Handles.DrawBezier(startPos, endPos, startTan, endTan, linkColor, null, 4f);

				startPos = endPos + new Vector3(buttonSize.x, 0, 0);
				endPos = new Vector3(toNodeRect.x, toNodeRect.y + 10, 0) + (Vector3) scrollPosition;
				startTan = startPos + Vector3.right * 35;
				endTan = endPos - Vector3.right * 25;

				Handles.DrawBezier(startPos, endPos, startTan, endTan, linkColor, null, 4f);

				connectionCounter++;

				GUI.color = originalColor;
			}

			buttonSize.x = 48;
			buttonRelativePosition = new Vector3(connectFromPos.x + 10, connectFromPos.y + connectionCounter * (buttonSize.y + 2), 0);
			buttonRect = new Rect(buttonRelativePosition.x, buttonRelativePosition.y, buttonSize.x, buttonSize.y);

			if(GUI.Button(buttonRect, ">>>"))
			{
				linkNode = node;
				linkPosition = new Vector2(buttonRect.xMax, buttonRect.yMin + buttonRect.height / 2f);
			}

			if(linkNode != null)
			{
				endPos = Event.current.mousePosition;
				startTan = (Vector3)linkPosition + Vector3.right * 75;
				endTan = endPos + ((Vector3)linkPosition - endPos).normalized * 35 - Vector3.up * 40;

				Handles.DrawBezier((Vector3)linkPosition, endPos, startTan, endTan, Color.cyan, null, 3f);

				Repaint();
			}
		}

		public static BrainWindow window;

		private Vector2 scrollPosition;

		private Node dragNode;

		private Node linkNode = null;
		private Vector2 linkPosition = Vector2.zero;

		public Brain brain;
		public Node selectedNode;
		public Connection selectedConnection;

	}

}