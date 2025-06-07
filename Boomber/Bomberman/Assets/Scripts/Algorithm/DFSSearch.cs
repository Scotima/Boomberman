using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// DFS �˰��� ���->���ڸ� ã�� ��ƿ��Ƽ Ŭ����
/// </summary>

public class DFSSearch
{
    private static readonly Vector2Int[] directions =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

    /// <summary>
    /// DFS�� ��ǥ Ÿ���� ã�´�.
    /// </summary>
    /// <param name="map">��(������)</param>
    /// <param name="start">���� ��ġ</param>
    /// <param name="isTarget">��ǥ ����(��: �ش� ��ġ�� �����ΰ�?)</param>
    /// <param name="maxDepth">�ִ� Ž�� ����(���� ������, �ʿ� ������ ����)</param>
    /// <returns>��ǥ�� ã���� �� ��ǥ, �� ã���� null</returns>
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
                Debug.Log($"[DFS] from={now} dir={dir} next={next} �� IsWalkable={walkable}");

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
