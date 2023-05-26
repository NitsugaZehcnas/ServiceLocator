using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameService : MonoBehaviour
{
    #region Variables

    public static GameService Instance { get; private set; }

    public AssetService AssetService { get; private set; }
    public CoroutineService CoroutineService { get; private set; }
    public EventService EventService { get; private set; }
    public GameDataService GameDataService { get; private set; }
    public InputService InputService { get; private set; }
    public SceneService SceneService { get; private set; }

    #endregion

    #region MonoBehaviour Methods

    private void Awake()
    {
        //Generate singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        //Create Services
        AssetService = new(this);
        CoroutineService = CreateMonoBehaviourService<CoroutineService>("CoroutineService");
        EventService = new(this);
        GameDataService = new(this);    
        InputService = new(this);
        SceneService = new(this);

        //Initialize services
        CoroutineService.Initialize(this);
        GameDataService.Initialize();
        InputService.Initialize();
    }

    private void Update()
    {

    }

    #endregion

    #region GameService Methods

    private T CreateMonoBehaviourService<T>(string _serviceName) where T : MonoBehaviour
    {
        GameObject _go = Instantiate(new GameObject(_serviceName), transform);
        var _script = _go.AddComponent<T>();

        return _script;
    }

    #endregion
}
