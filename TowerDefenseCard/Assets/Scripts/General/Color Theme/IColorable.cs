public interface IColorable
{
    public bool UseColorTheme { get; }
    public ColorTheme ColorTheme { get; }
    public ColorID ColorID { get; }
    public void OnChangeColorTheme(ColorTheme colorTheme);
    
}
