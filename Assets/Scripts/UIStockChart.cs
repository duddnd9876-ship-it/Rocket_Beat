using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStockChart : MaskableGraphic
{
    [Header("Chart Settings")]
    public float thickness = 4f; // 선 두께
    [Header("Draw Area")]
    public RectTransform drawArea;

    private List<double> dataPoints = new List<double>();

    // 새로운 데이터를 받아와 차트 갱신
    public void UpdateData(List<double> newData)
    {
        dataPoints = new List<double>(newData);
        SetAllDirty(); // 그래픽을 다시 그리도록 설정 (OnPopulateMesh 호출 유도)
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (dataPoints == null || dataPoints.Count < 2)
            return;

        // 최대/최소값 계산 (데이터 스케일링용)
        double min = dataPoints[0];
        double max = dataPoints[0];
        for (int i = 0; i < dataPoints.Count; i++)
        {
            if (dataPoints[i] < min) min = dataPoints[i];
            if (dataPoints[i] > max) max = dataPoints[i];
        }

        // 최대값과 최소값이 같은 경우 나누기 오류 방지
        double range = max - min;
        if (range == 0) range = 1.0;

        Rect rect;

        // DrawArea가 있으면 그 영역만 사용
        if (drawArea != null)
        {
            Vector2 localPos = rectTransform.InverseTransformPoint(drawArea.position);

            rect = new Rect(
                localPos.x - drawArea.rect.width * drawArea.pivot.x,
                localPos.y - drawArea.rect.height * drawArea.pivot.y,
                drawArea.rect.width,
                drawArea.rect.height
            );
        }
        else
        {
            rect = rectTransform.rect;
        }

        float width = rect.width;
        float height = rect.height;

        // UI 사각형 내에서의 각 데이터 포인트의 좌표 계산
        float xStep = width / (dataPoints.Count - 1);
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < dataPoints.Count; i++)
        {
            float x = rect.xMin + i * xStep;
            // 데이터를 UI 높이에 맞춰 정규화 (yMin ~ yMax 사이로 매핑)
            float y = rect.yMin + (float)((dataPoints[i] - min) / range) * height;
            points.Add(new Vector2(x, y));
        }

        // 계산된 점들을 잇는 선분 그리기
        for (int i = 0; i < points.Count - 1; i++)
        {
            DrawLineSegment(points[i], points[i + 1], vh);
        }
    }

    // 두 점 사이에 두께를 가진 사각형(선분)을 그려 메시에 추가
    private void DrawLineSegment(Vector2 start, Vector2 end, VertexHelper vh)
    {
        Vector2 direction = (end - start).normalized;
        // 선의 방향에 수직인 벡터(법선 벡터) 구하기
        Vector2 normal = new Vector2(-direction.y, direction.x) * (thickness / 2f);

        // 사각형의 네 꼭짓점 설정
        Vector2 v1 = start - normal;
        Vector2 v2 = start + normal;
        Vector2 v3 = end + normal;
        Vector2 v4 = end - normal;

        int vertexCount = vh.currentVertCount;

        // 컬러 설정 (인스펙터의 Graphic Color 필드 값 사용)
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = v1;
        vh.AddVert(vertex);

        vertex.position = v2;
        vh.AddVert(vertex);

        vertex.position = v3;
        vh.AddVert(vertex);

        vertex.position = v4;
        vh.AddVert(vertex);

        // 두 개의 삼각형으로 사각형 구성
        vh.AddTriangle(vertexCount, vertexCount + 1, vertexCount + 2);
        vh.AddTriangle(vertexCount, vertexCount + 2, vertexCount + 3);
    }
}
