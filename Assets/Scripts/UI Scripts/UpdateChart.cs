using UnityEngine;
using ChartAndGraph;
using System.Collections.Generic;
using TMPro;
public class UpdateChart : MonoBehaviour
{
    public enum ChartDataType
    {
        Vx,
        Roll,
        Pitch,
        Yaw
    }
    public TextMeshProUGUI chartTitle;
    public ChartDataType chartDatatype;
    public FMISimulator FMI;
    public GraphChartBase graph;  // ��Ʈ ������Ʈ ����
    private float stepTime = 0;
    private List<KeyValuePair<float, float>> chartData = new List<KeyValuePair<float, float>>();  // x, y �� ���� ����Ʈ
    public float timeWindow = 1f;  // x�� ���� 10�ʷ� ����
    private bool isTimeWindowReached = false;  // 10�� ���� ���� ����

    void Start()
    {
        chartTitle.text = chartDatatype.ToString();
        // �ʱ�ȭ
        graph.DataSource.StartBatch();
        graph.DataSource.AutomaticHorizontalView = false;  // �ڵ� Ȯ�� ��
        graph.DataSource.HorizontalViewSize = timeWindow;  // x�� ������ 10�ʷ� ����
        graph.DataSource.HorizontalViewOrigin = 0;  // ó���� x���� �������� 0���� ���� (���ʺ��� ����)
        graph.DataSource.EndBatch();
    }

    void FixedUpdate()
    {
        // step time ����
        stepTime += Time.fixedDeltaTime;

        // y���� ���ϴ� ������� ��� (���� ������ ����)
        float yValue = GetYValue(stepTime);

        // ������ �߰� �� ����
        UpdateChartData(stepTime, yValue);
    }
    private float GetYValue(float stepTime)
    {
        switch(chartDatatype)
        {
            case ChartDataType.Vx:
                return (float)FMI.simulationResult[13];
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
    void UpdateChartData(float stepTime, float yValue)
    {
        // ���ο� ������ �߰�
        chartData.Add(new KeyValuePair<float, float>(stepTime, yValue));

        // 10�ʰ� ������ ó�� �� ���� (10�� ���� ����)
        if (stepTime >= timeWindow)
        {
            isTimeWindowReached = true;
        }

        // 10�� ���� ������ ������ ����
        if (isTimeWindowReached)
        {
            while (chartData.Count > 0 && chartData[0].Key < stepTime - timeWindow)
            {
                chartData.RemoveAt(0);  // ���� ������ �����͸� ����
            }
        }

        // ��Ʈ ������ ����
        graph.DataSource.StartBatch();

        // ī�װ� �ʱ�ȭ �� �ٽ� ä��
        graph.DataSource.ClearCategory("Player 1");

        foreach (var dataPoint in chartData)
        {
            graph.DataSource.AddPointToCategory("Player 1", dataPoint.Key, dataPoint.Value);
        }

        // x���� �������� �����Ͽ� ���ʿ��� ���������� ������Ʈ
        if (isTimeWindowReached)
        {
            graph.DataSource.HorizontalViewOrigin = stepTime - timeWindow;
        }
        else
        {
            // �ʱ� 10�� ������ ���ʿ������� �������� �ױ� ���� Origin�� 0���� ����
            graph.DataSource.HorizontalViewOrigin = 0;
        }

        graph.DataSource.EndBatch();
    }
}
