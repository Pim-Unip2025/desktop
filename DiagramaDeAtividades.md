# 🔄 Diagrama de Atividades (UML) — Tronus Desktop

**Projeto Integrado Multidisciplinar (PIM III) — UNIP 2026**  
**Curso:** Superior de Tecnologia em Análise e Desenvolvimento de Sistemas (ADS)  
**Disciplina de Referência:** Engenharia de Software Ágil Aplicada  
**Plataforma de Aplicação:** Tronus Desktop (.NET 10 / C# Windows Forms)  
**Arquivos Modelados (`.asta` e `.astah`):**
- `docs/DiagramasDeAtividades.asta` / `docs/DiagramasDeAtividades.astah`
- `docs/Engenharia de software ágil aplicada.asta` (Pacote `03_DiagramasDeAtividades`)

---

## 📌 Sumário

1. [Visão Geral e Metodologia](#1-visão-geral-e-metodologia)
2. [Notação e Elementos da UML Adotados](#2-notação-e-elementos-da-uml-adotados)
3. [DA01 — Jornada Gamificada do Aluno (Resolução de Desafios e Evolução Nobiliárquica)](#3-da01--jornada-gamificada-do-aluno-resolução-de-desafios-e-evolução-nobiliárquica)
4. [DA02 — Resiliência Operacional e Fallback para o Modo Offline](#4-da02--resiliência-operacional-e-fallback-para-o-modo-offline)
5. [DA03 — Gestão Pedagógica de Questões pelo Professor (CRUD e Avaliação)](#5-da03--gestão-pedagógica-de-questões-pelo-professor-crud-e-avaliação)
6. [DA04 — Recuperação Segura de Credenciais (Wizard SMS em 3 Etapas)](#6-da04--recuperação-segura-de-credenciais-wizard-sms-em-3-etapas)
7. [DA05 — Governança Global e Gestão de Usuários pelo Administrador (RBAC)](#7-da05--governança-global-e-gestão-de-usuários-pelo-administrador-rbac)
8. [DA06 — Ciclo Universal de Acessibilidade, Temas WCAG AAA e Escala de Fonte](#8-da06--ciclo-universal-de-acessibilidade-temas-wcag-aaa-e-escala-de-fonte)
9. [Matriz de Rastreabilidade (Casos de Uso vs. Atividades vs. Telas)](#9-matriz-de-rastreabilidade-casos-de-uso-vs-atividades-vs-telas)

---

## 1. Visão Geral e Metodologia

O **Diagrama de Atividades** descreve o **fluxo de controle operacional e de dados** passo a passo dentro do sistema **Tronus**. Cada diagrama segrega as ações em **Raias de Responsabilidade (*Swimlanes / Partitions*)**, delimitando claramente o que é executado pelo **Ator Humano**, pelo cliente **Tronus Desktop (.NET 10 / C#)** e pelo **Backend API / PostgreSQL / Motor Offline**.

---

## 2. Notação e Elementos da UML Adotados

| Elemento Gráfico | Nome UML | Significado e Função no Tronus Desktop |
| :---: | :---: | :--- |
| 🟢 `([Início])` | **Nó Inicial (Initial Node)** | Ponto de partida do fluxo operacional na raia do ator iniciador. |
| 🔲 `[Ação]` | **Nó de Ação (Action Node)** | Execução de um passo atômico de trabalho posicionado na raia responsável. |
| 🔶 `{Decisão}` | **Nó de Decisão (Decision Node)** | Ponto de ramificação baseado em condições de guarda (`[Guard]`). |
| 🛑 `([Fim])` | **Nó Final de Atividade (Final Node)** | Encerramento do processo em execução. |
| 🏊 `Raias` | **Partições (Swimlanes)** | Colunas verticais que isolam as responsabilidades de cada ator ou camada arquitetural. |

---

## 3. DA01 — Jornada Gamificada do Aluno (Resolução de Desafios e Evolução Nobiliárquica)

Representa o fluxo nuclear da experiência do estudante: exploração do mapa curricular (`UcReinos`), resolução das 5 questões da fase (`UcQuiz`), cálculo de estrelas com preservação da maior nota histórica (`GREATEST`) e promoção automática de patentes nobiliárquicas (*Plebeu*, *Cavaleiro*, *Duque* e *Rei*).

### 3.1 Imagem Exportada do Astah UML (`DA01`)

![DA01 - Jornada Gamificada do Aluno](IMG/DiagramaDeAtividades/DA01%20-%20Jornada%20Gamificada%20do%20Aluno.png)

### 3.2 Fluxo Estruturado com Raias (Mermaid)

```mermaid
flowchart TD
    subgraph Aluno [Raia 1: Aluno / Estudante]
        A_Start([Início]) --> A_Map[Explora Mapa dos Reinos UcReinos]
        A_Map --> A_Play[Clica em 'Jogar' na Fase Liberada]
        A_Read[Lê enunciado e seleciona alternativa] --> A_Dec5{Questão < 5?}
        A_Dec5 -- "[Questão < 5]" --> A_Next[Clica em 'Próxima Questão']
        A_Next --> A_Read
        A_Dec5 -- "[5ª Questão]" --> A_Fin[Clica em 'FINALIZAR DESAFIO']
        A_Vict[Visualiza estrelas conquistadas e patente] --> A_Dash[Consulta evolução no Dashboard / Mapa]
        A_Dash --> A_End([Fim])
    end

    subgraph Desktop [Raia 2: Tronus Desktop .NET 10 / C#]
        D_Load[UcQuiz solicita GET /fases/id/questoes]
        D_Rend[Renderiza enunciado, alternativas A-D e timer]
        D_Post[Empacota respostas e envia POST /fases/id/responder]
        D_Vict[Exibe Cartão de Vitória e desbloqueia próxima fase]
    end

    subgraph Backend [Raia 3: API REST / PostgreSQL / Motor Offline]
        B_GetQ[Consulta 5 questões da fase sem expor gabarito]
        B_Corr[Confere gabarito e calcula estrelas 0 a 3★]
        B_Save[Salva maior nota GREATEST e soma TotalEstrelas]
        B_DecT{TotalEstrelas?}
        B_Rei[Título: Rei >= 45★]
        B_Duq[Título: Duque 30..44★]
        B_Cav[Título: Cavaleiro 15..29★]
        B_Ple[Título: Plebeu < 15★]
        B_Ret[Retorna ResultadoQuizResponse e PersonagemDto]
    end

    A_Play --> D_Load --> B_GetQ --> D_Rend --> A_Read
    A_Fin --> D_Post --> B_Corr --> B_Save --> B_DecT
    B_DecT -- "[>= 45★]" --> B_Rei --> B_Ret
    B_DecT -- "[30..44★]" --> B_Duq --> B_Ret
    B_DecT -- "[15..29★]" --> B_Cav --> B_Ret
    B_DecT -- "[< 15★]" --> B_Ple --> B_Ret
    B_Ret --> D_Vict --> A_Vict
```

---

## 4. DA02 — Resiliência Operacional e Fallback para o Modo Offline

Modela a tolerância a falhas de conectividade do cliente desktop, permitindo entrada direta em modo demonstração ou chaveamento automático assistido caso o servidor nuvem esteja inacessível.

### 4.1 Imagem Exportada do Astah UML (`DA02`)

![DA02 - Resiliência e Fallback Offline](IMG/DiagramaDeAtividades/DA02%20-%20Resiliencia%20e%20Fallback%20Offline.png)

### 4.2 Fluxo Estruturado com Raias (Mermaid)

```mermaid
flowchart TD
    subgraph Usuario [Raia 1: Usuário / Avaliador]
        U_Init([Início]) --> U_Open[Abre tela de autenticação UcLogin]
        U_Open --> U_DecIn{Modo de Entrada?}
        U_DecIn -- "[Clique Direto]" --> U_Dir[Clica em 'MODO OFFLINE DEMO']
        U_DecIn -- "[Login Padrão]" --> U_Cred[Informa e-mail/senha e clica em 'ENTRAR']
        U_DecFb{Ativar Offline?} -- "[Não]" --> U_Wait[Permanece na tela de Login] --> U_End1([Fim])
        U_Nav[Navega por Reinos, Quizzes, Ranking e Admin]
        U_End2([Fim])
    end

    subgraph Desktop [Raia 2: Tronus Desktop ApiService & MainForm]
        D_Http[Dispara POST /auth/login timeout 8s]
        D_Catch[Captura falha de rede / TimeoutException]
        D_Ask[Exibe diálogo oferecendo Modo Offline]
        D_Flag[Ativa ApiService.IsOfflineMode = true e Badge Dourado]
        D_Sess[SessionManager inicia sessão local e abre UcReinos]
    end

    subgraph Offline [Raia 3: OfflineDataService Motor em Memória]
        O_Seed[Carrega Herói, 5 Reinos, 15 Fases e 18 Usuários]
        O_Eval[Processa quizzes e atualiza estado em RAM]
    end

    U_Cred --> D_Http -- "[Sem Rede]" --> D_Catch --> D_Ask --> U_DecFb
    U_DecFb -- "[Sim]" --> D_Flag
    U_Dir --> D_Flag
    D_Flag --> O_Seed --> D_Sess --> U_Nav --> O_Eval --> U_End2
```

---

## 5. DA03 — Gestão Pedagógica de Questões pelo Professor (CRUD e Avaliação)

Modela as atividades do corpo docente (Requisitos RF11 e RF12) no `UcProfessor`, cobrindo a criação e exclusão de questões pedagógicas e o monitoramento do desempenho dos estudantes da disciplina.

### 5.1 Imagem Exportada do Astah UML (`DA03`)

![DA03 - Gestão Pedagógica Docente](IMG/DiagramaDeAtividades/DA03%20-%20Gestao%20Pedagogica%20Docente.png)

### 5.2 Fluxo Estruturado com Raias (Mermaid)

```mermaid
flowchart TD
    subgraph Professor [Raia 1: Professor / Docente]
        P_Init([Início]) --> P_Login[Autentica-se com perfil Professor]
        P_Dec{Operação Docente?}
        P_New[Preenche Nova Questão, Alternativas e Gabarito]
        P_Del[Clica em 'Excluir' no card e confirma remoção]
        P_Alu[Acessa aba Alunos & Desempenho e filtra busca]
        P_End([Fim])
    end

    subgraph Desktop [Raia 2: Tronus Desktop UcProfessor]
        V_Reino[Carrega UcProfessor e identifica Reino vinculado]
        V_Post[Valida campos e envia POST /fases/id/questoes]
        V_Del[Dispara DELETE /questoes/id]
        V_Filt[Filtra tabela por nome, e-mail, estrelas e % acerto]
        V_Sync[Atualiza cards de questões e indicadores na tela]
    end

    subgraph Backend [Raia 3: API Central & Banco PostgreSQL]
        B_Ins[Valida RBAC docente e grava questão + alternativas]
        B_Del[Remove questão e alternativas em cascata]
        B_Aggr[Retorna métricas consolidadas da turma]
    end

    P_Login --> V_Reino --> P_Dec
    P_Dec -- "[Nova Questão]" --> P_New --> V_Post --> B_Ins --> V_Sync
    P_Dec -- "[Excluir Questão]" --> P_Del --> V_Del --> B_Del --> V_Sync
    P_Dec -- "[Monitorar Alunos]" --> P_Alu --> V_Filt --> B_Aggr --> V_Sync
    V_Sync --> P_End
```

---

## 6. DA04 — Recuperação Segura de Credenciais (Wizard SMS em 3 Etapas)

Modela o fluxo do Requisito RF10 (`UcForgotPassword`) para redefinição segura de senha em três etapas verificáveis.

### 6.1 Imagem Exportada do Astah UML (`DA04`)

![DA04 - Recuperação Segura de Credenciais](IMG/DiagramaDeAtividades/DA04%20-%20Recuperacao%20Segura%20de%20Credenciais.png)

### 6.2 Fluxo Estruturado com Raias (Mermaid)

```mermaid
flowchart TD
    subgraph Usuario [Raia 1: Usuário / Estudante]
        U4_Init([Início]) --> U4_Fgt[Clica em 'Esqueci a senha' e informa celular]
        U4_Code[Digita código de 6 dígitos e clica em 'VERIFICAR']
        U4_Pass[Etapa 3: Digita Nova Senha e Confirmação]
        U4_Done[Retorna pronto para autenticar no UcLogin] --> U4_End([Fim])
    end

    subgraph Wizard [Raia 2: Wizard Desktop UcForgotPassword]
        W4_Gen[Valida celular Regex e gera código SMS 6 dígitos]
        W4_Ban[Exibe banner de homologação TRONUS-XXXXXX]
        W4_DecC{Código Confere?}
        W4_Eye[PasswordToggleButton alterna visibilidade e valida >= 8 chars]
        W4_DecP{Senha Válida?}
        W4_Post[Envia POST /auth/reset-password]
        W4_Rout[Exibe confirmação e redireciona para UcLogin]
    end

    subgraph Backend [Raia 3: API Auth & PostgreSQL]
        B4_Upd[Gera hash BCrypt e executa UPDATE usuarios]
    end

    U4_Fgt --> W4_Gen --> W4_Ban --> U4_Code --> W4_DecC
    W4_DecC -- "[Código Incorreto]" --> U4_Code
    W4_DecC -- "[Código Válido]" --> U4_Pass --> W4_Eye --> W4_DecP
    W4_DecP -- "[Senha Inválida]" --> U4_Pass
    W4_DecP -- "[Senha Válida]" --> W4_Post --> B4_Upd --> W4_Rout --> U4_Done
```

---

## 7. DA05 — Governança Global e Gestão de Usuários pelo Administrador (RBAC)

Modela as atividades executivas no `UcAdmin` (Requisito RF13): listagem completa de todos os cadastros ativos (Alunos, Professores e Administradores), indicadores dinâmicos (6 KPIs), filtro por papel, alternância de privilégios RBAC, criação de contas e blindagem contra exclusão da conta Admin Mestre.

### 7.1 Imagem Exportada do Astah UML (`DA05`)

![DA05 - Governança Global e RBAC](IMG/DiagramaDeAtividades/DA05%20-%20Governanca%20Global%20e%20RBAC.png)

### 7.2 Fluxo Estruturado com Raias (Mermaid)

```mermaid
flowchart TD
    subgraph Admin [Raia 1: Administrador do Sistema]
        Ad_Init([Início]) --> Ad_Open[Acessa Painel de Governança UcAdmin]
        Ad_Dec{Ação Administrativa?}
        Ad_Flt[Seleciona filtro: Todos, Alunos, Profs, Admins]
        Ad_Role[Clica em 'Papel' para promover usuário RBAC]
        Ad_Del[Clica em 'Excluir' na linha do usuário]
        Ad_New[Clica em '+ NOVO USUÁRIO' e preenche modal]
        Ad_End([Fim])
    end

    subgraph Painel [Raia 2: Painel Admin UcAdmin & ApiService]
        S5_Kpi[Sincroniza cadastros ativos e calcula 6 KPIs]
        S5_Flt[Filtra grid em tempo real sem piscar tela]
        S5_Role[Dispara PATCH /admin/usuarios/id/papel]
        S5_DecM{É Admin Mestre?}
        S5_Blk[Bloqueia exclusão de Conta Admin Mestre]
        S5_New[Valida dados e envia POST /admin/usuarios]
        S5_Ref[Recarrega tabela de cadastros ativos e KPIs]
    end

    subgraph Backend [Raia 3: API Central & PostgreSQL / Catálogo]
        B5_List[Retorna lista completa de Alunos, Profs e Admins]
        B5_Role[Atualiza papel aluno / professor / admin]
        B5_Del[Executa DELETE /admin/usuarios/id]
        B5_New[Persiste novo cadastro ativo e atualiza contadores]
    end

    Ad_Open --> S5_Kpi --> B5_List --> Ad_Dec
    Ad_Dec -- "[Filtrar Cadastros]" --> Ad_Flt --> S5_Flt --> S5_Ref
    Ad_Dec -- "[Alternar Papel]" --> Ad_Role --> S5_Role --> B5_Role --> S5_Ref
    Ad_Dec -- "[Excluir Conta]" --> Ad_Del --> S5_DecM
    S5_DecM -- "[Admin Mestre]" --> S5_Blk --> S5_Ref
    S5_DecM -- "[Conta Comum]" --> B5_Del --> S5_Ref
    Ad_Dec -- "[Novo Usuário]" --> Ad_New --> S5_New --> B5_New --> S5_Ref
    S5_Ref --> Ad_End
```

---

## 8. DA06 — Ciclo Universal de Acessibilidade, Temas WCAG AAA e Escala de Fonte

Modela os recursos universais de acessibilidade disponíveis na barra superior (`TopBar`) e no menu **`⚙️ Setup`**: alternância de contraste visual (Dark Fantasy vs. Parchment Claro WCAG AAA) e redimensionamento proporcional de fontes (`A-`, `100%`, `A+`).

### 8.1 Imagem Exportada do Astah UML (`DA06`)

![DA06 - Acessibilidade e Temas WCAG](IMG/DiagramaDeAtividades/DA06%20-%20Acessibilidade%20e%20Temas%20WCAG.png)

### 8.2 Fluxo Estruturado com Raias (Mermaid)

```mermaid
flowchart TD
    subgraph Usuario [Raia 1: Usuário em Qualquer Tela]
        U6_Init([Início]) --> U6_Dec{Recurso de Acessibilidade?}
        U6_Dec -- "[Alternar Tema]" --> U6_Thm[Aciona Botão de Tema ou Ctrl+T / Alt+A]
        U6_Dec -- "[Ajustar Fonte]" --> U6_Fnt[Abre Menu Setup e clica em A- / 100% / A+ ou Ctrl +/-]
        U6_Obs[Visualiza alto contraste e fontes ajustadas em tempo real] --> U6_End([Fim])
    end

    subgraph TopBar [Raia 2: TopBar & Menu Setup MainForm]
        M6_Tgl[AccessibilityThemeButton chama Theme.ToggleTheme]
        M6_Fnt[Menu Setup ajusta Theme.FontScale 85% a 145%]
        M6_Rep[MainForm e UserControl ativo repintam sem flicker]
    end

    subgraph Engine [Raia 3: Motor Gráfico Theme.cs & GDI+]
        T6_DecT{Modo Ativo?}
        T6_Dark[Ativa Paleta Dark Fantasy #1A1A1A / Ouro]
        T6_Lght[Ativa Paleta Parchment WCAG AAA #F4EEDB]
        T6_Scale[ApplyFontScaleToControl reescala fontes recursivamente]
        T6_Evt[Dispara ThemeChanged e invalida buffers DoubleBuffered]
    end

    U6_Thm --> M6_Tgl --> T6_DecT
    T6_DecT -- "[Dark Mode]" --> T6_Dark --> T6_Evt
    T6_DecT -- "[Light Mode]" --> T6_Lght --> T6_Evt
    U6_Fnt --> M6_Fnt --> T6_Scale --> T6_Evt
    T6_Evt --> M6_Rep --> U6_Obs
```

---

## 9. Matriz de Rastreabilidade (Casos de Uso vs. Atividades vs. Telas)

| Caso de Uso (UML) | Código do DA | Fluxo de Atividade | Componente Desktop | Regra de Negócio Associada |
| :--- | :---: | :--- | :--- | :--- |
| **UC01 / UC04: Cadastro e Forja** | `DA01` | Jornada Inicial do Estudante | `UcRegister.cs` / `UcCharacter.cs` | Unicidade de e-mail, escolha de avatar e patente inicial *Plebeu*. |
| **UC02: Efetuar Login** | `DA02` | Autenticação e Fallback Offline | `UcLogin.cs` / `ApiService.cs` | Roteamento por papel RBAC e chaveamento assistido para modo offline. |
| **UC03: Recuperar Senha** | `DA04` | Restauração de Senha em 3 Etapas | `UcForgotPassword.cs` | Validação de celular, código SMS de 6 dígitos e senha forte. |
| **UC05 / UC06 / UC07: Reinos e Quiz** | `DA01` | Resolução de Desafio e Evolução | `UcReinos.cs` / `UcQuiz.cs` | Desbloqueio sequencial (`Estrelas > 0`), `GREATEST` e títulos (15, 30, 45★). |
| **UC08 / UC09: Dashboard e Ranking** | `DA01` | Métricas e Hall dos Heróis | `UcDashboard.cs` / `UcRanking.cs` | Consolidação de precisão por reino e pódio classificatório. |
| **UC10 / UC11: Gestão Docente** | `DA03` | CRUD de Questões e Turmas | `UcProfessor.cs` | Cadastro/exclusão de questões por fase e busca instantânea de alunos. |
| **UC12: Governança Global** | `DA05` | Gestão de Cadastros Ativos e RBAC | `UcAdmin.cs` | Exibição de todos os cadastros ativos, 6 KPIs, filtro de papel e proteção Admin Mestre. |
| **UC13: Acessibilidade Universal** | `DA06` | Temas WCAG AAA e Escala de Fonte | `MainForm.cs` / `Theme.cs` | Contraste 11:1 e escala dinâmica de fonte (`85%` a `145%`) via menu **`⚙️ Setup`**. |
