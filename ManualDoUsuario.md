# ⚔️ Tronus Desktop — Manual do Usuário

**Versão da Aplicação:** 1.0.0  
**Data da Documentação:** Outubro de 2026  
**Público-Alvo:** Alunos, Professores e Avaliadores Acadêmicos (UNIP 2026)  
**Plataforma:** Windows 10 / 11 (64-bit)  

---

## 📌 Sumário

1. [Apresentação do Sistema](#1-apresentação-do-sistema)
2. [Requisitos e Como Iniciar](#2-requisitos-e-como-iniciar)
3. [Interface e Controles da Janela](#3-interface-e-controles-da-janela)
4. [Primeiro Acesso: Cadastro de Usuário](#4-primeiro-acesso-cadastro-de-usuário)
5. [Criação do Personagem (Wizard do Herói)](#5-criação-do-personagem-wizard-do-herói)
6. [Autenticação e Login](#6-autenticação-e-login)
7. [Mapa dos Reinos (Tela Principal do Aluno)](#7-mapa-dos-reinos-tela-principal-do-aluno)
8. [Desafios e Quizzes Acadêmicos](#8-desafios-e-quizzes-acadêmicos)
9. [Resultado da Conquista e Progressão](#9-resultado-da-conquista-e-progressão)
10. [Painel de Desempenho (Dashboard)](#10-painel-de-desempenho-dashboard)
11. [Logout e Troca de Perfil](#11-logout-e-troca-de-perfil)
12. [Perguntas Frequentes e Solução de Problemas](#12-perguntas-frequentes-e-solução-de-problemas)

---

## 1. Apresentação do Sistema

O **Tronus Desktop** é uma aplicação nativa para Windows desenvolvida em C# (.NET 10) que transforma o aprendizado das disciplinas de Ciência da Computação em uma jornada épica no estilo **Dark Fantasy / RPG Medieval**.

### 🌟 O Conceito: "O Caminho do Herói Acadêmico"
Cada matéria da grade curricular representa um **Reino** independente:
* 🐍 **Python:** O Reino dos Códigos Serpenteantes
* ⚙️ **C++:** A Fortaleza de Alto Desempenho e Ponteiros
* 📐 **Engenharia de Software:** O Bastião dos Padrões e Arquitetura
* 🗄️ **Banco de Dados:** As Minas do Conhecimento Estruturado e Relacional
* 🌐 **Redes de Computadores:** As Rotas e Conexões do Império

O objetivo do aluno é responder a desafios teóricos e práticos (quizzes) de cada fase, acumulando **Estrelas de Conquista** para evoluir na nobreza:
* 🛡️ **Plebeu:** 0 a 14 estrelas (Iniciante)
* 🎖️ **Cavaleiro:** 15 a 29 estrelas (Guerreiro experiente)
* 🦅 **Duque:** 30 a 44 estrelas (Lorde do conhecimento)
* 👑 **Rei:** 45+ estrelas (Dominador de todos os reinos)

---

## 2. Requisitos e Como Iniciar

### 🖥️ Requisitos Mínimos do Sistema
* **Sistema Operacional:** Windows 10 (Build 19041+) ou Windows 11 de 64-bit.
* **Resolução de Tela:** Mínimo de 1024×728 pixels (ideal: 1366×768 ou 1920×1080).
* **Conexão:** Acesso estável à Internet (para sincronização com a API em nuvem e banco de dados Supabase).
* **Ambiente .NET:** .NET Runtime 10.0 instalado (ou executar o pacote autônomo publicado).

### 🚀 Executando o Aplicativo
1. **Pelo Executável:** Dê um duplo clique no arquivo `TronusDesktop.exe`.
2. **Pelo Terminal (.NET SDK):**
   ```bash
   dotnet run --project TronusDesktop/TronusDesktop.csproj
   ```

---

## 3. Interface e Controles da Janela

O Tronus Desktop possui uma interface imersiva sem as bordas cinzas do Windows clássico, trazendo acabamento estilizado com molduras douradas e painéis translúcidos (*Glassmorphism*).

### 📐 Auto-Ajuste Inteligente de Resolução
Ao abrir o sistema, a janela detecta automaticamente o tamanho do seu monitor:
* **Em Notebooks e Telas Compactas (≤ 1366×768):** A janela se abre **automaticamente em tela cheia maximizada**, garantindo que nenhum botão ou rodapé seja encoberto e que a visualização fique limpa e espaçosa.
* **Em Monitores Full HD ou Superiores (≥ 1920×1080):** A janela se abre em modo janela balanceado (1200×800) centralizado na tela.

### 🕹️ Controles da Barra Superior (TopBar)
No canto superior direito da barra de título, você encontra os botões de controle da aplicação:

| Botão | Ícone | Ação Realizada |
| :--- | :---: | :--- |
| **Acessibilidade (Tema)** | `☀️ / 🌙` | Alterna instantaneamente entre o **Modo Escuro Medieval** e o **Modo Claro de Alto Contraste (Pergaminho)**. Presente na barra superior e em todas as telas do sistema. |
| **Minimizar** | `—` | Reduz a janela para a barra de tarefas do Windows. |
| **Maximizar / Restaurar** | `🗖` ou `🗗` | Alterna instantaneamente entre o modo janela e o modo tela cheia inteligente. |
| **Fechar** | `✕` | Encerra com segurança o aplicativo. Fica vermelho ao passar o mouse. |

### ☀️ Acessibilidade Visual: Modo Claro e Modo Escuro
O Tronus Desktop disponibiliza um recurso completo de acessibilidade visual projetado para atender aos critérios de legibilidade e alto contraste **WCAG AAA**:
* **Botão Contextual em Todas as Telas:** O botão em formato de pílula estilizada está presente no cabeçalho fixo da janela e diretamente no topo de cada uma das telas (Boas-vindas, Login, Cadastro, Recuperação de Senha, Mapa dos Reinos, Desempenho, Ranking, Portal do Professor e Painel do Administrador).
* **Ícones Vetoriais Nativos (GDI+):** O botão desenha um Sol dourado nítido para convidar à ativação do Modo Claro e uma Lua Crescente refinada para o retorno ao Modo Escuro, sem risco de falha ou incompatibilidade de fontes do Windows.
* **Paleta Pergaminho Acessível:** Ao alternar para o Modo Claro, o aplicativo remove os papéis de parede escuros e adota uma paleta em marfim suave (`#F4F1EA`) com tipografia em carvão profundo (`#1C1916`) e detalhes em ouro envelhecido (`#AA780F`), proporcionando máxima clareza em salas de aula iluminadas ou para usuários com sensibilidade visual.
* **Atalho Global pelo Teclado:** A qualquer momento, pressione **`Ctrl + T`** ou **`Alt + A`** para alternar entre os temas sem precisar usar o mouse.

> **Dicas de Produtividade:**
> * **Clique Duplo:** Dê um duplo clique na barra de título dourada para alternar rapidamente entre maximizar e restaurar.
> * **Arrastar:** Em modo janela, clique e segure na barra superior para mover o aplicativo para qualquer parte da tela ou para outro monitor.
> * **Redimensionar:** Você pode puxar qualquer uma das quatro bordas ou dos quatro cantos da janela para definir o tamanho que preferir.

---

## 4. Primeiro Acesso: Cadastro de Usuário

Se você ainda não possui uma conta de aluno no Tronus:

1. Na tela inicial (**Landing**), clique no botão verde com gradiente **"CADASTRE-SE AGORA →"**.
2. Preencha os campos obrigatórios do formulário:
   * **Nome Completo:** Seu nome de identificação como estudante.
   * **Celular:** Telefone com DDD (ex: `(11) 98765-4321`).
   * **E-mail:** E-mail válido (ex: `aluno@unip.br`).
   * **Senha:** Crie uma senha de acesso pessoal.
   * **Confirmar Senha:** Digite a mesma senha para verificação de integridade.
3. Clique em **"CRIAR CONTA"**.
4. O sistema valida os campos, cadastra o aluno no banco de dados, efetua o login automático e direciona você imediatamente para a criação do seu personagem.

> Caso já tenha uma conta, basta clicar em **"← Voltar"** para retornar à tela inicial.

---

## 5. Criação do Personagem (Wizard do Herói)

Todo aluno no Tronus atua através de um **Herói Acadêmico**. Este assistente de 4 etapas orienta você na criação:

### Etapa 1: Escolha do Gênero
* Selecione **"Masculino"** ou **"Feminino"**. A seleção definirá a galeria de avatares disponíveis na etapa seguinte.

### Etapa 2: Nome do Herói
* Digite o codinome de batalha do seu herói (ex: *Sir Artur*, *Valquíria*, *Mestre dos Algoritmos*).
* Clique em **"Próximo →"**.

### Etapa 3: Seleção do Avatar
* O sistema apresenta uma galeria de ilustrações medievais temáticas.
* Clique sobre a arte que melhor representa o seu estilo para selecioná-la.
* Clique em **"Próximo →"**.

### Etapa 4: Ficha do Herói & Confirmação
* Visualize a prévia da sua ficha de personagem: Avatar em destaque, Nome do Herói, Gênero e Título inicial de **🛡️ Plebeu**.
* Clique no botão **"ENTRAR NO REINO ⚔"** para forjar seu herói e ingressar no Mapa dos Reinos!

---

## 6. Autenticação e Login

Para alunos que já possuem conta cadastrada:

1. Na tela inicial, clique em **"JÁ TENHO CONTA"**.
2. No cartão de acesso:
   * Digite seu **E-mail ou Usuário**.
   * Digite sua **Senha** (caracteres mascarados com segurança).
3. Pressione a tecla **`Enter`** ou clique no botão **"ENTRAR"**.
4. **Comportamento Inteligente:**
   * Se você já criou seu herói anteriormente, você será levado direto ao **Mapa dos Reinos**.
   * Se você acabou de se cadastrar mas ainda não havia finalizado seu herói, o Tronus abrirá o Wizard de Criação automaticamente.

---

## 7. Mapa dos Reinos (Tela Principal do Aluno)

Esta é a central de comando da sua jornada acadêmica. O layout é dividido em duas áreas principais:

```
┌─────────────────────────────────────────────────────────────────────────────┐
│ [Avatar] Nome do Herói - 🛡️ Plebeu   ⭐ 12 estrelas   [====    ] para Cavaleiro   [Desempenho] [Sair] │
├─────────────────────────────────────────────────────────────────────────────┤
│ ⚔️ MAPA DOS REINOS                                                          │
│ Escolha uma matéria, enfrente os desafios das fases e conquiste suas estrelas.│
│                                                                             │
│ ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│ │ 🐍 PYTHON    │  │ ⚙️ C++       │  │ 📐 ENG SOFT  │  │ 🗄️ B. DADOS  │ ...  │
│ │ [DISPONÍVEL] │  │ [DISPONÍVEL] │  │ [EM BREVE]   │  │ [DISPONÍVEL] │      │
│ │ Fase 1 ★★★   │  │ Fase 1 ★☆☆   │  │ Fase 1 ☆☆☆   │  │ Fase 1 ★★★   │      │
│ │ Fase 2 ★★☆   │  │ Fase 2 ☆☆☆   │  │ Fase 2 ☆☆☆   │  │ Fase 2 ★☆☆   │      │
│ │ Fase 3 ☆☆☆   │  │ Fase 3 ☆☆☆   │  │ Fase 3 ☆☆☆   │  │ Fase 3 ☆☆☆   │      │
│ │ [JOGAR]      │  │ [JOGAR]      │  │ [AGUARDE]    │  │ [JOGAR]      │      │
│ └──────────────┘  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 1. HUD Superior (Barra do Jogador)
* **Avatar em Miniatura:** Exibe o rosto do herói escolhido no cadastro.
* **Nome e Título:** O nome do seu herói e a insígnia correspondente ao nível de prestígio acadêmico (`🛡️ Plebeu`, `🎖️ Cavaleiro`, `🦅 Duque`, `👑 Rei`).
* **Estrelas Totais:** Soma de todas as melhores pontuações conquistadas em todas as matérias.
* **Barra de Progresso Dinâmica:** Mostra visualmente quanto falta para a próxima patente nobre (ex: `12 / 15 para Cavaleiro`).
* **Botão "Meu Desempenho 📊":** Abre o relatório detalhado de rendimento.
* **Botão "Sair":** Efetua o logout seguro da sua conta.

### 2. Mural dos Reinos
Cada reino é ilustrado com seu brasão e possui:
* **Badge de Disponibilidade:**
  * `DISPONÍVEL` (verde): O reino possui fases e desafios prontos para jogar.
  * `EM BREVE` (ocre): Questões ainda em fase de elaboração pela banca docente.
* **As 3 Fases de Cada Matéria:**
  * **Fase 1:** Fundamentos e Introdução.
  * **Fase 2:** Aprofundamento e Prática Intermediária.
  * **Fase 3:** Domínio Avançado e Desafio Mestre.
* **Estrelas por Fase:** Cada fase possui 3 estrelas possíveis (`★` conquistada, `☆` pendente).
* **Regra de Desbloqueio:** Para jogar a Fase 2, você deve conquistar pelo menos 1 estrela na Fase 1. Para liberar a Fase 3, deve obter pelo menos 1 estrela na Fase 2.

---

## 8. Desafios e Quizzes Acadêmicos

Ao clicar em **"JOGAR"** em um reino desbloqueado, o desafio da fase é iniciado:

1. **Identificação da Questão:** No topo, o indicador exibe em qual pergunta você está (ex: `Questão 2 de 5`) e o nível de dificuldade (`FÁCIL`, `MÉDIO`, `DIFÍCIL`).
2. **Caixa do Enunciado:** Texto com rolagem dedicada para leitura confortável de contextualizações teóricas, códigos-fonte e esquemas.
3. **Seleção de Alternativas:**
   * Clique em uma das 5 alternativas `[A]`, `[B]`, `[C]`, `[D]` ou `[E]`.
   * A alternativa selecionada ganha destaque visual imediato com contorno dourado e preenchimento verde esmeralda.
   * Você pode mudar de alternativa a qualquer momento antes da submissão final.
4. **Navegação:**
   * Clique em **"← Anterior"** para revisar uma questão anterior.
   * Clique em **"Próxima →"** para avançar para o próximo desafio.
5. **Botão de Abandono:** Caso precise sair antes de terminar, clique em **"Abandonar Desafio"**. O sistema solicitará uma confirmação alertando que as respostas da tentativa atual não serão salvas.
6. **Finalização da Prova:** Na última questão (Questão 5), o botão de avançar se transforma em **"FINALIZAR ⚔"**. Clique nele para enviar seu caderno de respostas para correção instantânea pela API.

---

## 9. Resultado da Conquista e Progressão

Assim que você finaliza o quiz, o cartão de resultado é exibido em destaque central:

* **Estrelas Conquistadas na Rodada:**
  * `★★★` (3 estrelas): Excelente domínio (80% a 100% de acertos).
  * `★★☆` (2 estrelas): Bom desempenho (60% a 79% de acertos).
  * `★☆☆` (1 estrela): Nível básico atingido (40% a 59% de acertos).
  * `☆☆☆` (0 estrelas): Abaixo da meta mínima (menos de 40%). Permite tentar novamente sem penalidade.
* **Métricas da Rodada:** Número de acertos sobre o total e taxa percentual de precisão.
* **Melhor Marca Registrada:** O sistema sempre preserva sua maior pontuação histórica na fase (`GREATEST`). Se você já tinha 3 estrelas e jogou novamente obtendo 2, suas 3 estrelas continuam seguras no banco de dados!
* **Evolução do Herói:** Exibe o total consolidado de estrelas do herói e o novo título acadêmico caso você tenha alcançado uma promoção de patente.
* Clique no botão dourado **"VOLTAR AO MAPA DE REINOS"** para escolher seu próximo desafio.

---

## 10. Painel de Desempenho (Dashboard)

Acessível pelo botão **"Meu Desempenho 📊"** no HUD superior do mapa, esta tela fornece um panorama consolidado do seu rendimento de estudos:

* **Total de Respostas Enviadas:** Volume de questões submetidas ao longo do semestre.
* **Total de Acertos:** Quantidade líquida de questões respondidas com sucesso.
* **Taxa de Acerto Geral (%):** Média global de precisão acadêmica do estudante.
* **Total de Estrelas Acumuladas:** Pontuação geral para ranqueamento nobiliárquico.
* **Fases Iniciadas:** Quantidade de etapas exploradas pelo herói.
* **Fases 100% Concluídas:** Total de fases dominadas com nota máxima (3 estrelas).

Clique em **"Voltar ao Mapa"** para retornar à navegação dos reinos.

---

## 11. Logout e Troca de Perfil

Para sair da sua conta ou permitir que outro estudante utilize o mesmo computador:

1. No HUD superior do Mapa dos Reinos, clique no botão **"Sair"**.
2. Uma caixa de diálogo de confirmação será exibida: *"Deseja realmente sair da sua conta?"*.
3. Clique em **"Sim"**.
4. Suas credenciais e token JWT são removidos da memória do computador de forma segura e o sistema retorna para a tela de Login.

---

## 12. Perguntas Frequentes e Solução de Problemas

### ❓ O aplicativo abre, mas diz que não foi possível conectar com a API.
* **Causa:** O computador está sem acesso à internet ou o firewall institucional está bloqueando conexões HTTPS externas.
* **Solução:** Verifique sua conexão com a internet. O Tronus Desktop consome a API central na nuvem (`https://king-blue-nine.vercel.app`). Certifique-se de que o tráfego HTTPS na porta 443 esteja liberado.

### ❓ Quais portas de rede e permissões de firewall são necessárias para o Tronus Desktop?
* **Porta de Saída Obrigatória:** O Tronus Desktop necessita exclusivamente da **porta 443 (TCP / HTTPS)** liberada para tráfego de saída (Outbound) em direção ao domínio `king-blue-nine.vercel.app`.
* **Ambiente Acadêmico e Laboratórios:** Por utilizar a porta padrão mundial de navegação web criptografada (HTTPS 443), o aplicativo opera normalmente em redes corporativas, laboratórios universitários e redes Wi-Fi públicas sem exigir abertura de portas arbitrárias ou permissões de administrador.

### ❓ Minha sessão foi finalizada e o sistema voltou para a tela de login.
* **Causa:** Por motivos de segurança bancária e acadêmica, os tokens de acesso possuem validade temporária de 1 hora.
* **Solução:** Isso é normal. Basta digitar sua senha novamente na tela de login para renovar a sessão.

### ❓ Posso jogar uma mesma fase mais de uma vez?
* **Sim!** O sistema incentiva o treinamento contínuo. Você pode jogar qualquer fase quantas vezes desejar para melhorar seu entendimento e buscar as 3 estrelas máximas. Sua melhor marca nunca é diminuída.

### ❓ Como faço para usar em tela cheia no meu notebook?
* O aplicativo já inicia maximizado em notebooks com tela de até 1366×768. Caso esteja em modo janela, basta clicar no botão de maximizar (`🗖`) ou dar um duplo clique na barra de título dourada.

---

**Tronus Desktop — Universidade Paulista (UNIP 2026)**  
*Forjando o Futuro através do Conhecimento.*
