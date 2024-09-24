using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Задание_2
{
    [Serializable]
    [XmlInclude(typeof(HomeAnimal))]
    public class Animal
    {
        private string name; // кличка животного
        private string type; // вид животного 
        private float age; // возраст животного
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public float Age
        {
            get { return age; }
            set { age = value; }
        }
        public Animal()
        {
            this.Name = "";
            this.Age = 0;
            this.Type = "";
        }
    }
    [Serializable]
    public class HomeAnimal:Animal // класс домашнего животного
    {
        private string owner; // имя владельца
        public string Owner
        {
            get { return owner; }
            set { owner = value; }
        }
        public HomeAnimal(): base () 
        {
            this.Owner = "";
        }
        public void CopyFromAnimal(Animal animal)
        {
            this.Type = animal.Type;
            this.Name = animal.Name;
            this.Age= animal.Age;
        }
    }
}
