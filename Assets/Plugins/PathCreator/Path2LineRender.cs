using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using Sirenix.OdinInspector;

public class Path2LineRender : MonoBehaviour
{
    [InfoBox("在线段上按shift并单击，添加一个点，在线段上的红点按ctrl并单击，移除一个点")]
    public PathCreator pathCreator;
    public LineRenderer lineRenderer;

    public int SkipPointNum = 20;

    [Button("SetLine")]
    void SetLine()
    {
        if (pathCreator.path != null)
        {
            pathCreator.path.UpdateTransform(transform);
            int num = pathCreator.path.NumPoints - SkipPointNum;
            lineRenderer.SetPositions(new Vector3[num + 1]);
            print($"设置贝塞尔曲线的显示，如果报错，将LineRender的数组长度设置为{num - 1}");
            Vector3 vec = new Vector3();
            Vector3 vec_last = new Vector3();
            for (int i = 0; i < num; i++)
            {
                int nextI = i + 1;
                if (nextI >= num)
                {
                    if (pathCreator.path.isClosedLoop)
                    {
                        nextI %= pathCreator.path.NumPoints;
                    }
                    else
                    {
                        break;
                    }
                }
                vec = pathCreator.path.GetPoint(i);
                if (vec.x == 0 && vec.y == 0 && vec.z == 0 && vec_last != null)
                {
                    vec = vec_last;
                }
                else
                {
                    vec_last = vec;
                }
                lineRenderer.SetPosition(i, vec);
            }
        }

        //lineRenderer.SetPositions(pathCreator.editorData.bezierPath.points.ToArray());
    }

    void DrawQuadraticBezierCurve()
    {
        //for (int i = 0; i < numberOfPoints; i++)
        //{
        //    float t = i / (float)(numberOfPoints - 1);
        //    Vector3 position = CalculateQuadraticBezierPoint(t, startPoint.position, controlPoint.position, endPoint.position);
        //    lineRenderer.SetPosition(i, position);
        //}
    }

    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0; // (1-t)^2 * P0
        p += 2 * u * t * p1; // 2 * (1-t) * t * P1
        p += tt * p2; // t^2 * P2

        return p;
    }
}
