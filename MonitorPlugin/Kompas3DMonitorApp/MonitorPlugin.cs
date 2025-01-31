using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Perfomance;
using MonitorModel;
using MonitorBuilder;

namespace Kompas3DMonitorUI
{
    /// <summary>
    /// Основная форма для интерфейса пользователя, позволяющая взаимодействовать с моделью монитора.
    /// </summary>
    public partial class MonitorPlugin : Form
    {
        /// <summary>
        /// Объект, отвечающий за построение модели.
        /// </summary>
        private Builder _builder;

        /// <summary>
        /// Параметры монитора, используемые для построения.
        /// </summary>
        private Parameters _parameters = new Parameters();

        /// <summary>
        /// Словарь ошибок валидации ввода, связанных с текстовыми полями.
        /// </summary>
        private Dictionary<TextBox, Exception> _validationErrors = new Dictionary<TextBox, Exception>();

        /// <summary>
        /// Отображаемые значения соотношений сторон экрана.
        /// </summary>
        private Dictionary<AspectRatio, string> _aspectRatioDisplayValues = new Dictionary<AspectRatio, string>
        {
            { AspectRatio.Custom, "Пользовательское" },
            { AspectRatio.FourThree, "4_3" },
            { AspectRatio.SixteenTen, "16_10" },
            { AspectRatio.SixteenNine, "16_9" },
            { AspectRatio.TwentyOneNine, "21_9" }
        };

        /// <summary>
        /// Отображаемые значения форм подставки.
        /// </summary>
        private Dictionary<BaseShape, string> _baseShapeDisplayValues = new Dictionary<BaseShape, string>
        {
            { BaseShape.Rectangle, "Прямоугольник" },
            { BaseShape.Circle, "Круг" },
            { BaseShape.Trapeze, "Трапеция" }
        };

        /// <summary>
        /// Инициализирует форму и настраивает элементы управления.
        /// </summary>
        public MonitorPlugin()
        {
            InitializeComponent();
            UpdateTextBoxValues();
            InitializeComboBox();

            _builder = new Builder();

            // стресс тест
            //StressTester st = new StressTester();
            //st.StressTesting();
        }

        /// <summary>
        /// Обновляет значения в текстовых полях в зависимости от текущих параметров.
        /// </summary>
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

                    if (_validationErrors.ContainsKey(textBox) && _validationErrors[textBox] == null)
                    {
                        textBox.BackColor = Color.White;
                    }
                    else if (_validationErrors.ContainsKey(textBox) && _validationErrors[textBox] != null)
                    {
                        textBox.BackColor = Color.Red;
                    }
                }
            }
        }  

        /// <summary>
        /// Инициализирует элементы управления ComboBox для выбора соотношения сторон и формы основания.
        /// </summary>
        private void InitializeComboBox()
        {
            comboBoxRatio.DataSource = new BindingSource(_aspectRatioDisplayValues, null);
            comboBoxRatio.DisplayMember = "Value";
            comboBoxRatio.ValueMember = "Key";

            comboBoxShape.DataSource = new BindingSource(_baseShapeDisplayValues, null);
            comboBoxShape.DisplayMember = "Value";
            comboBoxShape.ValueMember = "Key";
        }

        /// <summary>
        /// Фильтрует пользовательский ввод.
        /// </summary>
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != ',')
            {
                e.Handled = true; 
            }

            var textBox = sender as TextBox;
            if (e.KeyChar == ',' && textBox.Text.Contains(','))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Проверяет корректность введенных данных в текстовом поле и обновляет параметры модели.
        /// </summary>
        /// <param name="parameterType">Тип параметра для обновления.</param>
        /// <param name="textBoxTemp">Текстовое поле для ввода значения.</param>
        private void MainValidate(ParameterType parameterType, ref TextBox textBoxTemp)
        {
            try
            {
                double value;
                if (double.TryParse(textBoxTemp.Text, out value))
                {
                    _parameters.AddValueToParameter(parameterType, value);
                    _validationErrors[textBoxTemp] = null; 
                }
                else
                {
                    throw new FormatException($"Строка '{textBoxTemp.Text}' не может быть преобразована в тип double.");
                }
            }
            catch (Exception ex)
            {
                textBoxLog.Text += parameterType.ToString() + ": " + ex.Message + "\r\n";
                _validationErrors[textBoxTemp] = ex; 
            }

            UpdateTextBoxValues();
        }

        /// <summary>
        /// Проверяет, все ли текстовые поля корректны.
        /// </summary>
        /// <returns>Возвращает <c>true</c>, если все текстовые поля корректны; иначе <c>false</c>.</returns>
        private bool AreAllTextBoxesValid()
        {
            foreach (var value in _validationErrors.Values)
            {
                if (value != null)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Обработчик события выхода из текстового поля. Выполняет валидацию значения.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
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

        /// <summary>
        /// Обработчик события изменения выбранного соотношения сторон.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void comboBoxRatio_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxRatio.SelectedItem is KeyValuePair<AspectRatio, string> selectedItem)
                {
                    AspectRatio selectedAspectRatio = selectedItem.Key;
                    _parameters.SetAspectRatio(selectedAspectRatio);
                }

                MainValidate(ParameterType.ScreenWidth, ref textBox1);
                MainValidate(ParameterType.ScreenHeight, ref textBox2);
            }
            catch
            {
                MainValidate(ParameterType.ScreenWidth, ref textBox1);
                MainValidate(ParameterType.ScreenHeight, ref textBox2);
            }

            UpdateTextBoxValues();        
        }

        /// <summary>
        /// Обработчик события изменения выбранной формы основания.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void comboBoxShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxShape.SelectedItem is KeyValuePair<BaseShape, string> selectedItem)
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

        /// <summary>
        /// Строит модель монитора с текущими параметрами.
        /// </summary>
        private void BuildModel()
        {
            if (!AreAllTextBoxesValid())
            {
                MessageBox.Show("Не все параметры корректны. Исправьте ошибки перед построением.",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _builder.Build(_parameters);
        }

        /// <summary>
        /// Обработчик события нажатия кнопки "Построить модель".
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void BuildButton_Click(object sender, EventArgs e)
        {
            BuildModel();
        }
    }


}
