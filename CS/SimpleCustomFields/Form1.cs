using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
#region #usings
using DevExpress.XtraScheduler;
using System.Diagnostics;
#endregion #usings

namespace SimpleCustomFields
{
    public partial class Form1 : Form
    {
        bool isUpdating = false;

        public Form1()
        {
            InitializeComponent();

            schedulerDataStorage1.AppointmentsInserted += new PersistentObjectsEventHandler(this.OnApptChangedInsertedDeleted);
            schedulerDataStorage1.AppointmentsChanged += new PersistentObjectsEventHandler(this.OnApptChangedInsertedDeleted);
            schedulerDataStorage1.AppointmentsDeleted += new PersistentObjectsEventHandler(this.OnApptChangedInsertedDeleted);

            this.schedulerControl1.Start = new DateTime(2010, 07, 01);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'carsDBDataSet.Cars' table. You can move, or remove it, as needed.
            this.carsTableAdapter.Fill(this.carsDBDataSet.Cars);
            // TODO: This line of code loads data into the 'carsDBDataSet.CarScheduling' table. You can move, or remove it, as needed.
            this.carSchedulingTableAdapter.Fill(this.carsDBDataSet.CarScheduling);
            this.carsDBDataSet.CarScheduling.RowChanged += CarScheduling_RowChanged;

        }
                
        void OnApptChangedInsertedDeleted(object sender, PersistentObjectsEventArgs e)
        {
            this.isUpdating = true;
            this.carSchedulingTableAdapter.Update(carsDBDataSet);
            this.isUpdating = false;
            this.carsDBDataSet.AcceptChanges();
        }

        void CarScheduling_RowChanged(object sender, DataRowChangeEventArgs e) {
            if (e.Action == DataRowAction.Commit && this.isUpdating) {
                int id = 0;
                using (OleDbCommand cmd = new OleDbCommand("SELECT @@IDENTITY",
                    this.carSchedulingTableAdapter.Connection)) {
                    id = (int)cmd.ExecuteScalar();
                }
                e.Row["ID"] = id;
            }
        }


        #region #EditAppointmentFormShowing
        private void schedulerControl1_EditAppointmentFormShowing(object sender, AppointmentFormEventArgs e)
        {
            MyAppointmentForm form = new MyAppointmentForm(sender as SchedulerControl, e.Appointment, e.OpenRecurrenceForm);
            try
            {
                e.DialogResult = form.ShowDialog();
                e.Handled = true;
            }
            finally
            {
                form.Dispose();
            }
        }
        #endregion #EditAppointmentFormShowing

        #region #InitNewAppointment
        private void schedulerControl1_InitNewAppointment(object sender, DevExpress.XtraScheduler.AppointmentEventArgs e)
        {
            e.Appointment.Description += "Created at runtime at " + String.Format("{0:g}", DateTime.Now);
            e.Appointment.CustomFields["Price"] = 00.01d;
            e.Appointment.CustomFields["ContactInfo"] = "someone@somecompany.com";
        }
        #endregion #InitNewAppointment       
    }
}