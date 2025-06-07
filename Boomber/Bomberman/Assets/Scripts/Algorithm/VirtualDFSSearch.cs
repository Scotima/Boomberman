using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class VirtualDFSSearch
{
    private struct DFSNode
    {
        public Vector2Int tile;
        public int nextDirIndex;
        public DFSNode(Vector2Int t, int idx)
        {
            tile = t;
            nextDirIndex = idx;
        }
    }

    //변수 선언

    private readonly Stack<DFSNode> stack = new();
    private readonly HashSet<Vector2Int> visited = new();
    private readonly List<Vector2Int> path = new();

    private readonly MakeMap map;
    private readonly float radius;
    private readonly int maxDepth;
    private readonly Vector2Int goal;

    public bool FoundBox { get; private set; } = false;
    public bool IsFinished => stack.Count == 0;
    public List<Vector2Int> Path => new(path);

    private static readonly Vector2Int[] directions =
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    public VirtualDFSSearch(MakeMap map, Vector2Int start,Vector2Int goal, float radius, int maxDepth)
    {
        this.map = map;
        this.radius = radius;
        this.maxDepth = maxDepth;
        this.goal = goal;

        stack.Push(new DFSNode(start, 0));
        visited.Add(start);
        path.Add(start);
    }

    public void Step()
    {


        if (stack.Count == 0 || FoundBox)
        {
            Debug.Log("[DFS] 종료 조건 도달 → stack empty or box found");
            return;

        }

        DFSNode node = stack.Pop();
        Debug.Log($"[DFS] Pop: {node.tile}, dir={node.nextDirIndex}, stack={stack.Count}");
        Vector2Int now = node.tile;
        int dirIndex = node.nextDirIndex;

        while (path.Count > stack.Count + 1)
            path.RemoveAt(path.Count - 1);

        if (now == goal)
        {
            Debug.Log($"[DFS] 목표 도달 성공: {goal}");
            FoundBox = true;
            stack.Clear();
            return;
        }


        if (stack.Count + 1 >= maxDepth)
            return;

        for (int i = dirIndex; i < directions.Length; i++)
        {
            Vector2Int next = now + directions[i];

           if(!visited.Contains(next) && map.IsWalkable(next, false))
            {
                stack.Push(new DFSNode(now, i + 1)); // 현재 상태 저장.
                stack.Push(new DFSNode(next, 0)); // 다음 이동.
                visited.Add(next);
                path.Add(next);
                return;
            }
        }

        // 더 이상 자식이 없으면 path에서 현재 노드 제거 (백트래킹)
        if (path.Count > 0 && path[^1] == now)
            path.RemoveAt(path.Count - 1);
    }


}
