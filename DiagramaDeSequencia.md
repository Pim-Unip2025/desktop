# 🔄 Diagramas de Sequência — Sistema Tronus

**Projeto Integrado Multidisciplinar (PIM III) — UNIP 2026**  
**Curso:** Superior de Tecnologia em Análise e Desenvolvimento de Sistemas (ADS)  
**Disciplina de Referência:** Análise e Modelagem de Sistemas / Engenharia de Software  
**Ferramenta CASE Oficial:** Astah Community / Astah UML  
**Arquivos Modelados (`.asta` e `.astah`):**
* `docs/DiagramasDeSequencia.asta`
* `docs/DiagramasDeSequencia.astah`
* `docs/Engenharia de software ágil aplicada.asta`
* `docs/Engenharia de software ágil aplicada.astah`
* `TronusDesktop/DiagramasDeSequencia.asta`
* `TronusDesktop/DiagramasDeSequencia.astah`

---

## 📌 Sumário

1. [Visão Geral e Fundamentação Teórica](#1-visão-geral-e-fundamentação-teórica)
2. [Arquitetura em Camadas do Tronus (Desktop & Web)](#2-arquitetura-em-camadas-do-tronus-desktop--web)
3. [Dicionário de Linhas de Vida (Lifelines) e Participantes](#3-dicionário-de-linhas-de-vida-lifelines-e-participantes)
4. [Matriz de Correspondência entre Telas e Diagramas](#4-matriz-de-correspondência-entre-telas-e-diagramas)
5. [Diagramas de Sequência Detalhados por Tela](#5-diagramas-de-sequência-detalhados-por-tela)
   - [5.1 Tela 01 — Apresentação e Boas-Vindas (`UcLanding` / `index.html`)](#51-tela-01--apresentação-e-boas-vindas-uclanding--indexhtml)
   - [5.2 Tela 02 — Autenticação e Login (`UcLogin` / `login.html`)](#52-tela-02--autenticação-e-login-uclogin--loginhtml)
   - [5.3 Tela 03 — Cadastro de Estudante (`UcRegister` / `register.html`)](#53-tela-03--cadastro-de-estudante-ucregister--registerhtml)
   - [5.4 Tela 04 — Recuperação de Senha (`UcForgotPassword` / `forgot_password.html`)](#54-tela-04--recuperação-de-senha-ucforgotpassword--forgot_passwordhtml)
   - [5.5 Tela 05 — Forja e Criação do Herói (`UcCharacter` / `character.html`)](#55-tela-05--forja-e-criação-do-herói-uccharacter--characterhtml)
   - [5.6 Tela 06 — Mapa dos Reinos e Seleção de Fases (`UcReinos` / `reinos.html`)](#56-tela-06--mapa-dos-reinos-e-seleção-de-fases-ucreinos--reinoshtml)
   - [5.7 Tela 07 — Desafio e Quiz Gamificado (`UcQuiz`)](#57-tela-07--desafio-e-quiz-gamificado-ucquiz)
   - [5.8 Tela 08 — Painel de Desempenho e Dashboard (`UcDashboard` / `dashboard.html`)](#58-tela-08--painel-de-desempenho-e-dashboard-ucdashboard--dashboardhtml)
   - [5.9 Tela 09 — Quadro de Honra e Ranking (`UcRanking`)](#59-tela-09--quadro-de-honra-e-ranking-ucranking)
   - [5.10 Tela 10 — Painel Pedagógico do Professor (`UcProfessor` / `professor.html`)](#510-tela-10--painel-pedagógico-do-professor-ucprofessor--professorhtml)
   - [5.11 Tela 11 — Painel de Governança do Administrador (`UcAdmin` / `admin.html`)](#511-tela-11--painel-de-governança-do-administrador-ucadmin--adminhtml)
6. [Integração e Visualização no Astah UML (`.asta` / `.astah`)](#6-integração-e-visualização-no-astah-uml-asta--astah)
7. [Matriz de Rastreabilidade (Casos de Uso vs. Telas vs. Diagramas)](#7-matriz-de-rastreabilidade-casos-de-uso-vs-telas-vs-diagramas)

---

## 1. Visão Geral e Fundamentação Teórica

O **Diagrama de Sequência** é o principal diagrama comportamental e de interação da UML 2.5. Ele modela a **dinâmica temporal** do sistema Tronus, ilustrando a ordem cronológica em que objetos, telas, serviços e tabelas de banco de dados trocam mensagens síncronas, assíncronas e de retorno para executar as operações de cada tela.

Enquanto o **Diagrama de Casos de Uso** expressa *o que* o sistema deve realizar externamente e o **Diagrama de Classes** descreve *a estrutura estática*, os **Diagramas de Sequência** detalham *como* as camadas de software colaboram internamente para concretizar as regras de negócio de cada tela.

No contexto acadêmico do PIM III (UNIP), este documento fundamenta a **Seção de Modelagem Dinâmica / Interação de Telas** do relatório escrito (`docs/PIM_III_Tronus.docx`).

---

## 2. Arquitetura em Camadas do Tronus (Desktop & Web)

A plataforma Tronus opera segundo uma arquitetura distribuída em camadas bem desacopladas:

```mermaid
graph LR
    subgraph Apresentacao [Camada de Apresentação]
        Desk[Desktop .NET 10 WinForms<br/>MainForm + UserControls]
        Web[Web Responsive<br/>HTML5 / CSS3 / Vanilla JS]
    end

    subgraph Servicos [Camada de Cliente & Sessão]
        API[ApiService HttpClient<br/>Bearer JWT Auth]
        SM[SessionManager<br/>Estado do Usuário Logado]
    end

    subgraph Backend [Camada de Serviços & Regras de Negócio]
        AuthAPI[Auth Router & Controller<br/>BCrypt + JWT Engine]
        GameAPI[Game & Quiz Controller<br/>Cálculo de Estrelas & Patentes]
        AdminAPI[Admin & Docente Controller<br/>Governança de Turmas]
    end

    subgraph Persistencia [Camada de Dados]
        PostgreSQL[(Banco de Dados Relacional<br/>PostgreSQL / Supabase)]
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

> 🌐 **Protocolos de Transporte e Portas de Rede nos Diagramas de Sequência:**
> - **Porta 443 (HTTPS / TLS 1.2 e 1.3):** Todas as mensagens síncronas/assíncronas trocadas entre o cliente desktop (`API: ApiService`) e a API central na nuvem (`SRV: Backend API`) trafegam obrigatoriamente pela **porta 443 (TCP)** com criptografia de ponta a ponta.
> - **Porta 3000 (HTTP):** Utilizada exclusivamente em ambiente de desenvolvimento local para depuração dos endpoints da API.
> - **Portas 5432 / 6543 (PostgreSQL / Pooler):** Camada interna de banco de dados entre a API e o Supabase na nuvem, isolada da rede pública e inacessível diretamente pelo cliente.

---

## 3. Dicionário de Linhas de Vida (Lifelines) e Participantes

Em cada diagrama de sequência, os seguintes elementos exercem papéis específicos:

| Linha de Vida (Lifeline) | Tipo UML | Descrição e Responsabilidade |
| :--- | :---: | :--- |
| **`Ator (Estudante/Docente/Admin)`** | Ator Humano | Usuário que dispara os eventos por cliques, digitação ou seleção em tela. |
| **`UI: UserControl / Page`** | View / Fronteira | Formulário ou controle visual (`UcLogin`, `UcQuiz`, etc.) responsável por capturar eventos, validar entradas no front e renderizar dados. |
| **`MF: MainForm`** | Controller de Janela | Formulário principal do C# Desktop que hospeda os `UserControls` dinamicamente no `contentPanel` via `LoadControl(uc)`. |
| **`API: ApiService`** | Client Service | Singleton HTTP que serializa DTOs em JSON, anexa cabeçalho `Authorization: Bearer <token>` e gerencia chamadas assíncronas. |
| **`SM: SessionManager`** | State Manager | Gerenciador estático em memória que armazena a sessão ativa (`CurrentUser`, `AccessToken`, `PersonagemAtivo`). |
| **`SRV: Backend API`** | Controller / Endpoint | API Node.js/Express que processa requisições REST, aplica regras de segurança, hashing e responde em JSON. |
| **`DB: PostgreSQL_Tronus`** | Persistência Relacional (Supabase) | Banco de dados relacional PostgreSQL que executa comandos SQL com integridade referencial, RLS e constraints estritas. |
| **`TRG: Triggers / Procedures`** | Automação no Banco | Gatilhos de banco (`trg_progresso_atualiza_personagem`) que promovem o herói de patente conforme a pontuação. |

---

## 4. Matriz de Correspondência entre Telas e Diagramas

O sistema Tronus contempla **11 telas principais**, com cobertura completa em Desktop (C# .NET 10) e Web:

| # | Nome da Tela | Componente Desktop (C#) | Arquivo Web | Papel de Acesso | Diagrama Astah |
| :-: | :--- | :--- | :--- | :---: | :--- |
| **01** | Tela de Apresentação / Landing | `UcLanding.cs` | `index.html` | Público / Todos | `DS01 - Tela Landing (UcLanding)` |
| **02** | Tela de Autenticação / Login | `UcLogin.cs` | `login.html` | Público | `DS02 - Tela Login (UcLogin)` |
| **03** | Tela de Cadastro de Usuário | `UcRegister.cs` | `register.html` | Público | `DS03 - Tela Cadastro (UcRegister)` |
| **04** | Recuperação de Senha | `UcForgotPassword.cs` | `forgot_password.html` | Público | `DS04 - Recuperar Senha (UcForgotPassword)` |
| **05** | Forja do Herói (Criação de Personagem) | `UcCharacter.cs` | `character.html` | Aluno (Sem Herói) | `DS05 - Forja do Heroi (UcCharacter)` |
| **06** | Mapa dos Reinos e Trilha de Fases | `UcReinos.cs` | `reinos.html` / `trilha.html` | Aluno | `DS06 - Selecao de Reinos (UcReinos)` |
| **07** | Desafio e Resolução de Quiz | `UcQuiz.cs` | Quiz Interativo | Aluno | `DS07 - Desafio e Quiz (UcQuiz)` |
| **08** | Painel do Aluno / Dashboard | `UcDashboard.cs` | `dashboard.html` | Aluno | `DS08 - Dashboard do Aluno (UcDashboard)` |
| **09** | Quadro de Honra / Ranking Acadêmico | `UcRanking.cs` | Ranking Interativo | Aluno / Docente | `DS09 - Ranking Academico (UcRanking)` |
| **10** | Painel Pedagógico do Professor | `UcProfessor.cs` | `professor.html` | Professor | `DS10 - Painel do Professor (UcProfessor)` |
| **11** | Painel de Governança do Administrador | `UcAdmin.cs` | `admin.html` | Administrador | `DS11 - Painel do Administrador (UcAdmin)` |

---

## 5. Diagramas de Sequência Detalhados por Tela

---

### 5.1 Tela 01 — Apresentação e Boas-Vindas (`UcLanding` / `index.html`)

A tela de boas-vindas apresenta a narrativa imersiva de Tronus, permitindo que o usuário inicie sua jornada ou conheça as regras da plataforma.

```mermaid
sequenceDiagram
    autonumber
    actor Aluno as 👤 Estudante (Calouro)
    participant UI as 🖥️ UI: UcLanding
    participant MF as 🖼️ MF: MainForm
    participant SM as 🔐 SM: SessionManager

    Aluno->>UI: Abrir Aplicação
    UI->>SM: verificarSessaoAtiva()
    SM-->>UI: false (nenhuma sessão salva)
    UI->>UI: renderizarHeroBanner() & animacoes
    Aluno->>UI: clicarIniciarJornada()
    UI->>MF: LoadControl(new UcLogin())
    MF->>MF: contentPanel.Controls.Clear()
    MF->>MF: contentPanel.Controls.Add(ucLogin)
    MF-->>Aluno: Exibe Tela de Login com Efeito de Transição
```

#### Tabela de Mensagens — Tela 01
| Passo | Origem | Destino | Mensagem / Operação | Tipo | Descrição |
| :-: | :--- | :--- | :--- | :---: | :--- |
| 1 | Estudante | `UcLanding` | `abrirAplicacao()` | Síncrono | Usuário inicializa o executável ou acessa `index.html`. |
| 2 | `UcLanding` | `SessionManager` | `verificarSessaoAtiva()` | Síncrono | Inspeciona se há token JWT em cache ou no armazenamento local. |
| 3 | `SessionManager` | `UcLanding` | `false` | Retorno | Confirma que não há usuário autenticado no momento. |
| 4 | `UcLanding` | `UcLanding` | `renderizarHeroBanner()` | Auto-chamada | Carrega brasões medievais e textos temáticos. |
| 5 | Estudante | `UcLanding` | `clicarIniciarJornada()` | Síncrono | Dispara a ação do botão primário "INICIAR JORNADA". |
| 6 | `UcLanding` | `MainForm` | `LoadControl(new UcLogin())` | Síncrono | Solicita ao orquestrador a troca de UserControl. |
| 7-8 | `MainForm` | `MainForm` | `Controls.Add(ucLogin)` | Auto-chamada | Atualiza a árvore de controles da janela sem recriar o formulário. |

---

### 5.2 Tela 02 — Autenticação e Login (`UcLogin` / `login.html`)

A tela de login recebe o e-mail/usuário e senha, valida credenciais na API com hashing seguro (BCrypt), gera a sessão JWT e roteia o usuário de acordo com seu papel e a existência prévia de um herói acadêmico.

```mermaid
sequenceDiagram
    autonumber
    actor Usuario as 👤 Usuário (Aluno / Prof / Admin)
    participant UI as 🖥️ UI: UcLogin
    participant API as 🌐 API: ApiService
    participant SM as 🔐 SM: SessionManager
    participant SRV as ⚙️ SRV: BackendAuthAPI
    participant DB as 🐘 DB: PostgreSQL_Tronus
    participant MF as 🖼️ MF: MainForm

    Usuario->>UI: informarCredenciais(email, senha)
    Usuario->>UI: clicarEntrar()
    UI->>UI: validarCamposNaoVazios()
    UI->>API: PostAsync<LoginRequest, AuthResponse>("/auth/login", req)
    API->>SRV: HTTP POST /auth/login { identificador, senha }
    SRV->>DB: SELECT id, nome, email, celular, papel, senha_hash FROM usuarios WHERE email = ?
    DB-->>SRV: registroUsuario
    SRV->>SRV: BCrypt.Verify(senha, senha_hash)
    SRV->>SRV: gerarTokenJWT(usuario)
    SRV-->>API: 200 OK { token, usuario }
    API-->>UI: AuthResponse
    UI->>SM: Login(authResponse)
    
    alt Papel == 'aluno'
        UI->>API: GetAsync<PersonagemDto>("/personagens/me")
        API->>SRV: HTTP GET /personagens/me [Bearer JWT]
        SRV->>DB: SELECT * FROM personagens WHERE usuario_id = ?
        
        alt Personagem Encontrado (200 OK)
            DB-->>SRV: dadosPersonagem
            SRV-->>API: 200 OK (PersonagemDto)
            API-->>UI: PersonagemDto
            UI->>MF: LoadControl(new UcReinos())
            MF-->>Usuario: Exibe Mapa dos Reinos
        else Personagem Não Existe (404 Not Found)
            SRV-->>API: 404 Not Found
            API-->>UI: ApiException (404)
            UI->>MF: LoadControl(new UcCharacter())
            MF-->>Usuario: Exibe Forja do Herói para Primeiro Acesso
        end
    else Papel == 'professor'
        UI->>MF: LoadControl(new UcProfessor())
        MF-->>Usuario: Exibe Portal Pedagógico do Professor
    else Papel == 'admin'
        UI->>MF: LoadControl(new UcAdmin())
        MF-->>Usuario: Exibe Painel de Governança
    end
```

#### Tabela de Mensagens — Tela 02
| Passo | Origem | Destino | Mensagem / Operação | Tipo | Descrição |
| :-: | :--- | :--- | :--- | :---: | :--- |
| 1-2 | Usuário | `UcLogin` | `informarCredenciais()`, `clicarEntrar()` | Síncrono | Entrada dos dados de autenticação e clique no botão. |
| 3 | `UcLogin` | `UcLogin` | `validarCamposNaoVazios()` | Auto-chamada | Bloqueia submissão caso campos estejam em branco. |
| 4-5 | `UcLogin` | `Backend` | `POST /auth/login` | Assíncrono | Transmite credenciais via HTTPS seguro. |
| 6-7 | `Backend` | `PostgreSQL` | `SELECT ... WHERE email = ?` | Síncrono | Localiza conta correspondente no banco de dados. |
| 8-9 | `Backend` | `Backend` | `BCrypt.Verify()` / `gerarTokenJWT()` | Auto-chamada | Valida hash da senha e emite o token assinado. |
| 10-12 | `Backend` | `UcLogin` | `200 OK` $\rightarrow$ `SessionManager.Login()` | Retorno | Armazena token na sessão ativa do cliente. |
| 13-16 | `UcLogin` | `Backend` | `GET /personagens/me` | Assíncrono | Inspeciona se o estudante já forjou seu personagem. |
| 17-20 | `MainForm` | Usuário | `LoadControl(UcReinos | UcCharacter)` | Síncrono | Roteia para os Reinos (se já tem herói) ou Forja (se calouro novo). |

---

### 5.3 Tela 03 — Cadastro de Estudante (`UcRegister` / `register.html`)

Permite que novos calouros forjem sua conta institucional na plataforma com validação de formato de e-mail acadêmico, celular e confirmação de senha.

```mermaid
sequenceDiagram
    autonumber
    actor Calouro as 👤 Calouro (Estudante)
    participant UI as 🖥️ UI: UcRegister
    participant API as 🌐 API: ApiService
    participant SRV as ⚙️ SRV: BackendAuthAPI
    participant DB as 🐘 DB: PostgreSQL_Tronus
    participant MF as 🖼️ MF: MainForm

    Calouro->>UI: preencherFormulario(nome, email, celular, senha, confirma)
    Calouro->>UI: clicarForjarConta()
    UI->>UI: validarFormulario(emailValido, senhasIguais, celularValido)
    UI->>API: PostAsync<RegisterRequest, AuthResponse>("/auth/register", req)
    API->>SRV: HTTP POST /auth/register
    SRV->>DB: SELECT id FROM usuarios WHERE email = ?
    
    alt E-mail Já Cadastrado
        DB-->>SRV: idExistente
        SRV-->>API: 409 Conflict { message: "E-mail já está em uso" }
        API-->>UI: ApiException (409)
        UI-->>Calouro: Exibe Alerta: "E-mail já cadastrado no Reino"
    else E-mail Disponível
        DB-->>SRV: null
        SRV->>SRV: BCrypt.HashPassword(senha, salt=12)
        SRV->>DB: INSERT INTO usuarios (nome, email, celular, senha_hash, papel) VALUES (?, ?, ?, ?, 'aluno')
        DB-->>SRV: novoUsuarioId
        SRV->>SRV: gerarTokenJWT(novoUsuario)
        SRV-->>API: 201 Created { token, usuario }
        API-->>UI: AuthResponse
        UI->>MF: LoadControl(new UcCharacter())
        MF-->>Calouro: Transição Imediata para a Forja do Personagem
    end
```

#### Tabela de Mensagens — Tela 03
| Passo | Origem | Destino | Mensagem / Operação | Tipo | Descrição |
| :-: | :--- | :--- | :--- | :---: | :--- |
| 1-2 | Calouro | `UcRegister` | `preencherFormulario()`, `clicarForjarConta()` | Síncrono | Entrada cadastral completa do estudante. |
| 3 | `UcRegister` | `UcRegister` | `validarFormulario()` | Auto-chamada | Checagem de regex de e-mail e conferência de senha dupla. |
| 4-5 | `UcRegister` | `Backend` | `POST /auth/register` | Assíncrono | Envio da requisição de cadastro via HTTP JSON. |
| 6-7 | `Backend` | `PostgreSQL` | `SELECT id WHERE email = ?` | Síncrono | Garante unicidade cadastral no banco. |
| 8-9 | `Backend` | `PostgreSQL` | `INSERT INTO usuarios (...)` | Síncrono | Persiste o novo usuário com papel `'aluno'`. |
| 10-12 | `Backend` | `MainForm` | `LoadControl(new UcCharacter())` | Síncrono | Direciona o novo estudante diretamente para criar seu herói. |

---

### 5.4 Tela 04 — Recuperação de Senha (`UcForgotPassword` / `forgot_password.html`)

Garante a segurança e a redefinição de credenciais por meio da validação combinada de dois fatores: e-mail institucional e número de telefone celular previamente cadastrado.

```mermaid
sequenceDiagram
    autonumber
    actor Usuario as 👤 Usuário
    participant UI as 🖥️ UI: UcForgotPassword
    participant API as 🌐 API: ApiService
    participant SRV as ⚙️ SRV: BackendAuthAPI
    participant DB as 🐘 DB: PostgreSQL_Tronus
    participant MF as 🖼️ MF: MainForm

    Usuario->>UI: informarEmailECelular(email, celular)
    Usuario->>UI: digitarNovaSenha(senha, confirmacao)
    Usuario->>UI: clicarRedefinirSenha()
    UI->>UI: validarSenhasIguais(min 6 caracteres)
    UI->>API: PostAsync("/auth/forgot-password", req)
    API->>SRV: HTTP POST /auth/forgot-password { email, celular, novaSenha }
    SRV->>DB: SELECT id, nome FROM usuarios WHERE email = ? AND celular = ?
    
    alt Dados Incorretos / Usuário Não Localizado
        DB-->>SRV: null
        SRV-->>API: 404 Not Found { message: "Dados não conferem com o registro" }
        API-->>UI: ApiException (404)
        UI-->>Usuario: Exibe Alerta: "E-mail ou celular inválido"
    else Identidade Confirmada
        DB-->>SRV: usuarioValido
        SRV->>SRV: BCrypt.HashPassword(novaSenha, salt=12)
        SRV->>DB: UPDATE usuarios SET senha_hash = ? WHERE id = ?
        DB-->>SRV: 1 linha afetada
        SRV-->>API: 200 OK { message: "Senha redefinida com sucesso" }
        API-->>UI: Sucesso
        UI->>MF: LoadControl(new UcLogin())
        MF-->>Usuario: Exibe Notificação e Redireciona para o Login
    end
```

#### Tabela de Mensagens — Tela 04
| Passo | Origem | Destino | Mensagem / Operação | Tipo | Descrição |
| :-: | :--- | :--- | :--- | :---: | :--- |
| 1-3 | Usuário | `UcForgotPassword` | `informarEmailECelular()`, `digitarNovaSenha()` | Síncrono | Fornecimento dos fatores de validação e da nova senha. |
| 4 | `UcForgotPassword` | `UcForgotPassword` | `validarSenhasIguais()` | Auto-chamada | Verificação de conformidade no cliente. |
| 5-6 | `UcForgotPassword` | `Backend` | `POST /auth/forgot-password` | Assíncrono | Requisição de redefinição para a API. |
| 7-8 | `Backend` | `PostgreSQL` | `SELECT WHERE email = ? AND celular = ?` | Síncrono | Validação cruzada de segurança no banco de dados. |
| 9-10 | `Backend` | `PostgreSQL` | `UPDATE usuarios SET senha_hash = ?` | Síncrono | Atualização do novo hash de senha. |
| 11-14 | `UcForgotPassword` | `MainForm` | `LoadControl(new UcLogin())` | Síncrono | Redirecionamento automático para a tela de Login. |

---

### 5.5 Tela 05 — Forja e Criação do Herói (`UcCharacter` / `character.html`)

Tela em que o estudante escolhe seu arquétipo, gênero, arte do avatar e consagra seu nome de batalha acadêmico, persistindo a entidade `Personagem` com patente inicial `'Plebeu'`.

```mermaid
sequenceDiagram
    autonumber
    actor Estudante as 👤 Estudante (Calouro)
    participant UI as 🖥️ UI: UcCharacter
    participant API as 🌐 API: ApiService
    participant SRV as ⚙️ SRV: BackendHeroAPI
    participant DB as 🐘 DB: PostgreSQL_Tronus
    participant MF as 🖼️ MF: MainForm

    Estudante->>UI: selecionarGenero("male" / "female")
    Estudante->>UI: escolherAvatar("img/masc1.png")
    Estudante->>UI: digitarNomeHeroi("Sir Arthur")
    Estudante->>UI: clicarConsagrarHeroi()
    UI->>UI: validarNomeHeroi(min 3 caracteres)
    UI->>API: PostAsync<PersonagemDto>("/personagens", req)
    API->>SRV: HTTP POST /personagens [Bearer JWT]
    SRV->>DB: INSERT INTO personagens (usuario_id, nome, genero, avatar, titulo_atual, total_estrelas) VALUES (?, ?, ?, ?, 'Plebeu', 0)
    DB-->>SRV: novoPersonagemId
    SRV->>DB: INSERT INTO progresso_fases (personagem_id, fase_id, estrelas) VALUES (?, 1, 0)
    DB-->>SRV: progressoInicialSalvo
    SRV-->>API: 201 Created (PersonagemDto)
    API-->>UI: PersonagemDto
    UI->>MF: LoadControl(new UcReinos())
    MF-->>Estudante: Abre Mapa dos Reinos com o Herói Forjado!
```

#### Tabela de Mensagens — Tela 05
| Passo | Origem | Destino | Mensagem / Operação | Tipo | Descrição |
| :-: | :--- | :--- | :--- | :---: | :--- |
| 1-3 | Estudante | `UcCharacter` | `selecionarGenero()`, `escolherAvatar()`, `digitarNomeHeroi()` | Síncrono | Customização interativa do avatar e nome medieval. |
| 4-5 | `UcCharacter` | `UcCharacter` | `validarNomeHeroi()` | Auto-chamada | Garante preenchimento e regras de nomenclatura. |
| 6-7 | `UcCharacter` | `Backend` | `POST /personagens` | Assíncrono | Envia payload com gênero, avatar e nome do herói. |
| 8-9 | `Backend` | `PostgreSQL` | `INSERT INTO personagens (...)` | Síncrono | Insere o herói com 0 estrelas e patente de Plebeu. |
| 10-11 | `Backend` | `PostgreSQL` | `INSERT INTO progresso_fases (...)` | Síncrono | Inicializa o registro de fases desbloqueadas. |
| 12-14 | `UcCharacter` | `MainForm` | `LoadControl(new UcReinos())` | Síncrono | Encaminha o novo herói ao Mapa dos Reinos. |

---

### 5.6 Tela 06 — Mapa dos Reinos e Seleção de Fases (`UcReinos` / `reinos.html`)

Exibe a cartografia dos Reinos (disciplinas do curso), avalia o status de desbloqueio de cada uma das 3 fases por reino e gerencia a entrada no desafio.

```mermaid
sequenceDiagram
    autonumber
    actor Estudante as 👤 Estudante
    participant MF as 🖼️ MF: MainForm
    participant UI as 🖥️ UI: UcReinos
    participant API as 🌐 API: ApiService
    participant SRV as ⚙️ SRV: BackendReinosAPI
    participant DB as 🐘 DB: PostgreSQL_Tronus

    MF->>UI: inicializarTela()
    par Carregar Reinos e Progresso
        UI->>API: GetAsync<List<ReinoDto>>("/reinos")
        API->>SRV: HTTP GET /reinos [Bearer JWT]
        SRV->>DB: SELECT * FROM reinos WHERE ativo = 1 ORDER BY ordem
        DB-->>SRV: listaReinos
        SRV-->>API: 200 OK (List<ReinoDto>)
        API-->>UI: listaReinos
    and
        UI->>API: GetAsync<List<ProgressoFaseDto>>("/progresso/me")
        API->>SRV: HTTP GET /progresso/me [Bearer JWT]
        SRV->>DB: SELECT pf.*, f.reino_id, f.numero FROM progresso_fases pf JOIN fases f ON pf.fase_id = f.id WHERE pf.personagem_id = ?
        DB-->>SRV: listaProgresso
        SRV-->>API: 200 OK (List<ProgressoFaseDto>)
        API-->>UI: listaProgresso
    end
    UI->>UI: renderizarCardsEFases(badges, estrelas, status)
    
    Estudante->>UI: clicarFase(reinoId, faseNumero)
    UI->>UI: verificarPreRequisitoDesbloqueio(faseNumero)
    
    alt Fase Bloqueada (Fase anterior sem estrelas)
        UI-->>Estudante: Alerta: "Conquiste ao menos 1 estrela na fase anterior!"
    else Fase Liberada
        UI->>MF: LoadControl(new UcQuiz(faseId, reinoNome))
        MF-->>Estudante: Inicia a Tela de Quiz da Fase
    end
```

#### Tabela de Mensagens — Tela 06
| Passo | Origem | Destino | Mensagem / Operação | Tipo | Descrição |
| :-: | :--- | :--- | :--- | :---: | :--- |
| 1 | `MainForm` | `UcReinos` | `inicializarTela()` | Síncrono | Invocado ao abrir o controle de visualização do mapa. |
| 2-7 | `UcReinos` | `Backend` | `GET /reinos` | Assíncrono | Busca catálogo de matérias ativas no sistema. |
| 8-13 | `UcReinos` | `Backend` | `GET /progresso/me` | Assíncrono | Busca histórico de estrelas obtidas pelo herói logado. |
| 14 | `UcReinos` | `UcReinos` | `renderizarCardsEFases()` | Auto-chamada | Desenha brasões, estrelas conquistadas e cadeados. |
| 15-16 | Estudante | `UcReinos` | `clicarFase()` / `verificarPreRequisito()` | Síncrono | Valida se a fase anterior foi concluída com $\ge 1$ estrela. |
| 17-18 | `UcReinos` | `MainForm` | `LoadControl(new UcQuiz(...))` | Síncrono | Inicia a arena de desafio acadêmico. |

---

### 5.7 Tela 07 — Desafio e Quiz Gamificado (`UcQuiz`)

Arena gamificada onde o estudante responde a questões de múltipla escolha com temporizador, recebe feedback visual instantâneo e pontua em direção a patentes nobiliárquicas.

```mermaid
sequenceDiagram
    autonumber
    actor Estudante as 👤 Estudante
    participant MF as 🖼️ MF: MainForm
    participant UI as 🖥️ UI: UcQuiz
    participant API as 🌐 API: ApiService
    participant SRV as ⚙️ SRV: BackendQuizAPI
    participant DB as 🐘 DB: PostgreSQL_Tronus
    participant TRG as ⚡ TRG: trg_progresso_atualiza_personagem

    MF->>UI: carregarFase(faseId)
    UI->>API: GetAsync<FaseQuestoesDto>("/fases/{id}/questoes")
    API->>SRV: HTTP GET /fases/{id}/questoes
    SRV->>DB: SELECT q.*, a.* FROM questoes q JOIN alternativas a ON a.questao_id = q.id WHERE q.fase_id = ? ORDER BY q.ordem, a.letra
    DB-->>SRV: questoesEAlternativas
    SRV-->>API: 200 OK (FaseQuestoesDto)
    API-->>UI: FaseQuestoesDto
    UI->>UI: iniciarTimerRegressivo()
    
    loop Para cada Questão da Fase
        Estudante->>UI: selecionarAlternativa(letra = 'B')
        Estudante->>UI: clicarConfirmarResposta()
        UI->>UI: calcularFeedbackVisual(cor, som, explicacao)
        UI->>UI: proximaQuestaoOuFinalizar()
    end
    
    UI->>API: PostAsync<EnviarRespostasRequest, ResultadoQuizResponse>("/fases/{id}/responder", payload)
    API->>SRV: HTTP POST /fases/{id}/responder
    SRV->>DB: INSERT INTO respostas_usuario (personagem_id, questao_id, fase_id, acertou) VALUES (...)
    SRV->>DB: CALL sp_registrar_progresso(?, ?, ?)  -- ON CONFLICT DO UPDATE SET estrelas = GREATEST(...)
    
    DB->>TRG: Disparo: trg_progresso_atualiza_personagem
    TRG->>DB: UPDATE personagens SET total_estrelas = (SELECT SUM(estrelas)...), titulo_atual = sp_obter_titulo(...)
    DB-->>SRV: Resultado Persistido
    SRV-->>API: 200 OK { estrelas, totalEstrelas, novoTitulo, xpGanho }
    API-->>UI: ResultadoFaseDto
    UI-->>Estudante: Exibe Modal Triunfal de Vitória (Estrelas & Promoção)
    Estudante->>UI: clicarVoltarTrilha()
    UI->>MF: LoadControl(new UcReinos())
```

#### Tabela de Mensagens — Tela 07
| Passo | Origem | Destino | Mensagem / Operação | Tipo | Descrição |
| :-: | :--- | :--- | :--- | :---: | :--- |
| 1-6 | `MainForm` | `Backend` | `GET /fases/{id}/questoes` | Assíncrono | Carrega questões e alternativas embaralhadas. |
| 7-10 | Estudante | `UcQuiz` | `selecionarAlternativa()`, `clicarConfirmar()` | Síncrono | Registro da escolha e ativação de som/animação. |
| 11-12 | `UcQuiz` | `Backend` | `POST /fases/{id}/responder` | Assíncrono | Submete todas as respostas ao final da fase. |
| 13-14 | `Backend` | `PostgreSQL` | `sp_registrar_progresso (GREATEST)` | Síncrono | Persiste o melhor score conquistado pelo aluno via procedure atômica. |
| 15-16 | `PostgreSQL` | `Trigger` | `trg_progresso_atualiza_personagem` | Gatilho DB | Promove patente nobiliárquica no banco de dados. |
| 17-20 | `UcQuiz` | `MainForm` | `exibirModalVitoria()` $\rightarrow$ `LoadControl(UcReinos)` | Síncrono | Exibe premiações e retorna ao mapa curricular. |

---

### 5.8 Tela 08 — Painel de Desempenho e Dashboard (`UcDashboard` / `dashboard.html`)

Apresenta a visão analítica de evolução acadêmica do herói: estrelas conquistadas, patente atual, barra de progresso para a próxima promoção e percentual de domínio por reino.

```mermaid
sequenceDiagram
    autonumber
    actor Estudante as 👤 Estudante
    participant MF as 🖼️ MF: MainForm
    participant UI as 🖥️ UI: UcDashboard
    participant API as 🌐 API: ApiService
    participant SRV as ⚙️ SRV: BackendStatsAPI
    participant DB as 🐘 DB: PostgreSQL_Tronus

    MF->>UI: inicializarDashboard()
    UI->>API: GetAsync<DashboardDto>("/dashboard/me")
    API->>SRV: HTTP GET /dashboard/me [Bearer JWT]
    SRV->>DB: SELECT * FROM personagens WHERE usuario_id = ?
    DB-->>SRV: dadosPersonagem
    SRV->>DB: SELECT r.nome, SUM(pf.estrelas) as estrelas, COUNT(pf.id) as concluidas FROM reinos r LEFT JOIN fases f ON f.reino_id = r.id LEFT JOIN progresso_fases pf ON pf.fase_id = f.id AND pf.personagem_id = ? GROUP BY r.id
    DB-->>SRV: estatisticasPorReino
    SRV-->>API: 200 OK (DashboardDto)
    API-->>UI: DashboardDto
    UI->>UI: atualizarBarraTituloNobiliarquico(metaProximoTitulo)
    UI->>UI: renderizarGraficosDeConclusao()
    Estudante->>UI: clicarContinuarJornada()
    UI->>MF: LoadControl(new UcReinos())
    MF-->>Estudante: Direciona para o Mapa dos Reinos
```

#### Tabela de Mensagens — Tela 08
| Passo | Origem | Destino | Mensagem / Operação | Tipo | Descrição |
| :-: | :--- | :--- | :--- | :---: | :--- |
| 1 | `MainForm` | `UcDashboard` | `inicializarDashboard()` | Síncrono | Instancia a tela de estatísticas do aluno. |
| 2-3 | `UcDashboard` | `Backend` | `GET /dashboard/me` | Assíncrono | Requisita consolidado pedagógico do estudante logado. |
| 4-7 | `Backend` | `PostgreSQL` | `SELECT personagens`, `SELECT progresso_fases` | Síncrono | Executa agregações (`SUM`, `COUNT`, `GROUP BY`). |
| 8-9 | `Backend` | `UcDashboard` | `200 OK (DashboardDto)` | Retorno | Devolve métricas estruturadas em JSON. |
| 10-11 | `UcDashboard` | `UcDashboard` | `atualizarBarraTituloNobiliarquico()` | Auto-chamada | Atualiza progresso visual (ex: 22/30 estrelas para Duque). |
| 12-14 | Estudante | `MainForm` | `LoadControl(new UcReinos())` | Síncrono | Retorna o estudante ao ciclo de estudos práticos. |

---

### 5.9 Tela 09 — Quadro de Honra e Ranking (`UcRanking`)

Exibe a classificação comparativa de todos os estudantes por total de estrelas e velocidade de resposta, permitindo filtragem geral ou por reino de disciplina.

```mermaid
sequenceDiagram
    autonumber
    actor Estudante as 👤 Estudante
    participant UI as 🖥️ UI: UcRanking
    participant API as 🌐 API: ApiService
    participant SRV as ⚙️ SRV: BackendRankingAPI
    participant DB as 🐘 DB: PostgreSQL_Tronus

    Estudante->>UI: abrirTelaRanking()
    UI->>API: GetAsync<RankingResponse>("/ranking?reinoId=all")
    API->>SRV: HTTP GET /ranking?reinoId=all [Bearer JWT]
    SRV->>DB: SELECT p.nome, p.avatar, p.titulo_atual, p.total_estrelas, u.nome as nome_completo FROM personagens p JOIN usuarios u ON p.usuario_id = u.id ORDER BY p.total_estrelas DESC, p.data_atualizacao ASC LIMIT 50
    DB-->>SRV: topEstudantes
    SRV->>SRV: calcularPosicaoEstudanteLogado(usuarioId)
    SRV-->>API: 200 OK (RankingResponse)
    API-->>UI: RankingResponse
    UI->>UI: renderizarPodioTop3(Ouro, Prata, Bronze)
    UI->>UI: destacarPosicaoAtualDoEstudante()
    
    opt Filtragem por Disciplina
        Estudante->>UI: filtrarPorReino(reinoId = 2)
        UI->>API: GetAsync<RankingResponse>("/ranking?reinoId=2")
        API->>SRV: HTTP GET /ranking?reinoId=2
        SRV-->>API: 200 OK (RankingFiltrado)
        API-->>UI: RankingFiltrado
        UI->>UI: atualizarTabelaComClassificacaoDoReino()
    end
```

#### Tabela de Mensagens — Tela 09
| Passo | Origem | Destino | Mensagem / Operação | Tipo | Descrição |
| :-: | :--- | :--- | :--- | :---: | :--- |
| 1-3 | Estudante | `Backend` | `GET /ranking?reinoId=all` | Assíncrono | Solicita o leaderboard global acadêmico. |
| 4-5 | `Backend` | `PostgreSQL` | `SELECT ... ORDER BY total_estrelas DESC` | Síncrono | Ordena os calouros mais dedicados no banco. |
| 6-8 | `Backend` | `UcRanking` | `200 OK (RankingResponse)` | Retorno | Retorna o Top 50 e a posição do usuário solicitante. |
| 9-10 | `UcRanking` | `UcRanking` | `renderizarPodioTop3()` | Auto-chamada | Destaca o pódio com coroas e louros visuais. |
| 11-14 | Estudante | `UcRanking` | `filtrarPorReino(2)` | Opcional | Permite ver quem lidera em Python, C++, etc. |

---

### 5.10 Tela 10 — Painel Pedagógico do Professor (`UcProfessor` / `professor.html`)

Portal docente que permite ao professor acompanhar o aproveitamento pedagógico das turmas, criar e editar novas questões para as fases e ajustar gabaritos.

```mermaid
sequenceDiagram
    autonumber
    actor Professor as 👨‍🏫 Professor
    participant MF as 🖼️ MF: MainForm
    participant UI as 🖥️ UI: UcProfessor
    participant API as 🌐 API: ApiService
    participant SRV as ⚙️ SRV: BackendProfessorAPI
    participant DB as 🐘 DB: PostgreSQL_Tronus

    MF->>UI: carregarPainelDocente()
    UI->>API: GetAsync<ProfResumoDto>("/professor/resumo")
    API->>SRV: HTTP GET /professor/resumo [Bearer JWT]
    SRV->>DB: SELECT COUNT(DISTINCT u.id) as alunos, AVG(pf.estrelas) as media FROM usuarios u JOIN progresso_fases pf...
    DB-->>SRV: dadosDesempenhoTurmas
    SRV-->>API: 200 OK (ProfResumoDto)
    API-->>UI: ProfResumoDto
    
    Professor->>UI: preencherNovaQuestao(reinoId, faseId, enunciado, alternativas, gabarito)
    Professor->>UI: clicarSalvarQuestao()
    UI->>UI: validarCamposEGabarito()
    UI->>API: PostAsync<NovaQuestaoDto>("/fases/{id}/questoes", novaQuestao)
    API->>SRV: HTTP POST /fases/{id}/questoes
    SRV->>DB: INSERT INTO questoes (fase_id, enunciado, nivel_dificuldade) VALUES (?, ?, ?)
    DB-->>SRV: novaQuestaoId
    SRV->>DB: INSERT INTO alternativas (questao_id, letra, texto, correta) VALUES (?, ?, ?, ?)
    DB-->>SRV: alternativasInseridas
    SRV-->>API: 201 Created { id: novaQuestaoId }
    API-->>UI: Sucesso
    UI->>UI: atualizarGradeQuestoes()
    UI-->>Professor: Exibe Notificação: "Questão inserida com sucesso no Reino!"
```

#### Tabela de Mensagens — Tela 10
| Passo | Origem | Destino | Mensagem / Operação | Tipo | Descrição |
| :-: | :--- | :--- | :--- | :---: | :--- |
| 1-5 | `MainForm` | `Backend` | `GET /professor/resumo` | Assíncrono | Carrega estatísticas de adesão das turmas. |
| 6-7 | Professor | `UcProfessor` | `preencherNovaQuestao()`, `clicarSalvarQuestao()` | Síncrono | Elaboração pedagógica do desafio e do gabarito. |
| 8 | `UcProfessor` | `UcProfessor` | `validarCamposEGabarito()` | Auto-chamada | Confirma presença de pelo menos 1 alternativa correta. |
| 9-10 | `UcProfessor` | `Backend` | `POST /fases/{id}/questoes` | Assíncrono | Envio do payload com questão e alternativas. |
| 11-14 | `Backend` | `PostgreSQL` | `INSERT INTO questoes`, `INSERT INTO alternativas` | Síncrono | Gravação relacional em cascata no banco. |
| 15-18 | `UcProfessor` | Professor | `atualizarGradeQuestoes()` $\rightarrow$ Notificação | Retorno | Confirmação de inclusão do exercício no quiz dos alunos. |

---

### 5.11 Tela 11 — Painel de Governança do Administrador (`UcAdmin` / `admin.html`)

Painel executivo com poderes de gerenciamento de ciclo de vida de contas de usuários (alunos, professores, administradores), supervisão da infraestrutura e auditoria.

```mermaid
sequenceDiagram
    autonumber
    actor Admin as 🛡️ Administrador
    participant MF as 🖼️ MF: MainForm
    participant UI as 🖥️ UI: UcAdmin
    participant API as 🌐 API: ApiService
    participant SRV as ⚙️ SRV: BackendAdminAPI
    participant DB as 🐘 DB: PostgreSQL_Tronus

    MF->>UI: carregarPainelAdmin()
    UI->>API: GetAsync<AdminOverviewDto>("/admin/overview")
    API->>SRV: HTTP GET /admin/overview [Bearer JWT]
    SRV->>DB: SELECT papel, COUNT(*) FROM usuarios GROUP BY papel
    DB-->>SRV: metricasUsuarios
    SRV-->>API: 200 OK (AdminOverviewDto)
    API-->>UI: AdminOverviewDto
    
    Admin->>UI: buscarUsuarios(filtro = "aluno")
    UI->>API: GetAsync<List<UsuarioDto>>("/admin/usuarios?papel=aluno")
    API->>SRV: HTTP GET /admin/usuarios?papel=aluno
    SRV->>DB: SELECT id, nome, email, papel, ativo, data_criacao FROM usuarios WHERE papel = 'aluno'
    DB-->>SRV: listaUsuarios
    SRV-->>API: 200 OK (List<UsuarioDto>)
    API-->>UI: listaUsuarios
    
    Admin->>UI: promoverUsuarioParaProfessor(usuarioId = 15)
    UI->>API: PutAsync("/admin/usuarios/15/role", { papel: "professor" })
    API->>SRV: HTTP PUT /admin/usuarios/15/role
    SRV->>DB: UPDATE usuarios SET papel = 'professor' WHERE id = 15
    DB-->>SRV: 1 linha atualizada
    SRV-->>API: 200 OK { message: "Papel atualizado" }
    API-->>UI: Sucesso
    UI->>UI: atualizarGridUsuarios()
    UI-->>Admin: Exibe Alerta: "Privilégio de Professor concedido!"
```

#### Tabela de Mensagens — Tela 11
| Passo | Origem | Destino | Mensagem / Operação | Tipo | Descrição |
| :-: | :--- | :--- | :--- | :---: | :--- |
| 1-5 | `MainForm` | `Backend` | `GET /admin/overview` | Assíncrono | Carrega contadores globais do sistema. |
| 6-10 | Admin | `Backend` | `GET /admin/usuarios?papel=aluno` | Assíncrono | Lista e filtra usuários por papel e status. |
| 11-13 | Admin | `Backend` | `PUT /admin/usuarios/15/role` | Assíncrono | Altera a função de um usuário para professor/admin. |
| 14-15 | `Backend` | `PostgreSQL` | `UPDATE usuarios SET papel = ?` | Síncrono | Persiste alteração de permissão na base de dados. |
| 16-18 | `UcAdmin` | Admin | `atualizarGridUsuarios()` $\rightarrow$ Notificação | Retorno | Confirmação em tela do comando administrativo. |

---

## 6. Integração e Visualização no Astah UML (`.asta` / `.astah`)

Todos os 11 Diagramas de Sequência modelados neste documento foram implementados com precisão no arquivo oficial de modelagem do projeto:

### 📂 Arquivos Disponíveis
* **Projeto Integrado Completo:**
  * `docs/Engenharia de software ágil aplicada.asta` (Formato Astah Nativo)
  * `docs/Engenharia de software ágil aplicada.astah` (Extensão alternativa requerida)
* **Projeto Específico de Diagramas de Sequência:**
  * `docs/DiagramasDeSequencia.asta`
  * `docs/DiagramasDeSequencia.astah`
  * `TronusDesktop/DiagramasDeSequencia.asta`
  * `TronusDesktop/DiagramasDeSequencia.astah`

### 🛠️ Como Abrir e Inspecionar no Astah
1. Inicie o aplicativo **Astah Community** ou **Astah UML**.
2. Acesse o menu: **`File` $\rightarrow$ `Open`** (ou `Ctrl + O`).
3. Selecione o arquivo `docs/Engenharia de software ágil aplicada.asta` (ou `docs/DiagramasDeSequencia.asta`).
4. Na árvore de estrutura do projeto (*Structure Tree*, à esquerda):
   - Expanda o nó raiz do projeto.
   - Navegue até a pasta de Diagramas de Sequência.
   - Você verá os **11 diagramas**:
     - `DS01 - Tela Landing (UcLanding)`
     - `DS02 - Tela Login (UcLogin)`
     - `DS03 - Tela Cadastro (UcRegister)`
     - `DS04 - Recuperar Senha (UcForgotPassword)`
     - `DS05 - Forja do Heroi (UcCharacter)`
     - `DS06 - Selecao de Reinos (UcReinos)`
     - `DS07 - Desafio e Quiz (UcQuiz)`
     - `DS08 - Dashboard do Aluno (UcDashboard)`
     - `DS09 - Ranking Academico (UcRanking)`
     - `DS10 - Painel do Professor (UcProfessor)`
     - `DS11 - Painel do Administrador (UcAdmin)`
5. Dê um duplo clique sobre qualquer diagrama para abri-lo na área de desenho visual do Astah!

### 📸 Como Exportar as Imagens para o Relatório do PIM III
Com o diagrama aberto no Astah:
1. Vá em: **`Tool` $\rightarrow$ `Export Image` $\rightarrow$ `Save Diagram as PNG/JPEG`**.
2. Salve com 300 DPI de qualidade para inserção na seção de Modelagem de Interação do relatório acadêmico (`docs/PIM_III_Tronus.docx`).

---

## 7. Matriz de Rastreabilidade (Casos de Uso vs. Telas vs. Diagramas)

A matriz abaixo estabelece a rastreabilidade bidirecional dos requisitos e casos de uso do Tronus com suas respectivas telas e diagramas de sequência:

| Caso de Uso do Sistema | Tela Responsável | Diagrama de Sequência Astah | Entidades de Banco Impactadas |
| :--- | :--- | :--- | :--- |
| **UC01: Efetuar Cadastro** | `UcRegister` (`register.html`) | `DS03 - Tela Cadastro` | `usuarios` |
| **UC02: Efetuar Login** | `UcLogin` (`login.html`) | `DS02 - Tela Login` | `usuarios`, `personagens` |
| **UC03: Recuperar Senha** | `UcForgotPassword` (`forgot_password.html`) | `DS04 - Recuperar Senha` | `usuarios` |
| **UC04: Forjar Personagem** | `UcCharacter` (`character.html`) | `DS05 - Forja do Heroi` | `personagens`, `progresso_fases` |
| **UC05: Explorar Mapa dos Reinos** | `UcReinos` (`reinos.html`) | `DS06 - Selecao de Reinos` | `reinos`, `fases`, `progresso_fases` |
| **UC06: Responder Desafio / Quiz** | `UcQuiz` | `DS07 - Desafio e Quiz` | `questoes`, `alternativas`, `respostas_tentativas` |
| **UC07: Avaliar e Pontuar Respostas** | `UcQuiz` (Trigger) | `DS07 - Desafio e Quiz` | `progresso_fases`, `personagens` |
| **UC08: Desbloquear Nova Fase** | `UcReinos` & `UcQuiz` | `DS06` e `DS07` | `fases`, `progresso_fases` |
| **UC09: Promover Título Nobiliárquico** | Banco de Dados / `UcQuiz` | `DS07` e `DS08` | `personagens`, `trg_progresso_atualiza_personagem` |
| **UC10: Visualizar Desempenho** | `UcDashboard` (`dashboard.html`) | `DS08 - Dashboard do Aluno` | `personagens`, `progresso_fases`, `reinos` |
| **UC11: Acompanhar Classificação** | `UcRanking` | `DS09 - Ranking Academico` | `personagens`, `usuarios` |
| **UC12: Gestão Pedagógica Docente** | `UcProfessor` (`professor.html`) | `DS10 - Painel do Professor` | `questoes`, `alternativas`, `fases` |
| **UC13: Governança Administrativa** | `UcAdmin` (`admin.html`) | `DS11 - Painel do Administrador` | `usuarios`, `reinos`, auditoria |

---

*Documento gerado em conformidade com os padrões da Associação Brasileira de Normas Técnicas (ABNT) e as diretrizes do PIM III — Universidade Paulista (UNIP).*
