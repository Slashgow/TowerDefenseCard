public interface IColorable
{
    public bool UseColorTheme { get; }
    public ColorID ColorID { get; }
    public void OnChangeColorTheme(ColorTheme colorTheme);
    
}
