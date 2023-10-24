using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;

namespace AttackSimulator
{
    public partial class AttackSimulatorForm : Form
    {
        private NumericUpDown numAttacksInput;
        private NumericUpDown successProbabilityInput;
        private NumericUpDown numSystemsInput;
        private Button startSimulationButton;
        private CartesianChart chart;
        private List<LineSeries> securityScoreSeries;

        private int numAttacks;
        private double successProbability;
        private int numSystems;
        private int attackIndex = 0;

        public AttackSimulatorForm()
        {
            InitializeComponent();

            numAttacksInput = new NumericUpDown
            {
                Value = 100,
                Location = new System.Drawing.Point(120, 10)
            };
            successProbabilityInput = new NumericUpDown
            {
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 1,
                Value = 0.5M,
                Location = new System.Drawing.Point(120, 40)
            };
            numSystemsInput = new NumericUpDown
            {
                Value = 5,
                Location = new System.Drawing.Point(120, 70)
            };
            startSimulationButton = new Button
            {
                Text = "Start Simulation",
                Location = new System.Drawing.Point(20, 100)
            };

            chart = new CartesianChart
            {
                Location = new System.Drawing.Point(250, 10),
                Width = 600,
                Height = 400
            };

            startSimulationButton.Click += StartSimulationButton_Click;

            Controls.Add(numAttacksInput);
            Controls.Add(successProbabilityInput);
            Controls.Add(numSystemsInput);
            Controls.Add(startSimulationButton);
            Controls.Add(chart);

            securityScoreSeries = new List<LineSeries>();
        }

        private void StartSimulationButton_Click(object sender, EventArgs e)
        {
            numAttacks = (int)numAttacksInput.Value;
            successProbability = (double)successProbabilityInput.Value;
            numSystems = (int)numSystemsInput.Value;

            InitializeChart();

            Timer timer = new Timer();
            timer.Interval = 500;
            timer.Tick += (s, args) => SimulateAttack();
            timer.Start();
        }

        private void InitializeChart()
        {
            chart.Series.Clear();
            securityScoreSeries.Clear();

            for (int i = 0; i < numSystems; i++)
            {
                var series = new LineSeries
                {
                    Title = $"System {i + 1}",
                    Values = new ChartValues<int>(new int[numAttacks])
                };
                securityScoreSeries.Add(series);
                chart.Series.Add(series);
            }

            chart.AxisX.Add(new Axis
            {
                Title = "Number of Attacks Received"
            });
            chart.AxisY.Add(new Axis
            {
                Title = "Security Score",
                LabelFormatter = value => value.ToString()
            });
        }

        private void SimulateAttack()
        {
            if (attackIndex >= numAttacks)
                return;

            for (int i = 0; i < numSystems; i++)
            {
                var series = securityScoreSeries[i];
                int previousScore = attackIndex > 0 ? (int)series.Values[attackIndex - 1] : 0;

                if (new Random().NextDouble() < successProbability)
                    series.Values.Add(previousScore - 1);
                else
                    series.Values.Add(previousScore + 1);
            }

            attackIndex++;

            if (attackIndex == numAttacks)
                attackIndex = 0;
        }
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new AttackSimulatorForm());
    }
}