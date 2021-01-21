using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using mem;
using System.Xml.Serialization;

namespace mem
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        List<memeClass> memList = new List<memeClass>(); //список объектов типа мем

        List<string> categories = new List<string>() {"any", "cat1", "cat2", "cat3", "cat4" }; // список категорий, можно менять, кол-во не важно, "any" рекомендую оставить
        public MainWindow()
        {
            InitializeComponent();
            mCategoryCb.ItemsSource = categories;//говорим листбоксу с категориями из какого списка брать категории
            mList.DisplayMemberPath = "_mName"; // говорим списку с мемами, что нужно отображать не тип класса, а определенное поле из класса
        }

        private void mSearch_Click(object sender, RoutedEventArgs e) //поиск
        {
            mList.Items.Clear();
           foreach (memeClass mTemp in memList) //каждый элемент типа мем в списке мемеов
            {
                if (mTemp.getName().Contains(mNameTb.Text)) //проверяем: если  имя экземпляра содержит текст из текстбокса, то добавляем этот экземпляр в листбокс
                {
                    mList.Items.Add(mTemp); // добавили
                }

            }
        }

        private void add_mems_Click(object sender, RoutedEventArgs e) //кнопка открытия формы добавления
        {
            add add = new add(memList.Count,categories); // создаем экземпляр формы "добавить", передаем в него количество мемов в списке и список категорий 
            add.Show(); //выводим на экран форму
            add_mems.IsEnabled = false; //выключаем кнопку добавления
            add.Closing += Add_Closed; //создаем событие закрытие формы добавить, при закрытие будет вызываться метод add_closed, он ниже
        }

        private void Add_Closed(object sender, EventArgs e) 
        {
            
            var s = sender as add; //тут мы создаем для удобсва переменную, сендер это и есть форма, нужно обозначить что она имеет тип добавить

            if (s.GetMeme() != null)  //если функция получить мем сработала
            {
                memList.Add(s.GetMeme()); //добавляем мем в список мемеов

                mList.Items.Add(memList.Last()); //добавляем последний мем из списка в листбокс
            }
            add_mems.IsEnabled = true; //включаем кнопку
            
            Closing -= Add_Closed; //отписуемся от события 
        }

        private void mList_SelectionChanged(object sender, SelectionChangedEventArgs e) //обработка выбора мемов в листбоксе
        {
            if (mList.SelectedItem != null) // если выбран не пустой элемент
            {
                memeTagsTb.Text = ""; 
                memeClass temp = mList.SelectedItem as memeClass; //создаем переменную типа мем из выбранного элемента из листбокса с мемами
                imageBox1.Source = temp.getFName(); //в имаджбокс пишем путь до картинки 
                memeNameTb.Text = temp.getName() + " : " + temp.getId(); // в текстбокс имени пишем имя и id мема в списке (+ " : " + temp.getId() - эту часть можно удалить и айди не будет оборажаться)
                foreach (string str in temp.getTags()) //получаем список тэгов 
                    memeTagsTb.Text += str + ", "; //пишем тэги в бокс для тэгов
                memeCategorTb.Text = temp.getCategory(); //вывод категории в тб категорий
            }
        }

        private void mCategoryCb_SelectionChanged(object sender, SelectionChangedEventArgs e) //сортировка по категориям
        {
            if ((mCategoryCb.SelectedIndex != -1) && mCategoryCb.SelectedItem.ToString() != "any") // если выбранная категория не за гранницей и не равно любой 
            {
                mList.Items.Clear();
                foreach (memeClass memSampl in memList) //проход по списку мемов
                {
                    if (memSampl.getCategory() == mCategoryCb.SelectedItem.ToString()) //сравнием категории мема и выбраннную
                    {
                        mList.Items.Add(memSampl); // добавляем если сошлось
                    }
                }
            }
            else  //если нет
                if (mCategoryCb.SelectedItem.ToString() == "any")  //если категория любая 
            {
                    mList.Items.Clear();
                    foreach (memeClass temp in memList) // выводим все мемы по очереди
                    mList.Items.Add(temp);
                }
        }

        private void removButton_Click(object sender, RoutedEventArgs e) // кнопка удаления
        {
            if (mList.SelectedItem != null) // если ывбран не пустой
            {
                memList.Remove(mList.SelectedItem as memeClass); // удаляем мем из списка мемов выбранный элемент из листбокса мемов, который приведен к типу мем

                mList.Items.Remove(mList.SelectedItem); // удаление из листбокса мемов
            }
        }

        private void tagsSearchBut_Click(object sender, RoutedEventArgs e) //поиск по тэгам
        {
            mList.Items.Clear();
            List<string> sTgs = new List<string>(); //список тэгов создаем
            string[] temp = tagsSearchTb.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); //из текстбокса введеные тэги через пробел делим на слова, пихаем их в массив слов темп
            foreach (string sTemp in temp)
            {
                sTgs.Add(sTemp); //добавляем слова из массива в список
            }

            foreach (memeClass memSample in memList) // проходим список мемов
            {
                List<string> tagsList = memSample.getTags(); //получаем список тэгов мема
                foreach (string tag in tagsList) // получаем конкретный тэг 
                    foreach (string st in sTgs) //получаем тэг мема
                        if (tag == st) // если тэг есть
                        {
                            mList.Items.Add(memSample);//добавляем мем в список
                            break; //выходим из цикла чтоб не искать дальше
                        }
            }
        }

        private void savejson_Click(object sender, RoutedEventArgs e) //сохранение в джейсон файл 
        {

            saveXml(memList);
        }

        private void openJson_Click(object sender, RoutedEventArgs e) //открыть из джсон
        {
            mList.Items.Clear();
            var t = openXml();
            if(t != null)
            foreach (memeClass temp in t)
            {
                mList.Items.Add(temp);
            }

        }

        void saveXml (List<memeClass> mem)
        {
            string path = "";
            SaveFileDialog openFileDialog = new SaveFileDialog(); 
            openFileDialog.Filter = "Файлы xml (*.xml) | (*.xml;)"; 
            var result = openFileDialog.ShowDialog(); 
            if (result == true)
            {
               
                path = openFileDialog.FileName; 
                XmlSerializer serializer = new XmlSerializer(typeof(List<memeClass>));
                using (FileStream f = new FileStream(path, FileMode.OpenOrCreate))
                {
                    serializer.Serialize(f, mem);
                }
            }
            
        }

        List<memeClass> openXml ()
        {
            string path = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файлы xml (*.xml) | (*.xml;)";
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                path = openFileDialog.FileName;
                XmlSerializer serializer = new XmlSerializer(typeof(List<memeClass>));
                List<memeClass> temp;
                using (FileStream f = new FileStream(path, FileMode.OpenOrCreate))
                {
                    temp = serializer.Deserialize(f) as List<memeClass>;
                }
                return temp;
            }
            else return null;
        }
    }
}
