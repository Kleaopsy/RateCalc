using Functions;
using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Input;

namespace RateCalc
{
    public partial class calcSaveDialog : MetroWindow
    {
        public string? ResponseText { get; private set; } // Bu property'yi ekledim

        public calcSaveDialog(string question, string title, string defaultResponse)
        {
            InitializeComponent(); // Bu XAML'den gelecek
            lblQuestionTitle.Content = title;
            lblQuestion.Content = question;
            txtResponse.Text = defaultResponse;
            txtResponse.Focus();
            txtResponse.SelectAll();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            ResponseText = txtResponse.Text;
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void txtResponse_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOK_Click(sender, e);
            }
        }
        private void taskBarDrag_Down(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TaskFunctions.DragWindow(sender, e);
        }
    }
}