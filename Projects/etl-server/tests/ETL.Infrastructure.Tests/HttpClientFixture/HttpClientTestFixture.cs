using System.Net;
namespace ETL.Infrastructure.Tests.HttpClientFixture;
public class HttpClientTestFixture
{
    public FakeHttpMessageHandler Handler { get; } = new FakeHttpMessageHandler();
    public HttpClient Client { get; }

    public HttpClientTestFixture()
    {
        Client = new HttpClient(Handler);
    }

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private HttpStatusCode _status = HttpStatusCode.OK;
        private string _body = "{}"; // default empty JSON
        public HttpRequestMessage? LastRequest { get; private set; }
        public string? LastRequestContent { get; private set; }

        public void SetupResponse(HttpStatusCode status, string body = "{}")
        {
            _status = status;
            _body = body;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;

            if (request.Content != null)
            {
                LastRequestContent = await request.Content.ReadAsStringAsync(cancellationToken);
            }
            else
            {
                LastRequestContent = null;
            }

            return new HttpResponseMessage(_status)
            {
                Content = new StringContent(_body)
            };

        }

    }
}