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

        private void lvPersons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

    public class PersonService : DependencyObject
    {
        static FrameworkPropertyMetadata propertyMetadata = new FrameworkPropertyMetadata("Comes as Default", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, new PropertyChangedCallback(MyCustom_PropertyChanged), new CoerceValueCallback(MyCustom_CoerceValue), false, UpdateSourceTrigger.PropertyChanged);

        public static readonly DependencyProperty MyCustomProperty = DependencyProperty.Register("MyCustom", typeof(string), typeof(PersonService));

        private static void MyCustom_PropertyChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            //to be called whenever the DP is changed
            MessageBox.Show(string.Format("Property changed is fired : OldValue {0} NewValue : {1}", e.OldValue, e.NewValue));
        }

        public static object MyCustom_CoerceValue(DependencyObject dobj, object Value)
        {
            //Called whenever dependency property value is reevaluated. The return value is the
            //  latest value set to the dependency property
            MessageBox.Show(string.Format("CoerceValue is fired : Value {0}", Value));
            return Value;
        }

        private static bool MyCustom_Vaidate(object Value)
        {
            //Custom validation block which takes in the value of DP
            //  Returns true/false based on success/failure of the validation
            MessageBox.Show(string.Format("DataValidation is Fired : Value {0}", Value));
            return true;
        }

        public string MyCustom
        {
            get
            {
                return GetValue(MyCustomProperty) as string;
            }
            set
            {
                this.SetValue(MyCustomProperty, value);
            }
        }


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
