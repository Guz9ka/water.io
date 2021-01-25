using UnityEditor;

[CustomEditor(typeof(ExtendedButton))]
public class ExtendedButtonUIRenderer : UnityEditor.UI.ButtonEditor
{
	public override void OnInspectorGUI()
	{
		ExtendedButton extendedButton = (ExtendedButton)this.target;

		base.OnInspectorGUI();

		extendedButton.IsTextButton = EditorGUILayout.Toggle("Text-only button", extendedButton.IsTextButton);

		extendedButton.DefaultForegroundColor = EditorGUILayout.ColorField("Default foreground", extendedButton.DefaultForegroundColor);
		extendedButton.HoverForegroundColor = EditorGUILayout.ColorField("Hover foreground", extendedButton.HoverForegroundColor);
		extendedButton.HoverBackgroundColor = EditorGUILayout.ColorField("Hover background", extendedButton.HoverBackgroundColor);
		extendedButton.BoldOnHover = EditorGUILayout.Toggle("Bold on hover", extendedButton.BoldOnHover);
	}
}
