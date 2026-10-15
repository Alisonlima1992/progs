using Microsoft.Win32;
using progs;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;

namespace EnemyEditor
{
    public partial class MainWindow : Window
    {
        private const int IconListSize = 64;

        private List<EnemyIcon> enemyIcons = new List<EnemyIcon>();

        private CEnemyTemplateList enemyTemplates = new CEnemyTemplateList();

        private string selectedIconName = null;

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

                selectedIconName = Path.GetFileName(selectedImage.Source.ToString());

                IconNameBox.Text = selectedIconName;

                this.Title = "Редактор противников — выбрана: " + selectedIconName;
            }
        }

        private void AddEnemyButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите имя противника.");
                return;
            }

            if (string.IsNullOrWhiteSpace(selectedIconName))
            {
                MessageBox.Show("Выберите иконку в списке слева.");
                return;
            }

            if (enemyTemplates.GetEnemyByName(NameBox.Text) != null)
            {
                MessageBox.Show("Противник с таким именем уже существует.");
                return;
            }

            if (!int.TryParse(BaseLifeBox.Text, out int baseLife))
            {
                MessageBox.Show("BaseLife — целое число.");
                return;
            }

            if (!double.TryParse(LifeModifierBox.Text,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double lifeModifier))
            {
                MessageBox.Show("LifeModifier — число (например, 1.0).");
                return;
            }

            if (!int.TryParse(BaseGoldBox.Text, out int baseGold))
            {
                MessageBox.Show("BaseGold — целое число.");
                return;
            }

            if (!double.TryParse(GoldModifierBox.Text,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double goldModifier))
            {
                MessageBox.Show("GoldModifier — число (например, 1.0).");
                return;
            }

            if (!double.TryParse(SpawnChanceBox.Text,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double spawnChance))
            {
                MessageBox.Show("SpawnChance — число (например, 0.5).");
                return;
            }

            enemyTemplates.AddEnemy(
                NameBox.Text, selectedIconName,
                baseLife, lifeModifier,
                baseGold, goldModifier,
                spawnChance);

            RefreshEnemiesList();

            NameBox.Text = "";
        }

        private void DeleteEnemyButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnemiesListBox.SelectedItem is CEnemyTemplate selected)
            {
                enemyTemplates.DeleteEnemyByName(selected.Name);
                RefreshEnemiesList();
            }
            else
            {
                MessageBox.Show("Выберите противника в списке.");
            }
        }
        private void RefreshEnemiesList()
        {
            EnemiesListBox.ItemsSource = null;
            EnemiesListBox.ItemsSource = enemyTemplates.GetEnemies();
        }

        private void SaveJsonButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Сохранить список противников",
                Filter = "JSON файлы (*.json)|*.json",
                DefaultExt = ".json",
                FileName = "enemies"
            };

            if (dialog.ShowDialog() == true)
            {
                enemyTemplates.SaveToJson(dialog.FileName);
                MessageBox.Show("Сохранено: " + dialog.FileName);
            }
        }

        private void LoadJsonButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Загрузить список противников",
                Filter = "JSON файлы (*.json)|*.json",
                DefaultExt = ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    enemyTemplates.LoadFromJson(dialog.FileName);
                    RefreshEnemiesList();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки JSON: " + ex.Message);
                }
            }
        }

        private void EnemiesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EnemiesListBox.SelectedItem is CEnemyTemplate selected)
            {
                NameBox.Text = selected.Name;
                IconNameBox.Text = selected.IconName;
                BaseLifeBox.Text = selected.BaseLife.ToString();
                LifeModifierBox.Text = selected.LifeModifier.ToString(
                    System.Globalization.CultureInfo.InvariantCulture);
                BaseGoldBox.Text = selected.BaseGold.ToString();
                GoldModifierBox.Text = selected.GoldModifier.ToString(
                    System.Globalization.CultureInfo.InvariantCulture);
                SpawnChanceBox.Text = selected.SpawnChance.ToString(
                    System.Globalization.CultureInfo.InvariantCulture);

                selectedIconName = selected.IconName;

                var icon = enemyIcons.FirstOrDefault(i => i.Name == selected.IconName);
                if (icon != null)
                {
                    MainEnemyIcon.Source = new BitmapImage(new System.Uri(icon.ImagePath));
                }
            }
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(EnemiesListBox.SelectedItem is CEnemyTemplate selected))
            {
                MessageBox.Show("Выберите противника в списке.");
                return;
            }

            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите имя противника.");
                return;
            }

            var existing = enemyTemplates.GetEnemyByName(NameBox.Text);
            if (existing != null && existing != selected)
            {
                MessageBox.Show("Противник с таким именем уже существует.");
                return;
            }

            if (!int.TryParse(BaseLifeBox.Text, out int baseLife)) { MessageBox.Show("BaseLife — целое число."); return; }
            if (!double.TryParse(LifeModifierBox.Text, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double lifeModifier))
            { MessageBox.Show("LifeModifier — число."); return; }
            if (!int.TryParse(BaseGoldBox.Text, out int baseGold)) { MessageBox.Show("BaseGold — целое число."); return; }
            if (!double.TryParse(GoldModifierBox.Text, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double goldModifier))
            { MessageBox.Show("GoldModifier — число."); return; }
            if (!double.TryParse(SpawnChanceBox.Text, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double spawnChance))
            { MessageBox.Show("SpawnChance — число."); return; }

            enemyTemplates.DeleteEnemyByName(selected.Name);
            enemyTemplates.AddEnemy(NameBox.Text, selectedIconName ?? selected.IconName,
                baseLife, lifeModifier, baseGold, goldModifier, spawnChance);

            RefreshEnemiesList();

        }
    }
}
