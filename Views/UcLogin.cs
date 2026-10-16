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
        private TronusButton btnSubmit;
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
                Size = new Size(400, 500),
                Location = new Point((1024 - 400) / 2, (728 - 500) / 2) // Center in 1024x728
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
                Location = new Point(40, 120),
                BackColor = Color.Transparent
            };
            loginCard.Controls.Add(emailLabel);

            emailBox = new TextBox
            {
                Size = new Size(320, 30),
                Location = new Point(40, 150),
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
                Location = new Point(40, 200),
                BackColor = Color.Transparent
            };
            loginCard.Controls.Add(passLabel);

            passBox = new TextBox
            {
                Size = new Size(320, 30),
                Location = new Point(40, 230),
                Font = new Font("Segoe UI", 12F),
                PasswordChar = '•',
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            passBox.KeyDown += TextBox_KeyDown;
            loginCard.Controls.Add(passBox);

            btnSubmit = new TronusButton
            {
                Text = "ENTRAR",
                Size = new Size(320, 45),
                Location = new Point(40, 300)
            };
            btnSubmit.Click += BtnSubmit_Click;
            loginCard.Controls.Add(btnSubmit);

            backLabel = new Label
            {
                Text = "← Voltar",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(40, 380),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            backLabel.Click += (s, e) => {
                (this.FindForm() as MainForm)?.LoadControl(new UcLanding());
            };
            backLabel.MouseEnter += (s, e) => backLabel.ForeColor = Theme.Gold;
            backLabel.MouseLeave += (s, e) => backLabel.ForeColor = Theme.DimText;
            loginCard.Controls.Add(backLabel);

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
                else 
                {
                    MessageBox.Show($"Bem-vindo(a), {res.Usuario.Nome}!\nPapel: {res.Usuario.Papel}", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // TODO: Redirect to professor/admin dashboard
                }
            }
            catch (TronusDesktop.Services.ApiException ex)
            {
                MessageBox.Show(ex.Message, "Falha no Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro de conexão: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSubmit.Enabled = true;
                btnSubmit.Text = "ENTRAR";
            }
        }
        #endregion
    }
}
