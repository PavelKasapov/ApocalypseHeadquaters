using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class ViewField : MonoBehaviour
{
    [SerializeField] PolygonCollider2D polygonCollider;
    [SerializeField] Light2D frontLight;
    [SerializeField] Light2D backLight;
    [SerializeField] int viewFieldDegrees = 60;
    [SerializeField] float viewFieldDistance = 10;
    [SerializeField] float backViewDistance = 2f;
#if UNITY_EDITOR
    [SerializeField] bool editorRearranging = false;
#endif

    public float ViewFieldDistance => viewFieldDistance;

    private void OnEnable()
    {
        RearrangeCollider();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (editorRearranging)
        {
            SetAnglesAndDistance(viewFieldDegrees, viewFieldDistance, backViewDistance);
        }
    }
#endif

    public void SetAnglesAndDistance(int angle, float distance, float backDistance)
    {
        viewFieldDegrees = angle;
        viewFieldDistance = distance;
        backViewDistance = backDistance;

        if (frontLight != null && backLight != null)
        {
            frontLight.pointLightOuterAngle = angle;
            frontLight.pointLightInnerAngle = frontLight.pointLightOuterAngle * 0.95f ;
            frontLight.pointLightOuterRadius = distance / 2;
            frontLight.pointLightInnerRadius = frontLight.pointLightOuterRadius * 0.95f;
            backLight.pointLightOuterRadius = backDistance / 2;
            backLight.pointLightInnerRadius = backLight.pointLightOuterRadius * 0.95f;
        }
        
        RearrangeCollider();
    }

    private void RearrangeCollider()
    {
        if (viewFieldDegrees <= 0 
            || viewFieldDegrees > 360 
            || viewFieldDistance <= 0)
        {
            return;
        }
       
        var pointCount = viewFieldDegrees / 13 + 1 + (viewFieldDegrees % 13 ==  0 ? 0 : 1);
        var pointCount2 = (360 - viewFieldDegrees) / 13 + 1 + ((360 - viewFieldDegrees) % 13 == 0 ? 0 : 1);
        var points = new Vector2[pointCount + pointCount2];

        //TODO: Remake this two cycles for one;
        for (int i = 0; i < pointCount; i++)
        {
            var relativeUpDegree = (-(float)viewFieldDegrees / 2) + 90 + ((float)viewFieldDegrees / (pointCount - 1) * i);
            var radians = relativeUpDegree * Mathf.PI / 180;
            points[i] = new Vector2(viewFieldDistance * Mathf.Cos(radians), viewFieldDistance * Mathf.Sin(radians));
        }

        for (int i = 0; i < pointCount2; i++)
        {
            var relativeUpDegree = (-(float)viewFieldDegrees / 2) + 90 + ((float)(360 - viewFieldDegrees) / (pointCount2 - 1) * i) + ((float)viewFieldDegrees);
            var radians = relativeUpDegree * Mathf.PI / 180;
            points[i + pointCount] = new Vector2(backViewDistance * Mathf.Cos(radians), backViewDistance * Mathf.Sin(radians));
        }
        //------------------------------------

        polygonCollider.SetPath(0, points);
    }
}