using UnityEngine;

[ExecuteInEditMode] // This ensures that the component works in edit mode as well as during play mode
public class NoteComponent : MonoBehaviour
{
    [TextArea(3, 10)] // This makes the field a multi-line text area in the Inspector
    public string note;

}
