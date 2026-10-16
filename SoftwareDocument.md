# Documentação do Software — Tronus Desktop

**Data de Atualização:** 16 de Outubro de 2026  
**Tecnologia:** C# (.NET 10.0 Windows Forms)  
**Objetivo:** Migração do sistema web acadêmico gamificado "Tronus" para uma aplicação Desktop nativa em C#, mantendo a identidade visual "Dark Fantasy" (RPG) original e operando no modelo **Thin Client** integrado à API REST centralizada e banco de dados **PostgreSQL (Supabase)**.

---

## 1. Arquitetura e Framework Visual Customizado
Devido à limitação visual do Windows Forms clássico, o projeto foi arquitetado não utilizando o *Designer* padrão para as telas base, mas sim componentes criados e desenhados nativamente via código (usando `System.Drawing.Drawing2D` e GDI+).

### 1.1 `Theme.cs`
Centraliza a paleta de cores globais do sistema:
- `Background`: Fundo escuro base (`#1A1A1A`)
- `CardBackground`: Preto semi-transparente para criar efeito *Glassmorphism* (`rgba(0, 0, 0, 0.5)`)
- `Gold`: Dourado nobre característico da marca (`#D4AF37`)
- `PrimaryText`: Pergaminho suave para leitura (`#F0E6C8`)
- `DimText`: Dourado ocre atenuado (`#A89060`)
- `ButtonPrimaryStart` & `ButtonPrimaryEnd`: Gradiente esmeralda e petróleo (`#1A6B4A` → `#1A4A6B`)
- `ButtonPrimaryHoverStart` & `HoverEnd`: Gradientes iluminados para feedback de interação
- `BorderColor`: Dourado translúcido a 30% (`rgba(212, 175, 55, 0.3)`)

### 1.2 `TronusButton.cs`
Componente que herda de `Button`:
- **Modificação:** O método `OnPaint` foi reescrito para desenhar bordas arredondadas com `GraphicsPath` e aplicar preenchimento em gradiente linear.
- **Interatividade:** Implementa detecção de mouse para alternar as cores do gradiente quando o usuário passa o mouse por cima (*Hover effect*), alterando também o cursor para `Cursors.Hand`.

### 1.3 `TronusPanel.cs`
Componente que herda de `Panel`:
- **Modificação:** O método `OnPaint` recria o efeito dos cards estilizados translúcidos, possuindo fundo escuro semi-transparente, cantos arredondados e borda dourada fina de 1 pixel.

---

## 2. Janela Base (A Casca)

### 2.1 `MainForm.cs`
O ponto central do programa (A *Single Page Application* adaptada para o desktop):
- **Sem Bordas:** A propriedade `FormBorderStyle` é configurada para `None`, removendo a barra de título padrão do Windows e garantindo visual imersivo *Dark Fantasy*.
- **Auto-Ajuste Inteligente ao Iniciar (`AutoAjustarAoIniciar`):**
  - O sistema inspeciona as dimensões da área de trabalho do usuário (`Screen.PrimaryScreen.WorkingArea`).
  - **Monitores Compactos (Notebooks ≤ 1366×768 ou telas HD):** Inicia automaticamente em modo maximizado inteligente, garantindo que nenhum botão ou rodapé seja encoberto e que a barra de tarefas do Windows continue visível e acessível.
  - **Monitores Full HD, 2K ou 4K:** Inicia em modo janela balanceado (ex: 1200×800 ou 75% da resolução) perfeitamente centralizado.
- **Barra de Título Customizada (TopBar):**
  - Título estilizado em dourado ("TRONUS — O Caminho do Herói Acadêmico").
  - Três botões de controle de janela com efeitos no hover:
    - **Minimizar (`—`):** Reduz o aplicativo para a barra de tarefas do Windows.
    - **Maximizar / Restaurar (`🗖` / `🗗`):** Alterna suavemente entre o modo janela e o modo maximizado. O ícone se altera automaticamente conforme o estado.
    - **Fechar (`✕`):** Encerra a aplicação com highlight vermelho de perigo.
  - Suporta clique duplo na barra de título para alternar entre maximizar e restaurar.
  - Implementa eventos (`MouseDown`, `MouseMove`, `MouseUp`) para permitir arrastar a janela livremente em modo normal.
- **Prevenção de Sobreposição da Barra de Tarefas (Win32 `WM_GETMINMAXINFO`):**
  - Formulários sem borda padrão cobrem a barra de tarefas do Windows ao serem maximizados. O Tronus Desktop intercepta a mensagem `WM_GETMINMAXINFO` (código `0x24`) no `WndProc`, restringindo as coordenadas máximas exatamente à `WorkingArea` do monitor ativo.
- **Redimensionamento Manual por Bordas (Win32 `WM_NCHITTEST`):**
  - Permite que o usuário redimensione o formulário livremente arrastando as 4 bordas e os 4 cantos da janela através da interceptação de `WM_NCHITTEST` (código `0x84`).
- **Navegador SPA (`LoadControl`):** Possui um `contentPanel` com preenchimento total (`Dock = DockStyle.Fill`). O método `LoadControl(UserControl uc)` limpa os controles atuais e anexa a nova visualização solicitada, garantindo navegação suave sem piscar ou recriar janelas.

---

## 3. Telas do Sistema (UserControls)
Todas as telas do aplicativo são construídas como `UserControls` injetáveis no `MainForm`:

### 3.1 `UcLanding.cs` (Tela Inicial / Hero)
- Reproduz a experiência da *Hero Section* da aplicação web.
- Carrega imagem de fundo `img/hero.png` com gradiente escuro dinâmico via GDI+, repintando fluidamente ao redimensionar a janela (`Invalidate` no `Resize`).
- Logotipo copiado automaticamente pela build (`img/logo.png`).
- Apresenta os botões "CADASTRE-SE AGORA →" e "JÁ TENHO CONTA" instanciados com `TronusButton`, direcionando para `UcRegister` e `UcLogin`.

### 3.2 `UcLogin.cs` (Autenticação)
- Utiliza a imagem `img/fundo_reinos.jpg` como fundo esticado para ambientação.
- Cartão central flutuante (`TronusPanel`) com auto-centralização dinâmica (`CenterCard` no evento `Resize`), mantendo alinhamento perfeito em qualquer resolução (HD, Full HD ou 4K).
- Campos de texto para Identificador (usuário ou e-mail) e Senha mascarada (`PasswordChar = '•'`).
- Suporta submissão direta ao pressionar a tecla `Enter`.
- **Integração:** Comunica-se com o endpoint `POST /auth/login`. Ao obter sucesso, salva o token JWT no `SessionManager` e consulta `GET /personagens/me`:
  - Se retornar `404` (aluno sem personagem): Redireciona para `UcCharacter`.
  - Se retornar `200` (personagem já cadastrado): Redireciona diretamente para `UcReinos`.
- Botão "Voltar" funcional retornando à `UcLanding`.

### 3.3 `UcRegister.cs` (Cadastro de Aluno)
- Fundo contínuo com `img/fundo_reinos.jpg`.
- Painel central estilizado com auto-centralização dinâmica (`CenterCard` no evento `Resize`), prevenindo cortes em telas menores.
- Campos com validações de Nome, Celular, E-mail, Senha e Confirmação de Senha.
- **Validações:** Valida campos obrigatórios, formato de e-mail e conferência de senhas idênticas.
- **Integração:** Envia os dados para `POST /auth/register`. Em seguida, realiza o login automático do aluno e o encaminha para a criação de personagem em `UcCharacter`.

### 3.4 `UcCharacter.cs` (Criação de Personagem)
- Cartão central responsivo (`CenterCard`) contendo o fluxo multi-etapas (*Wizard*):
  - **Passo 1:** Escolha de Gênero (Masculino / Feminino).
  - **Passo 2:** Escolha do Nome do Herói com validação de campo vazio.
  - **Passo 3:** Seleção dinâmica do Avatar (carrega imagens `masc1.png` a `masc3.png` ou `fem1.png` a `fem3.png` da pasta `img`).
  - **Passo 4:** Ficha final com preview do herói e botão para "Entrar no Reino".
- **Integração:** Envia os dados para `POST /personagens` (`nome`, `genero`, `avatar`), persistindo o personagem no PostgreSQL e avançando para `UcReinos`.

### 3.5 `UcReinos.cs` (Mapa dos Reinos)
- Tela principal da jornada acadêmica do aluno no Desktop, com layout 100% responsivo.
- **HUD Superior do Aluno (Player Topbar):**
  - Acoplado ao topo (`DockStyle.Top`) preenchendo toda a largura da tela.
  - Avatar do Herói: Miniatura carregada dinamicamente com base no arquivo salvo (`masc1..3` ou `fem1..3`).
  - Nome e Insígnia do Título em destaque dourado (`🛡️ Plebeu`, `🎖️ Cavaleiro`, `🦅 Duque`, `👑 Rei`).
  - Contador de Estrelas Total (`⭐ X estrelas`).
  - Barra de Progresso Visual proporcional à meta do próximo título (`X / 15 para Cavaleiro`, `X / 30 para Duque`, `X / 50 para Rei`).
  - Ações no cabeçalho dinamicamente alinhadas à direita: botão "Meu Desempenho 📊" e botão "Sair" (Logout com confirmação).
- **Integração:** Consome simultaneamente `GET /personagens/me`, `GET /reinos` e `GET /personagens/me/progresso`.
- **Cards dos Reinos Fluidos (`flowReinos`):**
  - `FlowLayoutPanel` com auto-ajuste de largura e altura no `Resize`, distribuindo os cards em múltiplas colunas automaticamente (adaptando-se de 2 a 5 colunas dependendo se a janela está maximizada ou restaurada).
  - Imagens temáticas de cada disciplina (`img/python.png`, `img/c++.png`, `img/soft.png`, `img/bd.png`, `img/redes.png`).
  - Badge de status: "DISPONÍVEL" (verde) ou "EM BREVE".
  - Fases com estrelas conquistadas (`★`) e pendentes (`☆`).
  - Regra de desbloqueio sequencial e botão de ação direta para o `UcQuiz`.

### 3.6 `UcDashboard.cs` (Painel de Desempenho)
- Painel de análise individual do aluno com dimensionamento fluido de cards e botão voltar alinhado à direita.
- Apresenta métricas consolidadas, total de estrelas acumuladas, título honorífico (Plebeu a Rei) e taxa percentual de acertos.
- Botão "Voltar ao Mapa" para retorno ágil a `UcReinos`.

### 3.7 `UcQuiz.cs` (Desafio / Quiz da Fase)
- Núcleo da experiência gamificada de aprendizado do Tronus no Desktop.
- **Layout Responsivo Inteligente (`AdjustLayout`):** Card de questões (`questionCard`), caixa do enunciado e alternativas que se expandem dinamicamente acompanhando o tamanho da janela, com botões de navegação reposicionados automaticamente no rodapé do card.
- **Carregamento Seguro:** Consome `GET /fases/{id}/questoes` sem expor a alternativa correta na memória inicial.
- **Interface e Navegação:** Enunciado com rolagem dedicada, badge de dificuldade, botões de alternativas (A a E) com realce visual em dourado/verde esmeralda, e navegação sequencial com botões "Anterior" e "Próxima".
- **Submissão e Regras:** Na 5ª questão, o botão avança para "FINALIZAR ⚔". O sistema envia `POST /fases/{id}/responder` contendo `{ questaoId, alternativaId }`.
- **Tela de Conquista / Resultados:** Cartão de resultado centralizado dinamicamente (`resultCard`), exibindo estrelas (`★`/`☆`), taxa percentual de acerto, comparação com a melhor pontuação da fase, título atualizado e retorno ao mapa.

---

## 4. Camada de Integração e Comunicação (Thin Client + API REST)
A arquitetura do Tronus Desktop evoluiu para o modelo **Thin Client**. O aplicativo desktop não se conecta diretamente ao banco de dados via driver SQL, mas comunica-se de maneira segura com a API NestJS centralizada na nuvem (`https://king-blue-nine.vercel.app`), que por sua vez acessa o banco relacional PostgreSQL (Supabase).

### 4.1 `ApiService.cs` (Singleton HTTP)
Centraliza todas as requisições HTTP do sistema utilizando `System.Net.Http.Json`:
- **BaseAddress:** `https://king-blue-nine.vercel.app`
- **Cabeçalho de Autorização Automático:** Injeta `Authorization: Bearer <token>` sempre que `SessionManager.IsLoggedIn` for verdadeiro.
- **Métodos Genéricos:** `GetAsync<T>`, `PostAsync<TRequest, TResponse>`, `PostAsync<TRequest>` e `DeleteAsync`.
- **Tratamento de Exceções (`ApiException`):** Captura retornos de erro (`400`, `404`, etc.), deserializa mensagens enviadas pela API (`ApiErrorResponse`) e dispara exceções tratadas na camada visual com `MessageBox`.
- **Interceptação de Sessão Expirada (`401`):** Se a API retornar `401 Unauthorized` (token expirou após 1h), o `ApiService` automaticamente executa `SessionManager.Logout()`.

### 4.2 `SessionManager.cs` (Estado de Sessão em Memória)
Mantém as credenciais e o perfil do usuário logado durante o ciclo de vida da aplicação:
- `AccessToken`: Token JWT emitido pela API.
- `CurrentUser`: Objeto `UsuarioDto` contendo `id`, `nome`, `email` e `papel`.
- `IsLoggedIn`: Propriedade booleana que verifica a existência de sessão ativa.
- Métodos `Login(AuthResponse)` e `Logout()`.

### 4.3 Modelos de Dados (DTOs em `Models/`)
- `AuthModels.cs`: `LoginRequest`, `RegisterRequest`, `ForgotPasswordRequest`, `ResetPasswordRequest`, `AuthResponse`, `UsuarioDto`.
- `PersonagemModels.cs`: `CriarPersonagemRequest`, `PersonagemDto`.
- `ReinoModels.cs`: `ReinoDto`, `ProgressoFaseDto`.
- `DashboardModels.cs`: DTOs de resumo, histórico de respostas e métricas por reino.
- `QuizModels.cs`: `QuestaoDto`, `AlternativaDto`, `EnviarRespostasRequest`, `ItemRespostaRequest`, `ResultadoQuizResponse`, `ItemCorrecaoDto`.
- `ApiErrorResponse.cs`: Formato padrão de erro do NestJS (`statusCode`, `message`, `error`).

---

## 5. Banco de Dados e Backend (PostgreSQL no Supabase)
- **Persistência Centralizada:** As 8 tabelas do sistema estão hospedadas no **Supabase (PostgreSQL)** com Row Level Security (RLS) habilitado.
- **Regras no Banco:** As triggers do PostgreSQL (`04_triggers.sql`) e procedures (`03_procedures.sql`) gerenciam o progresso do aluno, calculam a pontuação máxima já obtida (`GREATEST`) e atualizam automaticamente o título do herói:
  - **Plebeu:** 0 a 14 estrelas
  - **Cavaleiro:** 15 a 29 estrelas
  - **Duque:** 30 a 44 estrelas
  - **Rei:** 45+ estrelas (domínio total)

---

## 6. Como Compilar e Executar

### Pré-requisitos:
- .NET SDK 10.0 instalado.
- Conexão à internet (para comunicação com a API).

### Executando em Modo Desenvolvimento:
```bash
dotnet run --project TronusDesktop/TronusDesktop.csproj
```

### Compilando a Solução:
```bash
dotnet build TronusDesktop/TronusDesktop.csproj
```

### Gerando o Executável para Distribuição (Publish):
```bash
dotnet publish TronusDesktop/TronusDesktop.csproj -c Release -r win-x64 --self-contained false
```
O executável final é gerado em `TronusDesktop/bin/Release/net10.0-windows/win-x64/publish/TronusDesktop.exe`.

---

## 7. Status do Roadmap de Desenvolvimento

- [x] Arquitetura base SPA com `MainForm` e `LoadControl`.
- [x] Framework visual customizado (`Theme`, `TronusButton`, `TronusPanel`).
- [x] Auto-ajuste de janela inteligente por resolução (`AutoAjustarAoIniciar`), tela cheia sem cobrir a barra de tarefas (`WM_GETMINMAXINFO`) e controles de janela na barra de título (Minimizar, Maximizar/Restaurar, Fechar).
- [x] Redimensionamento fluido por arrasto de bordas (`WM_NCHITTEST`) e controles responsivos (`CenterCard` e `AdjustLayout`).
- [x] `UcLanding.cs` com arte medieval e redirecionamento.
- [x] `UcLogin.cs` integrado à API REST e validação de sessão.
- [x] `UcRegister.cs` integrado com criação de conta e auto-login.
- [x] `UcCharacter.cs` com fluxo wizard e cadastro de herói na API.
- [x] `UcReinos.cs` com HUD superior completo, carregamento dinâmico dos 5 reinos e estrelas.
- [x] `UcQuiz.cs` com jogabilidade de desafios, submissão de respostas e tela de resultados.
- [x] `UcDashboard.cs` com visualização de desempenho.
- [x] Camada de serviços HTTP (`ApiService`, `SessionManager`, `ApiException`).
- [x] Modelo Thin Client conectado à API NestJS e PostgreSQL Supabase.
- [x] Exclusão de redundâncias no `.csproj` para compilação limpa com 0 erros.
