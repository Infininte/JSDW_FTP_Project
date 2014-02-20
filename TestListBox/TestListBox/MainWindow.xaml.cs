using System;
using System.Collections.Generic;
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

namespace TestListBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class PersonService
    {
        public List<Person> GetPersonList()
        {
            List<Person> personList = new List<Person>();
            personList.Add(new Person { name = "Bobby", age=25 });
            personList.Add(new Person { name = "Hilda", age = 17 });
            personList.Add(new Person { name = "Tyler", age = 23 });
            return personList;
        }
    }
    public class Person
    {
        public string name { get; set; }
        public int age { get; set; }
    }
}
