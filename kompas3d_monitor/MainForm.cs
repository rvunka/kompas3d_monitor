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

//TODO: RSDN
namespace kompas3d_monitor
{
    public partial class MainForm : Form
    {
        private Builder _builder;
        private Parameters _parameters = new Parameters();

        private Dictionary<AspectRatio, string> aspectRatioDisplayValues = new Dictionary<AspectRatio, string>
        {
            { AspectRatio.Custom, "Custom" },
            { AspectRatio.FourThree, "4_3" },
            { AspectRatio.SixteenTen, "16_10" },
            { AspectRatio.SixteenNine, "16_9" },
            { AspectRatio.TwentyOneNine, "21_9" }
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
            comboBox1.DisplayMember = "Value"; // Что отображать пользователю
            comboBox1.ValueMember = "Key"; // Реальное значение (AspectRatio)
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
        private void textBox1_Leave(object sender, EventArgs e)
        {
            MainValidate(ParameterType.ScreenWidth, ref textBox1);
            MainValidate(ParameterType.ScreenHeight, ref textBox2);
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            MainValidate(ParameterType.ScreenHeight, ref textBox2);
            MainValidate(ParameterType.ScreenWidth, ref textBox1);
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            MainValidate(ParameterType.ScreenThickness, ref textBox3);
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            MainValidate(ParameterType.BorderHeight, ref textBox4);
        }

        private void textBox5_Leave(object sender, EventArgs e)
        {
            MainValidate(ParameterType.BorderWidth, ref textBox5);
        }

        private void textBox6_Leave(object sender, EventArgs e)
        {
            MainValidate(ParameterType.BorderDepth, ref textBox6);
        }

        private void textBox7_Leave(object sender, EventArgs e)
        {
            MainValidate(ParameterType.StandHeight, ref textBox7);
        }

        private void textBox8_Leave(object sender, EventArgs e)
        {
            MainValidate(ParameterType.StandWidth, ref textBox8);
        }

        private void textBox9_Leave(object sender, EventArgs e)
        {
            MainValidate(ParameterType.StandThickness, ref textBox9);
        }

        private void textBox10_Leave(object sender, EventArgs e)
        {
            MainValidate(ParameterType.BaseHeight, ref textBox10);
        }

        private void textBox11_Leave(object sender, EventArgs e)
        {
            MainValidate(ParameterType.BaseWidth, ref textBox11);
        }

        private void textBox12_Leave(object sender, EventArgs e)
        {
            MainValidate(ParameterType.BaseThickness, ref textBox12);
        }

        private void textBox13_Leave(object sender, EventArgs e)
        {
            MainValidate(ParameterType.JointHeight, ref textBox13);
        }

        private void textBox14_Leave(object sender, EventArgs e)
        {
            MainValidate(ParameterType.JointWidth, ref textBox14); 
        }

        private void textBox15_Leave(object sender, EventArgs e)
        {
            MainValidate(ParameterType.JointLenght, ref textBox15);
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

        private void BuildModel()
        {
            try
            {
                if(!ValidateAllTextBoxes())
                {
                    throw new FormatException();
                }

                // Строим модель монитора с использованием параметров
                _builder.Build(_parameters);

                // Выводим сообщение о завершении построения
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

            // Перебор всех типов параметров
            foreach (ParameterType parameterType in Enum.GetValues(typeof(ParameterType)))
            {
                // Определяем соответствующее текстовое поле по имени
                string textBoxName = $"textBox{(int)parameterType + 1}";
                var textBox = groupBox1.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == textBoxName)
                              ?? groupBox2.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == textBoxName);

                if (textBox != null)
                {
                    try
                    {
                        // Пробуем преобразовать значение в double
                        double value;
                        if (double.TryParse(textBox.Text, out value))
                        {
                            // Если успешно, обновляем параметры
                            _parameters.AddValueToParameter(parameterType, value);
                            textBox.BackColor = Color.White; // Убираем подсветку
                        }
                        else
                        {
                            throw new FormatException($"Строка '{textBox.Text}' не может быть преобразована в тип double.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Логируем ошибку
                        textBoxLog.Text += parameterType.ToString() + ": " + ex.Message + "\r\n";
                        textBox.BackColor = Color.Red; // Подсвечиваем некорректное поле
                        allValid = false; // Устанавливаем флаг ошибки
                    }
                }
            }

            return allValid;
        }
    }


}
