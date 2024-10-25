using UnityEngine;
using ChartAndGraph;
using System.Collections.Generic;
using TMPro;

public class ChartManager : MonoBehaviour
{
    public enum ChartDataType
    {
        Vx,
        Roll,
        Pitch,
        Yaw
    }

    [System.Serializable]
    public class ChartInfo
    {
        public TextMeshProUGUI chartTitle;
        public ChartDataType chartDatatype;
        public GraphChartBase graph;
    }

    public FMISimulator FMI;
    public float timeWindow = 1f;
    private float stepTime = 0;
    private float timeSinceLastUpdate = 0f;

    public float updateRate = 10f;
    private float updateInterval;

    public ChartInfo[] charts;

    private Dictionary<GraphChartBase, Queue<KeyValuePair<float, float>>> chartDataMap =
        new Dictionary<GraphChartBase, Queue<KeyValuePair<float, float>>>();

    private string GetUnit(ChartDataType type)
    {
        switch (type)
        {
            case ChartDataType.Vx:
                return "[km/h]";
            case ChartDataType.Roll:
            case ChartDataType.Pitch:
            case ChartDataType.Yaw:
                return "[°]";
            default:
                return "";
        }
    }

    void UpdateChartTitle(ChartInfo chart)
    {
        if (chart.chartTitle != null)
        {
            string unit = GetUnit(chart.chartDatatype);
            chart.chartTitle.text = $"{chart.chartDatatype}{unit}";
        }
    }

    void OnValidate()
    {
        foreach (var chart in charts)
        {
            UpdateChartTitle(chart);
        }
    }

    void Start()
    {
        updateInterval = 1f / updateRate;

        foreach (var chart in charts)
        {
            UpdateChartTitle(chart);
            chart.graph.DataSource.StartBatch();
            chart.graph.DataSource.AutomaticHorizontalView = false;
            chart.graph.DataSource.HorizontalViewSize = timeWindow;
            chart.graph.DataSource.HorizontalViewOrigin = 0;
            chart.graph.DataSource.EndBatch();

            chartDataMap[chart.graph] = new Queue<KeyValuePair<float, float>>();
        }
    }

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            stepTime += timeSinceLastUpdate;
            timeSinceLastUpdate = 0f;

            foreach (var chart in charts)
            {
                float yValue = GetYValue(chart.chartDatatype, stepTime);
                UpdateChartData(chart.graph, stepTime, yValue);
            }
        }
    }

    private float GetYValue(ChartDataType chartDatatype, float stepTime)
    {
        switch (chartDatatype)
        {
            case ChartDataType.Vx:
                return (float)(FMI.simulationResult[19] * 3.6);
            case ChartDataType.Roll:
                return (float)FMI.simulationResult[5];
            case ChartDataType.Pitch:
                return (float)FMI.simulationResult[6];
            case ChartDataType.Yaw:
                return (float)FMI.simulationResult[3];
            default:
                return 0;
        }
    }

    void UpdateChartData(GraphChartBase graph, float stepTime, float yValue)
    {
        Queue<KeyValuePair<float, float>> chartData = chartDataMap[graph];

        chartData.Enqueue(new KeyValuePair<float, float>(stepTime, yValue));

        while (chartData.Count > 0 && chartData.Peek().Key < stepTime - timeWindow)
        {
            chartData.Dequeue();
        }

        graph.DataSource.StartBatch();
        graph.DataSource.AddPointToCategory("Player 1", stepTime, yValue);
        graph.DataSource.HorizontalViewOrigin = stepTime - timeWindow;
        graph.DataSource.EndBatch();
    }
}
