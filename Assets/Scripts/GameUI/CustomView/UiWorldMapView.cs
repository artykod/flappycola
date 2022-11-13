using UnityEngine;
using UnityEngine.UI;

public class UiWorldMapView : UiView
{
    private readonly Session _session;

    public UiWorldMapView(Session session, string prefabName) : base(prefabName)
    {
        _session = session;
    }

    protected override void OnInitialize(GameObject viewInstance)
    {
        base.OnInitialize(viewInstance);

        var levelButton = viewInstance.GetComponentInChildren<Button>();

        levelButton.onClick.AddListener(OnLevelClick);
    }

    private void OnLevelClick()
    {
        _session.GameStateFsm.GoToState(new FlappyGameState());
    }
}
