using System;
using System.Reflection;
using DataBinding;

public class UiViewModel : DataSource, IUiViewModel
{
    private bool _disposing;

    public event Action Disposed;

    public UiViewModel() : this((string)null)
    { }

    public UiViewModel(IDataSource dataSource) : this(dataSource?.Name ?? null)
    {
        PatchWith(dataSource);
    }

    public UiViewModel(string name) : base(name)
    {
        foreach (var f in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            var autoCreateAttribute = f.GetCustomAttribute<AutoCreateAttribute>(false);

            if (autoCreateAttribute != null && f.GetValue(this) == null)
            {
                var propertyName = autoCreateAttribute.CustomName ?? f.Name;
                var instance = (IDataNode)Activator.CreateInstance(f.FieldType, propertyName);

                f.SetValue(this, instance);

                AddNode(instance);
            }
        }
    }

    public void Dispose()
    {
        if (_disposing)
        {
            return;
        }

        _disposing = true;

        Disposed?.Invoke();
    }

    protected virtual void OnDispose() {}

    protected class AutoCreateAttribute : System.Attribute
    {
        public readonly string CustomName = null;

        public AutoCreateAttribute() {}
        public AutoCreateAttribute(string customName) => CustomName = customName;
    }
}
