using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kompas3d_monitor
{
    public partial class MainForm : Form
    {
        private Builder _builder;
        private Parameters _parameters = new Parameters();

        private void BuildModel()
        {

        }

        public MainForm()
        {

            InitializeComponent();
        }

        private void MainValidate(ParameterType parameterType, ref TextBox textBoxTemp)
        {
            try
            {
                double value;
                if (double.TryParse(textBoxTemp.Text, out value))
                {
                    _parameters.AddValueToParameter(parameterType, value);
                    textBoxTemp.BackColor = Color.White;
                }
                else
                    throw new FormatException($"Строка '{textBoxTemp.Text}' не может быть преобразована в тип double.");
            }
            catch (Exception ex)
            {
                textBoxLog.Text += parameterType.ToString() + ": " + ex.Message + "\r\n";
                textBoxTemp.BackColor = Color.Red;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
           MainValidate(ParameterType.BorderHeight, ref textBox3);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            MainValidate(ParameterType.BorderThickness, ref textBox4);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
