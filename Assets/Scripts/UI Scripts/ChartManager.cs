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

    // 그래프 정보 저장 배열
    public ChartInfo[] charts;

    private Dictionary<GraphChartBase, Queue<KeyValuePair<float, float>>> chartDataMap =
        new Dictionary<GraphChartBase, Queue<KeyValuePair<float, float>>>();

    void OnValidate()
    {
        // Inspector에서 Enum 값이 변경되면 제목을 업데이트
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
            // 각 차트 초기화
            chart.chartTitle.text = chart.chartDatatype.ToString();

            chart.graph.DataSource.StartBatch();
            chart.graph.DataSource.AutomaticHorizontalView = false;  // 자동 확장 끔
            chart.graph.DataSource.HorizontalViewSize = timeWindow;  // x축 범위 10초로 고정
            chart.graph.DataSource.HorizontalViewOrigin = 0;  // 처음엔 x축의 시작점을 0으로 설정
            chart.graph.DataSource.EndBatch();

            // 각 그래프에 해당하는 데이터를 저장할 큐 초기화
            chartDataMap[chart.graph] = new Queue<KeyValuePair<float, float>>();
        }
    }

    void Update()
    {
        // step time 증가
        stepTime += Time.deltaTime;

        foreach (var chart in charts)
        {
            // y값을 계산하고 그래프 업데이트
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
        // 그래프에 해당하는 데이터 큐 가져오기
        Queue<KeyValuePair<float, float>> chartData = chartDataMap[graph];

        // 새로운 데이터 추가
        chartData.Enqueue(new KeyValuePair<float, float>(stepTime, yValue));

        // 10초가 넘는 오래된 데이터 제거
        while (chartData.Count > 0 && chartData.Peek().Key < stepTime - timeWindow)
        {
            chartData.Dequeue();
        }

        // 차트 데이터 갱신
        graph.DataSource.StartBatch();

        // 기존 데이터를 지우지 않고 마지막에 추가된 데이터만 추가
        graph.DataSource.AddPointToCategory("Player 1", stepTime, yValue);

        // x축의 시작점을 설정하여 왼쪽에서 오른쪽으로 업데이트
        graph.DataSource.HorizontalViewOrigin = stepTime - timeWindow;

        graph.DataSource.EndBatch();
    }
}
