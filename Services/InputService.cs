using System;

public class InputService : IDisposable
{
    public InputService(GameService _service) => gameService = _service;

    #region Variables

    private GameService gameService;
    private PlayerControls playerControls;

    #endregion

    #region Base Methods

    public void Initialize()
    {
        playerControls = new();

        gameService.EventService.AddListener<OnGameplayStartEvent>(OnInputEnable);
        gameService.EventService.AddListener<OnGameplayFinishEvent>(OnInputEnable);
        gameService.EventService.AddListener<OnGamePausedEvent>(OnInputEnable);
        gameService.EventService.AddListener<OnGameResumedEvent>(OnInputEnable);

        //TODO: Block the gameplayInput in the gameOver
        //gameService.EventService.AddListener<OnPlayerDeathEvent>(OnPlayerDeath);
        //gameService.EventService.AddListener<OnPlayerRespawnEvent>(OnPlayerRespawn);
    }

    void IDisposable.Dispose()
    {
        gameService.EventService.RemoveListener<OnGameplayStartEvent>(OnInputEnable);
        gameService.EventService.RemoveListener<OnGameplayFinishEvent>(OnInputEnable);
        gameService.EventService.RemoveListener<OnGamePausedEvent>(OnInputEnable);
        gameService.EventService.RemoveListener<OnGameResumedEvent>(OnInputEnable);

        //gameService.EventService.RemoveListener<OnPlayerDeathEvent>(OnPlayerDeath);
        //gameService.EventService.RemoveListener<OnPlayerRespawnEvent>(OnPlayerRespawn);
    }

    #endregion

    #region Input Methods

    public PlayerControls GetInputPlayer() => playerControls;

    public BaseInputController CreateInputController(InputType type)
    {
        BaseInputController controller = null;

        switch (type)
        {
            case InputType.Player:
                controller = new PlayerInputController();
                break;
            case InputType.AI:
                controller = new AIInputController();
                break;
            default:
                break;
        }

        return controller;
    }

    public void EnableGameplayInput(bool enable)
    {
        Action action = enable ? playerControls.Gameplay.Enable : playerControls.Gameplay.Disable;
        action.Invoke();
    }

    private void OnInputEnable<T>(T data)
    {
        EnableGameplayInput(data is OnGameplayStartEvent || data is OnGameResumedEvent);
    }

    #endregion
}
