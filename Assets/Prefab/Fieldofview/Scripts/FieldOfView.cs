using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfView : MonoBehaviour
{

    public float ViewRadius;
    [Range(0, 360)]
    public float ViewAngle;

    public float MeshResolution;
    public int EdgeResolveIterations = 1;
    private ViewCastInfo _viewCastInfo;


    public MeshFilter ViewMeshFilter;
    private Mesh _viewMesh;

    public float EdgeDistanceThreshold;

    public Transform player;

    private void Start()
    {

        _viewMesh = new Mesh();
        _viewMesh.name = "View Mesh";

        ViewMeshFilter.mesh = _viewMesh;

    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }


    private void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(ViewAngle * MeshResolution);
        float stepAngleSize = ViewAngle / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i < stepCount; i++)
        {
            float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;
            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * ViewRadius, Color.black);
            ViewCastInfo newCast = ViewCast(angle);


            if (i > 0)
            {

                bool edgeThresholdExceed = Mathf.Abs(oldViewCast.Dist - newCast.Dist) > EdgeDistanceThreshold;


                if (oldViewCast.Hit != newCast.Hit || oldViewCast.Hit && newCast.Hit && edgeThresholdExceed)
                {
                    //find the edge to prevent edge jittering

                    EdgeInfo edge = FindEdge(oldViewCast, newCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newCast.Point);

            oldViewCast = newCast;
        }

        int vertexCount = viewPoints.Count + 1;


        Vector3[] vertices = new Vector3[vertexCount];

        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount - 1; i++)
        {

            if (i < vertexCount - 2)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

        }

        _viewMesh.Clear();

        _viewMesh.vertices = vertices;

        _viewMesh.triangles = triangles;

        _viewMesh.RecalculateNormals();

    }

    private ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, ViewRadius))
        {
            _viewCastInfo.Hit = true;
            _viewCastInfo.Dist = hit.distance;
            _viewCastInfo.Angle = globalAngle;
            _viewCastInfo.Point = hit.point;
            if (hit.collider.name == "Player")
            {
                ViewMeshFilter.GetComponent<MeshRenderer>().material.color = Color.red;//указываем игроку что мы в активности за ним бегать

                Vector3 pl = player.position;
                pl.y = transform.position.y;

                transform.LookAt(pl);//смотрим на игрока

                GetComponent<NavMeshAgent>().enabled = true;//включаем компонент ИИ
                GetComponent<NavMeshAgent>().SetDestination(player.position);//задаем таргет

                if (Vector3.Distance(transform.position, pl) < 1)//Дистанция до игрока проверяем на коллизию
                {
                    player.GetComponent<Player>().End(false);
                    player.GetComponent<Player>().enabled = false;
                    Destroy(gameObject);
                }
            }
            return _viewCastInfo;
        }
        else
        {
          //  GetComponent<NavMeshAgent>().enabled = false;//включаем компонент ИИ
           // ViewMeshFilter.GetComponent<MeshRenderer>().material.color = Color.green;//указываем игроку что мы в активности за ним бегать
            _viewCastInfo.Hit = false;
            _viewCastInfo.Dist = ViewRadius;
            _viewCastInfo.Angle = globalAngle;
            _viewCastInfo.Point = transform.position + dir * ViewRadius;

            return _viewCastInfo;
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {

        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private EdgeInfo FindEdge(ViewCastInfo minCast, ViewCastInfo maxCast)
    {
        float minAngle = minCast.Angle;
        float maxAngle = maxCast.Angle;

        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < EdgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newCast = ViewCast(angle);

            bool edgeThresholdExceed = Mathf.Abs(minCast.Dist - newCast.Dist) > EdgeDistanceThreshold;

            if (newCast.Hit == minCast.Hit && !edgeThresholdExceed)
            {
                minAngle = angle;
                minPoint = newCast.Point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newCast.Point;
            }
        }


        return new EdgeInfo(minPoint, maxPoint);


    }

}
