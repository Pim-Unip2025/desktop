# Documentação do Software — Tronus Desktop

**Data de Atualização:** 22 de Setembro de 2026
**Tecnologia:** C# (.NET 8.0 Windows Forms)
**Objetivo:** Migração do sistema web acadêmico gamificado "Tronus" para uma aplicação Desktop nativa, mantendo a identidade visual "Dark Fantasy" (RPG) original.

---

## 1. Arquitetura e Framework Visual Customizado
Devido à limitação visual do Windows Forms clássico, o projeto foi arquitetado não utilizando o *Designer* padrão para as telas base, mas sim componentes criados e desenhados nativamente via código (usando `System.Drawing.Drawing2D`).

### 1.1 `Theme.cs`
Centraliza a paleta de cores globais do sistema:
- `Background`: Fundo escuro base (`#1A1A1A`)
- `CardBackground`: Preto semi-transparente para criar efeito *Glassmorphism*
- `Gold`: Dourado característico da marca
- `ButtonPrimary`: Gradientes do Verde esmeralda (normal e estado *hover*)

### 1.2 `TronusButton.cs`
Componente que herda de `Button`.
- **Modificação:** O `OnPaint` foi reescrito para desenhar bordas arredondadas e aplicar preenchimento em gradiente.
- **Interatividade:** Implementa detecção de mouse para alternar as cores do gradiente quando o usuário passa o mouse por cima (*Hover effect*).

### 1.3 `TronusPanel.cs`
Componente que herda de `Panel`.
- **Modificação:** O `OnPaint` recria o efeito dos "cards estilo Riot" originais, possuindo fundo translúcido escuro, bordas arredondadas e um contorno fino de 1 pixel levemente esbranquiçado.

---

## 2. Janela Base (A Casca)

### 2.1 `MainForm.cs`
O ponto central do programa (A *Single Page Application* adaptada para o desktop).
- **Sem Bordas:** A propriedade `FormBorderStyle` foi configurada para `None`, removendo a barra de título cinza do Windows.
- **Barra de Título Customizada:** Foi criada uma `topBar` contendo o nome do software em dourado e um botão de fechar customizado.
- **Arrastável:** Implementados eventos (`MouseDown`, `MouseMove`, `MouseUp`) na barra de título para permitir que o usuário movimente a janela normalmente.
- **Navegador (LoadControl):** Possui um `contentPanel` responsável por hospedar os `UserControls`. O método `LoadControl()` destrói a visualização atual e anexa a próxima página (ex: ao ir do Login para o Cadastro).

---

## 3. Telas (UserControls)
Todas as telas do aplicativo são construídas como `UserControls` injetáveis no `MainForm`.

### 3.1 `UcLanding.cs` (Tela Inicial)
- Reproduz a experiência da *Hero Section* da web.
- Carrega a logomarca via arquivo físico copiado automaticamente pela *build* (`img/logo.png`).
- Apresenta botões instanciados via `TronusButton` redirecionando para Login e Cadastro.

### 3.2 `UcLogin.cs` (Tela de Acesso)
- Utiliza a imagem esticada `img/fundo_reinos.jpg` como `BackgroundImage` do controle para ambientação.
- Cria um cartão central flutuante (`TronusPanel`) contendo campos de texto padronizados e mascarados (`PasswordChar = '•'`).
- Integra uma função de "Voltar" (através de um Label clicável) que invoca o `LoadControl` no `MainForm` para regressar à tela `UcLanding`.

### 3.3 `UcRegister.cs` (Tela de Cadastro)
- Utiliza a mesma imagem esticada `img/fundo_reinos.jpg` como fundo, criando continuidade visual.
- Painel central estendido contendo as validações de Nome, E-mail, Senha e Confirmação de Senha.
- Regra de negócio simples validando senhas divergentes.
- Botão "Voltar" funcional.

---

### 3.4 `UcCharacter.cs` (Criação de Personagem)
- Implementa um fluxo multi-etapas (*Wizard*) em uma única tela, controlando a visibilidade de Paineis Internos (`step1Panel` a `step4Panel`).
- **Passo 1:** Escolha de Gênero (Masculino / Feminino).
- **Passo 2:** Escolha do Nome do Personagem.
- **Passo 3:** Seleção do Avatar (carrega dinamicamente imagens `masc1.png` a `masc3.png` ou equivalentes femininas da pasta `img`).
- **Passo 4:** Ficha final e botão para Entrar no Reino.

---

---

## 4. Guia de Integração para a Equipe de Backend (Banco de Dados)
Como a persistência de dados está separada da camada visual (UX/UI), a equipe responsável pela implementação do Banco de Dados deve seguir estas diretrizes para "plugar" a regra de negócio na interface em C# (WinForms):

### 4.1 Instalação do SQLite
Recomendamos o uso da biblioteca nativa da Microsoft para SQLite. 
No console de gerenciador de pacotes (ou terminal), execute:
`dotnet add package Microsoft.Data.Sqlite`

### 4.2 Camada de Dados (DAL)
Crie uma classe estática ou Singleton (ex: `DatabaseHelper.cs`) na raiz do projeto ou em uma pasta `Data/` para abstrair as queries SQL (INSERT, SELECT, UPDATE). As tabelas originais devem ser mapeadas (Usuarios, Personagens, Reinos, etc.).

### 4.3 Onde "Plugar" o Backend nas Telas
Todas as telas já possuem marcações com comentários `// TODO:` para facilitar a identificação dos eventos de botões.
1. **Cadastro (`UcRegister.cs`)**: No método `BtnSubmit_Click`, os dados visuais já estão validados. O backend deve pegar as strings (ex: `emailBox.Text`), invocar o `DatabaseHelper.CadastrarUsuario(...)` e, em caso de sucesso, permitir que a linha `LoadControl(new UcCharacter())` seja executada.
2. **Login (`UcLogin.cs`)**: No método `BtnSubmit_Click`, invoque a consulta ao banco de dados utilizando `emailBox.Text` e `passBox.Text`. Se o usuário existir e a senha bater, realize o redirecionamento.
3. **Personagem (`UcCharacter.cs`)**: No método `PopulateStep4()`, dentro do evento `btnPlay.Click`, o sistema deve realizar o `INSERT INTO personagens...` utilizando as variáveis globais da classe (`characterName`, `selectedGender`, `selectedAvatar`) antes de avançar para a tela de Reinos.

---

## Próximas Atualizações (Roadmap)
- `UcReinos.cs` e fluxos gamificados (Dashboard, Reinos, Quizzes).
- Efetivação do Guia de Integração (Backend conectando as APIs ao código comentado).
