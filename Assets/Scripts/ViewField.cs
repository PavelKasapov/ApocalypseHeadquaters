using UnityEngine;

[ExecuteAlways]
public class ViewField : MonoBehaviour
{
    [SerializeField] PolygonCollider2D polygonCollider;
    [SerializeField] int viewFieldDegrees = 60;
    [SerializeField] float viewFieldDistance = 10;
#if UNITY_EDITOR
    [SerializeField] bool editorRearranging = false;
#endif

    private void OnEnable()
    {
        RearrangeCollider();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (editorRearranging)
        {
            RearrangeCollider();
        }
    }
#endif

    public void SetAnglesAndDistance(int angle, float distance)
    {
        viewFieldDegrees = angle;
        viewFieldDistance = distance;
        RearrangeCollider();
    }

    private void RearrangeCollider()
    {
        if (viewFieldDegrees <= 0 || viewFieldDistance <= 0)
        {
            return;
        }
       
        var pointCount = viewFieldDegrees / 13 + 2 + (viewFieldDegrees % 13 ==  0 ? 0 : 1);
        var points = new Vector2[pointCount];
        points[0] = new Vector2(0, 0);
        for (int i = 1; i < pointCount; i++)
        {
            var relativeUpDegree = (-(float)viewFieldDegrees / 2) + 90 + ((float)viewFieldDegrees / (pointCount - 2) * (i - 1));
            var radians = relativeUpDegree * Mathf.PI / 180;
            points[i] = new Vector2(viewFieldDistance * Mathf.Cos(radians), viewFieldDistance * Mathf.Sin(radians));
        }
        polygonCollider.SetPath(0, points);
    }
}
