//using System;
//using UnityEditor;
//using UnityEngine;

//namespace _root_NameSpace
//{
//	public class NewEditorWindow : EditorWindow
//	{
//		event Action keyDownAction;

//		[MenuItem( "AAA/Custom Input Editor" )]
//		public static void ShowWindow()
//		{
//			GetWindow<NewEditorWindow>( "Custom Input Editor" );
//		}

//		private void OnGUI()
//		{
//			ProcessInputEvents( Event.current );
//		}

//		private void ProcessInputEvents( Event e )
//		{
//			if ( e.type == EventType.KeyDown )
//			{
//				switch ( e.keyCode )
//				{
//					case KeyCode.Space:
//						keyDownAction?.Invoke(); // Invoke event handlers if any
//						Repaint(); // Optionally repaint if the event affects the GUI
//						break;
//						// Add cases for other keys as needed
//				}
//			}
//		}

//		private void OnEnable()
//		{
//			// Subscribe a method to the space key press event
//			keyDownAction += OnKeyPress;
//			//Event.current. += OnKeyPress;
//		}

//		private void OnDisable()
//		{
//			// Unsubscribe the method when the window is closed or recompiled
//			keyDownAction -= OnKeyPress;
//		}

//		private void OnKeyPress()
//		{
			
//		}

//	}
//}