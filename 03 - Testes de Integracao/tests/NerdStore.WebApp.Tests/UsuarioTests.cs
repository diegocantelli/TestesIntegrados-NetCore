using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Features.Tests;
using NerdStore.WebApp.MVC;
using NerdStore.WebApp.Tests.Config;
using Xunit;

namespace NerdStore.WebApp.Tests
{
    [TestCaseOrderer("Features.Tests.PriorityOrderer", "Features.Tests")]

    //Necessário usar esta collection para poder ser injetada as dependências corretamente
    [Collection(nameof(IntegrationWebTestsFixtureCollection))]
    public class UsuarioTests
    {
        private readonly IntegrationTestsFixture<StartupWebTests> _testsFixture;

        //criando o construtor e configs básicas para o teste relacionado ao usuário na app mvc
        public UsuarioTests(IntegrationTestsFixture<StartupWebTests> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Realizar cadastro com sucesso"), TestPriority(1)]
        [Trait("Categoria", "Integração Web - Usuário")]
        public async Task Usuario_RealizarCadastro_DeveExecutarComSucesso()
        {
            // Arrange
            //GetAsync -> Faz uma chamada assíncrona para a URL passada no parâmetro
            var initialResponse = await _testsFixture.Client.GetAsync("/Identity/Account/Register");

            //EnsureSuccessStatusCode -> Garante que o retorno da requisição seja 200, caso contrário lança uma exceção
            initialResponse.EnsureSuccessStatusCode();

            //ObterAntiForgeryToken -> método que retorna o token que será necessário ao enviar um post e retornar 200
            var antiForgeryToken = _testsFixture.ObterAntiForgeryToken(await initialResponse.Content.ReadAsStringAsync());

            //Metodo que gera usuário e senha randomicamente, para não dar problema de usuário já cadastrado
            // ao efetuar os testes
            _testsFixture.GerarUserSenha();

            //Simulando os dados do formulário de cadastro através de uma estrutura chave valor
            var formData = new Dictionary<string, string>
            {
                //passando o token no dicionário
                { _testsFixture.AntiForgeryFieldName, antiForgeryToken },

                {"Input.Email", _testsFixture.UsuarioEmail },
                {"Input.Password", _testsFixture.UsuarioSenha },
                {"Input.ConfirmPassword", _testsFixture.UsuarioSenha }
            };

            //Configurando o post para a URL
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Register")
            {
                //passando os dados do formulário no post
                Content = new FormUrlEncodedContent(formData)
            };

            // Act
            //Enviando o post através do método SendAsync
            var postResponse = await _testsFixture.Client.SendAsync(postRequest);

            // Assert
            //postResponse.Content.ReadAsStringAsync() -> retornando o conteúdo do response em formato string
            var responseString = await postResponse.Content.ReadAsStringAsync();

            //garantindo que o response tenha retornado com sucesso - 200
            postResponse.EnsureSuccessStatusCode();

            //Validando se na string retornada no post há o email informado pelo usuário
            Assert.Contains($"Hello {_testsFixture.UsuarioEmail}!", responseString);
        }

        [Fact(DisplayName = "Realizar cadastro senha fraca"), TestPriority(3)]
        [Trait("Categoria", "Integração Web - Usuário")]
        public async Task Usuario_RealizarCadastroComSenhaFraca_DeveRetornarMensagemDeErro()
        {
            // Arrange
            var initialResponse = await _testsFixture.Client.GetAsync("/Identity/Account/Register");
            initialResponse.EnsureSuccessStatusCode();

            var antiForgeryToken = _testsFixture.ObterAntiForgeryToken(await initialResponse.Content.ReadAsStringAsync());

            _testsFixture.GerarUserSenha();
            const string senhaFraca = "123456";

            var formData = new Dictionary<string, string>
            {
                { _testsFixture.AntiForgeryFieldName, antiForgeryToken },
                {"Input.Email", _testsFixture.UsuarioEmail },
                {"Input.Password", senhaFraca },
                {"Input.ConfirmPassword", senhaFraca }
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Register")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            // Act
            var postResponse = await _testsFixture.Client.SendAsync(postRequest);

            // Assert
            var responseString = await postResponse.Content.ReadAsStringAsync();

            postResponse.EnsureSuccessStatusCode();
            Assert.Contains("Passwords must have at least one non alphanumeric character.", responseString);
            Assert.Contains("Passwords must have at least one lowercase (&#x27;a&#x27;-&#x27;z&#x27;).", responseString);
            Assert.Contains("Passwords must have at least one uppercase (&#x27;A&#x27;-&#x27;Z&#x27;).", responseString);
        }

        [Fact(DisplayName = "Realizar login com sucesso"), TestPriority(2)]
        [Trait("Categoria", "Integração Web - Usuário")]
        public async Task Usuario_RealizarLogin_DeveExecutarComSucesso()
        {
            // Arrange
            var initialResponse = await _testsFixture.Client.GetAsync("/Identity/Account/Login");
            initialResponse.EnsureSuccessStatusCode();

            var antiForgeryToken = _testsFixture.ObterAntiForgeryToken(await initialResponse.Content.ReadAsStringAsync());

            var formData = new Dictionary<string, string>
            {
                {_testsFixture.AntiForgeryFieldName, antiForgeryToken},
                {"Input.Email", _testsFixture.UsuarioEmail},
                {"Input.Password", _testsFixture.UsuarioSenha}
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Login")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            // Act
            var postResponse = await _testsFixture.Client.SendAsync(postRequest);

            // Assert
            var responseString = await postResponse.Content.ReadAsStringAsync();

            postResponse.EnsureSuccessStatusCode();
            Assert.Contains($"Hello {_testsFixture.UsuarioEmail}!", responseString);
        }
    }
}