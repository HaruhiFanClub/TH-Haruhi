
public static class Layers
{
    public const int Default = 0;
    public const int TransparentFx = 1;
    public const int IgoreRaycast = 2;
    public const int Water = 4;
    public const int Ui = 5;

    public const int Character = 8;
    public const int Enemy = 9;
    public const int Obstacle = 10;
    public const int BackGround = 11;
}

public static class Tags
{

}

public static class LayersMask
{
    public const int None = 0;
    public const int Default = 1 << Layers.Default;
    public const int Water = 1 << Layers.Water;
    public const int Ui = 1 << Layers.Ui;
    public const int Character = 1 << Layers.Character;
    public const int Monster = 1 << Layers.Enemy;
    public const int Creature = Character | Monster;
    public const int Ground = 1 << Layers.BackGround;
    public const int Obstacle = 1 << Layers.Obstacle;
    public const int All = 0x0fffffff;

    public static int GetLayerMask(int layer)
    {
        return 1 << layer;
    }
}