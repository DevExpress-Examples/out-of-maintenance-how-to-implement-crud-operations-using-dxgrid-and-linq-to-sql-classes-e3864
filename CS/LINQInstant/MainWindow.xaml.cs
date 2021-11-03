using DevExpress.Mvvm.Xpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LINQInstant {
    public partial class MainWindow: Window {
        public MainWindow() {
            InitializeComponent();
            var source = new DevExpress.Data.Linq.LinqInstantFeedbackSource {
                KeyExpression = nameof(Item.Id)
            };
            source.GetQueryable += (sender, e) => {
                var context = new DataClasses1DataContext();
                e.QueryableSource = context.Items;
            };
            grid.ItemsSource = source;
        }
        void OnCreateEditEntityViewModel(System.Object sender, DevExpress.Mvvm.Xpf.CreateEditItemViewModelArgs e) {
            var context = new DataClasses1DataContext();
            Item item;
            if(e.IsNewItem) {
                item = new Item() { Id = context.Items.Max(x => x.Id) + 1 };
                context.Items.InsertOnSubmit(item);
            } else {
                var key = (int)e.Key;
                item = context.Items.Single(x => x.Id == key);
            }
            e.ViewModel = new EditItemViewModel(
                item,
                context,
                title: (e.IsNewItem ? "New " : "Edit ") + nameof(Item)
            );
        }

        void OnValidateRow(System.Object sender, DevExpress.Mvvm.Xpf.EditFormRowValidationArgs e) {
            var context = (DataClasses1DataContext)e.EditOperationContext;
            context.SubmitChanges();
        }

        void OnValidateRowDeletion(System.Object sender, DevExpress.Mvvm.Xpf.EditFormValidateRowDeletionArgs e) {
            var key = (int)e.Keys.Single();
            var context = new DataClasses1DataContext();
            var item = context.Items.Single(x => x.Id == key);
            context.Items.DeleteOnSubmit(item);
            context.SubmitChanges();
        }
    }
}