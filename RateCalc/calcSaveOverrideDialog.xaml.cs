using Functions;
using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Input;

namespace RateCalc
{
    public partial class calcSaveOverrideDialog : MetroWindow
    {
        public bool OverwriteConfirmed { get; private set; }

        public calcSaveOverrideDialog(string message, string title)
        {
            InitializeComponent();
            lblQuestionTitle.Content = title;
            lblMessage.Content = message;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            OverwriteConfirmed = true;
            DialogResult = true;
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            OverwriteConfirmed = false;
            DialogResult = false;
        }
        private void taskBarDrag_Down(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TaskFunctions.DragWindow(sender, e);
        }
    }
}