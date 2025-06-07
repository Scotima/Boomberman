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

    //���� ����

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
            Debug.Log("[DFS] ���� ���� ���� �� stack empty or box found");
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
            Debug.Log($"[DFS] ��ǥ ���� ����: {goal}");
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
                stack.Push(new DFSNode(now, i + 1)); // ���� ���� ����.
                stack.Push(new DFSNode(next, 0)); // ���� �̵�.
                visited.Add(next);
                path.Add(next);
                return;
            }
        }

        // �� �̻� �ڽ��� ������ path���� ���� ��� ���� (��Ʈ��ŷ)
        if (path.Count > 0 && path[^1] == now)
            path.RemoveAt(path.Count - 1);
    }


}
