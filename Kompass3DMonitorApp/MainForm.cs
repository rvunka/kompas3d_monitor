using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Perfomance;
using MonitorModel;
using MonitorBuilder;
using System.Runtime.CompilerServices;

//TODO: RSDN
namespace Kompass3DMonitorUI
{
    public partial class MainForm : Form
    {
        private Builder _builder;
        private Parameters _parameters = new Parameters();

        private Dictionary<AspectRatio, string> aspectRatioDisplayValues = new Dictionary<AspectRatio, string>
        {
            { AspectRatio.Custom, "Пользовательское" },
            { AspectRatio.FourThree, "4_3" },
            { AspectRatio.SixteenTen, "16_10" },
            { AspectRatio.SixteenNine, "16_9" },
            { AspectRatio.TwentyOneNine, "21_9" }
        };

        private Dictionary<BaseShape, string> baseShapeDisplayValues = new Dictionary<BaseShape, string>
        {
            { BaseShape.Rectangle, "Прямоугольник" },
            { BaseShape.Circle, "Круг" },
            { BaseShape.Trapeze, "Трапеция" }
        };

        public MainForm()
        {
            InitializeComponent();
            InitializeComboBox();
            UpdateTextBoxValues();

            _builder = new Builder();

            // стресс тест
            //StressTester st = new StressTester();
            //st.StressTesting();
        }

        private void UpdateTextBoxValues()
        {
            var parameterTypes = Enum.GetValues(typeof(ParameterType)).Cast<ParameterType>().ToArray();
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                var parameterType = parameterTypes[i];
                string textBoxName = $"textBox{i + 1}";
                var textBox = groupBox1.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == textBoxName);

                if (textBox == null)
                {
                    textBox = groupBox2.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == textBoxName);
                }

                if (textBox != null && _parameters.ParametersDict.ContainsKey(parameterType))
                {
                    textBox.Text = _parameters.ParametersDict[parameterType].Value.ToString();
                }
            }
        }
        private void InitializeComboBox()
        {
            comboBox1.DataSource = new BindingSource(aspectRatioDisplayValues, null);
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";

            comboBox2.DataSource = new BindingSource(baseShapeDisplayValues, null);
            comboBox2.DisplayMember = "Value";
            comboBox2.ValueMember = "Key";
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

            UpdateTextBoxValues();
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                switch (textBox.Name)
                {
                    case "textBox1":
                        MainValidate(ParameterType.ScreenWidth, ref textBox);
                        MainValidate(ParameterType.ScreenHeight, ref textBox2); 
                        break;
                    case "textBox2":
                        MainValidate(ParameterType.ScreenHeight, ref textBox);
                        MainValidate(ParameterType.ScreenWidth, ref textBox1); 
                        break;
                    case "textBox3":
                        MainValidate(ParameterType.ScreenThickness, ref textBox);
                        break;
                    case "textBox4":
                        MainValidate(ParameterType.BorderHeight, ref textBox);
                        break;
                    case "textBox5":
                        MainValidate(ParameterType.BorderWidth, ref textBox);
                        break;
                    case "textBox6":
                        MainValidate(ParameterType.BorderDepth, ref textBox);
                        break;
                    case "textBox7":
                        MainValidate(ParameterType.StandHeight, ref textBox);
                        break;
                    case "textBox8":
                        MainValidate(ParameterType.StandWidth, ref textBox);
                        break;
                    case "textBox9":
                        MainValidate(ParameterType.StandThickness, ref textBox);
                        break;
                    case "textBox10":
                        MainValidate(ParameterType.BaseHeight, ref textBox);
                        break;
                    case "textBox11":
                        MainValidate(ParameterType.BaseWidth, ref textBox);
                        break;
                    case "textBox12":
                        MainValidate(ParameterType.BaseThickness, ref textBox);
                        break;
                    case "textBox13":
                        MainValidate(ParameterType.JointHeight, ref textBox);
                        break;
                    case "textBox14":
                        MainValidate(ParameterType.JointWidth, ref textBox);
                        break;
                    case "textBox15":
                        MainValidate(ParameterType.JointLenght, ref textBox);
                        break;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem is KeyValuePair<AspectRatio, string> selectedItem)
            {
                AspectRatio selectedAspectRatio = selectedItem.Key;
                _parameters.SetAspectRatio(selectedAspectRatio);
            }

            UpdateTextBoxValues();
            MainValidate(ParameterType.ScreenWidth, ref textBox1);
            MainValidate(ParameterType.ScreenHeight, ref textBox2);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem is KeyValuePair<BaseShape, string> selectedItem)
            {
                _parameters.BaseShape = selectedItem.Key;

                if (_parameters.BaseShape == BaseShape.Circle)
                {
                    label10.Text = "Радиус подставки";

                    label9.Visible = false;
                    label11.Visible = false;
                    label21.Visible = false;
                    label23.Visible = false;

                    textBox10.Visible = false;
                    textBox12.Visible = false;
                }
                else
                {
                    label10.Text = "Ширина подставки";

                    label9.Visible = true;
                    label11.Visible = true;
                    label21.Visible = true;
                    label23.Visible = true;

                    textBox10.Visible = true;
                    textBox12.Visible = true;
                }
            }

            UpdateTextBoxValues();
        }

        private void BuildModel()
        {
            try
            {
                if (!ValidateAllTextBoxes())
                {
                    throw new FormatException();
                }

                _builder.Build(_parameters);
                textBoxLog.Text += "Модель монитора построена.\r\n";
            }
            catch (Exception ex)
            {
                textBoxLog.Text += "Ошибка при построении модели: " + ex.Message + "\r\n";
            }
        }

        private void BuildButton_Click(object sender, EventArgs e)
        {
            BuildModel();
        }

        private bool ValidateAllTextBoxes()
        {
            bool allValid = true;

            foreach (ParameterType parameterType in Enum.GetValues(typeof(ParameterType)))
            {
                string textBoxName = $"textBox{(int)parameterType + 1}";
                var textBox = groupBox1.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == textBoxName)
                              ?? groupBox2.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == textBoxName);

                if (textBox != null)
                {
                    try
                    {
                        double value;
                        if (double.TryParse(textBox.Text, out value))
                        {
                            _parameters.AddValueToParameter(parameterType, value);
                            textBox.BackColor = Color.White;
                        }
                        else
                        {
                            throw new FormatException($"Строка '{textBox.Text}' не может быть преобразована в тип double.");
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.Text += parameterType.ToString() + ": " + ex.Message + "\r\n";
                        textBox.BackColor = Color.Red;
                        allValid = false;
                    }
                }
            }

            return allValid;
        }    
    }


}
