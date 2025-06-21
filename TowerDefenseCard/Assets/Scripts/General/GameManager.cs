using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public GameMode GameMode {  get; private set; }
}
