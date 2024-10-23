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
    string valuetype;
    public TextMeshProUGUI chartTitle;
    public ChartDataType chartDatatype;
    public FMISimulator FMI;
    public GraphChartBase graph;  // ��Ʈ ������Ʈ ����
    private float stepTime = 0;
    private Queue<KeyValuePair<float, float>> chartData = new Queue<KeyValuePair<float, float>>();  // x, y �� ���� ť
    public float timeWindow = 1f;  // x�� ���� 10�ʷ� ����
    //private bool isTimeWindowReached = false;  // 10�� ���� ���� ����

    void OnValidate()
    {
        // Inspector���� Enum ���� ����Ǹ� ������ ������Ʈ
        if (chartTitle != null)
        {
            chartTitle.text = chartDatatype.ToString()+valuetype;
        }
    }

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

    void Update()
    {
        // step time ����
        stepTime += Time.deltaTime;

        // y���� ���ϴ� ������� ���
        float yValue = GetYValue(stepTime);

        // ������ �߰� �� ����
        UpdateChartData(stepTime, yValue);
    }

    private float GetYValue(float stepTime)
    {
        switch (chartDatatype)
        {
            case ChartDataType.Vx:
                valuetype = "[m/s]";
                return (float)FMI.simulationResult[19];
            case ChartDataType.Roll:
                valuetype = "[��]";
                return (float)FMI.simulationResult[5];
            case ChartDataType.Pitch:
                valuetype = "[��]";
                return (float)FMI.simulationResult[6];
            case ChartDataType.Yaw:
                valuetype = "[��]";
                return (float)FMI.simulationResult[3];
            default:
                return 0;
        }
    }

    void UpdateChartData(float stepTime, float yValue)
    {
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
