using UnityEngine;
using UnitySensors.Data.PointCloud;
using UnitySensors.Sensor.LiDAR;

public class LiDARLineVisualizer : MonoBehaviour
{
    public RaycastLiDARSensor lidarSensor;
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    private void Update()
    {
        if (lidarSensor == null || lidarSensor.pointCloud.points.Length == 0)
        {
            Debug.Log("No points available.");
            return;
        }
        else
        {
            Debug.Log("Number of points: " + lidarSensor.pointCloud.points.Length);
            //Debug.Log(lidarSensor.pointCloud.points);
        }

        lineRenderer.positionCount = lidarSensor.pointCloud.points.Length;

        for (int i = 0; i < lidarSensor.pointCloud.points.Length; i++)
        {
            lineRenderer.SetPosition(i, lidarSensor.pointCloud.points[i].position);
        }
    }
}
