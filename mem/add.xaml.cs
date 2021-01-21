using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace mem
{
    /// <summary>
    /// Логика взаимодействия для add.xaml
    /// </summary>
    public partial class add : Window
    {
        private string filename = "";//путь до файла

        private string category = ""; //категория

        private string name = ""; //имя файла

        private List<string> tags = new List<string>(); //список тэгов

        private memeClass mem; //экземпляр мем

        private int mId = 0; //айди мема






        private string openImag() //функция открытие картинки 
        {
            OpenFileDialog openFileDialog = new OpenFileDialog(); // форма
            openFileDialog.Filter = "Файлы изображений (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png)"; // филтр файлов изображения
            var result = openFileDialog.ShowDialog(); // открытие формы
            if (result == true)
            {
                mNameTB.Text = openFileDialog.SafeFileName; //в текстобокс вывод пути до файла
                return openFileDialog.FileName; // вернуть путь до файла
            }
            else return null; //иначе ничего 
        }

        public memeClass GetMeme() => mem; //вернуть тип класса мем

        public add(int id,List<string> surs) //конструктор формы добавить, передаем в эту форму из главной айди и список категорий
        {
            InitializeComponent();
            mCategory.ItemsSource = surs; //категории из списка
            this.mId = id; //назначаем айди
            filepick.IsEnabled = false; //кнопка переключения файла \ ссылки выключена
        }

        private memeClass formMeme(string tags, string name, string category, string filename) // фукнция формирования мемов
        { 
            string[] temp = tags.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); //делим тэги на слова
            foreach (string sTemp in temp)
            {
                this.tags.Add(sTemp); // пишем в список тэгов тэги
            }

            this.name = name; // пишем имя в имя

            this.category = category; // категорию в категорию

            return new memeClass(this.name, this.category, this.tags, this.filename, mId); // возвращаем новый экземпляр мема с параметрами
        }

        private void openIMG_Click(object sender, RoutedEventArgs e) //кнопка открытие изобр
        {
            filename = openImag(); // в путь до файла пишем из функции путь до файла

            if (filename != null) //если путь не пустой
            {

                BitmapImage bitmap = new BitmapImage(); //новый экзмплр картинки
                bitmap.BeginInit(); //начало редкатирования 
                bitmap.UriSource = new Uri(filename); //путь до файла
                bitmap.EndInit(); // конец редактирования

                memPrev.Source = bitmap; //предпросмотр картинки

                fnametb.Text = filename; //путь до файла 
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((mTagsTB.Text != "") && (mNameTB.Text != "") && (mCategory.SelectedIndex != -1) && (filename != null)) //если поля заполнены
            {
               mem = formMeme(mTagsTB.Text, mNameTB.Text, mCategory.SelectedItem.ToString(), filename); // в переменную мем записываем мем с параметрами из функции гетмем

                this.Hide(); //прячем форму

                this.Close(); //закрываем форму
            }
            else
            {
                warnings.Content = "некоторые поля пустые"; //если какие то поля пустые, предупреждаем
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) //событие закрытия формы
        {
            this.Hide(); //прячем форму
        }


        private void filepick_Click(object sender, RoutedEventArgs e) //переключаем кнопки 
        {
            filepick.IsEnabled = false;
            urlpick.IsEnabled = true;
            urlpath.Visibility = (Visibility)1; 
            fnametb.Visibility = 0;
            openIMG.Visibility = 0;
        }

        private void urlpick_Click(object sender, RoutedEventArgs e) //тоже самое ничего сложного
        {
            filepick.IsEnabled = true;
            urlpick.IsEnabled = false;
            urlpath.Visibility = 0;
            fnametb.Visibility = (Visibility)1;
            openIMG.Visibility = (Visibility)1;
        }


        private void urlpath_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) //текстбокс для юрл картинки
        {
            if (urlpath.Text != "") // если он не пустой 
            {
                try //блок защиты от НЕ ссылки
                {
                    BitmapImage bitmap = new BitmapImage();//экзплр картинки 
                    bitmap.BeginInit();//начало
                    bitmap.UriSource = new Uri(urlpath.Text);//путь
                    bitmap.EndInit();//конец

                    memPrev.Source = bitmap;//превью

                    filename = urlpath.Text;//путь
                }
                catch (System.UriFormatException) //ловим ошибки если в тексбокс введен не юрл
                {

                }
            }
        }
    }
}
