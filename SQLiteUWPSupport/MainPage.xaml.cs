using SQLiteUWPSupport.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SQLiteUWPSupport
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    //create trigger
    public sealed partial class MainPage : Page
    {

        SQLite.Net.SQLiteConnection conn;
        string path;

        public MainPage()
        {
            this.InitializeComponent();
            CreateDatabase();
            CreateTable();
            
        }

        private void CreateDatabase()
        {
            path = Path.Combine(Windows.Storage.ApplicationData.
                   Current.LocalFolder.Path, "MyUserDatabase.sqlite");

             conn = new SQLite.Net.SQLiteConnection(new
             SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        private void CreateTable()
        {
            conn.CreateTable<User>();
            //get table name
            //string name = conn.Table<User>().Table.TableName;
        }

        private void CreateTrigger()
        {
            conn.Execute("CREATE TRIGGER mytrigger AFTER INSERT ON User \r\n BEGIN \r\n INSERT INTO User VALUES('xiao zhang','456'); \r\n END;");
        }

        private void CreateTrigger(object sender, RoutedEventArgs e)
        {
            CreateTrigger();
        }





        private void Insert(object sender, RoutedEventArgs e)
        {
            conn.Insert(new User()
            {
                Username = UserNameTextBox.Text,
                Password = PasswordTextBox.Text
            });
        }


        private void Query(object sender, RoutedEventArgs e)
        {
            MyListView.DataContext = ReadAllUser();
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            
            
        }
        public void add_user()
        {
            Debug.WriteLine("hello");
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            DeleteUser(1);
        }


        public User Query(int id)
        {
            User existingUser = conn.Query<User>("select * from User where Id =" + id).FirstOrDefault();
            return new User();
        }


        // Retrieve the specific user from the database. 
        public User QueryUserById(int id)
        {

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var existingconact = conn.Query<User>("select * from User where Id =" + id).FirstOrDefault();
                return existingconact;
            }
        }

        //Read All User details 
        public ObservableCollection<User> ReadAllUser()
        {

                List<User> myCollection = conn.Table<User>().ToList<User>();
                ObservableCollection<User> UserList = new ObservableCollection<User>(myCollection);
                return UserList;
        }

        //Update user details
        public void UpdateDetails(string name)
        {
            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var existingUser = conn.Query<User>("select * from User where Name =" + name).FirstOrDefault();
                if (existingUser != null)
                {
                    existingUser.Username = name;
                    existingUser.Password = "NewAddress";
                    conn.RunInTransaction(() =>
                    {
                        conn.Update(existingUser);
                    });
                }
            }
        }

        //Insert user
        public void InsertUser()
        {
            conn.Insert(new User()
            {
                Username = UserNameTextBox.Text,
                Password = PasswordTextBox.Text
            });
        }

        //Delete all User 
        public void DeleteAllUser()
        {

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(),path))
            {
                //delete table
                conn.DropTable<User>();
                conn.CreateTable<User>();
                conn.Dispose();
                conn.Close();
            }
        }

        //Delete specific User 
        public void DeleteUser(int Id)
        {

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var existingUser = conn.Query<User>("select * from User where Id =?",  Id).FirstOrDefault();
                if (existingUser != null)
                {
                    conn.RunInTransaction(() =>
                    {
                        conn.Delete(existingUser);
                    });
                }
            }
        }
    }
}
