using Microsoft.Win32;
using progs;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EnemyEditor
{
    public partial class MainWindow : Window
    {
        private const int IconListSize = 64;
        private const int MainIconSize = 128;

        private List<EnemyIcon> enemyIcons = new List<EnemyIcon>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Выберите папку с иконками противников (.png)"
            };

            if (dialog.ShowDialog() == true)
            {
                LoadIconsFromFolder(dialog.FolderName);
                ShowIconsInListBox();
            }
        }

        public void LoadIconsFromFolder(string path)
        {
            enemyIcons.Clear();

            string filter = "*.png";
            string[] files = Directory.GetFiles(path, filter);

            foreach (string file in files)
            {
                enemyIcons.Add(new EnemyIcon
                {
                    Name = Path.GetFileName(file),
                    ImagePath = file
                });
            }
        }

        private void ShowIconsInListBox()
        {
            IconsListBox.Items.Clear();

            foreach (EnemyIcon icon in enemyIcons)
            {
                var panel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(4)
                };

                var image = new Image
                {
                    Source = new BitmapImage(new System.Uri(icon.ImagePath)),
                    Width = IconListSize,
                    Height = IconListSize,
                    Stretch = Stretch.Uniform
                };

                var label = new TextBlock
                {
                    Text = icon.Name,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 0, 0, 0)
                };

                panel.Children.Add(image);
                panel.Children.Add(label);

                IconsListBox.Items.Add(panel);
            }
        }

        private void IconsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;

            if (listBox?.SelectedItem is StackPanel panel
                && panel.Children.Count > 0
                && panel.Children[0] is Image selectedImage)
            {
                MainEnemyIcon.Source = new BitmapImage(
                    new System.Uri(selectedImage.Source.ToString()));

                string iconName = Path.GetFileName(selectedImage.Source.ToString());
                this.Title = "Редактор противников — выбрана: " + iconName;
            }
        }
    }
}
