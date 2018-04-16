using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interactivity;
using System.Windows;
using DevExpress.Xpf.Grid;
using System.Windows.Controls;
using System.Data.Linq;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ServerMode;
using DevExpress.Xpf.Bars;

namespace LINQServer {
    public class LINQServerModeCRUDBehavior: CRUDBehaviorBase.CRUDBehaviorBase {
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(LinqServerModeDataSource), typeof(LINQServerModeCRUDBehavior), new PropertyMetadata(null));
        public LinqServerModeDataSource DataSource {
            get { return (LinqServerModeDataSource)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }
        protected override bool CanExecuteRemoveRowCommand() {
            if(DataSource == null || Grid == null || View == null || View.FocusedRow == null) return false;
            return true;
        }
        protected override void UpdateDataSource() {
            DataSource.Reload();
        }
        protected override void OnAttached() {
            base.OnAttached();
            Grid.ItemsSource = DataSource.Data;
        }
    }
}