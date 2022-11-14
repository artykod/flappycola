using System.Collections;
using DataBinding;

public class UiFlappyGameViewModel : UiViewModel
{
    private readonly Session _session;
    private readonly DataProperty<bool> IsPause;
    private readonly DataProperty<string> StartScreenText;
    private readonly DataProperty<float> StartScreenOpacity;

    private FlappyGame _game;

    public UiFlappyGameViewModel(Session session)
    {
        _session = session;

        AddNode(IsPause = new DataProperty<bool>("isPause", false));
        AddNode(StartScreenText = new DataProperty<string>("startScreenText", default));
        AddNode(StartScreenOpacity = new DataProperty<float>("startScreenOpacity", 1f));
    }

    public void AssignGame(FlappyGame game)
    {
        _game = game;

        _game.OnPause += OnGamePause;

        OnGamePause(_game.IsPaused);

        _session.AsyncProvider.StartAsync(UpdateStartTimer());
    }

    private void OnGamePause(bool pause)
    {
        IsPause.SetValue<bool>(pause);
    }

    protected override void OnDispose()
    {
        base.OnDispose();

        _game.OnPause -= OnGamePause;
    }

    private IEnumerator UpdateStartTimer()
    {
        while (true)
        {
            var time = (int)_game.StartTimer;

            if (time > -1)
            {
                StartScreenText.SetValue<string>(time <= 0 ? "FLY!" : time.ToString());
            }

            if (time < 1)
            {
                var opacity = StartScreenOpacity.GetValue<float>();

                opacity = UnityEngine.Mathf.Max(0f, opacity - UnityEngine.Time.deltaTime * 0.5f);

                StartScreenOpacity.SetValue<float>(opacity);

                if (opacity <= float.Epsilon)
                {
                    break;
                }
            }

            yield return null;
        }
    }
}
