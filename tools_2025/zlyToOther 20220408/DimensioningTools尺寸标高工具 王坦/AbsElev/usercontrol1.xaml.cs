using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace ANNO_ELEV_ABSL
{
	public partial class MyWindow : Window
	{
		public MyWindow()
		{
			this.InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(true);
			base.Close();
		}
	}
}