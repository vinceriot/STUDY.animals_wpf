using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Задание_2
{
    public class AnimalList
    {
        private static List<Animal> animals; // список животных
        private int currentIndex; // текущий индекс животного в списке

        public AnimalList()
        {
            animals = new List<Animal>();
            currentIndex = -1;
        }
        public bool HasAnimal()
        {
            return animals.Count > 0 && currentIndex >= 0;
        }
        public bool IsHomemade()// проверка, является ли животное домашним
        {
            if (currentIndex > (-1)) 
                return animals[currentIndex].GetType() == typeof(HomeAnimal);
            else 
                return false;
        }
        public int Count => animals.Count; // возвращает количество животных в списке
        public void Add(Animal animal) // добавление животного в список
        {
            animals.Add(animal);
            currentIndex = animals.Count - 1;
        }
        public bool HasNext() => (currentIndex < animals.Count - 1 && currentIndex >= 0); // проверка того, что есть следующее животно
        public bool HasPrevious() => currentIndex > 0 ; // проверка того, что есть предыдущее животное
        public void EditCurrentAnimal(string type, string name, float age, string owner = "") // сохранение изменений 
        {
            Animal currentAnimal = animals[currentIndex];
            currentAnimal.Type = type;
            currentAnimal.Name = name;
            currentAnimal.Age = age;

            // проверяем, является ли текущее животное домашним и устанавливаем владельца, если задан
            if (currentAnimal is HomeAnimal && owner != "")
            {
                HomeAnimal homeAnimal = (HomeAnimal)currentAnimal;
                homeAnimal.Owner = owner;
            }
        }
        public Animal GetCurrentAnimal() => animals[currentIndex]; // получение текущего животного
        public void PrevAnimal() // для перехода к предыдущему животному
        {
            currentIndex--;
        }
        public void NextAnimal() // для перехода к следующему животному
        {
            currentIndex++;
        }
        public bool HasNextFiltered(ConditionCheckDelegate conditionCheck, string value)
        {
            if (currentIndex == -1)
                return false;
            else
            {
                int i = currentIndex+1;
                bool found = false;
                while (i<animals.Count && !found)
                {
                    if (conditionCheck(animals[i], value))
                        found = true;
                    else
                        i++;
                }
                return found;
            }
        }
        public bool HasPreviousFiltered(ConditionCheckDelegate conditionCheck, string value)
        {
            if (currentIndex == -1)
                return false;
            else
            {
                int i = currentIndex - 1;
                bool found = false;
                while (i > (-1) && !found)
                {
                    if (conditionCheck(animals[i], value))
                        found = true;
                    else
                        i--;
                }
                return found;
            }   
        }
        public void NextFilteredAnimal(ConditionCheckDelegate conditionCheck, string value)
        {
            int i = currentIndex;
            do
            {
                i++;
            }
            while (!conditionCheck(animals[i], value));
            currentIndex = i;
        }
        public void PreviousFilteredAnimal(ConditionCheckDelegate conditionCheck, string value)
        {
            int i = currentIndex;
            do
            {
                i--;
            }
            while (!conditionCheck(animals[i], value));
            currentIndex = i;
        }
        public void DelAnimal() // удаление текущего животного
        {
            animals.RemoveAt(currentIndex);
            if (currentIndex == animals.Count) // если удаляется последнее животное в списке
                currentIndex = animals.Count - 1; // то отображаем предыдущее
        }
        public void SaveToXMLFile(string filename) // метод для сохранения списка в XML файл
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Animal>));
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                serializer.Serialize(fs, animals);
            }

        }
        public void LoadFromXMLFile(string filename) // метод загрузки из XML файла
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Animal>));
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                animals = (List<Animal>)serializer.Deserialize(fs);
            }
            currentIndex = 0; // отображаем первое животное из списка
        }
        public void DomesticateAnimal(HomeAnimal homeAnimal)
        {
            animals[currentIndex]= homeAnimal;
        }
        public delegate bool ConditionCheckDelegate(Animal animal, string value); // делегат для результата сравнения
        public void FindAnimal(ConditionCheckDelegate conditionCheck, string value) 
        {
            currentIndex = -1; // если подходящее животное найдётся, то индекс поменятся
            bool found = false;
            int i = 0;
            while (!found && i<animals.Count)
            {
                Animal animal = animals[i];
                if (conditionCheck(animal, value))
                {
                    currentIndex = i;
                    found = true;
                }
                i++;
            }
        }
        public static bool CheckType(Animal animal, string value)
        {
            return animal.Type.Equals(value);
        }

        public static bool CheckName(Animal animal, string value)
        {
            return animal.Name.Equals(value);
        }

        public static bool CheckAge(Animal animal, string value)
        {
            float age;
            if (float.TryParse(value, out age))
            {
                return animal.Age == age;
            }
            return false;
        }

        public static bool CheckOwner(Animal animal, string value)
        {
            if (animal is HomeAnimal)
            {
                HomeAnimal homeAnimal = (HomeAnimal)animal;
                return homeAnimal.Owner.Equals(value);
            }
            return false;
        }
        public void ChooseFirst()
        {
            if (animals.Count>0)
                currentIndex = 0;
        }
    }
}
