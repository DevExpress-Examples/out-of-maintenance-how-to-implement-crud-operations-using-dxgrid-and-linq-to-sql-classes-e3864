using DevExpress.Mvvm.Xpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace LINQServer {
    public partial class MainWindow: Window {
        public MainWindow() {
            InitializeComponent();
            var context = new DataClassesDataContext();
            var source = new DevExpress.Data.Linq.LinqServerModeSource{
                KeyExpression = nameof(Item.Id),
                QueryableSource = context.Items,
            };
            grid.ItemsSource = source;
        }

        void OnCreateEditEntityViewModel(System.Object sender, DevExpress.Mvvm.Xpf.CreateEditItemViewModelArgs e) {
            var context = new DataClassesDataContext();
            Item item;
            if(e.IsNewItem) {
                item = new Item() { Id = context.Items.Max(x => x.Id) + 1 };
                context.Items.InsertOnSubmit(item);
            } else {
                var key = (int)e.Key;
                item = context.Items.Single(x => x.Id == key);
            }
            e.ViewModel = new EditItemViewModel(item, context);
        }

        void OnValidateRow(System.Object sender, DevExpress.Mvvm.Xpf.EditFormRowValidationArgs e) {
            var context = (DataClassesDataContext)e.EditOperationContext;
            context.SubmitChanges();
        }

        void OnValidateRowDeletion(System.Object sender, DevExpress.Mvvm.Xpf.EditFormValidateRowDeletionArgs e) {
            var key = (int)e.Keys.Single();
            var context = new DataClassesDataContext();
            var item = context.Items.Single(x => x.Id == key);
            context.Items.DeleteOnSubmit(item);
            context.SubmitChanges();
        }
    }
}