# 🔄 Diagramas de Sequência (UML) — Sistema Tronus

**Projeto Integrado Multidisciplinar (PIM III) — UNIP 2026**  
**Curso:** Superior de Tecnologia em Análise e Desenvolvimento de Sistemas (ADS)  
**Disciplina de Referência:** Análise e Modelagem de Sistemas / Engenharia de Software Ágil Aplicada  
**Ferramenta CASE Oficial:** Astah UML  
**Arquivos Modelados (`.asta` e `.astah`):**
- `docs/DiagramasDeSequencia.asta` / `docs/DiagramasDeSequencia.astah`
- `docs/Engenharia de software ágil aplicada.asta` (Pacote `04_DiagramasDeSequencia`)

---

## 📌 Sumário

1. [Visão Geral e Fundamentação Teórica](#1-visão-geral-e-fundamentação-teórica)
2. [Arquitetura em Camadas do Tronus (Desktop & Web)](#2-arquitetura-em-camadas-do-tronus-desktop--web)
3. [Dicionário de Linhas de Vida (Lifelines) e Participantes](#3-dicionário-de-linhas-de-vida-lifelines-e-participantes)
4. [Matriz de Correspondência entre Telas e Diagramas](#4-matriz-de-correspondência-entre-telas-e-diagramas)
5. [Diagramas de Sequência Detalhados por Tela (DS01 a DS11)](#5-diagramas-de-sequência-detalhados-por-tela-ds01-a-ds11)
   - [5.1 DS01 — Apresentação e Boas-Vindas (`UcLanding`)](#51-ds01--apresentação-e-boas-vindas-uclanding)
   - [5.2 DS02 — Autenticação e Login (`UcLogin`)](#52-ds02--autenticação-e-login-uclogin)
   - [5.3 DS03 — Cadastro de Estudante (`UcRegister`)](#53-ds03--cadastro-de-estudante-ucregister)
   - [5.4 DS04 — Recuperação de Senha (`UcForgotPassword`)](#54-ds04--recuperação-de-senha-ucforgotpassword)
   - [5.5 DS05 — Forja e Criação do Herói (`UcCharacter`)](#55-ds05--forja-e-criação-do-herói-uccharacter)
   - [5.6 DS06 — Mapa dos Reinos e Seleção de Fases (`UcReinos`)](#56-ds06--mapa-dos-reinos-e-seleção-de-fases-ucreinos)
   - [5.7 DS07 — Desafio e Quiz Gamificado (`UcQuiz`)](#57-ds07--desafio-e-quiz-gamificado-ucquiz)
   - [5.8 DS08 — Painel de Desempenho e Dashboard (`UcDashboard`)](#58-ds08--painel-de-desempenho-e-dashboard-ucdashboard)
   - [5.9 DS09 — Quadro de Honra e Ranking (`UcRanking`)](#59-ds09--quadro-de-honra-e-ranking-ucranking)
   - [5.10 DS10 — Painel Pedagógico do Professor (`UcProfessor`)](#510-ds10--painel-pedagógico-do-professor-ucprofessor)
   - [5.11 DS11 — Painel de Governança do Administrador (`UcAdmin`)](#511-ds11--painel-de-governança-do-administrador-ucadmin)
6. [Matriz de Rastreabilidade (Casos de Uso vs. Telas vs. Diagramas)](#6-matriz-de-rastreabilidade-casos-de-uso-vs-telas-vs-diagramas)

---

## 1. Visão Geral e Fundamentação Teórica

O **Diagrama de Sequência** modela a **dinâmica temporal** do sistema **Tronus**, ilustrando a ordem cronológica em que atores, telas (`UserControls`), serviços (`ApiService`, `SessionManager`), controladores de API e o banco de dados relacional (`PostgreSQL_Tronus`) trocam mensagens síncronas, assíncronas e de retorno.

---

## 2. Arquitetura em Camadas do Tronus (Desktop & Web)

```mermaid
graph LR
    subgraph Apresentacao [Camada de Apresentação]
        Desk[Desktop .NET 10 WinForms<br/>MainForm + UserControls]
        Web[Web Responsive<br/>HTML5 / CSS3 / Vanilla JS]
    end

    subgraph Servicos [Camada de Cliente & Sessão]
        API[ApiService HttpClient<br/>Bearer JWT + Offline Fallback]
        SM[SessionManager<br/>Estado do Usuário Logado]
    end

    subgraph Backend [Camada de Serviços & Regras de Negócio]
        AuthAPI[Auth Controller<br/>BCrypt + JWT Engine]
        GameAPI[Game & Quiz Controller<br/>Cálculo de Estrelas & Patentes]
        AdminAPI[Admin & Docente Controller<br/>Governança RBAC & Turmas]
    end

    subgraph Persistencia [Camada de Dados]
        PostgreSQL[(Banco Relacional<br/>PostgreSQL / Supabase)]
        Triggers[Triggers & Procedures<br/>trg_progresso_atualiza_personagem]
    end

    Desk --> API
    Web --> API
    API --> SM
    API --> AuthAPI
    API --> GameAPI
    API --> AdminAPI
    AuthAPI --> PostgreSQL
    GameAPI --> PostgreSQL
    GameAPI --> Triggers
    AdminAPI --> PostgreSQL
```

---

## 3. Dicionário de Linhas de Vida (Lifelines) e Participantes

| Linha de Vida (Lifeline) | Tipo UML | Descrição e Responsabilidade |
| :--- | :---: | :--- |
| **`Ator (Estudante / Professor / Admin)`** | Ator Humano | Usuário que dispara os eventos por clique, digitação ou seleção em tela. |
| **`UI: UserControl`** | View / Fronteira | Controle visual (`UcLogin`, `UcQuiz`, `UcAdmin`, etc.) responsável por capturar eventos, validar entradas e renderizar dados. |
| **`MF: MainForm`** | Controller de Janela | Formulário principal do C# Desktop que hospeda os `UserControls` dinamicamente no `contentPanel` via `LoadControl(uc)`. |
| **`API: ApiService`** | Client Service | Singleton HTTP que serializa DTOs em JSON, anexa `Authorization: Bearer <token>` e gerencia chamadas ou fallback para `OfflineDataService`. |
| **`SM: SessionManager`** | State Manager | Gerenciador estático em memória que armazena a sessão ativa (`CurrentUser`, `AccessToken`, `CurrentHero`). |
| **`SRV: BackendAPI`** | Controller REST | API que processa requisições REST, aplica hashing BCrypt, valida permissões RBAC e responde em JSON. |
| **`DB: PostgreSQL_Tronus`** | Persistência Relacional | Banco de dados relacional PostgreSQL (Supabase) que executa consultas e transações com integridade referencial. |
| **`TRG: TriggerPatentes`** | Automação no Banco | Gatilho de banco (`trg_progresso_atualiza_personagem`) que recalcula `total_estrelas` e promove a patente nobiliárquica do herói. |

---

## 4. Matriz de Correspondência entre Telas e Diagramas

| # | Nome da Tela | Componente Desktop (C#) | Arquivo Web | Papel de Acesso | Diagrama Astah |
| :-: | :--- | :--- | :--- | :---: | :--- |
| **01** | Apresentação / Boas-Vindas | `UcLanding.cs` | `index.html` | Público / Todos | `DS01 - Tela Landing (UcLanding)` |
| **02** | Autenticação / Login | `UcLogin.cs` | `login.html` | Público | `DS02 - Tela Login (UcLogin)` |
| **03** | Cadastro de Estudante | `UcRegister.cs` | `register.html` | Público | `DS03 - Tela Cadastro (UcRegister)` |
| **04** | Recuperação de Senha (SMS) | `UcForgotPassword.cs` | `forgot_password.html` | Público | `DS04 - Recuperar Senha (UcForgotPassword)` |
| **05** | Forja do Herói (Personagem) | `UcCharacter.cs` | `character.html` | Aluno (Sem Herói) | `DS05 - Forja do Heroi (UcCharacter)` |
| **06** | Mapa dos Reinos e Fases | `UcReinos.cs` | `reinos.html` / `trilha.html` | Aluno | `DS06 - Selecao de Reinos (UcReinos)` |
| **07** | Desafio e Resolução de Quiz | `UcQuiz.cs` | `trilha.html` (Quiz) | Aluno | `DS07 - Desafio e Quiz (UcQuiz)` |
| **08** | Painel do Aluno / Dashboard | `UcDashboard.cs` | `dashboard.html` | Aluno | `DS08 - Dashboard do Aluno (UcDashboard)` |
| **09** | Quadro de Honra / Ranking | `UcRanking.cs` | Ranking Interativo | Aluno / Docente | `DS09 - Ranking Academico (UcRanking)` |
| **10** | Painel Pedagógico do Professor | `UcProfessor.cs` | `professor.html` | Professor | `DS10 - Painel do Professor (UcProfessor)` |
| **11** | Painel de Governança Admin | `UcAdmin.cs` | `admin.html` | Administrador | `DS11 - Painel do Administrador (UcAdmin)` |

---

## 5. Diagramas de Sequência Detalhados por Tela (DS01 a DS11)

---

### 5.1 DS01 — Apresentação e Boas-Vindas (`UcLanding`)

![DS01 - Tela Landing (UcLanding)](IMG/DiagramasDeSequencia/DS01%20-%20Tela%20Landing%20(UcLanding).png)

```mermaid
sequenceDiagram
    autonumber
    actor Aluno as Estudante
    participant UI as UI:UcLanding
    participant SM as SM:SessionManager
    participant MF as MF:MainForm

    Aluno->>UI: abrirAplicacao()
    UI->>SM: verificarSessaoAtiva()
    SM-->>UI: false (sem sessao salva)
    UI->>UI: renderizarHeroBanner(tema, fontes)
    Aluno->>UI: clicarIniciarJornada()
    UI->>MF: LoadControl(new UcLogin())
    MF->>MF: atualizarContentPanel(ucLogin)
    MF-->>Aluno: exibirTelaLogin()
```

---

### 5.2 DS02 — Autenticação e Login (`UcLogin`)

![DS02 - Tela Login (UcLogin)](IMG/DiagramasDeSequencia/DS02%20-%20Tela%20Login%20(UcLogin).png)

```mermaid
sequenceDiagram
    autonumber
    actor Usuario as Usuario
    participant UI as UI:UcLogin
    participant API as API:ApiService
    participant SM as SM:SessionManager
    participant SRV as SRV:BackendAuthAPI
    participant DB as DB:PostgreSQL_Tronus
    participant MF as MF:MainForm

    Usuario->>UI: informarCredenciais(email, senha)
    Usuario->>UI: clicarEntrar()
    UI->>UI: validarCamposNaoVazios()
    UI->>API: PostAsync("/auth/login", req)
    API->>SRV: HTTP_POST_auth_login(email, senha)
    SRV->>DB: SELECT_usuario_por_email(email)
    DB-->>SRV: registroUsuario
    SRV->>SRV: BCrypt_Verify_e_GerarJWT(senha, senhaHash)
    SRV-->>API: HTTP_200_OK(token, usuarioDto)
    API-->>UI: retornarAuthResponse(AuthResponse)
    UI->>SM: Login(authResponse)
    UI->>API: GetAsync("/personagens/me")
    API->>SRV: HTTP_GET_personagens_me(Bearer JWT)
    SRV->>DB: SELECT_personagem_por_usuario(usuarioId)
    DB-->>SRV: dadosPersonagem
    SRV-->>API: HTTP_200_OK(PersonagemDto)
    API-->>UI: retornarPersonagem(PersonagemDto)
    UI->>MF: LoadControl(new UcReinos())
    MF-->>Usuario: exibirMapaDosReinos()
```

---

### 5.3 DS03 — Cadastro de Estudante (`UcRegister`)

![DS03 - Tela Cadastro (UcRegister)](IMG/DiagramasDeSequencia/DS03%20-%20Tela%20Cadastro%20(UcRegister).png)

```mermaid
sequenceDiagram
    autonumber
    actor Calouro as Calouro
    participant UI as UI:UcRegister
    participant API as API:ApiService
    participant SRV as SRV:BackendAuthAPI
    participant DB as DB:PostgreSQL_Tronus
    participant MF as MF:MainForm

    Calouro->>UI: preencherFormulario(nome, email, celular, senha)
    Calouro->>UI: clicarForjarConta()
    UI->>UI: validarFormulario(emailValido, senhasIguais)
    UI->>API: PostAsync("/auth/register", req)
    API->>SRV: HTTP_POST_auth_register(RegisterRequest)
    SRV->>DB: SELECT_verificar_email_unico(email)
    DB-->>SRV: null (disponivel)
    SRV->>SRV: BCrypt_HashPassword(senha, salt=12)
    SRV->>DB: INSERT_INTO_usuarios(nome, email, celular, hash, aluno)
    DB-->>SRV: novoUsuarioId
    SRV->>SRV: gerarTokenJWT(novoUsuario)
    SRV-->>API: HTTP_201_Created(token, usuarioDto)
    API-->>UI: retornarAuthResponse(AuthResponse)
    UI->>MF: LoadControl(new UcCharacter())
    MF-->>Calouro: abrirForjaDoHeroi()
```

---

### 5.4 DS04 — Recuperação de Senha (`UcForgotPassword`)

![DS04 - Recuperar Senha (UcForgotPassword)](IMG/DiagramasDeSequencia/DS04%20-%20Recuperar%20Senha%20(UcForgotPassword).png)

```mermaid
sequenceDiagram
    autonumber
    actor Usuario as Usuario
    participant UI as UI:UcForgotPassword
    participant API as API:ApiService
    participant SRV as SRV:BackendAuthAPI
    participant DB as DB:PostgreSQL_Tronus
    participant MF as MF:MainForm

    Usuario->>UI: informarCelular(celular)
    UI->>UI: gerarCodigoSmsHomologacao(6 digitos)
    Usuario->>UI: confirmarCodigoEDigitarNovaSenha(codigo, novaSenha)
    UI->>UI: validarCodigoESenhaForte(min 8 caracteres)
    UI->>API: PostAsync("/auth/reset-password", req)
    API->>SRV: HTTP_POST_reset_password(celular, novaSenha)
    SRV->>DB: SELECT_usuario_por_celular(celular)
    DB-->>SRV: usuarioValido
    SRV->>SRV: BCrypt_HashPassword(novaSenha, salt=12)
    SRV->>DB: UPDATE_usuarios_SET_senha_hash(usuarioId, novoHash)
    DB-->>SRV: 1 linha afetada
    SRV-->>API: HTTP_200_OK(Senha redefinida)
    API-->>UI: confirmarSucesso()
    UI->>MF: LoadControl(new UcLogin())
    MF-->>Usuario: redirecionarParaLogin()
```

---

### 5.5 DS05 — Forja e Criação do Herói (`UcCharacter`)

![DS05 - Forja do Herói (UcCharacter)](IMG/DiagramasDeSequencia/DS05%20-%20Forja%20do%20Heroi%20(UcCharacter).png)

```mermaid
sequenceDiagram
    autonumber
    actor Estudante as Estudante
    participant UI as UI:UcCharacter
    participant API as API:ApiService
    participant SRV as SRV:BackendHeroAPI
    participant DB as DB:PostgreSQL_Tronus
    participant MF as MF:MainForm

    Estudante->>UI: selecionarGeneroEAvatar(genero, img/masc1.png)
    Estudante->>UI: digitarNomeHeroi("Sir Arthur")
    Estudante->>UI: clicarConsagrarHeroi()
    UI->>UI: validarNomeHeroi(min 3 caracteres)
    UI->>API: PostAsync("/personagens", req)
    API->>SRV: HTTP_POST_personagens(Bearer JWT, payload)
    SRV->>DB: INSERT_INTO_personagens(usuarioId, nome, Plebeu, 0)
    DB-->>SRV: novoPersonagemId
    SRV->>DB: INSERT_INTO_progresso_fases(personagemId, faseInicial=1)
    DB-->>SRV: progressoSalvo
    SRV-->>API: HTTP_201_Created(PersonagemDto)
    API-->>UI: retornarPersonagemCriado(PersonagemDto)
    UI->>MF: LoadControl(new UcReinos())
    MF-->>Estudante: abrirMapaDosReinos()
```

---

### 5.6 DS06 — Mapa dos Reinos e Seleção de Fases (`UcReinos`)

![DS06 - Seleção de Reinos (UcReinos)](IMG/DiagramasDeSequencia/DS06%20-%20Selecao%20de%20Reinos%20(UcReinos).png)

```mermaid
sequenceDiagram
    autonumber
    actor Estudante as Estudante
    participant UI as UI:UcReinos
    participant API as API:ApiService
    participant SRV as SRV:BackendReinosAPI
    participant DB as DB:PostgreSQL_Tronus
    participant MF as MF:MainForm

    MF->>UI: inicializarTela()
    UI->>API: GetAsync("/reinos")
    API->>SRV: HTTP_GET_reinos(Bearer JWT)
    SRV->>DB: SELECT_reinos_ativos(ORDER BY ordem)
    DB-->>SRV: listaReinos
    SRV-->>API: HTTP_200_OK(List~ReinoDto~)
    API-->>UI: retornarListaReinos(List~ReinoDto~)
    UI->>API: GetAsync("/progresso/me")
    API->>SRV: HTTP_GET_progresso_me(Bearer JWT)
    SRV->>DB: SELECT_progresso_fases(personagemId)
    DB-->>SRV: listaProgresso
    SRV-->>API: HTTP_200_OK(List~ProgressoFaseDto~)
    API-->>UI: retornarProgresso(List~ProgressoFaseDto~)
    UI->>UI: renderizarCardsEFases(estrelas, cadeados)
    Estudante->>UI: clicarJogarFase(reinoId, faseId)
    UI->>UI: verificarPreRequisitoDesbloqueio(faseNumero)
    UI->>MF: LoadControl(new UcQuiz(faseId, reinoNome))
    MF-->>Estudante: iniciarDesafioQuiz()
```

---

### 5.7 DS07 — Desafio e Quiz Gamificado (`UcQuiz`)

![DS07 - Desafio e Quiz (UcQuiz)](IMG/DiagramasDeSequencia/DS07%20-%20Desafio%20e%20Quiz%20(UcQuiz).png)

```mermaid
sequenceDiagram
    autonumber
    actor Estudante as Estudante
    participant UI as UI:UcQuiz
    participant API as API:ApiService
    participant SRV as SRV:BackendQuizAPI
    participant DB as DB:PostgreSQL_Tronus
    participant TRG as TRG:TriggerPatentes
    participant MF as MF:MainForm

    MF->>UI: carregarFase(faseId)
    UI->>API: GetAsync("/fases/{id}/questoes")
    API->>SRV: HTTP_GET_questoes_da_fase(faseId)
    SRV->>DB: SELECT_questoes_JOIN_alternativas(faseId)
    DB-->>SRV: 5 questoes
    SRV-->>API: HTTP_200_OK(FaseQuestoesDto)
    API-->>UI: retornarQuestoes(FaseQuestoesDto)
    UI->>UI: iniciarTimerERenderizarQuestao()
    Estudante->>UI: selecionarAlternativasEFinalizar(respostas[1..5])
    UI->>API: PostAsync("/fases/{id}/responder", payload)
    API->>SRV: HTTP_POST_responder_fase(EnviarRespostasRequest)
    SRV->>DB: INSERT_INTO_respostas_tentativas(personagemId, questoes)
    DB-->>SRV: tentativasSalvas
    SRV->>DB: CALL_sp_registrar_progresso(GREATEST(estrelas))
    DB->>TRG: dispararTrigger(trg_progresso_atualiza_personagem)
    TRG->>DB: UPDATE_personagens_SET_titulo_e_estrelas(SUM(estrelas))
    DB-->>TRG: patenteAtualizada
    DB-->>SRV: retornarResultadoPersistido(estrelas, novoTitulo)
    SRV-->>API: HTTP_200_OK(ResultadoQuizResponse)
    API-->>UI: retornarResultadoQuiz(ResultadoQuizResponse)
    UI-->>Estudante: exibirCartaoDeVitoria(estrelas, patente)
    Estudante->>UI: clicarVoltarAoMapa()
    UI->>MF: LoadControl(new UcReinos())
```

---

### 5.8 DS08 — Painel de Desempenho e Dashboard (`UcDashboard`)

![DS08 - Dashboard do Aluno (UcDashboard)](IMG/DiagramasDeSequencia/DS08%20-%20Dashboard%20do%20Aluno%20(UcDashboard).png)

```mermaid
sequenceDiagram
    autonumber
    actor Estudante as Estudante
    participant UI as UI:UcDashboard
    participant API as API:ApiService
    participant SRV as SRV:BackendStatsAPI
    participant DB as DB:PostgreSQL_Tronus
    participant MF as MF:MainForm

    MF->>UI: inicializarDashboard()
    UI->>API: GetAsync("/dashboard/me")
    API->>SRV: HTTP_GET_dashboard_me(Bearer JWT)
    SRV->>DB: SELECT_personagem_logado(usuarioId)
    DB-->>SRV: dadosPersonagem
    SRV->>DB: SELECT_SUM_estrelas_GROUP_BY_reino(personagemId)
    DB-->>SRV: metricasPorReino
    SRV-->>API: HTTP_200_OK(DashboardDto)
    API-->>UI: retornarDashboardDto(DashboardDto)
    UI->>UI: atualizarBarraProgressoPatente(metaProximoTitulo)
    UI->>UI: renderizarCardsDesempenhoPorReino()
    Estudante->>UI: clicarContinuarJornada()
    UI->>MF: LoadControl(new UcReinos())
```

---

### 5.9 DS09 — Quadro de Honra e Ranking (`UcRanking`)

![DS09 - Ranking Acadêmico (UcRanking)](IMG/DiagramasDeSequencia/DS09%20-%20Ranking%20Academico%20(UcRanking).png)

```mermaid
sequenceDiagram
    autonumber
    actor Estudante as Estudante
    participant UI as UI:UcRanking
    participant API as API:ApiService
    participant SRV as SRV:BackendRankingAPI
    participant DB as DB:PostgreSQL_Tronus

    Estudante->>UI: abrirTelaRanking()
    UI->>API: GetAsync("/ranking?reinoId=all")
    API->>SRV: HTTP_GET_ranking(reinoId=all)
    SRV->>DB: SELECT_TOP_personagens_ORDER_BY_estrelas(DESC LIMIT 50)
    DB-->>SRV: topEstudantes
    SRV->>SRV: calcularPosicaoUsuarioLogado(usuarioId)
    SRV-->>API: HTTP_200_OK(RankingResponse)
    API-->>UI: retornarRanking(RankingResponse)
    UI->>UI: renderizarPodioTop3(Ouro, Prata, Bronze)
    UI->>UI: destacarPosicaoDoEstudante()
    Estudante->>UI: filtrarPorReino(reinoId)
    UI->>API: GetAsync("/ranking?reinoId=2")
    API->>SRV: HTTP_GET_ranking_reino(reinoId=2)
    SRV->>DB: SELECT_ranking_por_reino(reinoId=2)
    DB-->>SRV: rankingFiltrado
    SRV-->>API: HTTP_200_OK(RankingResponse)
    API-->>UI: atualizarTabelaClassificacao(RankingFiltrado)
```

---

### 5.10 DS10 — Painel Pedagógico do Professor (`UcProfessor`)

![DS10 - Painel do Professor (UcProfessor)](IMG/DiagramasDeSequencia/DS10%20-%20Painel%20do%20Professor%20(UcProfessor).png)

```mermaid
sequenceDiagram
    autonumber
    actor Professor as Professor
    participant UI as UI:UcProfessor
    participant API as API:ApiService
    participant SRV as SRV:BackendProfessorAPI
    participant DB as DB:PostgreSQL_Tronus
    participant MF as MF:MainForm

    MF->>UI: carregarPainelDocente()
    UI->>API: GetAsync("/professor/resumo")
    API->>SRV: HTTP_GET_professor_resumo(Bearer JWT)
    SRV->>DB: SELECT_metricas_turma_e_questoes(reinoDocente)
    DB-->>SRV: resumoTurma
    SRV-->>API: HTTP_200_OK(ProfResumoDto)
    API-->>UI: retornarResumoDocente(ProfResumoDto)
    Professor->>UI: preencherNovaQuestao(faseId, enunciado, alternativas, gabarito)
    Professor->>UI: clicarSalvarQuestao()
    UI->>UI: validarCamposEGabarito()
    UI->>API: PostAsync("/fases/{id}/questoes", novaQuestao)
    API->>SRV: HTTP_POST_criar_questao(NovaQuestaoDto)
    SRV->>DB: INSERT_INTO_questoes(faseId, enunciado, dificuldade)
    DB-->>SRV: novaQuestaoId
    SRV->>DB: INSERT_INTO_alternativas(questaoId, A..D, correta)
    DB-->>SRV: alternativasSalvas
    SRV-->>API: HTTP_201_Created(novaQuestaoId)
    API-->>UI: confirmarQuestaoCriada()
    UI->>UI: atualizarGradeQuestoes()
    UI-->>Professor: exibirNotificacaoSucesso()
```

---

### 5.11 DS11 — Painel de Governança do Administrador (`UcAdmin`)

![DS11 - Painel do Administrador (UcAdmin)](IMG/DiagramasDeSequencia/DS11%20-%20Painel%20do%20Administrador%20(UcAdmin).png)

```mermaid
sequenceDiagram
    autonumber
    actor Admin as Administrador
    participant UI as UI:UcAdmin
    participant API as API:ApiService
    participant SRV as SRV:BackendAdminAPI
    participant DB as DB:PostgreSQL_Tronus
    participant MF as MF:MainForm

    MF->>UI: carregarPainelAdmin()
    UI->>API: GetAsync("/admin/usuarios")
    API->>SRV: HTTP_GET_admin_usuarios(Bearer JWT)
    SRV->>DB: SELECT_todos_usuarios_ativos(ORDER BY id)
    DB-->>SRV: listaUsuariosAtivos
    SRV-->>API: HTTP_200_OK(List~UsuarioDto~)
    API-->>UI: sincronizarUsuariosECalcularKPIs(List~UsuarioDto~)
    Admin->>UI: filtrarPorPapel(papel = aluno | professor | admin)
    UI->>UI: aplicarFiltroGridAtivos()
    Admin->>UI: alternarPapelUsuario(usuarioId, novoPapel)
    UI->>API: PutAsync("/admin/usuarios/{id}/papel", req)
    API->>SRV: HTTP_PUT_atualizar_papel(usuarioId, novoPapel)
    SRV->>DB: UPDATE_usuarios_SET_papel(usuarioId, novoPapel)
    DB-->>SRV: 1 linha atualizada
    SRV-->>API: HTTP_200_OK(Papel atualizado)
    API-->>UI: confirmarAlteracaoRBAC()
    UI->>UI: recarregarGridEKPIs()
    UI-->>Admin: exibirConfirmacaoRBAC()
```

---

## 6. Matriz de Rastreabilidade (Casos de Uso vs. Telas vs. Diagramas)

| Caso de Uso do Sistema | Tela Responsável | Diagrama de Sequência Astah | Entidades de Banco Impactadas |
| :--- | :--- | :--- | :--- |
| **UC01: Efetuar Cadastro** | `UcRegister` (`register.html`) | `DS03 - Tela Cadastro (UcRegister)` | `usuarios` |
| **UC02: Efetuar Login** | `UcLogin` (`login.html`) | `DS02 - Tela Login (UcLogin)` | `usuarios`, `personagens` |
| **UC03: Recuperar Senha** | `UcForgotPassword` (`forgot_password.html`) | `DS04 - Recuperar Senha (UcForgotPassword)` | `usuarios` |
| **UC04: Forjar Personagem** | `UcCharacter` (`character.html`) | `DS05 - Forja do Heroi (UcCharacter)` | `personagens`, `progresso_fases` |
| **UC05: Explorar Mapa dos Reinos** | `UcReinos` (`reinos.html`) | `DS06 - Selecao de Reinos (UcReinos)` | `reinos`, `fases`, `progresso_fases` |
| **UC06 / UC07: Responder e Pontuar Quiz** | `UcQuiz` | `DS07 - Desafio e Quiz (UcQuiz)` | `questoes`, `alternativas`, `respostas_tentativas`, `progresso_fases` |
| **UC08 / UC09: Desbloquear Fase e Promover Título** | `UcReinos` & `UcQuiz` (Trigger) | `DS06` e `DS07` | `personagens`, `trg_progresso_atualiza_personagem` |
| **UC10: Visualizar Desempenho** | `UcDashboard` (`dashboard.html`) | `DS08 - Dashboard do Aluno (UcDashboard)` | `personagens`, `progresso_fases`, `reinos` |
| **UC11: Acompanhar Classificação** | `UcRanking` | `DS09 - Ranking Academico (UcRanking)` | `personagens`, `usuarios` |
| **UC12: Gestão Pedagógica Docente** | `UcProfessor` (`professor.html`) | `DS10 - Painel do Professor (UcProfessor)` | `questoes`, `alternativas`, `fases` |
| **UC13: Governança Administrativa** | `UcAdmin` (`admin.html`) | `DS11 - Painel do Administrador (UcAdmin)` | `usuarios`, `reinos` |
