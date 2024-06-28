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
            DatabaseManager.Add(new SQLiteDatabase<Patient>()); //0
            DatabaseManager.Add(new SQLiteDatabase<Gender>()); //1
            DatabaseManager.Add(new SQLiteDatabase<JobTitle>()); //2

            //survey tables
            DatabaseManager.Add(new SQLiteDatabase<Survey>()); //3
            DatabaseManager.Add(new SQLiteDatabase<SurveyData>());//4 
            DatabaseManager.Add(new SQLiteDatabase<SurveyQuestion>());//5
            DatabaseManager.Add(new SQLiteDatabase<SurveyQuestionCategory>());//6

            //Treatment And Services
            DatabaseManager.Add(new SQLiteDatabase<Treatment>());//7
            DatabaseManager.Add(new SQLiteDatabase<Service>());//8
            DatabaseManager.Add(new SQLiteDatabase<Appointment>());//9

            //Dentist and Clinics
            DatabaseManager.Add(new SQLiteDatabase<Dentist>());//10
            DatabaseManager.Add(new SQLiteDatabase<Clinic>());//11

            //Invoice
            DatabaseManager.Add(new SQLiteDatabase<Invoice>());//12
            DatabaseManager.Add(new SQLiteDatabase<PaymentType>());//13
            DatabaseManager.Add(new SQLiteDatabase<InvoicedTreatment>());//14

            //Screening
            DatabaseManager.Add(new SQLiteDatabase<TeethScreen>());//15
            DatabaseManager.Add(new SQLiteDatabase<TeethScreenData>());//16


            DatabaseManager.Add(new SQLiteDatabase<PatientReport>());
            DatabaseManager.Add(new SQLiteDatabase<PatientWithTreatment>());

            Exit += OnExit;
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            DatabaseManager.Dispose();
            Exit -= OnExit;
        }
    }

}
