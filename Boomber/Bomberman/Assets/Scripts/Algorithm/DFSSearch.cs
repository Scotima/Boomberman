using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// DFS 알고리즘 사용->상자를 찾는 유틸리티 클래스
/// </summary>

public class DFSSearch
{
    private static readonly Vector2Int[] directions =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

    /// <summary>
    /// DFS로 목표 타일을 찾는다.
    /// </summary>
    /// <param name="map">맵(참조용)</param>
    /// <param name="start">시작 위치</param>
    /// <param name="isTarget">목표 조건(예: 해당 위치가 상자인가?)</param>
    /// <param name="maxDepth">최대 탐색 깊이(루프 방지용, 필요 없으면 생략)</param>
    /// <returns>목표를 찾으면 그 좌표, 못 찾으면 null</returns>
    /// 

    public static Vector2Int? FindTarget(MakeMap map, Vector2Int start, Func<Vector2Int, bool> isTarget, int maxDepth = 20)
    {
        Stack<(Vector2Int pos, int depth)> stack = new Stack<(Vector2Int, int)>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        stack.Push((start, 0));
        visited.Add(start);

        while (stack.Count > 0)
        {
            var (now, depth) = stack.Pop();

            if(isTarget(now))
            {
                return now;
            }

            if(depth >= maxDepth)
            {
                continue;
            }

            foreach (var dir in directions)
            {
                var next = now + dir;
                bool walkable = map.IsWalkable(next);
                Debug.Log($"[DFS] from={now} dir={dir} next={next} → IsWalkable={walkable}");

                if (!visited.Contains(next) && map.IsWalkable(next))
                {
                    stack.Push((next, depth + 1));
                    visited.Add(next);
                }
            }
        }

        return null;
    }
}
