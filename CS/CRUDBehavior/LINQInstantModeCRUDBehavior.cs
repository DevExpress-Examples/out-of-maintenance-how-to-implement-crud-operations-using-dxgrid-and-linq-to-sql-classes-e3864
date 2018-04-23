using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Grid;
using System.Windows.Controls;
using System.Data.Linq;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.ServerMode;
using System.Collections;
using System.Reflection;

namespace LINQInstant {
    public class LINQInstantModeCRUDBehavior: CRUDBehaviorBase.CRUDBehaviorBase {
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(LinqInstantFeedbackDataSource), typeof(LINQInstantModeCRUDBehavior), new PropertyMetadata(null));
        public LinqInstantFeedbackDataSource DataSource {
            get { return (LinqInstantFeedbackDataSource)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }
        PropertyInfo Info;
        protected override bool CanExecuteRemoveRowCommand() {
            if(DataSource == null || Grid == null || View == null || Grid.CurrentItem == null) return false;
            return true;
        }
        protected override void UpdateDataSource() {
            DataSource.Refresh();
        }
        protected override void OnAttached() {
            base.OnAttached();
            Grid.ItemsSource = DataSource.Data;
        }
        public override void EditRow() {
            if(View == null || Grid.CurrentItem == null) return;
            IEnumerator e = this.DataContext.GetTable(RowType).GetEnumerator();
            while(e.MoveNext()) {
                object v = Info.GetValue(e.Current, null);
                object v1 = Grid.GetCellValue(View.FocusedRowHandle, DataSource.KeyExpression);
                if(v.Equals(v1)) {
                    DXWindow dialog = CreateDialogWindow(e.Current, true);
                    dialog.Closed += OnEditRowDialogClosed;
                    dialog.ShowDialog();
                    return;
                }
            }
        }
        public override void RemoveRow() {
            IEnumerator e = this.DataContext.GetTable(RowType).GetEnumerator();
            while(e.MoveNext()) {
                object v = Info.GetValue(e.Current, null);
                object v1 = Grid.GetCellValue(View.FocusedRowHandle, DataSource.KeyExpression);
                if(v.Equals(v1)) {
                    DataContext.GetTable(RowType).DeleteOnSubmit(e.Current);
                    DataContext.SubmitChanges();
                    UpdateDataSource();
                    return;
                }
            }
        }
        public override void RemoveSelectedRows() {
            List<int> selectedRowsHandles = Grid.GetSelectedRowHandles().ToList<int>();
            if(selectedRowsHandles != null && selectedRowsHandles.Count != 0) {
                IEnumerator e = this.DataContext.GetTable(RowType).GetEnumerator();
                while(e.MoveNext()) {
                    foreach(int handle in selectedRowsHandles) {
                        object v = Info.GetValue(e.Current, null);
                        object v1 = Grid.GetCellValue(handle, DataSource.KeyExpression);
                        if(v.Equals(v1)) {
                            DataContext.GetTable(RowType).DeleteOnSubmit(e.Current);
                            selectedRowsHandles.Remove(handle);
                            break;
                        }
                    }
                }
                DataContext.SubmitChanges();
                UpdateDataSource();
            }
            else if(Grid.CurrentItem != null)
                RemoveRow();
        }
        protected override void Initialize() {
            base.Initialize();
            PropertyInfo[] infos = RowType.GetProperties();
            foreach(PropertyInfo info in infos) {
                if(info.Name == DataSource.KeyExpression) {
                    Info = info;
                    break;
                }
            }
        }
    }
}