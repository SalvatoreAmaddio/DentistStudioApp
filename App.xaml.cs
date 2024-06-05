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
            DatabaseManager.Add(new SQLiteDatabase(new Patient())); //0
            DatabaseManager.Add(new SQLiteDatabase(new Gender())); //1
            DatabaseManager.Add(new SQLiteDatabase(new JobTitle())); //2

            //survey tables
            DatabaseManager.Add(new SQLiteDatabase(new Survey())); //3
            DatabaseManager.Add(new SQLiteDatabase(new SurveyData()));//4 
            DatabaseManager.Add(new SQLiteDatabase(new SurveyQuestion()));//5
            DatabaseManager.Add(new SQLiteDatabase(new SurveyQuestionCategory()));//6

            //Treatment And Services
            DatabaseManager.Add(new SQLiteDatabase(new Treatment()));//7
            DatabaseManager.Add(new SQLiteDatabase(new Service()));//8
            DatabaseManager.Add(new SQLiteDatabase(new Appointment()));//9

            //Dentist and Clinics
            DatabaseManager.Add(new SQLiteDatabase(new Dentist()));//10
            DatabaseManager.Add(new SQLiteDatabase(new Clinic()));//11

            //Invoice
            DatabaseManager.Add(new SQLiteDatabase(new Invoice()));//12
            DatabaseManager.Add(new SQLiteDatabase(new PaymentType()));//13
            DatabaseManager.Add(new SQLiteDatabase(new InvoicedTreatment()));//14
        }
    }

}
