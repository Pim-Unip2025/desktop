using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TronusDesktop.UI;

namespace TronusDesktop.Views
{
    public partial class UcCharacter : UserControl
    {
        private TronusPanel mainCard;
        private Label titleLabel;
        
        // Steps
        private Panel step1Panel;
        private Panel step2Panel;
        private Panel step3Panel;
        private Panel step4Panel;

        // Dados coletados durante o fluxo (Para uso da API)
        private string selectedGender = "";
        private string characterName = "";
        private string selectedAvatar = "";

        public UcCharacter()
        {
            InitializeComponent();
            SetupUI();
        }

        #region Configuração Principal
        private void SetupUI()
        {
            this.BackColor = Theme.Background;
            string bgPath = Path.Combine(Application.StartupPath, "img", "fundo_reinos.jpg");
            if (File.Exists(bgPath))
            {
                this.BackgroundImage = Image.FromFile(bgPath);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }

            mainCard = new TronusPanel
            {
                Size = new Size(600, 500),
                Location = new Point((1024 - 600) / 2, (728 - 500) / 2)
            };
            this.Controls.Add(mainCard);

            titleLabel = new Label
            {
                Text = "CRIAÇÃO DE PERSONAGEM",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(600, 40),
                Location = new Point(0, 20),
                BackColor = Color.Transparent
            };
            mainCard.Controls.Add(titleLabel);

            BuildStep1();
            BuildStep2();
            BuildStep3();
            BuildStep4();

            ShowStep(1); // Inicia no passo 1
        }
        #endregion

        #region Lógica de Etapas (Wizard)
        
        /// <summary>
        /// Passo 1: O usuário seleciona o gênero do personagem.
        /// </summary>
        private void BuildStep1()
        {
            step1Panel = new Panel { Size = new Size(600, 400), Location = new Point(0, 80), BackColor = Color.Transparent };
            mainCard.Controls.Add(step1Panel);

            var lbl = new Label { Text = "1. Escolha seu Gênero", ForeColor = Color.White, Font = new Font("Segoe UI", 14F), AutoSize = true, Location = new Point(200, 20) };
            step1Panel.Controls.Add(lbl);

            var btnM = new TronusButton { Text = "Masculino", Size = new Size(200, 45), Location = new Point(100, 100) };
            btnM.Click += (s, e) => { selectedGender = "male"; ShowStep(2); };
            step1Panel.Controls.Add(btnM);

            var btnF = new TronusButton { Text = "Feminino", Size = new Size(200, 45), Location = new Point(320, 100) };
            btnF.Click += (s, e) => { selectedGender = "female"; ShowStep(2); };
            step1Panel.Controls.Add(btnF);
        }

        /// <summary>
        /// Passo 2: O usuário digita o nome do personagem.
        /// </summary>
        private void BuildStep2()
        {
            step2Panel = new Panel { Size = new Size(600, 400), Location = new Point(0, 80), BackColor = Color.Transparent };
            mainCard.Controls.Add(step2Panel);

            var lbl = new Label { Text = "2. Nome do Personagem", ForeColor = Color.White, Font = new Font("Segoe UI", 14F), AutoSize = true, Location = new Point(190, 20) };
            step2Panel.Controls.Add(lbl);

            var nameBox = new TextBox { Size = new Size(400, 30), Location = new Point(100, 100), Font = new Font("Segoe UI", 12F), BackColor = Color.FromArgb(40, 40, 40), ForeColor = Color.White };
            step2Panel.Controls.Add(nameBox);

            var btnNext = new TronusButton { Text = "Continuar", Size = new Size(200, 45), Location = new Point(200, 180) };
            btnNext.Click += (s, e) => {
                if (string.IsNullOrWhiteSpace(nameBox.Text)) { MessageBox.Show("Digite o nome."); return; }
                characterName = nameBox.Text;
                LoadAvatars();
                ShowStep(3);
            };
            step2Panel.Controls.Add(btnNext);
        }

        /// <summary>
        /// Passo 3: Dinamicamente carrega os avatares baseados no gênero selecionado.
        /// </summary>
        private void LoadAvatars()
        {
            step3Panel.Controls.Clear();
            var lbl = new Label { Text = "3. Escolha sua Aparência", ForeColor = Color.White, Font = new Font("Segoe UI", 14F), AutoSize = true, Location = new Point(190, 10) };
            step3Panel.Controls.Add(lbl);

            string prefix = selectedGender == "male" ? "masc" : "fem";
            for (int i = 1; i <= 3; i++)
            {
                string imgName = $"{prefix}{i}.png";
                string imgPath = Path.Combine(Application.StartupPath, "img", imgName);

                var pb = new PictureBox
                {
                    Size = new Size(120, 120),
                    Location = new Point(70 + ((i - 1) * 160), 80),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.Transparent,
                    Cursor = Cursors.Hand,
                    BorderStyle = BorderStyle.FixedSingle
                };

                if (File.Exists(imgPath)) pb.Image = Image.FromFile(imgPath);

                int index = i;
                pb.Click += (s, e) => {
                    selectedAvatar = imgPath;
                    ShowStep(4);
                };

                step3Panel.Controls.Add(pb);
            }
        }

        private void BuildStep3()
        {
            step3Panel = new Panel { Size = new Size(600, 400), Location = new Point(0, 80), BackColor = Color.Transparent };
            mainCard.Controls.Add(step3Panel);
        }

        private void BuildStep4()
        {
            step4Panel = new Panel { Size = new Size(600, 400), Location = new Point(0, 80), BackColor = Color.Transparent };
            mainCard.Controls.Add(step4Panel);
            
            // Populated dynamically in ShowStep(4)
        }

        private void PopulateStep4()
        {
            step4Panel.Controls.Clear();
            var lbl = new Label { Text = "Ficha do Personagem", ForeColor = Theme.Gold, Font = new Font("Segoe UI", 16F, FontStyle.Bold), AutoSize = true, Location = new Point(200, 10) };
            step4Panel.Controls.Add(lbl);

            var pb = new PictureBox
            {
                Size = new Size(150, 150),
                Location = new Point(225, 60),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            if (File.Exists(selectedAvatar)) pb.Image = Image.FromFile(selectedAvatar);
            step4Panel.Controls.Add(pb);

            var nameLbl = new Label { Text = $"Herói: {characterName}", ForeColor = Color.White, Font = new Font("Segoe UI", 14F), AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Size = new Size(600, 30), Location = new Point(0, 230) };
            step4Panel.Controls.Add(nameLbl);

            var btnPlay = new TronusButton { Text = "ENTRAR NO REINO", Size = new Size(250, 50), Location = new Point(175, 290) };
            
            btnPlay.Click += async (s, e) => 
            {
                btnPlay.Enabled = false;
                btnPlay.Text = "CRIANDO...";

                try
                {
                    string avatarRelativo = "img/" + Path.GetFileName(selectedAvatar);

                    var req = new TronusDesktop.Models.CriarPersonagemRequest
                    {
                        Nome = characterName,
                        Genero = selectedGender,
                        Avatar = avatarRelativo
                    };

                    var res = await TronusDesktop.Services.ApiService.Instance.PostAsync<TronusDesktop.Models.CriarPersonagemRequest, TronusDesktop.Models.PersonagemDto>("/personagens", req);

                    MessageBox.Show($"Personagem {res.Nome} criado!\nTítulo: {res.TituloAtual}", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Avançar para a tela de Reinos
                    (this.FindForm() as MainForm)?.LoadControl(new UcReinos());
                }
                catch (TronusDesktop.Services.ApiException ex)
                {
                    MessageBox.Show(ex.Message, "Erro na Criação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro de conexão: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnPlay.Enabled = true;
                    btnPlay.Text = "ENTRAR NO REINO";
                }
            };
            
            step4Panel.Controls.Add(btnPlay);
        }

        /// <summary>
        /// Gerencia qual painel (passo) deve estar visível na tela.
        /// </summary>
        private void ShowStep(int stepIndex)
        {
            step1Panel.Visible = stepIndex == 1;
            step2Panel.Visible = stepIndex == 2;
            step3Panel.Visible = stepIndex == 3;
            step4Panel.Visible = stepIndex == 4;

            if (stepIndex == 4) PopulateStep4();
        }
        #endregion
    }
}
