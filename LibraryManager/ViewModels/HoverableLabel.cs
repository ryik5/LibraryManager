namespace LibraryManager.ViewModels;

public class HoverableLabel : Label
{
    // Pointer events for hover functionality
    public event EventHandler PointerEntered;
    public event EventHandler PointerExited;

    // Platform handlers will invoke these protected methods
    protected internal void OnPointerEntered()
    {
        PointerEntered?.Invoke(this, EventArgs.Empty);
    }

    protected internal void OnPointerExited()
    {
        PointerExited?.Invoke(this, EventArgs.Empty);
    }
}
