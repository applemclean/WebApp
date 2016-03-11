namespace WebApp.Presenters
{
    public interface ICSRFPresenter
    {
        string Code { get; }
        string TokenName { get; }
    }
}