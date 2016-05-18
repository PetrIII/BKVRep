using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace DemoApp
{
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {

            InitializeComponent();
            List<String> names = new List<string>();
            names.Add("Быков Константин Викторович");
            names.Add("Александрова Надежда Анатольевна");
            names.Add("Кашин Сергей Владимирович");
            names.Add("Карпенко Илья Сергеевич");
            names.Add("Шек Ирина Андреевна");
            names.Add("Маслов Роман Михайлович");

            FilteredComboBox1.IsEditable = true;
            FilteredComboBox1.IsTextSearchEnabled = false;
            FilteredComboBox1.ItemsSource = names;

            var comboBoxItemstyle = new Style(typeof(ComboBoxItem));
            comboBoxItemstyle.Setters.Add(
                  new EventSetter(PreviewKeyDownEvent,
                          new KeyEventHandler(OnPreviewKeyDown)));
            this.Resources.Add(typeof(ComboBoxItem), comboBoxItemstyle);

        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ComboBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("dflkj");
        }

    }
}