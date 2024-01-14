using Zenject;

public class Enemy: Character
{
    public override EntityType EntityType => EntityType.Enemy;

    public class Factory : PlaceholderFactory<Enemy>
    {
    }
}
