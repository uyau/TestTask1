using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTestTask1
{
    public partial class MainWindow : Window
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Listener(object sender, TextChangedEventArgs args)
        {
            if (Regex.IsMatch(TexBox.Text, @"^[0-9 ;,]+$") == false)
            {
                TexBox.Background = Brushes.Red;
            }
            else
                TexBox.Background = Brushes.White;

        }

        private async void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (!TexBox.Text.Any() || Regex.IsMatch(TexBox.Text, @"^[0-9 ;,]+$") == false)
            {
                MessageBox.Show("Вводимый формат: числа, разделенные запятой или точкой с запятой");
                return;
            }

            searchGrid.Children.Clear();
            // creating table for request
            Border brdr = new Border();
            brdr.BorderThickness = new Thickness(1, 1, 1, 1);
            brdr.BorderBrush = new SolidColorBrush(Colors.Black);
            Grid.SetRow(brdr, 1);
            myGrid.Children.Add(brdr);

            
            searchGrid.Width = 800;
            searchGrid.RowDefinitions.Add(new RowDefinition());
            searchGrid.ColumnDefinitions.Add(new ColumnDefinition());
            searchGrid.ColumnDefinitions.Add(new ColumnDefinition());
            searchGrid.ColumnDefinitions.Add(new ColumnDefinition());
            searchGrid.ColumnDefinitions[0].Width = new GridLength(500);
            searchGrid.ColumnDefinitions[1].Width = new GridLength(150);
            searchGrid.ColumnDefinitions[2].Width = new GridLength(150);
            searchGrid.RowDefinitions[0].Height = new GridLength(30);
            
            TextBlock title1 = new();
            TextBlock title2 = new();
            TextBlock title3 = new();
            title1.TextAlignment = TextAlignment.Center;
            title2.TextAlignment = TextAlignment.Center;
            title3.TextAlignment = TextAlignment.Center;
            title1.Text = "Текст";
            title2.Text = "Количество\n слов";
            title3.Text = "Количество\n гласных";
            title1.FontWeight = FontWeights.Bold;
            title2.FontWeight = FontWeights.Bold;
            title3.FontWeight = FontWeights.Bold;
            Grid.SetColumn(title1, 0);
            Grid.SetRow(title1, 0);
            Grid.SetColumn(title2, 1);
            Grid.SetRow(title2, 0);
            Grid.SetColumn(title3, 2);
            Grid.SetRow(title3, 0);
            searchGrid.Children.Add(title1);
            searchGrid.Children.Add(title2);
            searchGrid.Children.Add(title3);
            
            string textBoxString = TexBox.Text;
            var deleteSpace = textBoxString.Replace(" ", "");//remove spacese
            string[] idOfStrings = textBoxString.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i=0;i<idOfStrings.Length;i++)//remove extra zeros
            {
                byte temp = byte.Parse(idOfStrings[i]);
                int toInt = (int)temp;
                idOfStrings[i] = toInt.ToString();
            }

            idOfStrings = idOfStrings.Distinct().ToArray();
            int tempRow = 1;
            foreach (string id in idOfStrings)//add new row with request contains
            {
                searchGrid.RowDefinitions.Add(new RowDefinition());
                searchGrid.ColumnDefinitions.Add(new ColumnDefinition());
                searchGrid.ColumnDefinitions.Add(new ColumnDefinition());
                searchGrid.ColumnDefinitions.Add(new ColumnDefinition());

                HttpClient.DefaultRequestHeaders.Add("TMG-Api-Key", "0J/RgNC40LLQtdGC0LjQutC4IQ==");
                try
                {
                    HttpResponseMessage response = await HttpClient.GetAsync($"http://tmgwebtest.azurewebsites.net/api/textstrings/{id}");
                    var responseBody = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(responseBody);
                    string value = json.Value<string>("text");
                    TextBlock text = new();
                    text.Text = value;
                    Grid.SetRow(text,tempRow);
                    Grid.SetColumn(text, 0);
                    text.TextWrapping = TextWrapping.Wrap;
                    text.Padding = new Thickness(0, 7, 0, 0);
                    searchGrid.Children.Add(text);

                    string[] source = value.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',', '-' }, StringSplitOptions.RemoveEmptyEntries);
                    int wordsnumber = source.Length;
                    TextBlock wordsNum = new();
                    wordsNum.Text = wordsnumber.ToString();
                    Grid.SetRow(wordsNum, tempRow);
                    Grid.SetColumn(wordsNum,1);
                    wordsNum.TextAlignment = TextAlignment.Center;
                    wordsNum.Padding = new Thickness(0, 7, 0, 0);
                    searchGrid.Children.Add(wordsNum);

                    int count = Regex.Matches(value, @"[ауоыиэяюёеAEIOUиеъауоáéíóőúűαεηιουωaeiouyæøåáíéóúāēīūąęėįųūɐɛiɔʊäöüęąăâîáäéíóôúýåäöýůúóíěéáåäöõäöü]", RegexOptions.IgnoreCase).Count;
                    TextBlock vowelsNum = new();
                    vowelsNum.Text = count.ToString();
                    Grid.SetRow(vowelsNum, tempRow);
                    Grid.SetColumn(vowelsNum, 2);
                    vowelsNum.Padding = new Thickness(0, 7, 0, 0);
                    vowelsNum.TextAlignment = TextAlignment.Center;
                    searchGrid.Children.Add(vowelsNum);

                }
                catch (Exception)
                {
                    string ups = $"Запрос к строкe: {id} вызвал ошибку";
                    TextBlock error = new();
                    error.Text = ups;
                    Grid.SetRow(error, tempRow);
                    Grid.SetColumn(error, 0);
                    error.Padding = new Thickness(0,7,0,0);
                    searchGrid.Children.Add(error);
                }
                HttpClient.DefaultRequestHeaders.Clear();
                tempRow++;
            }
            
        }
    }
}
