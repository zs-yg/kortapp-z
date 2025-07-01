using DevExpress.Utils.Design;
using DevExpress.Utils.Extensions;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace NetWorkManager
{
    public partial class Form1 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public Form1()
        {
            InitializeComponent();

            BindingList<Customer> dataSource = GetDataSource();
            gridControl.DataSource = dataSource;
            bsiRecordsCount.Caption = "RECORDS : " + dataSource.Count;
        }
        void bbiPrintPreview_ItemClick(object sender, ItemClickEventArgs e)
        {
            gridControl.ShowRibbonPrintPreview();
        }
        public BindingList<Customer> GetDataSource()
        {
            List<PortScanner.internet_info> internet_infolist = PortScanner.GetAllInfo();

            BindingList<Customer> result = new BindingList<Customer>();

            int index = 1;
            foreach(var item in internet_infolist)
            {
                result.Add(new Customer()
                {
                    ID = index++,
                    PID = item.pid,
                    Process = item.process,
                    Local = item.local,
                    Remote = item.remote,
                    State = item.state,
                    Protocal = item.protocal
                });
            }
           
            return result;
        }
        public class Customer
        {
            [Key, Display(AutoGenerateField = false)]
            public int ID { get; set; }
            [Required]
            public string PID { get; set; }
            public string Process { get; set; }
            public string Local { get; set; }
            public string Remote { get; set; }
           // [Display(Name = "Zip Code")]
            public string State { get; set; }
            public string Protocal { get; set; }
        }

        private void bbiRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            BindingList<Customer> dataSource = GetDataSource();
            gridControl.DataSource = dataSource;
            bsiRecordsCount.Caption = "RECORDS : " + dataSource.Count;
        }

        private void bbiDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            var tRcvrLineID = gridView.GetFocusedRowCellValue("PID").ToString();
            var processee = gridView.GetFocusedRowCellValue("Process").ToString();

            if (XtraMessageBox.Show("Do you want to terminate" +$" PID:{tRcvrLineID} Process:{processee}", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                try
                {
                    Process process = Process.GetProcessById(Convert.ToInt32(tRcvrLineID));
                    process.Kill();
                    process.WaitForExit(); 
                   
                    XtraMessageBox.Show($" PID:{tRcvrLineID} Process:{processee} Terminated!", "Succuess Execute!");
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($" PID:{tRcvrLineID} Process:{processee} {ex.Message}!", "error!");
                }


                BindingList<Customer> dataSource = GetDataSource();
                gridControl.DataSource = dataSource;
                bsiRecordsCount.Caption = "RECORDS : " + dataSource.Count;
            }

        }

        private void gridControl_Click(object sender, EventArgs e)
        {
            
        }

        private void gridControl_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void gridView_Click(object sender, EventArgs e)
        {
           
        }
    }
}