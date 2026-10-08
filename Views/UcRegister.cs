using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TronusDesktop.UI;

namespace TronusDesktop.Views
{
    public partial class UcRegister : UserControl
    {
        private TronusPanel registerCard;
        private Label titleLabel;
        
        private Label nameLabel;
        private TextBox nameBox;
        
        private Label phoneLabel;
        private TextBox phoneBox;
        
        private Label emailLabel;
        private TextBox emailBox;
        
        private Label passLabel;
        private TextBox passBox;
        
        private Label confirmPassLabel;
        private TextBox confirmPassBox;
        
        private TronusButton btnSubmit;
        private Label backLabel;

        public UcRegister()
        {
            InitializeComponent();
            SetupUI();
        }

        #region Configuração Visual (UI)
        /// <summary>
        /// Responsável por construir a interface e os inputs do formulário de cadastro.
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

            registerCard = new TronusPanel
            {
                Size = new Size(420, 650),
                Location = new Point((1024 - 420) / 2, (728 - 650) / 2)
            };
            this.Controls.Add(registerCard);

            titleLabel = new Label
            {
                Text = "NOVO HERÓI",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(420, 40),
                Location = new Point(0, 20),
                BackColor = Color.Transparent
            };
            registerCard.Controls.Add(titleLabel);

            nameLabel = new Label
            {
                Text = "Nome Completo",
                ForeColor = Theme.PrimaryText,
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(40, 80),
                BackColor = Color.Transparent
            };
            registerCard.Controls.Add(nameLabel);

            nameBox = new TextBox
            {
                Size = new Size(340, 30),
                Location = new Point(40, 110),
                Font = new Font("Segoe UI", 12F),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            registerCard.Controls.Add(nameBox);

            phoneLabel = new Label
            {
                Text = "Celular (ex: 11999999999)",
                ForeColor = Theme.PrimaryText,
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(40, 160),
                BackColor = Color.Transparent
            };
            registerCard.Controls.Add(phoneLabel);

            phoneBox = new TextBox
            {
                Size = new Size(340, 30),
                Location = new Point(40, 190),
                Font = new Font("Segoe UI", 12F),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            registerCard.Controls.Add(phoneBox);

            emailLabel = new Label
            {
                Text = "E-mail",
                ForeColor = Theme.PrimaryText,
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(40, 240),
                BackColor = Color.Transparent
            };
            registerCard.Controls.Add(emailLabel);

            emailBox = new TextBox
            {
                Size = new Size(340, 30),
                Location = new Point(40, 270),
                Font = new Font("Segoe UI", 12F),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            registerCard.Controls.Add(emailBox);

            passLabel = new Label
            {
                Text = "Senha",
                ForeColor = Theme.PrimaryText,
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(40, 320),
                BackColor = Color.Transparent
            };
            registerCard.Controls.Add(passLabel);

            passBox = new TextBox
            {
                Size = new Size(340, 30),
                Location = new Point(40, 350),
                Font = new Font("Segoe UI", 12F),
                PasswordChar = '•',
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            registerCard.Controls.Add(passBox);

            confirmPassLabel = new Label
            {
                Text = "Confirmar Senha",
                ForeColor = Theme.PrimaryText,
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(40, 400),
                BackColor = Color.Transparent
            };
            registerCard.Controls.Add(confirmPassLabel);

            confirmPassBox = new TextBox
            {
                Size = new Size(340, 30),
                Location = new Point(40, 430),
                Font = new Font("Segoe UI", 12F),
                PasswordChar = '•',
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            registerCard.Controls.Add(confirmPassBox);

            btnSubmit = new TronusButton
            {
                Text = "CRIAR CONTA",
                Size = new Size(340, 45),
                Location = new Point(40, 500)
            };
            btnSubmit.Click += BtnSubmit_Click;
            registerCard.Controls.Add(btnSubmit);

            backLabel = new Label
            {
                Text = "← Voltar",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(40, 570),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            backLabel.Click += (s, e) => {
                (this.FindForm() as MainForm)?.LoadControl(new UcLanding());
            };
            backLabel.MouseEnter += (s, e) => backLabel.ForeColor = Theme.Gold;
            backLabel.MouseLeave += (s, e) => backLabel.ForeColor = Theme.DimText;
            registerCard.Controls.Add(backLabel);
        }

        #endregion

        #region Eventos (Integração Backend)
        /// <summary>
        /// Evento de submissão do formulário.
        /// Aqui os dados coletados (nameBox, phoneBox, emailBox, passBox) devem ser salvos no Banco.
        /// </summary>
        private async void BtnSubmit_Click(object sender, EventArgs e)
        {
            // Validação visual de campos vazios
            if (string.IsNullOrWhiteSpace(nameBox.Text) || string.IsNullOrWhiteSpace(phoneBox.Text) || 
                string.IsNullOrWhiteSpace(emailBox.Text) || string.IsNullOrWhiteSpace(passBox.Text) || 
                string.IsNullOrWhiteSpace(confirmPassBox.Text))
            {
                MessageBox.Show("Preencha todos os campos obrigatórios.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validação de senhas idênticas
            if (passBox.Text != confirmPassBox.Text)
            {
                MessageBox.Show("As senhas não coincidem.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validação de senha forte (mínimo 8 caracteres, com letra maiúscula, minúscula, número e símbolo)
            var passRegex = new System.Text.RegularExpressions.Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");
            if (!passRegex.IsMatch(passBox.Text))
            {
                MessageBox.Show("A senha deve ter no mínimo 8 caracteres, incluindo letra maiúscula, minúscula, número e símbolo.", "Senha Fraca", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnSubmit.Enabled = false;
            btnSubmit.Text = "CRIANDO...";

            try
            {
                var req = new TronusDesktop.Models.CadastroRequest
                {
                    Nome = nameBox.Text,
                    Email = emailBox.Text,
                    Celular = phoneBox.Text.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", ""),
                    Senha = passBox.Text
                };

                var res = await TronusDesktop.Services.ApiService.Instance.PostAsync<TronusDesktop.Models.CadastroRequest, TronusDesktop.Models.AuthResponse>("/auth/register", req);

                TronusDesktop.Services.SessionManager.Login(res);
                
                // Navegação para a próxima tela
                (this.FindForm() as MainForm)?.LoadControl(new UcCharacter());
            }
            catch (TronusDesktop.Services.ApiException ex)
            {
                MessageBox.Show(ex.Message, "Erro no Cadastro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro de conexão: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSubmit.Enabled = true;
                btnSubmit.Text = "CRIAR CONTA";
            }
        }
        #endregion
    }
}
