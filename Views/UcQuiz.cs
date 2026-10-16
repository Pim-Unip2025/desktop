using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TronusDesktop.Models;
using TronusDesktop.Services;
using TronusDesktop.UI;

namespace TronusDesktop.Views
{
    public partial class UcQuiz : UserControl
    {
        private readonly int _faseId;
        private readonly string _faseNome;
        private readonly string _reinoNome;

        private List<QuestaoDto> _questoes = new();
        private Dictionary<int, int> _respostasMarcadas = new(); // questaoId -> alternativaId
        private int _indiceAtual = 0;

        // Elementos visuais
        private Label headerLabel;
        private Label questaoNumLabel;
        private TronusPanel questionCard;
        private Label dificuldadeLabel;
        private TextBox enunciadoBox;
        private FlowLayoutPanel flowAlternativas;
        private TronusButton btnAnterior;
        private TronusButton btnProxima;
        private TronusButton btnSair;

        // Painel de Resultados
        private TronusPanel resultCard;

        public UcQuiz(int faseId, string faseNome, string reinoNome)
        {
            _faseId = faseId;
            _faseNome = faseNome;
            _reinoNome = reinoNome;

            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "UcQuiz";
            this.Size = new Size(1024, 728);
            this.Load += UcQuiz_Load;
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            this.BackColor = Theme.Background;

            headerLabel = new Label
            {
                Text = $"{_reinoNome.ToUpper()} — {_faseNome.ToUpper()}",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(40, 25)
            };
            this.Controls.Add(headerLabel);

            questaoNumLabel = new Label
            {
                Text = "Carregando desafio...",
                ForeColor = Theme.DimText,
                Font = new Font("Segoe UI", 11F),
                AutoSize = true,
                Location = new Point(40, 65)
            };
            this.Controls.Add(questaoNumLabel);

            btnSair = new TronusButton
            {
                Text = "Abandonar Desafio",
                Size = new Size(160, 35),
                Location = new Point(824, 25),
                Font = new Font("Segoe UI", 9F)
            };
            btnSair.Click += (s, e) =>
            {
                var confirm = MessageBox.Show("Deseja realmente sair? As respostas não salvas serão perdidas.", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    (this.FindForm() as MainForm)?.LoadControl(new UcReinos());
                }
            };
            this.Controls.Add(btnSair);

            // Card principal da questão
            questionCard = new TronusPanel
            {
                Location = new Point(40, 100),
                Size = new Size(944, 580)
            };
            this.Controls.Add(questionCard);

            dificuldadeLabel = new Label
            {
                Text = "DIFICULDADE: MÉDIO",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 18)
            };
            questionCard.Controls.Add(dificuldadeLabel);

            enunciadoBox = new TextBox
            {
                Location = new Point(25, 45),
                Size = new Size(894, 180),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.FromArgb(20, 20, 20),
                ForeColor = Theme.PrimaryText,
                Font = new Font("Segoe UI", 11.5F),
                BorderStyle = BorderStyle.None
            };
            questionCard.Controls.Add(enunciadoBox);

            flowAlternativas = new FlowLayoutPanel
            {
                Location = new Point(25, 240),
                Size = new Size(894, 260),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            questionCard.Controls.Add(flowAlternativas);

            btnAnterior = new TronusButton
            {
                Text = "← Anterior",
                Location = new Point(25, 520),
                Size = new Size(130, 40),
                Enabled = false
            };
            btnAnterior.Click += (s, e) => NavegarQuestao(-1);
            questionCard.Controls.Add(btnAnterior);

            btnProxima = new TronusButton
            {
                Text = "Próxima →",
                Location = new Point(789, 520),
                Size = new Size(130, 40)
            };
            btnProxima.Click += BtnProxima_Click;
            questionCard.Controls.Add(btnProxima);
        }

        private async void UcQuiz_Load(object sender, EventArgs e)
        {
            try
            {
                _questoes = await ApiService.Instance.GetAsync<List<QuestaoDto>>($"/fases/{_faseId}/questoes");
                if (_questoes == null || _questoes.Count == 0)
                {
                    MessageBox.Show("Esta fase ainda não possui questões cadastradas.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    (this.FindForm() as MainForm)?.LoadControl(new UcReinos());
                    return;
                }

                ExibirQuestao(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar o quiz: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                (this.FindForm() as MainForm)?.LoadControl(new UcReinos());
            }
        }

        private void ExibirQuestao(int index)
        {
            if (index < 0 || index >= _questoes.Count) return;

            _indiceAtual = index;
            var q = _questoes[index];

            questaoNumLabel.Text = $"Questão {index + 1} de {_questoes.Count}";
            dificuldadeLabel.Text = $"DIFICULDADE: {q.NivelDificuldade.ToUpper()}";
            enunciadoBox.Text = q.Enunciado.Replace("\r\n", Environment.NewLine).Replace("\n", Environment.NewLine);

            flowAlternativas.Controls.Clear();

            int marcadaId = _respostasMarcadas.ContainsKey(q.Id) ? _respostasMarcadas[q.Id] : -1;

            foreach (var alt in q.Alternativas.OrderBy(a => a.Letra))
            {
                bool isSelected = alt.Id == marcadaId;

                var btnAlt = new Button
                {
                    Text = $"  [{alt.Letra}]  {alt.Texto}",
                    Size = new Size(865, 45),
                    Margin = new Padding(0, 0, 0, 8),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 10.5F),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = isSelected ? Color.FromArgb(40, 100, 60) : Color.FromArgb(30, 30, 30),
                    ForeColor = isSelected ? Theme.Gold : Theme.PrimaryText,
                    Cursor = Cursors.Hand
                };
                btnAlt.FlatAppearance.BorderColor = isSelected ? Theme.Gold : Color.FromArgb(60, 60, 60);
                btnAlt.FlatAppearance.BorderSize = isSelected ? 2 : 1;

                btnAlt.Click += (s, e) =>
                {
                    _respostasMarcadas[q.Id] = alt.Id;
                    ExibirQuestao(_indiceAtual); // Redesenha com a opção destacada
                };

                flowAlternativas.Controls.Add(btnAlt);
            }

            btnAnterior.Enabled = _indiceAtual > 0;
            if (_indiceAtual == _questoes.Count - 1)
            {
                btnProxima.Text = "FINALIZAR ⚔";
            }
            else
            {
                btnProxima.Text = "Próxima →";
            }
        }

        private void NavegarQuestao(int delta)
        {
            int novoIndice = _indiceAtual + delta;
            if (novoIndice >= 0 && novoIndice < _questoes.Count)
            {
                ExibirQuestao(novoIndice);
            }
        }

        private void BtnProxima_Click(object sender, EventArgs e)
        {
            var qAtual = _questoes[_indiceAtual];
            if (!_respostasMarcadas.ContainsKey(qAtual.Id))
            {
                MessageBox.Show("Por favor, selecione uma alternativa antes de prosseguir.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_indiceAtual < _questoes.Count - 1)
            {
                NavegarQuestao(1);
            }
            else
            {
                FinalizarQuiz();
            }
        }

        private async void FinalizarQuiz()
        {
            // Validação de que todas foram respondidas
            if (_respostasMarcadas.Count < _questoes.Count)
            {
                MessageBox.Show("Ainda há questões não respondidas. Responda todas para finalizar o desafio.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnProxima.Enabled = false;
            btnProxima.Text = "ENVIANDO...";

            try
            {
                var payload = new EnviarRespostasRequest();
                foreach (var kvp in _respostasMarcadas)
                {
                    payload.Respostas.Add(new ItemRespostaRequest
                    {
                        QuestaoId = kvp.Key,
                        AlternativaId = kvp.Value
                    });
                }

                var resultado = await ApiService.Instance.PostAsync<EnviarRespostasRequest, ResultadoQuizResponse>($"/fases/{_faseId}/responder", payload);

                ExibirResultado(resultado);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao enviar respostas: " + ex.Message, "Falha", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnProxima.Enabled = true;
                btnProxima.Text = "FINALIZAR ⚔";
            }
        }

        private void ExibirResultado(ResultadoQuizResponse res)
        {
            questionCard.Visible = false;
            btnSair.Visible = false;
            questaoNumLabel.Text = "DESAFIO CONCLUÍDO!";

            resultCard = new TronusPanel
            {
                Location = new Point(212, 100),
                Size = new Size(600, 520)
            };
            this.Controls.Add(resultCard);

            var title = new Label
            {
                Text = "RESULTADO DA CONQUISTA",
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(600, 50),
                Location = new Point(0, 30)
            };
            resultCard.Controls.Add(title);

            string estrelasTxt = new string('★', res.Estrelas) + new string('☆', 3 - res.Estrelas);
            var estrelasLbl = new Label
            {
                Text = estrelasTxt,
                ForeColor = Theme.Gold,
                Font = new Font("Segoe UI", 36F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(600, 60),
                Location = new Point(0, 90)
            };
            resultCard.Controls.Add(estrelasLbl);

            decimal taxa = res.Total > 0 ? Math.Round((decimal)res.Acertos / res.Total * 100, 1) : 0;
            string feedbackTxt = $@"Acertos: {res.Acertos} de {res.Total} ({taxa}%)
Estrelas da Rodada: {res.Estrelas}
Melhor Marca na Fase: {res.MelhorEstrelas} estrelas

Herói: {res.Personagem?.Nome}
Título Atual: {res.Personagem?.TituloAtual}
Total de Estrelas do Herói: {res.Personagem?.TotalEstrelas}";

            var lblFeedback = new Label
            {
                Text = feedbackTxt,
                ForeColor = Theme.PrimaryText,
                Font = new Font("Segoe UI", 12F),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(540, 190),
                Location = new Point(30, 170)
            };
            resultCard.Controls.Add(lblFeedback);

            var btnVoltar = new TronusButton
            {
                Text = "VOLTAR AO MAPA DE REINOS",
                Size = new Size(280, 45),
                Location = new Point(160, 420)
            };
            btnVoltar.Click += (s, e) => (this.FindForm() as MainForm)?.LoadControl(new UcReinos());
            resultCard.Controls.Add(btnVoltar);
        }
    }
}
