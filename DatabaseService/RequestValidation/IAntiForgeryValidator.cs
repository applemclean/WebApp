using System.ServiceModel.Channels;

namespace DatabaseService.RequestValidation
{
    public interface IAntiForgeryValidator
    {
        bool Validate(HttpRequestMessageProperty request);
    }
}