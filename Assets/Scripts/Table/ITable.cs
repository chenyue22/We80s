namespace We80s.GameData.Table
{
    public enum TableType
    {
        Event,
        Dialog,
        Choose,
        UI,
        Texture,
        Sprite,
        GameObject,
        Material,
        Mesh,
        Count
    }

    public interface ITableColumn
    {
        
    }

    public interface IAssetColumn
    {
        string AssetPath { get; }
    }
    
    public interface ITable
    {
        ITableColumn GetColumn(int id);
    }   
}
