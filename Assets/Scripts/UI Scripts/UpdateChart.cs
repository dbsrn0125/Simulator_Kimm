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
    public GraphChartBase graph;  // 차트 오브젝트 연결
    private float stepTime = 0;
    private Queue<KeyValuePair<float, float>> chartData = new Queue<KeyValuePair<float, float>>();  // x, y 값 저장 큐
    public float timeWindow = 1f;  // x축 길이 10초로 고정
    //private bool isTimeWindowReached = false;  // 10초 범위 도달 여부

    void OnValidate()
    {
        // Inspector에서 Enum 값이 변경되면 제목을 업데이트
        if (chartTitle != null)
        {
            chartTitle.text = chartDatatype.ToString()+valuetype;
        }
    }

    void Start()
    {
        chartTitle.text = chartDatatype.ToString();
        // 초기화
        graph.DataSource.StartBatch();
        graph.DataSource.AutomaticHorizontalView = false;  // 자동 확장 끔
        graph.DataSource.HorizontalViewSize = timeWindow;  // x축 범위를 10초로 고정
        graph.DataSource.HorizontalViewOrigin = 0;  // 처음엔 x축의 시작점을 0으로 설정 (왼쪽부터 시작)
        graph.DataSource.EndBatch();
    }

    void Update()
    {
        // step time 증가
        stepTime += Time.deltaTime;

        // y값을 원하는 방식으로 계산
        float yValue = GetYValue(stepTime);

        // 데이터 추가 및 갱신
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
                valuetype = "[°]";
                return (float)FMI.simulationResult[5];
            case ChartDataType.Pitch:
                valuetype = "[°]";
                return (float)FMI.simulationResult[6];
            case ChartDataType.Yaw:
                valuetype = "[°]";
                return (float)FMI.simulationResult[3];
            default:
                return 0;
        }
    }

    void UpdateChartData(float stepTime, float yValue)
    {
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
