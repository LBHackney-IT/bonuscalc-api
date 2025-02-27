using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Npgsql;
using NUnit.Framework;

namespace BonusCalcApi.Tests
{
    public abstract class IntegrationTests<TStartup> where TStartup : class
    {
        protected HttpClient Client { get; private set; }
        protected BonusCalcContext Context => _factory.Context;

        private MockWebApplicationFactory<TStartup> _factory;
        private IDbContextTransaction _transaction;
        private DbContextOptionsBuilder<BonusCalcContext> _builder;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _builder = new DbContextOptionsBuilder<BonusCalcContext>();

            var dataSource = DatabaseContextConfiguration.BuildDataSource(ConnectionString.TestDatabase());

            _builder
                .UseNpgsql(dataSource)
                .UseSnakeCaseNamingConvention();
        }

        [SetUp]
        public void BaseSetup()
        {
            _factory = new MockWebApplicationFactory<TStartup>(_builder);
            Client = _factory.CreateClient();
            _factory.Context.Database.Migrate();
            _transaction = _factory.Context.Database.BeginTransaction();

            // Empty trades table for tests
            _factory.Context.Trades.RemoveRange(_factory.Context.Trades);
        }

        [TearDown]
        public void BaseTearDown()
        {
            Client.Dispose();
            _factory.Dispose();
            _transaction.Rollback();
            _transaction.Dispose();
        }

        public async Task<(HttpStatusCode statusCode, TResponse response)> Get<TResponse>(string address)
        {
            var result = await InternalGet(address);

            var response = await ProcessResponse<TResponse>(result);

            return (result.StatusCode, response);
        }

        public async Task<HttpStatusCode> Get(string address)
        {
            var result = await InternalGet(address);

            return result.StatusCode;
        }

        public async Task<(HttpStatusCode statusCode, TResponse response)> Post<TResponse>(string address, object data)
        {
            HttpResponseMessage result = await InternalPost(address, data);

            TResponse response = await ProcessResponse<TResponse>(result);
            return (result.StatusCode, response);
        }

        public async Task<HttpStatusCode> Post(string address, object data)
        {
            HttpResponseMessage result = await InternalPost(address, data);
            return result.StatusCode;
        }

        private async Task<HttpResponseMessage> InternalGet(string uri)
        {
            var result = await Client.GetAsync(new Uri(uri, UriKind.Relative));
            return result;
        }

        private async Task<HttpResponseMessage> InternalPost(string uri, object data)
        {
            var serializedContent = JsonConvert.SerializeObject(data);
            var content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var result = await Client.PostAsync(new Uri(uri, UriKind.Relative), content);
            content.Dispose();
            return result;
        }

        private static async Task<TResponse> ProcessResponse<TResponse>(HttpResponseMessage result)
        {
            var responseContent = await result.Content.ReadAsStringAsync();

            try
            {
                var parseResponse = JsonConvert.DeserializeObject(responseContent, typeof(TResponse));
                var castedResponse = parseResponse is TResponse response ? response : default;
                return castedResponse;
            }
            catch (Exception e) when (e is JsonSerializationException || e is JsonReaderException)
            {
                throw new Exception($"Result Serialisation Failed. Response Had Code {result.StatusCode}, Response: {responseContent}", e);
            }
        }
    }
}
