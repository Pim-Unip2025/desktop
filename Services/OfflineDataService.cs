using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TronusDesktop.Models;

namespace TronusDesktop.Services
{
    /// <summary>
    /// Serviço de simulação e armazenamento local para o Modo Offline do TronusDesktop.
    /// Permite a execução 100% funcional do ecossistema gamificado sem necessidade de
    /// conexão com a internet ou dependência da API na nuvem durante apresentações da banca.
    /// </summary>
    public static class OfflineDataService
    {
        public static UsuarioDto CurrentUser { get; set; }
        public static PersonagemDto CurrentHero { get; set; }

        private static List<ReinoDto> _reinos = new();
        private static List<ProgressoFaseDto> _progresso = new();
        private static Dictionary<int, List<QuestaoDto>> _questoesPorFase = new();
        private static int _totalRespostasOffline = 30;
        private static int _totalAcertosOffline = 26;

        static OfflineDataService()
        {
            ResetToDefaults();
        }

        /// <summary>
        /// Inicializa o banco de dados em memória com heróis, fases e questões reais de computação.
        /// </summary>
        public static void ResetToDefaults()
        {
            CurrentUser = new UsuarioDto
            {
                Id = 101,
                Nome = "Arthur Pendragon",
                Email = "arthur.demo@tronus.unip.br",
                Papel = "aluno"
            };

            CurrentHero = new PersonagemDto
            {
                Id = 1,
                Nome = "Sir Arthur",
                Genero = "M",
                Avatar = @"img\masc1.png",
                TituloAtual = "Duque",
                TotalEstrelas = 38
            };

            _reinos = new List<ReinoDto>
            {
                new() { Id = 1, Nome = "Python", Descricao = "Lógica de programação, variáveis primitivas e operadores lógicos." },
                new() { Id = 2, Nome = "C++", Descricao = "Estruturas de decisão condicional, laços de repetição e vetores." },
                new() { Id = 3, Nome = "Banco de Dados", Descricao = "Modelagem relacional de banco de dados e comandos fundamentais SQL." },
                new() { Id = 4, Nome = "Redes", Descricao = "Arquitetura TCP/IP, portas de comunicação, protocolos e topologias." },
                new() { Id = 5, Nome = "Engenharia de Software", Descricao = "Engenharia de software, elicitação de requisitos e modelagem UML." }
            };

            _progresso = new List<ProgressoFaseDto>
            {
                // Reino 1: Python (Concluído com 8 estrelas)
                new() { ReinoId = 1, ReinoNome = "Python", FaseId = 1, FaseNome = "Conceitos e Variáveis", Ordem = 1, Estrelas = 3, Jogada = true },
                new() { ReinoId = 1, ReinoNome = "Python", FaseId = 2, FaseNome = "Operadores Aritméticos", Ordem = 2, Estrelas = 3, Jogada = true },
                new() { ReinoId = 1, ReinoNome = "Python", FaseId = 3, FaseNome = "Tabela Verdade e Proposições", Ordem = 3, Estrelas = 2, Jogada = true },

                // Reino 2: C++ (Parcialmente conquistado - 5 estrelas)
                new() { ReinoId = 2, ReinoNome = "C++", FaseId = 4, FaseNome = "Estruturas Condicionais (if/else)", Ordem = 1, Estrelas = 3, Jogada = true },
                new() { ReinoId = 2, ReinoNome = "C++", FaseId = 5, FaseNome = "Laços de Repetição (for/while)", Ordem = 2, Estrelas = 2, Jogada = true },
                new() { ReinoId = 2, ReinoNome = "C++", FaseId = 6, FaseNome = "Vetores e Matrizes", Ordem = 3, Estrelas = 0, Jogada = false },

                // Reino 3: Banco de Dados (Iniciado - 2 estrelas)
                new() { ReinoId = 3, ReinoNome = "Banco de Dados", FaseId = 7, FaseNome = "Entidades e Relacionamentos", Ordem = 1, Estrelas = 2, Jogada = true },
                new() { ReinoId = 3, ReinoNome = "Banco de Dados", FaseId = 8, FaseNome = "Normalização e Chaves", Ordem = 2, Estrelas = 0, Jogada = false },
                new() { ReinoId = 3, ReinoNome = "Banco de Dados", FaseId = 9, FaseNome = "Consultas SQL DML", Ordem = 3, Estrelas = 0, Jogada = false },

                // Reino 4: Redes (Bloqueado)
                new() { ReinoId = 4, ReinoNome = "Redes", FaseId = 10, FaseNome = "Camadas OSI e TCP/IP", Ordem = 1, Estrelas = 0, Jogada = false },
                new() { ReinoId = 4, ReinoNome = "Redes", FaseId = 11, FaseNome = "Protocolos e Portas", Ordem = 2, Estrelas = 0, Jogada = false },
                new() { ReinoId = 4, ReinoNome = "Redes", FaseId = 12, FaseNome = "Endereçamento IP e Máscaras", Ordem = 3, Estrelas = 0, Jogada = false },

                // Reino 5: Engenharia de Software (Bloqueado)
                new() { ReinoId = 5, ReinoNome = "Engenharia de Software", FaseId = 13, FaseNome = "Requisitos Funcionais e Não-Funcionais", Ordem = 1, Estrelas = 0, Jogada = false },
                new() { ReinoId = 5, ReinoNome = "Engenharia de Software", FaseId = 14, FaseNome = "Modelagem UML (Casos de Uso e Classes)", Ordem = 2, Estrelas = 0, Jogada = false },
                new() { ReinoId = 5, ReinoNome = "Engenharia de Software", FaseId = 15, FaseNome = "Metodologias Ágeis e Scrum", Ordem = 3, Estrelas = 0, Jogada = false }
            };

            CriarQuestoesPedagogicas();
        }

        private static void CriarQuestoesPedagogicas()
        {
            _questoesPorFase.Clear();

            // ═══════════════════════════════════════════════════
            // FASE 1: Reino da Lógica - Conceitos e Variáveis
            // ═══════════════════════════════════════════════════
            _questoesPorFase[1] = new List<QuestaoDto>
            {
                new()
                {
                    Id = 101, FaseId = 1, NivelDificuldade = "Fácil",
                    Enunciado = "Em ciência da computação, o que é uma variável?",
                    Alternativas = new List<AlternativaDto>
                    {
                        new() { Id = 1, Letra = "A", Texto = "Um componente físico permanente do disco rígido.", Correta = false },
                        new() { Id = 2, Letra = "B", Texto = "Um espaço nomeado na memória principal (RAM) para armazenar dados dinâmicos.", Correta = true },
                        new() { Id = 3, Letra = "C", Texto = "Um comando exclusivo de compilação sem efeito em tempo de execução.", Correta = false },
                        new() { Id = 4, Letra = "D", Texto = "Um algoritmo completo de inteligência artificial.", Correta = false }
                    }
                },
                new()
                {
                    Id = 102, FaseId = 1, NivelDificuldade = "Fácil",
                    Enunciado = "Qual dos seguintes tipos primitivos é indicado para armazenar valores numéricos inteiros sem casas decimais?",
                    Alternativas = new List<AlternativaDto>
                    {
                        new() { Id = 5, Letra = "A", Texto = "float / double", Correta = false },
                        new() { Id = 6, Letra = "B", Texto = "boolean / bool", Correta = false },
                        new() { Id = 7, Letra = "C", Texto = "int / integer", Correta = true },
                        new() { Id = 8, Letra = "D", Texto = "string / char", Correta = false }
                    }
                },
                new()
                {
                    Id = 103, FaseId = 1, NivelDificuldade = "Médio",
                    Enunciado = "Se uma variável booleana for inicializada com 'true', qual valor ela conterá após a aplicação do operador de negação NOT (!)?",
                    Alternativas = new List<AlternativaDto>
                    {
                        new() { Id = 9, Letra = "A", Texto = "false", Correta = true },
                        new() { Id = 10, Letra = "B", Texto = "null", Correta = false },
                        new() { Id = 11, Letra = "C", Texto = "0", Correta = false },
                        new() { Id = 12, Letra = "D", Texto = "true", Correta = false }
                    }
                },
                new()
                {
                    Id = 104, FaseId = 1, NivelDificuldade = "Médio",
                    Enunciado = "Qual é a tipagem atribuída a literais textuais como \"TRONUS UNIP 2026\"?",
                    Alternativas = new List<AlternativaDto>
                    {
                        new() { Id = 13, Letra = "A", Texto = "String (Cadeia de Caracteres)", Correta = true },
                        new() { Id = 14, Letra = "B", Texto = "Decimal com ponto flutuante", Correta = false },
                        new() { Id = 15, Letra = "C", Texto = "Ponteiro binário não seguro", Correta = false },
                        new() { Id = 16, Letra = "D", Texto = "Vetor de inteiros estático", Correta = false }
                    }
                },
                new()
                {
                    Id = 105, FaseId = 1, NivelDificuldade = "Difícil",
                    Enunciado = "O que caracteriza uma constante ('const') em relação a uma variável comum?",
                    Alternativas = new List<AlternativaDto>
                    {
                        new() { Id = 17, Letra = "A", Texto = "Seu valor pode ser alterado a qualquer momento em tempo de execução.", Correta = false },
                        new() { Id = 18, Letra = "B", Texto = "Seu valor é imutável após ser atribuído, prevenindo modificações acidentais.", Correta = true },
                        new() { Id = 19, Letra = "C", Texto = "Ela não ocupa memória física.", Correta = false },
                        new() { Id = 20, Letra = "D", Texto = "Pode armazenar simultaneamente múltiplos tipos de dados.", Correta = false }
                    }
                }
            };

            // ═══════════════════════════════════════════════════
            // FASE 4: Reino dos Algoritmos - Estruturas Condicionais
            // ═══════════════════════════════════════════════════
            _questoesPorFase[4] = new List<QuestaoDto>
            {
                new()
                {
                    Id = 401, FaseId = 4, NivelDificuldade = "Fácil",
                    Enunciado = "Qual instrução condicional é utilizada para desviar o fluxo quando uma condição é verdadeira?",
                    Alternativas = new List<AlternativaDto>
                    {
                        new() { Id = 41, Letra = "A", Texto = "for", Correta = false },
                        new() { Id = 42, Letra = "B", Texto = "if", Correta = true },
                        new() { Id = 43, Letra = "C", Texto = "while", Correta = false },
                        new() { Id = 44, Letra = "D", Texto = "return", Correta = false }
                    }
                },
                new()
                {
                    Id = 402, FaseId = 4, NivelDificuldade = "Médio",
                    Enunciado = "Em um bloco 'if (x > 10) ... else ...', quando a cláusula 'else' será executada?",
                    Alternativas = new List<AlternativaDto>
                    {
                        new() { Id = 45, Letra = "A", Texto = "Apenas se 'x' for igual a zero.", Correta = false },
                        new() { Id = 46, Letra = "B", Texto = "Quando a condição 'x > 10' for avaliada como falsa (ou seja, x <= 10).", Correta = true },
                        new() { Id = 47, Letra = "C", Texto = "Sempre, independente do valor de x.", Correta = false },
                        new() { Id = 48, Letra = "D", Texto = "Nunca, o bloco else é opcional e ignorado pelo compilador.", Correta = false }
                    }
                },
                new()
                {
                    Id = 403, FaseId = 4, NivelDificuldade = "Médio",
                    Enunciado = "Qual estrutura é mais adequada para avaliar uma variável contra múltiplos valores constantes pré-definidos?",
                    Alternativas = new List<AlternativaDto>
                    {
                        new() { Id = 49, Letra = "A", Texto = "switch / case", Correta = true },
                        new() { Id = 50, Letra = "B", Texto = "while infinito", Correta = false },
                        new() { Id = 51, Letra = "C", Texto = "try / catch", Correta = false },
                        new() { Id = 52, Letra = "D", Texto = "goto descontrolado", Correta = false }
                    }
                },
                new()
                {
                    Id = 404, FaseId = 4, NivelDificuldade = "Difícil",
                    Enunciado = "Em linguagens C#, Java e C++, qual operador ternário simplifica um 'if-else' em uma única linha?",
                    Alternativas = new List<AlternativaDto>
                    {
                        new() { Id = 53, Letra = "A", Texto = "condicao ? valorSeVerdadeiro : valorSeFalso", Correta = true },
                        new() { Id = 54, Letra = "B", Texto = "condicao :: valorSeVerdadeiro ?? valorSeFalso", Correta = false },
                        new() { Id = 55, Letra = "C", Texto = "condicao -> verdadeiro | falso", Correta = false },
                        new() { Id = 56, Letra = "D", Texto = "check(condicao, sim, nao)", Correta = false }
                    }
                },
                new()
                {
                    Id = 405, FaseId = 4, NivelDificuldade = "Difícil",
                    Enunciado = "O que ocorre se a instrução 'break' for omitida dentro de um bloco 'case' em estruturas 'switch'?",
                    Alternativas = new List<AlternativaDto>
                    {
                        new() { Id = 57, Letra = "A", Texto = "Ocorre 'fall-through', continuando a execução para os casos seguintes (ou erro de compilação em C#).", Correta = true },
                        new() { Id = 58, Letra = "B", Texto = "O computador desliga imediatamente.", Correta = false },
                        new() { Id = 59, Letra = "C", Texto = "O valor da variável é automaticamente zerado.", Correta = false },
                        new() { Id = 60, Letra = "D", Texto = "O laço se repete 100 vezes.", Correta = false }
                    }
                }
            };

            // Configura questões padrão para as demais fases (fallback dinâmico)
            for (int f = 1; f <= 15; f++)
            {
                if (!_questoesPorFase.ContainsKey(f))
                {
                    _questoesPorFase[f] = GerarQuestoesGenericasFase(f);
                }
            }
        }

        private static List<QuestaoDto> GerarQuestoesGenericasFase(int faseId)
        {
            string tema = faseId switch
            {
                <= 3 => "Lógica e Expressões",
                <= 6 => "Algoritmos e Controle de Fluxo",
                <= 9 => "Banco de Dados e Modelagem SQL",
                <= 12 => "Redes de Computadores e Protocolos",
                _ => "Engenharia de Software e Metodologias Ágeis"
            };

            return new List<QuestaoDto>
            {
                new()
                {
                    Id = faseId * 100 + 1, FaseId = faseId, NivelDificuldade = "Fácil",
                    Enunciado = $"[Desafio de {tema}] Qual é a boa prática recomendada para desenvolvimento de software na fase {faseId}?",
                    Alternativas = new List<AlternativaDto>
                    {
                        new() { Id = 1, Letra = "A", Texto = "Modularização, clareza no código e documentação técnica consistente.", Correta = true },
                        new() { Id = 2, Letra = "B", Texto = "Ignorar tratamento de exceções e testes.", Correta = false },
                        new() { Id = 3, Letra = "C", Texto = "Armazenar credenciais de banco no código fonte aberto.", Correta = false },
                        new() { Id = 4, Letra = "D", Texto = "Não seguir nenhuma convenção de nomenclatura.", Correta = false }
                    }
                },
                new()
                {
                    Id = faseId * 100 + 2, FaseId = faseId, NivelDificuldade = "Médio",
                    Enunciado = $"Qual princípio fundamental de engenharia da computação garante maior robustez nesta etapa?",
                    Alternativas = new List<AlternativaDto>
                    {
                        new() { Id = 5, Letra = "A", Texto = "Baixo acoplamento e alta coesão entre componentes.", Correta = true },
                        new() { Id = 6, Letra = "B", Texto = "Dependência mútua circular entre módulos.", Correta = false },
                        new() { Id = 7, Letra = "C", Texto = "Classes com milhares de linhas e múltiplas responsabilidades desnecessárias.", Correta = false },
                        new() { Id = 8, Letra = "D", Texto = "Ausência total de versionamento via Git.", Correta = false }
                    }
                },
                new()
                {
                    Id = faseId * 100 + 3, FaseId = faseId, NivelDificuldade = "Médio",
                    Enunciado = "Qual o impacto de utilizar a arquitetura em camadas (Thin Client / API / Banco)?",
                    Alternativas = new List<AlternativaDto>
                    {
                        new() { Id = 9, Letra = "A", Texto = "Segurança das credenciais, separação de responsabilidades e escalabilidade.", Correta = true },
                        new() { Id = 10, Letra = "B", Texto = "Lentidão irreparável e impossibilidade de manutenção.", Correta = false },
                        new() { Id = 11, Letra = "C", Texto = "Obrigatoriedade de usar um único computador para toda a aplicação.", Correta = false },
                        new() { Id = 12, Letra = "D", Texto = "Incompatibilidade com o protocolo HTTPS.", Correta = false }
                    }
                },
                new()
                {
                    Id = faseId * 100 + 4, FaseId = faseId, NivelDificuldade = "Difícil",
                    Enunciado = "Em caso de falha de conexão na camada de rede, qual padrão de projeto auxilia na resiliência da aplicação?",
                    Alternativas = new List<AlternativaDto>
                    {
                        new() { Id = 13, Letra = "A", Texto = "Fallback gracioso com Cache Local / Modo Offline com dados em memória.", Correta = true },
                        new() { Id = 14, Letra = "B", Texto = "Fechamento imediato do aplicativo com tela azul.", Correta = false },
                        new() { Id = 15, Letra = "C", Texto = "Laço infinito de requisições travando a CPU do usuário.", Correta = false },
                        new() { Id = 16, Letra = "D", Texto = "Deleção automática dos dados do disco.", Correta = false }
                    }
                },
                new()
                {
                    Id = faseId * 100 + 5, FaseId = faseId, NivelDificuldade = "Difícil",
                    Enunciado = "Como a gamificação do Tronus contribui para o aprendizado acadêmico?",
                    Alternativas = new List<AlternativaDto>
                    {
                        new() { Id = 17, Letra = "A", Texto = "Engajamento através de missões, feedback instantâneo de estrelas e evolução de títulos.", Correta = true },
                        new() { Id = 18, Letra = "B", Texto = "Substituindo completamente as aulas teóricas por sorteios aleatórios.", Correta = false },
                        new() { Id = 19, Letra = "C", Texto = "Punindo alunos com cancelamento de matrícula.", Correta = false },
                        new() { Id = 20, Letra = "D", Texto = "Tornando as matérias acadêmicas desnecessárias.", Correta = false }
                    }
                }
            };
        }

        #region Tratamento de Requisições HTTP Simuladas

        public static async Task<T> HandleGetAsync<T>(string endpoint)
        {
            await Task.CompletedTask;

            if (endpoint == "/personagens/me")
            {
                if (CurrentHero == null)
                {
                    throw new ApiException(404, new ApiErrorResponse { StatusCode = 404, Error = "Not Found", Message = "Personagem não encontrado." });
                }
                return (T)(object)CurrentHero;
            }

            if (endpoint == "/reinos")
            {
                return (T)(object)_reinos;
            }

            if (endpoint == "/personagens/me/progresso")
            {
                return (T)(object)_progresso;
            }

            if (endpoint.StartsWith("/fases/") && endpoint.EndsWith("/questoes"))
            {
                string[] parts = endpoint.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && int.TryParse(parts[1], out int faseId))
                {
                    if (_questoesPorFase.TryGetValue(faseId, out var questoes))
                    {
                        return (T)(object)questoes;
                    }
                    return (T)(object)GerarQuestoesGenericasFase(faseId);
                }
            }

            if (endpoint == "/personagens/me/resumo")
            {
                int fasesCompletas = _progresso.Count(p => p.Estrelas == 3);
                int fasesIniciadas = _progresso.Count(p => p.Jogada);
                decimal taxa = _totalRespostasOffline > 0 
                    ? Math.Round((decimal)_totalAcertosOffline / _totalRespostasOffline * 100m, 1) 
                    : 86.6m;

                var resumo = new ResumoDashboardDto
                {
                    TotalRespostas = _totalRespostasOffline,
                    TotalAcertos = _totalAcertosOffline,
                    TaxaGeral = taxa,
                    TotalEstrelas = CurrentHero?.TotalEstrelas ?? 38,
                    FasesIniciadas = fasesIniciadas,
                    FasesCompletas = fasesCompletas
                };
                return (T)(object)resumo;
            }

            return default;
        }

        public static async Task<TResponse> HandlePostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            await Task.CompletedTask;

            // 1. Login
            if (endpoint == "/auth/login" && data is LoginRequest loginReq)
            {
                string id = loginReq.Identificador?.ToLower() ?? "";
                string papel = "aluno";
                string nome = "Arthur Pendragon";

                if (id.Contains("prof"))
                {
                    papel = "professor";
                    nome = "Prof. Albus";
                }
                else if (id.Contains("adm"))
                {
                    papel = "admin";
                    nome = "Admin Morgana";
                }
                else if (!string.IsNullOrWhiteSpace(loginReq.Identificador))
                {
                    nome = loginReq.Identificador;
                }

                CurrentUser = new UsuarioDto
                {
                    Id = 101,
                    Nome = nome,
                    Email = $"{id.Replace(" ", ".")}@tronus.unip.br",
                    Papel = papel
                };

                var authRes = new AuthResponse
                {
                    AccessToken = "offline_demo_jwt_token_local",
                    TokenType = "Bearer",
                    ExpiresIn = 86400,
                    Usuario = CurrentUser
                };
                return (TResponse)(object)authRes;
            }

            // 2. Registro
            if (endpoint == "/auth/register" && data is CadastroRequest regReq)
            {
                CurrentUser = new UsuarioDto
                {
                    Id = 202,
                    Nome = regReq.Nome,
                    Email = regReq.Email,
                    Papel = "aluno"
                };

                CurrentHero = null;

                var authRes = new AuthResponse
                {
                    AccessToken = "offline_demo_jwt_new_user",
                    TokenType = "Bearer",
                    ExpiresIn = 86400,
                    Usuario = CurrentUser
                };
                return (TResponse)(object)authRes;
            }

            // 3. Criação de Personagem
            if (endpoint == "/personagens" && data is CriarPersonagemRequest charReq)
            {
                CurrentHero = new PersonagemDto
                {
                    Id = 10,
                    Nome = charReq.Nome,
                    Genero = charReq.Genero,
                    Avatar = charReq.Avatar,
                    TituloAtual = "Plebeu",
                    TotalEstrelas = 0
                };
                return (TResponse)(object)CurrentHero;
            }

            // 4. Submissão de Quiz / Responder
            if (endpoint.StartsWith("/fases/") && endpoint.EndsWith("/responder") && data is EnviarRespostasRequest envioReq)
            {
                string[] parts = endpoint.Split('/', StringSplitOptions.RemoveEmptyEntries);
                int faseId = 1;
                if (parts.Length >= 2 && int.TryParse(parts[1], out int fId)) faseId = fId;

                var questoes = _questoesPorFase.TryGetValue(faseId, out var qList) ? qList : GerarQuestoesGenericasFase(faseId);
                var correcoes = new List<ItemCorrecaoDto>();
                int acertos = 0;

                foreach (var resp in envioReq.Respostas)
                {
                    var questao = questoes.FirstOrDefault(q => q.Id == resp.QuestaoId);
                    if (questao != null)
                    {
                        var altCorreta = questao.Alternativas.FirstOrDefault(a => a.Correta);
                        bool acertou = altCorreta != null && altCorreta.Id == resp.AlternativaId;
                        if (acertou) acertos++;

                        correcoes.Add(new ItemCorrecaoDto
                        {
                            QuestaoId = resp.QuestaoId,
                            AlternativaEscolhidaId = resp.AlternativaId,
                            AlternativaCorretaId = altCorreta?.Id ?? 0,
                            Acertou = acertou
                        });
                    }
                }

                int estrelasGanhas = acertos switch
                {
                    5 => 3,
                    >= 3 => 2,
                    >= 1 => 1,
                    _ => 0
                };

                var faseProg = _progresso.FirstOrDefault(p => p.FaseId == faseId);
                int melhorEstrelas = estrelasGanhas;
                if (faseProg != null)
                {
                    faseProg.Jogada = true;
                    if (estrelasGanhas > faseProg.Estrelas)
                    {
                        faseProg.Estrelas = estrelasGanhas;
                    }
                    melhorEstrelas = faseProg.Estrelas;
                }

                if (CurrentHero != null)
                {
                    CurrentHero.TotalEstrelas = _progresso.Sum(p => p.Estrelas);
                    CurrentHero.TituloAtual = CurrentHero.TotalEstrelas switch
                    {
                        >= 45 => "Rei",
                        >= 30 => "Duque",
                        >= 15 => "Cavaleiro",
                        _ => "Plebeu"
                    };
                }

                _totalRespostasOffline += envioReq.Respostas.Count;
                _totalAcertosOffline += acertos;

                var resultado = new ResultadoQuizResponse
                {
                    FaseId = faseId,
                    Acertos = acertos,
                    Total = questoes.Count,
                    Estrelas = estrelasGanhas,
                    MelhorEstrelas = melhorEstrelas,
                    Personagem = CurrentHero,
                    Correcao = correcoes
                };

                return (TResponse)(object)resultado;
            }

            return default;
        }

        public static Task HandlePostAsync<TRequest>(string endpoint, TRequest data)
        {
            return Task.CompletedTask;
        }

        public static Task HandleDeleteAsync(string endpoint)
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}
