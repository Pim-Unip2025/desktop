using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TronusDesktop.UI;

namespace TronusDesktop.Views
{
    public partial class UcForgotPassword : UserControl
    {
        private TronusPanel forgotCard;
        private Label titleLabel;
        private Label subtitleLabel;

        // Passo 1: Celular
        private Panel step1Panel;
        private Label lblCelular;
        private TextBox txtCelular;
        private TronusButton btnEnviarCodigo;
        private Label lblVoltarLogin1;

        // Passo 2: Código
        private Panel step2Panel;
        private Label lblMsgCodigo;
        private Label lblCodigoExibido;
        private Label lblDigitarCodigo;
        private TextBox txtCodigo;
        private TronusButton btnVerificarCodigo;
        private Label lblVoltarPasso1;

        // Passo 3: Nova Senha
        private Panel step3Panel;
        private Label lblNovaSenha;
        private TextBox txtNovaSenha;
        private Label lblConfirmaSenha;
        private TextBox txtConfirmaSenha;
        private TronusButton btnSalvarSenha;
        private Label lblVoltarLogin3;

        // Estado do fluxo
        private string codigoGerado = "";
        private string celularInformado = "";

        public UcForgotPassword()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "UcForgotPassword";
            this.Size = new Size(1024, 728);
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            this.BackColor = Theme.Background;
            string bgPath = Path.Combine(Application.StartupPath, "img", "fundo_reinos.jpg");
            if (File.Exists(bgPath))
            {
                this.BackgroundImage = Image.FromFile(bgPath);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }

            // Camada escura sobre o fundo para manter o padrão Dark Fantasy
            this.Paint += (s, e) =>
            {
                using var brush = new SolidBrush(Color.FromArgb(140, 15, 12, 10));
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            };

            forgotCard = new TronusPanel
            {
                Size = new Size(460, 520),
                BackColor = Color.Transparent
            };
            this.Controls.Add(forgotCard);

            // Título
            titleLabel = new Label
            {
                Text = "🔑 RECUPERAR SENHA",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(460, 40),
                Location = new Point(0, 25),
                BackColor = Color.Transparent
            };
            forgotCard.Controls.Add(titleLabel);

            subtitleLabel = new Label
            {
                Text = "Restaure seu acesso ao Reino Acadêmico",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(460, 25),
                Location = new Point(0, 65),
                BackColor = Color.Transparent
            };
            forgotCard.Controls.Add(subtitleLabel);

            ConstruirPasso1();
            ConstruirPasso2();
            ConstruirPasso3();

            MostrarPasso(1);

            this.Resize += (s, e) => CenterCard();
            CenterCard();
        }

        private void CenterCard()
        {
            if (forgotCard != null)
            {
                forgotCard.Location = new Point(
                    Math.Max(10, (this.ClientSize.Width - forgotCard.Width) / 2),
                    Math.Max(10, (this.ClientSize.Height - forgotCard.Height) / 2)
                );
            }
        }

        #region Construção dos Passos

        private void ConstruirPasso1()
        {
            step1Panel = new Panel
            {
                Location = new Point(40, 105),
                Size = new Size(380, 380),
                BackColor = Color.Transparent
            };

            var lblDesc = new Label
            {
                Text = "Informe o número de celular vinculado à sua conta de Herói para enviarmos o código de segurança.",
                ForeColor = Theme.PrimaryText,
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(0, 10),
                Size = new Size(380, 45),
                BackColor = Color.Transparent
            };
            step1Panel.Controls.Add(lblDesc);

            lblCelular = new Label
            {
                Text = "📱 Celular Cadastrado",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 75),
                BackColor = Color.Transparent
            };
            step1Panel.Controls.Add(lblCelular);

            txtCelular = new TextBox
            {
                Size = new Size(380, 32),
                Location = new Point(0, 105),
                Font = new Font("Segoe UI", 12F),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Ex: (11) 99999-9999"
            };
            txtCelular.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    BtnEnviarCodigo_Click(s, e);
                }
            };
            step1Panel.Controls.Add(txtCelular);

            btnEnviarCodigo = new TronusButton
            {
                Text = "ENVIAR CÓDIGO →",
                Size = new Size(380, 45),
                Location = new Point(0, 165)
            };
            btnEnviarCodigo.Click += BtnEnviarCodigo_Click;
            step1Panel.Controls.Add(btnEnviarCodigo);

            lblVoltarLogin1 = CriarLinkVoltar("← Voltar ao Login", new Point(0, 240));
            step1Panel.Controls.Add(lblVoltarLogin1);

            forgotCard.Controls.Add(step1Panel);
        }

        private void ConstruirPasso2()
        {
            step2Panel = new Panel
            {
                Location = new Point(40, 105),
                Size = new Size(380, 380),
                BackColor = Color.Transparent,
                Visible = false
            };

            lblMsgCodigo = new Label
            {
                Text = "Um código de verificação SMS foi emitido para o número informado (simulação do sistema):",
                ForeColor = Theme.PrimaryText,
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(0, 10),
                Size = new Size(380, 40),
                BackColor = Color.Transparent
            };
            step2Panel.Controls.Add(lblMsgCodigo);

            // Card dourado que exibe o código simulado
            var codeBox = new Panel
            {
                Location = new Point(0, 60),
                Size = new Size(380, 50),
                BackColor = Color.FromArgb(30, 212, 175, 55)
            };
            codeBox.Paint += (s, e) =>
            {
                using var p = new Pen(Theme.Gold, 1.5F);
                e.Graphics.DrawRectangle(p, 0, 0, codeBox.Width - 1, codeBox.Height - 1);
            };

            lblCodigoExibido = new Label
            {
                Text = "CÓDIGO: ------",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            codeBox.Controls.Add(lblCodigoExibido);
            step2Panel.Controls.Add(codeBox);

            lblDigitarCodigo = new Label
            {
                Text = "Digite o código de 6 dígitos:",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 130),
                BackColor = Color.Transparent
            };
            step2Panel.Controls.Add(lblDigitarCodigo);

            txtCodigo = new TextBox
            {
                Size = new Size(380, 32),
                Location = new Point(0, 155),
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Theme.Gold,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center,
                MaxLength = 6
            };
            txtCodigo.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    BtnVerificarCodigo_Click(s, e);
                }
            };
            step2Panel.Controls.Add(txtCodigo);

            btnVerificarCodigo = new TronusButton
            {
                Text = "VERIFICAR CÓDIGO ✓",
                Size = new Size(380, 45),
                Location = new Point(0, 210)
            };
            btnVerificarCodigo.Click += BtnVerificarCodigo_Click;
            step2Panel.Controls.Add(btnVerificarCodigo);

            lblVoltarPasso1 = CriarLinkVoltar("← Tentar outro número", new Point(0, 280));
            lblVoltarPasso1.Click += (s, e) => MostrarPasso(1);
            step2Panel.Controls.Add(lblVoltarPasso1);

            forgotCard.Controls.Add(step2Panel);
        }

        private void ConstruirPasso3()
        {
            step3Panel = new Panel
            {
                Location = new Point(40, 105),
                Size = new Size(380, 380),
                BackColor = Color.Transparent,
                Visible = false
            };

            lblNovaSenha = new Label
            {
                Text = "🔒 Nova Senha",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 10),
                BackColor = Color.Transparent
            };
            step3Panel.Controls.Add(lblNovaSenha);

            txtNovaSenha = new TextBox
            {
                Size = new Size(380, 30),
                Location = new Point(0, 35),
                Font = new Font("Segoe UI", 12F),
                PasswordChar = '•',
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            step3Panel.Controls.Add(txtNovaSenha);

            lblConfirmaSenha = new Label
            {
                Text = "🔒 Confirmar Nova Senha",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 85),
                BackColor = Color.Transparent
            };
            step3Panel.Controls.Add(lblConfirmaSenha);

            txtConfirmaSenha = new TextBox
            {
                Size = new Size(380, 30),
                Location = new Point(0, 110),
                Font = new Font("Segoe UI", 12F),
                PasswordChar = '•',
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtConfirmaSenha.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    BtnSalvarSenha_Click(s, e);
                }
            };
            step3Panel.Controls.Add(txtConfirmaSenha);

            btnSalvarSenha = new TronusButton
            {
                Text = "SALVAR NOVA SENHA ⚔",
                Size = new Size(380, 45),
                Location = new Point(0, 175)
            };
            btnSalvarSenha.Click += BtnSalvarSenha_Click;
            step3Panel.Controls.Add(btnSalvarSenha);

            lblVoltarLogin3 = CriarLinkVoltar("← Cancelar e Voltar", new Point(0, 250));
            step3Panel.Controls.Add(lblVoltarLogin3);

            forgotCard.Controls.Add(step3Panel);
        }

        private Label CriarLinkVoltar(string texto, Point location)
        {
            var lbl = new Label
            {
                Text = texto,
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 9.5F),
                AutoSize = true,
                Location = location,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            lbl.Click += (s, e) => (this.FindForm() as MainForm)?.LoadControl(new UcLogin());
            lbl.MouseEnter += (s, e) => lbl.ForeColor = Theme.Gold;
            lbl.MouseLeave += (s, e) => lbl.ForeColor = Theme.DimText;
            return lbl;
        }

        private void MostrarPasso(int passo)
        {
            step1Panel.Visible = (passo == 1);
            step2Panel.Visible = (passo == 2);
            step3Panel.Visible = (passo == 3);

            if (passo == 1)
            {
                titleLabel.Text = "🔑 RECUPERAR SENHA";
                txtCelular.Focus();
            }
            else if (passo == 2)
            {
                titleLabel.Text = "🛡️ CÓDIGO DE SEGURANÇA";
                txtCodigo.Focus();
            }
            else if (passo == 3)
            {
                titleLabel.Text = "✨ DEFINIR NOVA SENHA";
                txtNovaSenha.Focus();
            }
        }

        #endregion

        #region Eventos do Fluxo

        private void BtnEnviarCodigo_Click(object sender, EventArgs e)
        {
            string celular = txtCelular.Text.Trim();
            if (string.IsNullOrWhiteSpace(celular) || celular.Length < 8)
            {
                MessageBox.Show("Por favor, informe um número de celular válido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCelular.Focus();
                return;
            }

            celularInformado = celular;

            // Gera código aleatório de 6 dígitos
            var rnd = new Random();
            codigoGerado = rnd.Next(100000, 999999).ToString();
            lblCodigoExibido.Text = $"CÓDIGO: {codigoGerado}";

            txtCodigo.Clear();
            MostrarPasso(2);
        }

        private void BtnVerificarCodigo_Click(object sender, EventArgs e)
        {
            string digitado = txtCodigo.Text.Trim();
            if (string.IsNullOrWhiteSpace(digitado))
            {
                MessageBox.Show("Digite o código recebido para continuar.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigo.Focus();
                return;
            }

            if (digitado != codigoGerado)
            {
                MessageBox.Show("Código incorreto. Por favor, verifique os 6 dígitos e tente novamente.", "Código Inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCodigo.SelectAll();
                txtCodigo.Focus();
                return;
            }

            txtNovaSenha.Clear();
            txtConfirmaSenha.Clear();
            MostrarPasso(3);
        }

        private void BtnSalvarSenha_Click(object sender, EventArgs e)
        {
            string nova = txtNovaSenha.Text;
            string confirma = txtConfirmaSenha.Text;

            if (string.IsNullOrWhiteSpace(nova) || string.IsNullOrWhiteSpace(confirma))
            {
                MessageBox.Show("Preencha ambos os campos de senha.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (nova.Length < 6)
            {
                MessageBox.Show("A nova senha deve possuir pelo menos 6 caracteres.", "Senha Curta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (nova != confirma)
            {
                MessageBox.Show("As senhas não coincidem. Digite-as novamente.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmaSenha.Focus();
                return;
            }

            // Sucesso na redefinição!
            MessageBox.Show(
                "Sua senha foi redefinida com sucesso!\n\nVocê já pode entrar no Reino utilizando a sua nova senha de acesso.",
                "Senha Atualizada!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            (this.FindForm() as MainForm)?.LoadControl(new UcLogin());
        }

        #endregion
    }
}
