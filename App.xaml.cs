using Backend.Database;
using Backend.Utils;
using DentistStudioApp.Model;
using System.Windows;

namespace DentistStudioApp
{
    public partial class App : Application
    {
        public App()
        {
            Sys.LoadAllEmbeddedDll(); //load some custom assemblies that could be used later on.
            DatabaseManager.Add(new SQLiteDatabase(new Patient())); //Add the database object responsible for dealing with this table.
            DatabaseManager.Add(new SQLiteDatabase(new Gender())); //Add the database object responsible for dealing with this table.
            DatabaseManager.Add(new SQLiteDatabase(new JobTitle())); //Add the database object responsible for dealing with this table.

            //survey tables
            DatabaseManager.Add(new SQLiteDatabase(new Survey())); 
            DatabaseManager.Add(new SQLiteDatabase(new SurveyData())); 
            DatabaseManager.Add(new SQLiteDatabase(new SurveyQuestion()));
            DatabaseManager.Add(new SQLiteDatabase(new SurveyQuestionCategory()));

        }
    }

}
