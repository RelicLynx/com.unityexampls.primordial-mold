using UnityEditor;
using UnityEngine;

namespace QuixoticTools.PrimordialMold
{

	public class TemplateEditor_Window : EditorWindow
	{
		// ================================================================ «» ================================================================ //

		#region | Fields |

		private static TemplateEditor_Window instance;
		public static TemplateEditor_Window Instance
		{
			get
			{
				if ( instance == null )
				{
					instance = new TemplateEditor_Window();
				}
				return instance;
			}
		}
		private void OnDestroy()
		{
			instance = null;
		}

		private TemplateHolder_SO_Script _SO_Ref;
		private SerializedObject serialized_SO;
		private SerializedProperty _prop;

		#endregion | Fields |

		// ================================================================ «» ================================================================ //

		#region | Events & Calls |

		public static void OpenWindow( TemplateHolder_SO_Script obj_SO )
		{
			instance ??= GetWindow<TemplateEditor_Window>();
			instance._SO_Ref = obj_SO;
			instance.Reload_SO( obj_SO );
			instance.Focus();
		}

		#endregion | Events & Calls |

		// ================================================================ «» ================================================================ //

		#region  | OnGui |

		private void OnGUI()
		{

			using ( new GUILayout.HorizontalScope( EditorStyles.helpBox ) )
			{
				if ( GUILayout.Button( "Add Blank" ) )
				{
					_SO_Ref.AddBlankTemplate();
					Reload_SO( _SO_Ref );
				}

				if ( GUILayout.Button( "RELOAD" ) )
				{
					Reload_SO( _SO_Ref );
				}
				if ( GUILayout.Button( "Apply Changes" ) )
				{
					Apply();
				}
			}

			GUILayout.Space( 20 );
			if ( _prop != null )
			{
				foreach ( SerializedProperty p in _prop )
				{
					EditorGUILayout.PropertyField( p, true );
				}
			}
		}

		#endregion | OnGui |

		// ================================================================ «» ================================================================ //

		#region | Functions |

		private void Reload_SO( TemplateHolder_SO_Script obj_SO )
		{
			serialized_SO = new SerializedObject( obj_SO );
			_prop = serialized_SO.FindProperty( "Templates" );
		}

		protected void Apply()
		{
			serialized_SO.ApplyModifiedProperties();
		}

		#endregion | Functions |

		// ================================================================ «» ================================================================ //
	}
}
