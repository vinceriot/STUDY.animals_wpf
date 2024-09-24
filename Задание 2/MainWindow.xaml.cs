using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
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
using System.Xml.Linq;
using static Задание_2.AnimalList;

namespace Задание_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AnimalList animalList;

        // для фильтра
        bool flagFilter = false;
        ConditionCheckDelegate conditionCheckDelegate = null;
        string selectedValue = "";

        bool flagTextBox = false; // изначально в TextBoxы ничего нельзя занести
        bool flagButton = false; // кнопки изначально неактивны
        bool flagAddDelAnimal = false;// изначально недоступны добавление и удаление животных
        //bool flagAdd = false;
        public MainWindow()
        {
            InitializeComponent();
            animalList = new AnimalList();
            UpdateUI();
        }
        private void UpdateUI() // метод для обновления элементов окна
        {
            bool hasAnimals = animalList.HasAnimal();

            bool hasNext;   // есть возможность перейти к следующему животному
            bool hasPrevious;// есть возможность перейти к предыдущему животному 
            if (flagFilter)
            {
                hasNext = animalList.HasNextFiltered(conditionCheckDelegate, FilterValueTextBox.Text);
                hasPrevious = animalList.HasPreviousFiltered(conditionCheckDelegate, FilterValueTextBox.Text);
            }
            else
            {
                hasNext = animalList.HasNext();
                hasPrevious = animalList.HasPrevious();
            }

            bool isHomemade = animalList.IsHomemade();

            DelAnimalMenuButton.IsEnabled = hasAnimals; // элементы активны, если есть животные
            TypeAnimalTextBox.IsEnabled=hasAnimals;
            NameAnimalTextBox.IsEnabled = hasAnimals;
            AgeAnimalTextBox.IsEnabled = hasAnimals;
            SaveButton.IsEnabled = hasAnimals;
            AddAnimalMenuButton.IsEnabled = !flagFilter;

            NextButton.IsEnabled = hasNext;
            NextMenuButton.IsEnabled = hasNext;
            PreviousButton.IsEnabled = hasPrevious;
            PreviousMenuButton.IsEnabled = hasPrevious;

            RemoveFilter.IsEnabled = flagFilter;

            OwnerLabel.Visibility = isHomemade ? Visibility.Visible : Visibility.Hidden;
            OwnerAnimalTextBox.Visibility = isHomemade ? Visibility.Visible : Visibility.Hidden;
            OwnerAnimalTextBox.IsEnabled = isHomemade;
            AssingOwner.IsEnabled=(!isHomemade && hasAnimals);
            AssingOwner.Visibility = !isHomemade ? Visibility.Visible : Visibility.Hidden;
            if (hasAnimals)
            {
                Animal currentAnimal = animalList.GetCurrentAnimal();
                TypeAnimalTextBox.Text = currentAnimal.Type;
                NameAnimalTextBox.Text = currentAnimal.Name;
                AgeAnimalTextBox.Text = currentAnimal.Age.ToString();
                if (isHomemade) 
                {
                    HomeAnimal homeAnimal = (HomeAnimal)currentAnimal;
                    OwnerAnimalTextBox.Text = homeAnimal.Owner;
                }
            }
            else
            {
                TypeAnimalTextBox.Clear();
                NameAnimalTextBox.Clear();
                AgeAnimalTextBox.Clear();
                OwnerAnimalTextBox.Clear();
            }
        }

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {

        }
        private void NewListClick(object sender, RoutedEventArgs e)
        {
            animalList = new AnimalList(); // создаём новый список животных
            UpdateUI();
        }
        private void OpenListClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            if ((bool)openFileDialog.ShowDialog())
            {
                try
                {
                    animalList.LoadFromXMLFile(openFileDialog.FileName);
                    UpdateUI();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка открытия файла! : " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void SaveListClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog(); // окно для сохранения файла
            saveFileDialog.Filter = "XML files (*.xml)|*.xml"; 
            saveFileDialog.DefaultExt = ".xml"; // если не укажем расширение, то устанавливаем расширение по умолчанию
            if (saveFileDialog.ShowDialog() == true) // если окно закрыто нажатием ОК (т.е. ввели имя файла и нажали ок), а не закрыли нажатием на X
            {
                try
                {
                    animalList.SaveToXMLFile(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения файла! : " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void PreviousClick(object sender, RoutedEventArgs e) 
        {
            if (flagFilter)
                animalList.PreviousFilteredAnimal(conditionCheckDelegate,FilterValueTextBox.Text);
            else
                animalList.PrevAnimal();
            UpdateUI();
        }
        private void NextClick(object sender, RoutedEventArgs e) 
        {
            if (flagFilter)
                animalList.NextFilteredAnimal(conditionCheckDelegate,FilterValueTextBox.Text);
            else
                animalList.NextAnimal();
            UpdateUI();
        }
        private void AddAnimalClick(object sender, RoutedEventArgs e)
        {
            animalList.Add(new Animal());
            UpdateUI();
        }
        private void DelAnimalClick(object sender, RoutedEventArgs e)
        {
            animalList.DelAnimal();
            UpdateUI();
        }
        private void SaveChangesClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // если информация о животном не заполнена, то генерируем ошибку
                if (animalList.IsHomemade())
                {
                    if (TypeAnimalTextBox.Text == "" || NameAnimalTextBox.Text == "" || !float.TryParse(AgeAnimalTextBox.Text, out _) || OwnerAnimalTextBox.Text=="")
                        throw new Exception();
                    animalList.EditCurrentAnimal(TypeAnimalTextBox.Text, NameAnimalTextBox.Text, float.Parse(AgeAnimalTextBox.Text), OwnerAnimalTextBox.Text);
                }
                else
                {
                    if (TypeAnimalTextBox.Text == "" || NameAnimalTextBox.Text == "" || !float.TryParse(AgeAnimalTextBox.Text, out _))
                        throw new Exception();
                    animalList.EditCurrentAnimal(TypeAnimalTextBox.Text, NameAnimalTextBox.Text, float.Parse(AgeAnimalTextBox.Text));
                }
                UpdateUI();
                MessageBox.Show("Данные о животном успешно добавлены в текущий спискок!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Корректно заполните данные!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AssignOwnerClick(object sender, RoutedEventArgs e)
        {
            HomeAnimal homeAnimal = new HomeAnimal();// создаём новое домашнее животное
            homeAnimal.CopyFromAnimal(animalList.GetCurrentAnimal()); // копируем из просто животного поля
            animalList.DomesticateAnimal(homeAnimal); // одомашниваем животное
            UpdateUI();
        }

        private void FilterComboBoxChange(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(FilterValueTextBox.Text) && FilterComboBox.SelectedItem!=null)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)FilterComboBox.SelectedItem;
                selectedValue = selectedItem.Content.ToString();
                switch (selectedValue)
                {
                    case "Вид":
                        conditionCheckDelegate = AnimalList.CheckType;
                        break;
                    case "Кличка":
                        conditionCheckDelegate = AnimalList.CheckName;
                        break;
                    case "Возраст":
                        conditionCheckDelegate = AnimalList.CheckAge;
                        break;
                    case "Хозяин":
                        conditionCheckDelegate = AnimalList.CheckOwner;
                        break;
                }
                flagFilter = true;
                animalList.FindAnimal(conditionCheckDelegate, FilterValueTextBox.Text);
                UpdateUI();
            }
        }

        private void RemoveFilterClick(object sender, RoutedEventArgs e)
        {
            animalList.ChooseFirst();
            flagFilter = false;
            FilterComboBox.SelectedItem= null;
            FilterValueTextBox.Text = "";
            selectedValue = "";
            UpdateUI();
        }
    }
}
