using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interactivity;
using DevExpress.Xpf.Grid;
using System.Windows.Input;
using System.Windows;
using DevExpress.Xpf.Core.ServerMode;
using System.Data.Linq;
using DevExpress.Xpf.Core;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;

namespace CRUDBehaviorBase {
    public class CRUDBehaviorBase: Behavior<GridControl> {
        public static readonly DependencyProperty NewRowFormProperty =
                DependencyProperty.Register("NewRowForm", typeof(DataTemplate), typeof(CRUDBehaviorBase), new PropertyMetadata(null));
        public static readonly DependencyProperty EditRowFormProperty =
            DependencyProperty.Register("EditRowForm", typeof(DataTemplate), typeof(CRUDBehaviorBase), new PropertyMetadata(null));
        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register("DataContext", typeof(DataContext), typeof(CRUDBehaviorBase), new PropertyMetadata(null));
        public static readonly DependencyProperty RowTypeProperty =
            DependencyProperty.Register("RowType", typeof(Type), typeof(CRUDBehaviorBase), new PropertyMetadata(null));
        public static readonly DependencyProperty AllowKeyDownActionsProperty =
            DependencyProperty.Register("AllowKeyDownActions", typeof(bool), typeof(CRUDBehaviorBase), new PropertyMetadata(false));

        public DataTemplate NewRowForm {
            get { return (DataTemplate)GetValue(NewRowFormProperty); }
            set { SetValue(NewRowFormProperty, value); }
        }
        public DataTemplate EditRowForm {
            get { return (DataTemplate)GetValue(EditRowFormProperty); }
            set { SetValue(EditRowFormProperty, value); }
        }
        public DataContext DataContext {
            get { return (DataContext)GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }
        public Type RowType {
            get { return (Type)GetValue(RowTypeProperty); }
            set { SetValue(RowTypeProperty, value); }
        }
        public bool AllowKeyDownActions {
            get { return (bool)GetValue(AllowKeyDownActionsProperty); }
            set { SetValue(AllowKeyDownActionsProperty, value); }
        }

        public GridControl Grid { get { return AssociatedObject; } }
        public TableView View { get { return Grid != null ? (TableView)Grid.View : null; } }

        #region Commands
        public ICommand NewRowCommand { get; private set; }
        public CustomCommand RemoveRowCommand { get; private set; }
        public CustomCommand EditRowCommand { get; private set; }
        protected virtual void ExecuteNewRowCommand() {
            AddNewRow();
        }
        protected virtual bool CanExecuteNewRowCommand() {
            return true;
        }
        protected virtual void ExecuteRemoveRowCommand() {
            RemoveSelectedRows();
        }
        protected virtual bool CanExecuteRemoveRowCommand() {
            return true;
        }
        protected virtual void ExecuteEditRowCommand() {
            EditRow();
        }
        protected virtual bool CanExecuteEditRowCommand() {
            return CanExecuteRemoveRowCommand();
        }
        #endregion

        public CRUDBehaviorBase() {
            NewRowCommand = new DelegateCommand(ExecuteNewRowCommand, CanExecuteNewRowCommand);
            RemoveRowCommand = new CustomCommand(ExecuteRemoveRowCommand, CanExecuteRemoveRowCommand);
            EditRowCommand = new CustomCommand(ExecuteEditRowCommand, CanExecuteEditRowCommand);
        }
        public virtual object CreateNewRow() {
            return Activator.CreateInstance(RowType);
        }
        public virtual void AddNewRow(object newRow) {
            if(DataContext == null) return;
            DataContext.GetTable(RowType).InsertOnSubmit(newRow);
            DataContext.SubmitChanges();
            UpdateDataSource();
        }
        public virtual void AddNewRow() {
            DXWindow dialog = CreateDialogWindow(CreateNewRow(), false);
            dialog.Closed += OnNewRowDialogClosed;
            dialog.ShowDialog();
        }
        public virtual void RemoveRow() {
            DataContext.GetTable(RowType).DeleteOnSubmit(Grid.CurrentItem);
            DataContext.SubmitChanges();
            UpdateDataSource();
        }
        public virtual void RemoveSelectedRows() {
            int[] selectedRowsHandles = Grid.GetSelectedRowHandles();
            if(selectedRowsHandles != null && selectedRowsHandles.Length != 0) {
                foreach(int handle in selectedRowsHandles)
                    DataContext.GetTable(RowType).DeleteOnSubmit(Grid.GetRow(handle));
                DataContext.SubmitChanges();
                UpdateDataSource();
            }
            else if(Grid.CurrentItem != null)
                RemoveRow();
        }
        public virtual void EditRow() {
            if(View == null || Grid.CurrentItem == null) return;
            DXWindow dialog = CreateDialogWindow(Grid.CurrentItem, true);
            dialog.Closed += OnEditRowDialogClosed;
            dialog.ShowDialog();
        }
        protected virtual DXWindow CreateDialogWindow(object content, bool isEditingMode = false) {
            DXDialog dialog = new DXDialog {
                Tag = content,
                Buttons = DialogButtons.OkCancel,
                Title = isEditingMode ? "Edit Row" : "Add New Row",
                SizeToContent = SizeToContent.WidthAndHeight
            };
            ContentControl c = new ContentControl { Content = content };
            if(isEditingMode) {
                dialog.Title = "Edit Row";
                c.ContentTemplate = EditRowForm;
            }
            else {
                dialog.Title = "Add New Row";
                c.ContentTemplate = NewRowForm;
            }
            dialog.Content = c;
            return dialog;
        }
        protected virtual void OnNewRowDialogClosed(object sender, EventArgs e) {
            ((DXWindow)sender).Closed -= OnNewRowDialogClosed;
            if((bool)((DXWindow)sender).DialogResult)
                AddNewRow(((DXWindow)sender).Tag);
        }
        protected virtual void OnEditRowDialogClosed(object sender, EventArgs e) {
            ((DXWindow)sender).Closed -= OnEditRowDialogClosed;
            if((bool)((DXDialog)sender).DialogResult) {
                DataContext.GetTable(RowType).DeleteOnSubmit(((DXWindow)sender).Tag);
                DataContext.GetTable(RowType).InsertOnSubmit(((Window)sender).Tag);
                DataContext.SubmitChanges();
                UpdateDataSource();
            }
            else
                DataContext.Refresh(RefreshMode.OverwriteCurrentValues, DataContext.GetTable(RowType));
        }
        protected virtual void OnViewKeyDown(object sender, KeyEventArgs e) {
            if(!AllowKeyDownActions)
                return;
            if(e.Key == Key.Delete) {
                RemoveSelectedRows();
                e.Handled = true;
            }
            if(e.Key == Key.Enter) {
                EditRow();
                e.Handled = true;
            }
        }
        protected virtual void OnViewRowDoubleClick(object sender, RowDoubleClickEventArgs e) {
            EditRow();
            e.Handled = true;
        }
        protected virtual void OnGridLoaded(object sender, RoutedEventArgs e) {
            Grid.Loaded -= OnGridLoaded;
            Initialize();
        }
        protected virtual void OnGridCurrentItemChanged(object sender, CurrentItemChangedEventArgs e) {
            RemoveRowCommand.RaiseCanExecuteChangedEvent();
            EditRowCommand.RaiseCanExecuteChangedEvent();
        }
        protected override void OnAttached() {
            base.OnAttached();
            if(View != null)
                Initialize();
            else Grid.Loaded += OnGridLoaded;
        }
        protected override void OnDetaching() {
            Uninitialize();
            base.OnDetaching();
        }
        protected virtual void Initialize() {
            View.KeyDown += OnViewKeyDown;
            View.RowDoubleClick += OnViewRowDoubleClick;
            Grid.CurrentItemChanged += OnGridCurrentItemChanged;
        }
        protected virtual void Uninitialize() {
            View.KeyDown -= OnViewKeyDown;
            View.RowDoubleClick -= OnViewRowDoubleClick;
            Grid.CurrentItemChanged -= OnGridCurrentItemChanged;
        }
        protected virtual void UpdateDataSource() {
        }
    }
    public class CustomCommand: ICommand {
        Action _executeMethod;
        Func<bool> _canExecuteMethod;
        public CustomCommand(Action executeMethod, Func<bool> canExecuteMethod) {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }
        public bool CanExecute(object parameter) {
            return _canExecuteMethod();
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter) {
            _executeMethod();
        }
        public void RaiseCanExecuteChangedEvent() {
            if(CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}