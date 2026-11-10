using System;

[Serializable]
public class GameSaveData
{
    public PlayerSaveData player;
    public EnemySaveData[] enemies;
    public int level;
}

[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public float rotation;

    public int lives;
    public int killedEnemies;
    public int loses;
}

[Serializable]
public class EnemySaveData
{

    public float[] position;
    public float rotation;

    public int lives;
}
