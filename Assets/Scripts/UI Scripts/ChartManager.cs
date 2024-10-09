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

    // �׷��� ���� ���� �迭
    public ChartInfo[] charts;

    private Dictionary<GraphChartBase, Queue<KeyValuePair<float, float>>> chartDataMap =
        new Dictionary<GraphChartBase, Queue<KeyValuePair<float, float>>>();

    void OnValidate()
    {
        // Inspector���� Enum ���� ����Ǹ� ������ ������Ʈ
        foreach (var chart in charts)
        {
            if (chart.chartTitle != null)
            {
                chart.chartTitle.text = chart.chartDatatype.ToString();
            }
        }
    }

    void Start()
    {
        foreach (var chart in charts)
        {
            // �� ��Ʈ �ʱ�ȭ
            chart.chartTitle.text = chart.chartDatatype.ToString();

            chart.graph.DataSource.StartBatch();
            chart.graph.DataSource.AutomaticHorizontalView = false;  // �ڵ� Ȯ�� ��
            chart.graph.DataSource.HorizontalViewSize = timeWindow;  // x�� ���� 10�ʷ� ����
            chart.graph.DataSource.HorizontalViewOrigin = 0;  // ó���� x���� �������� 0���� ����
            chart.graph.DataSource.EndBatch();

            // �� �׷����� �ش��ϴ� �����͸� ������ ť �ʱ�ȭ
            chartDataMap[chart.graph] = new Queue<KeyValuePair<float, float>>();
        }
    }

    void Update()
    {
        // step time ����
        stepTime += Time.deltaTime;

        foreach (var chart in charts)
        {
            // y���� ����ϰ� �׷��� ������Ʈ
            float yValue = GetYValue(chart.chartDatatype, stepTime);
            UpdateChartData(chart.graph, stepTime, yValue);
        }
    }

    private float GetYValue(ChartDataType chartDatatype, float stepTime)
    {
        switch (chartDatatype)
        {
            case ChartDataType.Vx:
                return (float)FMI.simulationResult[19];
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
        // �׷����� �ش��ϴ� ������ ť ��������
        Queue<KeyValuePair<float, float>> chartData = chartDataMap[graph];

        // ���ο� ������ �߰�
        chartData.Enqueue(new KeyValuePair<float, float>(stepTime, yValue));

        // 10�ʰ� �Ѵ� ������ ������ ����
        while (chartData.Count > 0 && chartData.Peek().Key < stepTime - timeWindow)
        {
            chartData.Dequeue();
        }

        // ��Ʈ ������ ����
        graph.DataSource.StartBatch();

        // ���� �����͸� ������ �ʰ� �������� �߰��� �����͸� �߰�
        graph.DataSource.AddPointToCategory("Player 1", stepTime, yValue);

        // x���� �������� �����Ͽ� ���ʿ��� ���������� ������Ʈ
        graph.DataSource.HorizontalViewOrigin = stepTime - timeWindow;

        graph.DataSource.EndBatch();
    }
}
