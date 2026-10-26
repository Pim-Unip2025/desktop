using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TronusDesktop.UI;

namespace TronusDesktop.Views
{
    public partial class UcLogin : UserControl
    {
        private TronusPanel loginCard;
        private Label titleLabel;
        private Label emailLabel;
        private TextBox emailBox;
        private Label passLabel;
        private TextBox passBox;
        private PasswordToggleButton btnTogglePass;
        private TronusButton btnSubmit;
        private Button btnOffline;
        private Label backLabel;

        public UcLogin()
        {
            InitializeComponent();
            SetupUI();
        }

        #region Configuração Visual (UI)
        /// <summary>
        /// Responsável por desenhar os componentes da tela de Login.
        /// </summary>
        private void SetupUI()
        {
            this.BackColor = Theme.Background;
            string bgPath = Path.Combine(Application.StartupPath, "img", "fundo_reinos.jpg");
            if (File.Exists(bgPath))
            {
                this.BackgroundImage = Image.FromFile(bgPath);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }

            loginCard = new TronusPanel
            {
                Size = new Size(400, 510),
                Location = new Point((1024 - 400) / 2, (728 - 510) / 2) // Center in 1024x728
            };
            this.Controls.Add(loginCard);

            titleLabel = new Label
            {
                Text = "ENTRAR NO REINO",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(400, 40),
                Location = new Point(0, 40),
                BackColor = Color.Transparent
            };
            loginCard.Controls.Add(titleLabel);

            emailLabel = new Label
            {
                Text = "E-mail ou Usuário",
                ForeColor = Theme.PrimaryText,
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(40, 110),
                BackColor = Color.Transparent
            };
            loginCard.Controls.Add(emailLabel);

            emailBox = new TextBox
            {
                Size = new Size(320, 30),
                Location = new Point(40, 140),
                Font = new Font("Segoe UI", 12F),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            emailBox.KeyDown += TextBox_KeyDown;
            loginCard.Controls.Add(emailBox);

            passLabel = new Label
            {
                Text = "Senha",
                ForeColor = Theme.PrimaryText,
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(40, 190),
                BackColor = Color.Transparent
            };
            loginCard.Controls.Add(passLabel);

            passBox = new TextBox
            {
                Size = new Size(280, 30),
                Location = new Point(40, 220),
                Font = new Font("Segoe UI", 12F),
                PasswordChar = '•',
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            passBox.KeyDown += TextBox_KeyDown;
            loginCard.Controls.Add(passBox);

            btnTogglePass = new PasswordToggleButton(passBox)
            {
                Size = new Size(36, 29),
                Location = new Point(324, 220)
            };
            loginCard.Controls.Add(btnTogglePass);

            btnSubmit = new TronusButton
            {
                Text = "ENTRAR",
                Size = new Size(320, 42),
                Location = new Point(40, 280)
            };
            btnSubmit.Click += BtnSubmit_Click;
            loginCard.Controls.Add(btnSubmit);

            btnOffline = new Button
            {
                Text = "⚡ ENTRAR EM MODO OFFLINE (DEMO)",
                Size = new Size(320, 38),
                Location = new Point(40, 335),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(35, 212, 175, 55),
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnOffline.FlatAppearance.BorderColor = Color.FromArgb(160, 212, 175, 55);
            btnOffline.FlatAppearance.BorderSize = 1;
            btnOffline.Click += BtnOffline_Click;
            btnOffline.MouseEnter += (s, e) => btnOffline.BackColor = Color.FromArgb(70, 212, 175, 55);
            btnOffline.MouseLeave += (s, e) => btnOffline.BackColor = Color.FromArgb(35, 212, 175, 55);
            loginCard.Controls.Add(btnOffline);

            backLabel = new Label
            {
                Text = "← Voltar",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(40, 400),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            backLabel.Click += (s, e) => {
                (this.FindForm() as MainForm)?.LoadControl(new UcLanding());
            };
            backLabel.MouseEnter += (s, e) => backLabel.ForeColor = Theme.Gold;
            backLabel.MouseLeave += (s, e) => backLabel.ForeColor = Theme.DimText;
            loginCard.Controls.Add(backLabel);

            var forgotLabel = new Label
            {
                Text = "Esqueci a senha 🔑",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 9.5F),
                AutoSize = true,
                Location = new Point(230, 400),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            forgotLabel.Click += (s, e) => {
                (this.FindForm() as MainForm)?.LoadControl(new UcForgotPassword());
            };
            forgotLabel.MouseEnter += (s, e) => forgotLabel.ForeColor = Theme.Gold;
            forgotLabel.MouseLeave += (s, e) => forgotLabel.ForeColor = Theme.DimText;
            loginCard.Controls.Add(forgotLabel);

            this.Resize += (s, e) => CenterCard();
            CenterCard();
        }

        private void CenterCard()
        {
            if (loginCard != null)
            {
                loginCard.Location = new Point(
                    Math.Max(10, (this.ClientSize.Width - loginCard.Width) / 2),
                    Math.Max(10, (this.ClientSize.Height - loginCard.Height) / 2)
                );
            }
        }

        #endregion

        #region Eventos (Integração Backend e Usabilidade)
        
        /// <summary>
        /// Permite o login pressionando a tecla ENTER nos campos de texto.
        /// </summary>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Remove o som de "ding" do Windows
                BtnSubmit_Click(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Evento de clique do botão de Entrar.
        /// A equipe de backend deve plugar a chamada de API/Banco de dados aqui.
        /// </summary>
        private async void BtnSubmit_Click(object sender, EventArgs e)
        {
            // Validação básica do Front-end
            if (string.IsNullOrWhiteSpace(emailBox.Text) || string.IsNullOrWhiteSpace(passBox.Text))
            {
                MessageBox.Show("Preencha todos os campos para entrar no reino.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnSubmit.Enabled = false;
            btnSubmit.Text = "ENTRANDO...";

            try
            {
                var req = new TronusDesktop.Models.LoginRequest
                {
                    Identificador = emailBox.Text,
                    Senha = passBox.Text
                };

                var res = await TronusDesktop.Services.ApiService.Instance.PostAsync<TronusDesktop.Models.LoginRequest, TronusDesktop.Models.AuthResponse>("/auth/login", req);
                
                TronusDesktop.Services.SessionManager.Login(res);

                if (res.Usuario.Papel == "aluno")
                {
                    try 
                    {
                        var personagem = await TronusDesktop.Services.ApiService.Instance.GetAsync<TronusDesktop.Models.PersonagemDto>("/personagens/me");
                        MessageBox.Show($"Bem-vindo de volta, {personagem.Nome}!\nPapel: {res.Usuario.Papel}", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        (this.FindForm() as MainForm)?.LoadControl(new UcReinos());
                    } 
                    catch (TronusDesktop.Services.ApiException ex) when (ex.StatusCode == 404)
                    {
                        // 404 means the character doesn't exist yet. Send to character creation.
                        MessageBox.Show("Você ainda não tem um personagem. Vamos criá-lo agora!", "Novo Jogador", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        (this.FindForm() as MainForm)?.LoadControl(new UcCharacter());
                    }
                }
                else if (res.Usuario.Papel == "professor")
                {
                    MessageBox.Show($"Bem-vindo(a), Professor(a) {res.Usuario.Nome}!\nAcessando o Portal Pedagógico...", "Painel do Professor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    (this.FindForm() as MainForm)?.LoadControl(new UcProfessor());
                }
                else if (res.Usuario.Papel == "admin")
                {
                    MessageBox.Show($"Bem-vindo(a), Administrador(a) {res.Usuario.Nome}!\nAcessando o Painel de Governança...", "Painel de Administração", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    (this.FindForm() as MainForm)?.LoadControl(new UcAdmin());
                }
                else 
                {
                    MessageBox.Show($"Bem-vindo(a), {res.Usuario.Nome}!\nPapel: {res.Usuario.Papel}", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    (this.FindForm() as MainForm)?.LoadControl(new UcReinos());
                }
            }
            catch (TronusDesktop.Services.ApiException ex)
            {
                MessageBox.Show(ex.Message, "Falha no Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                var resposta = MessageBox.Show(
                    $"Não foi possível conectar aos servidores de nuvem:\n{ex.Message}\n\n" +
                    "Deseja alternar para o MODO OFFLINE (Demonstração Local Resiliente) agora?",
                    "Conexão Indisponível — Tronus Desktop",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resposta == DialogResult.Yes)
                {
                    BtnOffline_Click(sender, e);
                }
            }
            finally
            {
                btnSubmit.Enabled = true;
                btnSubmit.Text = "ENTRAR";
            }
        }

        /// <summary>
        /// Realiza o login direto no Modo Offline / Demonstração Resiliente.
        /// Carrega o Herói padrão Sir Arthur (Duque, 38 estrelas, 5 reinos)
        /// ou cria um novo se especificado um identificador inexistente.
        /// </summary>
        private async void BtnOffline_Click(object sender, EventArgs e)
        {
            btnSubmit.Enabled = false;
            if (btnOffline != null)
            {
                btnOffline.Enabled = false;
                btnOffline.Text = "INICIANDO OFFLINE...";
            }

            try
            {
                TronusDesktop.Services.ApiService.IsOfflineMode = true;

                // Se o usuário digitou algum nome ou email no campo, usa; senão usa o padrão "Arthur Pendragon"
                string id = string.IsNullOrWhiteSpace(emailBox.Text) ? "Arthur Pendragon" : emailBox.Text.Trim();
                var req = new TronusDesktop.Models.LoginRequest
                {
                    Identificador = id,
                    Senha = string.IsNullOrWhiteSpace(passBox.Text) ? "offline" : passBox.Text
                };

                var res = await TronusDesktop.Services.ApiService.Instance.PostAsync<TronusDesktop.Models.LoginRequest, TronusDesktop.Models.AuthResponse>("/auth/login", req);
                TronusDesktop.Services.SessionManager.Login(res);

                if (res.Usuario.Papel == "aluno")
                {
                    try
                    {
                        var heroi = await TronusDesktop.Services.ApiService.Instance.GetAsync<TronusDesktop.Models.PersonagemDto>("/personagens/me");
                        MessageBox.Show(
                            $"⚡ MODO OFFLINE ATIVADO COM SUCESSO!\n\n" +
                            $"Bem-vindo ao Tronus, Herói {heroi.Nome}!\n" +
                            $"Título: {heroi.TituloAtual} · {heroi.TotalEstrelas} Estrelas acumuladas\n\n" +
                            $"• Simulação autônoma ativa em memória local.\n" +
                            $"• Reinos, desafios de combate e ranking disponíveis sem internet.",
                            "Tronus Desktop — Modo Offline",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        (this.FindForm() as MainForm)?.LoadControl(new UcReinos());
                    }
                    catch (TronusDesktop.Services.ApiException ex) when (ex.StatusCode == 404)
                    {
                        MessageBox.Show("Modo Offline ativado! Vamos criar o seu herói local.", "Novo Jogador Offline", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        (this.FindForm() as MainForm)?.LoadControl(new UcCharacter());
                    }
                }
                else if (res.Usuario.Papel == "professor")
                {
                    MessageBox.Show($"Bem-vindo(a), Professor(a) {res.Usuario.Nome}!\n[MODO OFFLINE ATIVO]", "Portal Pedagógico", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    (this.FindForm() as MainForm)?.LoadControl(new UcProfessor());
                }
                else if (res.Usuario.Papel == "admin")
                {
                    MessageBox.Show($"Bem-vindo(a), Administrador(a) {res.Usuario.Nome}!\n[MODO OFFLINE ATIVO]", "Painel Administrativo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    (this.FindForm() as MainForm)?.LoadControl(new UcAdmin());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Falha ao inicializar o Modo Offline: " + ex.Message, "Erro Modo Offline", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSubmit.Enabled = true;
                if (btnOffline != null)
                {
                    btnOffline.Enabled = true;
                    btnOffline.Text = "⚡ ENTRAR EM MODO OFFLINE (DEMO)";
                }
            }
        }
        #endregion
    }
}
