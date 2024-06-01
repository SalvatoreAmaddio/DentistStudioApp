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
            Sys.LoadAllEmbeddedDll(); 
            DatabaseManager.Add(new SQLiteDatabase(new Patient())); 
            DatabaseManager.Add(new SQLiteDatabase(new Gender())); 
            DatabaseManager.Add(new SQLiteDatabase(new JobTitle())); 

            //survey tables
            DatabaseManager.Add(new SQLiteDatabase(new Survey())); 
            DatabaseManager.Add(new SQLiteDatabase(new SurveyData())); 
            DatabaseManager.Add(new SQLiteDatabase(new SurveyQuestion()));
            DatabaseManager.Add(new SQLiteDatabase(new SurveyQuestionCategory()));

            //Treatments
            DatabaseManager.Add(new SQLiteDatabase(new Treatment()));

        }
    }

}
