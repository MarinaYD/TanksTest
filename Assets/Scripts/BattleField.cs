using UnityEngine;

public class BattleField : MonoBehaviour
{
    [SerializeField] private BoxCollider2D fieldCollider;
    [SerializeField] private LayerMask tankLayer;
    [SerializeField] private float spawnClearance = 0.2f;

    private const int MaxAttempts = 100;

    public bool TryGetEnemySpawn(
        Vector2 tankSize,
        out Vector2 position,
        out Quaternion rotation)
    {
        Bounds bounds = fieldCollider.bounds;

        //offset from borders
        float halfWidth = tankSize.x / 2f;
        float halfHeight = tankSize.y / 2f;

        for (int i = 0; i < MaxAttempts; i++)
        {
            Vector2 candidate;
            float angle;

            // choose border
            int side = Random.Range(0, 4);

            switch (side)
            {
                case 0: // top
                    candidate = new Vector2(
                        Random.Range(
                            bounds.min.x + halfWidth,
                            bounds.max.x - halfWidth),
                        bounds.max.y - halfHeight);
                    angle = -90f;
                    break;

                case 1: // bottom
                    candidate = new Vector2(
                        Random.Range(
                            bounds.min.x + halfWidth,
                            bounds.max.x - halfWidth),
                        bounds.min.y + halfHeight);
                    angle = 90f;
                    break;

                case 2: // left
                    candidate = new Vector2(
                        bounds.min.x + halfWidth,
                        Random.Range(
                            bounds.min.y + halfHeight,
                            bounds.max.y - halfHeight));
                    angle = 0f;
                    break;

                default: // right
                    candidate = new Vector2(
                        bounds.max.x - halfWidth,
                        Random.Range(
                            bounds.min.y + halfHeight,
                            bounds.max.y - halfHeight));
                    angle = 180f;
                    break;
            }

            if (IsPositionFree(candidate, tankSize))
            {
                position = candidate;
                rotation = Quaternion.Euler(0f, 0f, angle);
                return true;
            }
        }

        position = Vector2.zero;
        rotation = Quaternion.identity;
        return false;
    }

    public bool TryGetPlayerSpawn(
        Vector2 tankSize,
        out Vector2 position,
        out Quaternion rotation)
    {
        Bounds bounds = fieldCollider.bounds;

        float halfWidth = tankSize.x / 2f;
        float halfHeight = tankSize.y / 2f;

        Vector2[] corners =
        {
            new Vector2(bounds.min.x + halfWidth,
                        bounds.min.y + halfHeight),

            new Vector2(bounds.min.x + halfWidth,
                        bounds.max.y - halfHeight),

            new Vector2(bounds.max.x - halfWidth,
                        bounds.min.y + halfHeight),

            new Vector2(bounds.max.x - halfWidth,
                        bounds.max.y - halfHeight)
        };

        // mix corners
        for (int i = 0; i < corners.Length; i++)
        {
            int randomIndex = Random.Range(i, corners.Length);

            Vector2 temp = corners[i];
            corners[i] = corners[randomIndex];
            corners[randomIndex] = temp;
        }

        foreach (Vector2 corner in corners)
        {
            if (IsPositionFree(corner, tankSize))
            {
                position = corner;
                if ((bounds.max.x - halfWidth - corner.x) < 1f)
                    rotation = Quaternion.Euler(0f, 0f, 180f);
                else
                    rotation = Quaternion.identity;
                return true;
            }
        }

        position = Vector2.zero;
        rotation = Quaternion.identity;
        return false;
    }

    private bool IsPositionFree(Vector2 position, Vector2 tankSize)
    {
        Vector2 checkSize = tankSize +
            Vector2.one * spawnClearance * 2f;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            position,
            checkSize,
            0f,
            tankLayer
        );

        return colliders.Length == 0;
    }
}
