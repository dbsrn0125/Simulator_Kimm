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
    public GraphChartBase graph;  // 차트 오브젝트 연결
    private float stepTime = 0;
    private List<KeyValuePair<float, float>> chartData = new List<KeyValuePair<float, float>>();  // x, y 값 저장 리스트
    public float timeWindow = 1f;  // x축 길이 10초로 고정
    private bool isTimeWindowReached = false;  // 10초 범위 도달 여부

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

    void FixedUpdate()
    {
        // step time 증가
        stepTime += Time.fixedDeltaTime;

        // y값을 원하는 방식으로 계산 (선형 증가로 가정)
        float yValue = GetYValue(stepTime);

        // 데이터 추가 및 갱신
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
        // 새로운 데이터 추가
        chartData.Add(new KeyValuePair<float, float>(stepTime, yValue));

        // 10초가 지나면 처음 값 제거 (10초 범위 유지)
        if (stepTime >= timeWindow)
        {
            isTimeWindowReached = true;
        }

        // 10초 이후 오래된 데이터 제거
        if (isTimeWindowReached)
        {
            while (chartData.Count > 0 && chartData[0].Key < stepTime - timeWindow)
            {
                chartData.RemoveAt(0);  // 가장 오래된 데이터를 제거
            }
        }

        // 차트 데이터 갱신
        graph.DataSource.StartBatch();

        // 카테고리 초기화 후 다시 채움
        graph.DataSource.ClearCategory("Player 1");

        foreach (var dataPoint in chartData)
        {
            graph.DataSource.AddPointToCategory("Player 1", dataPoint.Key, dataPoint.Value);
        }

        // x축의 시작점을 설정하여 왼쪽에서 오른쪽으로 업데이트
        if (isTimeWindowReached)
        {
            graph.DataSource.HorizontalViewOrigin = stepTime - timeWindow;
        }
        else
        {
            // 초기 10초 동안은 왼쪽에서부터 차곡차곡 쌓기 위해 Origin을 0으로 유지
            graph.DataSource.HorizontalViewOrigin = 0;
        }

        graph.DataSource.EndBatch();
    }
}
