using UnityEngine;

public class PathNode : MonoBehaviour
{
    public PathNode[] adjacent;

    public PathNode GetRandomAdjacent()
    {
        if (adjacent.Length > 0)
            return adjacent[Random.Range(0, adjacent.Length)];

        return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        foreach (PathNode node in adjacent)
            Gizmos.DrawLine(transform.position, node.transform.position);
    }
#endif
}
