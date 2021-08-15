using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExplodeOnClick))]
public class ExplodeOnClickEditor : Editor
{
    private Collider[] Hits;
    private GUIStyle HitStyle;
    private GUIStyle NoHitStyle;

    private void OnEnable()
    {
        Hits = new Collider[((ExplodeOnClick)target).MaxHits];
        HitStyle = new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                textColor = Color.green
            },
            fontSize = 24
        };
        NoHitStyle = new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                textColor = Color.red
            },
            fontSize = 24
        };
    }

    private void OnSceneGUI()
    {
        ExplodeOnClick explodeOnClick = (ExplodeOnClick)target;

        if (explodeOnClick == null || SceneView.lastActiveSceneView == null)
        {
            return;
        }

        if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDrag)
        {
            SceneView.lastActiveSceneView.Repaint();
        }

        if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out RaycastHit hit))
        {
            Handles.color = Color.red;
            Handles.DrawWireDisc(hit.point, Vector3.up, explodeOnClick.Radius);
            int hits = Physics.OverlapSphereNonAlloc(hit.point, explodeOnClick.Radius, Hits, explodeOnClick.HitLayer);
            for (int i = 0; i < hits; i++)
            {
                if (Hits[i].TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
                {
                    float distance = Vector3.Distance(hit.point, Hits[i].transform.position);

                    if (!Physics.Raycast(hit.point, (Hits[i].transform.position - hit.point).normalized, distance, explodeOnClick.BlockExplosionLayer.value))
                    {
                        Handles.color = Color.green;
                        Handles.DrawWireArc(rigidbody.transform.position, Vector3.up, Vector3.one, 360, 1f);
                        Handles.DrawLine(hit.point, rigidbody.transform.position);
                        Handles.Label(rigidbody.transform.position, $"{Mathf.FloorToInt(Mathf.Lerp(explodeOnClick.MaxDamage, explodeOnClick.MinDamage, (distance / explodeOnClick.Radius)))} damage", HitStyle);
                    }
                    else
                    {
                        Handles.color = Color.red;
                        Handles.DrawWireArc(rigidbody.transform.position, Vector3.up, Vector3.one, 360, 1f);
                        Handles.DrawLine(hit.point, rigidbody.transform.position);
                        Handles.Label(rigidbody.transform.position, $"0 damage", NoHitStyle);
                    }
                }
            }
        }
    }
}
