using DataBinding;
using System;

public class UiFlappySkinSelectViewModel : UiViewModel
{
    private readonly Session _session;

    [AutoCreate] private readonly DataSource Skins;
    [AutoCreate] private readonly CommandProperty SaveClick;
    [AutoCreate] private readonly CommandProperty CloseClick;
    [AutoCreate] private readonly DataProperty<string> ActiveSkinId;

    public UiFlappySkinSelectViewModel(Session session)
    {
        _session = session;

        SaveClick.SetAction(OnSaveClick);
        CloseClick.SetAction(OnCloseClick);

        if (session.GameConfig.Data.TryGetNodeByPath<IDataSource>("flappySkins", out var flappySkinsConfig))
        {
            flappySkinsConfig.ForEach<IDataSource>(skin =>
            {
                var key = skin.GetDataValueOrDefault<string>("key");
                var label = skin.GetDataValueOrDefault<string>("label");

                Skins.AddNode(new SkinItemViewModel(skin.Name, key, label, OnSkinClick));
            });
        }

        ActiveSkinId.SetValue(_session.Managers.Progress.SkinId);

        UpdateActiveSkin();
    }

    private void OnSaveClick(IDataSource _)
    {
        _session.Managers.Progress.SkinId = ActiveSkinId.GetValue<string>();
        _session.Managers.Progress.Save();

        OnCloseClick(_);
    }

    private void OnCloseClick(IDataSource _)
    {
        _session.Managers.Ui.Close(this);
    }

    private void OnSkinClick(SkinItemViewModel skin)
    {
        ActiveSkinId.SetValue(skin.Id.GetValue<string>());

        UpdateActiveSkin();
    }

    private void UpdateActiveSkin()
    {
        var activeSkinId = ActiveSkinId.GetValue<string>();

        Skins.ForEach<SkinItemViewModel>(s => s.Selected.SetValue(s.Id.GetValue<string>() == activeSkinId));
    }

    private class SkinItemViewModel : UiViewModel
    {
        private readonly Action<SkinItemViewModel> _onClick;

        [AutoCreate] public readonly DataProperty<string> Id;
        [AutoCreate] public readonly DataProperty<string> Label;
        [AutoCreate] public readonly DataProperty<bool> Selected;
        [AutoCreate] private readonly CommandProperty Click;

        public SkinItemViewModel(string name, string id, string label, Action<SkinItemViewModel> onClick) : base(name)
        {
            _onClick = onClick;

            Id.SetValue(id);
            Label.SetValue(label);

            Click.SetAction(OnClick);
        }

        private void OnClick(IDataSource _)
        {
            _onClick?.Invoke(this);
        }
    }
}
