using Backend.Database;
using Backend.Utils;
using DentistStudioApp.Model;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp
{
    public partial class App : Application
    {
        public App()
        {
            Sys.LoadAllEmbeddedDll(); 

            DatabaseManager.Add(new SQLiteDatabase<Patient>());
            DatabaseManager.Add(new SQLiteDatabase<Gender>());
            DatabaseManager.Add(new SQLiteDatabase<JobTitle>());

            //survey tables
            DatabaseManager.Add(new SQLiteDatabase<Survey>());
            DatabaseManager.Add(new SQLiteDatabase<SurveyData>());
            DatabaseManager.Add(new SQLiteDatabase<SurveyQuestion>());
            DatabaseManager.Add(new SQLiteDatabase<SurveyQuestionCategory>());

            //Treatment And Services
            DatabaseManager.Add(new SQLiteDatabase<Treatment>());
            DatabaseManager.Add(new SQLiteDatabase<Service>());
            DatabaseManager.Add(new SQLiteDatabase<Appointment>());

            //Dentist and Clinics
            DatabaseManager.Add(new SQLiteDatabase<Dentist>());
            DatabaseManager.Add(new SQLiteDatabase<Clinic>());

            //Invoice
            DatabaseManager.Add(new SQLiteDatabase<Invoice>());
            DatabaseManager.Add(new SQLiteDatabase<PaymentType>());
            DatabaseManager.Add(new SQLiteDatabase<InvoicedTreatment>());

            //Screening
            DatabaseManager.Add(new SQLiteDatabase<TeethScreen>());
            DatabaseManager.Add(new SQLiteDatabase<TeethScreenData>());

            //Reporting
            DatabaseManager.Add(new SQLiteDatabase<PatientReport>());
            DatabaseManager.Add(new SQLiteDatabase<PatientWithTreatment>());

            this.DisposeOnExit();
        }
    }

}
