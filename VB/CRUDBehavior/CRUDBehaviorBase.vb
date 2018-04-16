Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows.Interactivity
Imports DevExpress.Xpf.Grid
Imports System.Windows.Input
Imports System.Windows
Imports DevExpress.Xpf.Core.ServerMode
Imports System.Data.Linq
Imports DevExpress.Xpf.Core
Imports System.Windows.Controls
Imports DevExpress.Xpf.Bars

Namespace CRUDBehaviorBase
	Public Class CRUDBehaviorBase
		Inherits Behavior(Of GridControl)
		Public Shared ReadOnly NewRowFormProperty As DependencyProperty = DependencyProperty.Register("NewRowForm", GetType(DataTemplate), GetType(CRUDBehaviorBase), New PropertyMetadata(Nothing))
		Public Shared ReadOnly EditRowFormProperty As DependencyProperty = DependencyProperty.Register("EditRowForm", GetType(DataTemplate), GetType(CRUDBehaviorBase), New PropertyMetadata(Nothing))
		Public Shared ReadOnly DataContextProperty As DependencyProperty = DependencyProperty.Register("DataContext", GetType(DataContext), GetType(CRUDBehaviorBase), New PropertyMetadata(Nothing))
		Public Shared ReadOnly RowTypeProperty As DependencyProperty = DependencyProperty.Register("RowType", GetType(Type), GetType(CRUDBehaviorBase), New PropertyMetadata(Nothing))
		Public Shared ReadOnly AllowKeyDownActionsProperty As DependencyProperty = DependencyProperty.Register("AllowKeyDownActions", GetType(Boolean), GetType(CRUDBehaviorBase), New PropertyMetadata(False))

		Public Property NewRowForm() As DataTemplate
			Get
				Return CType(GetValue(NewRowFormProperty), DataTemplate)
			End Get
			Set(ByVal value As DataTemplate)
				SetValue(NewRowFormProperty, value)
			End Set
		End Property
		Public Property EditRowForm() As DataTemplate
			Get
				Return CType(GetValue(EditRowFormProperty), DataTemplate)
			End Get
			Set(ByVal value As DataTemplate)
				SetValue(EditRowFormProperty, value)
			End Set
		End Property
		Public Property DataContext() As DataContext
			Get
				Return CType(GetValue(DataContextProperty), DataContext)
			End Get
			Set(ByVal value As DataContext)
				SetValue(DataContextProperty, value)
			End Set
		End Property
		Public Property RowType() As Type
			Get
				Return CType(GetValue(RowTypeProperty), Type)
			End Get
			Set(ByVal value As Type)
				SetValue(RowTypeProperty, value)
			End Set
		End Property
		Public Property AllowKeyDownActions() As Boolean
			Get
				Return CBool(GetValue(AllowKeyDownActionsProperty))
			End Get
			Set(ByVal value As Boolean)
				SetValue(AllowKeyDownActionsProperty, value)
			End Set
		End Property

		Public ReadOnly Property Grid() As GridControl
			Get
				Return AssociatedObject
			End Get
		End Property
		Public ReadOnly Property View() As TableView
			Get
				Return If(Grid IsNot Nothing, CType(Grid.View, TableView), Nothing)
			End Get
		End Property

		#Region "Commands"
		Private privateNewRowCommand As ICommand
		Public Property NewRowCommand() As ICommand
			Get
				Return privateNewRowCommand
			End Get
			Private Set(ByVal value As ICommand)
				privateNewRowCommand = value
			End Set
		End Property
		Private privateRemoveRowCommand As CustomCommand
		Public Property RemoveRowCommand() As CustomCommand
			Get
				Return privateRemoveRowCommand
			End Get
			Private Set(ByVal value As CustomCommand)
				privateRemoveRowCommand = value
			End Set
		End Property
		Private privateEditRowCommand As CustomCommand
		Public Property EditRowCommand() As CustomCommand
			Get
				Return privateEditRowCommand
			End Get
			Private Set(ByVal value As CustomCommand)
				privateEditRowCommand = value
			End Set
		End Property
		Protected Overridable Sub ExecuteNewRowCommand()
			AddNewRow()
		End Sub
		Protected Overridable Function CanExecuteNewRowCommand() As Boolean
			Return True
		End Function
		Protected Overridable Sub ExecuteRemoveRowCommand()
			RemoveSelectedRows()
		End Sub
		Protected Overridable Function CanExecuteRemoveRowCommand() As Boolean
			Return True
		End Function
		Protected Overridable Sub ExecuteEditRowCommand()
			EditRow()
		End Sub
		Protected Overridable Function CanExecuteEditRowCommand() As Boolean
			Return CanExecuteRemoveRowCommand()
		End Function
		#End Region

		Public Sub New()
			NewRowCommand = New DelegateCommand(AddressOf ExecuteNewRowCommand, AddressOf CanExecuteNewRowCommand)
			RemoveRowCommand = New CustomCommand(AddressOf ExecuteRemoveRowCommand, AddressOf CanExecuteRemoveRowCommand)
			EditRowCommand = New CustomCommand(AddressOf ExecuteEditRowCommand, AddressOf CanExecuteEditRowCommand)
		End Sub
		Public Overridable Function CreateNewRow() As Object
			Return Activator.CreateInstance(RowType)
		End Function
		Public Overridable Sub AddNewRow(ByVal newRow As Object)
			If DataContext Is Nothing Then
				Return
			End If
			DataContext.GetTable(RowType).InsertOnSubmit(newRow)
			DataContext.SubmitChanges()
			UpdateDataSource()
		End Sub
		Public Overridable Sub AddNewRow()
			Dim dialog As DXWindow = CreateDialogWindow(CreateNewRow(), False)
			AddHandler dialog.Closed, AddressOf OnNewRowDialogClosed
			dialog.ShowDialog()
		End Sub
		Public Overridable Sub RemoveRow()
			DataContext.GetTable(RowType).DeleteOnSubmit(View.FocusedRow)
			DataContext.SubmitChanges()
			UpdateDataSource()
		End Sub
		Public Overridable Sub RemoveSelectedRows()
			Dim selectedRowsHandles() As Integer = View.GetSelectedRowHandles()
			View.GetSelectedRowHandles()
			If selectedRowsHandles IsNot Nothing AndAlso selectedRowsHandles.Length <> 0 Then
				For Each handle As Integer In selectedRowsHandles
					DataContext.GetTable(RowType).DeleteOnSubmit(Grid.GetRow(handle))
				Next handle
				DataContext.SubmitChanges()
				UpdateDataSource()
			ElseIf View.FocusedRow IsNot Nothing Then
				RemoveRow()
			End If
		End Sub
		Public Overridable Sub EditRow()
			If View Is Nothing OrElse View.FocusedRow Is Nothing Then
				Return
			End If
			Dim dialog As DXWindow = CreateDialogWindow(View.FocusedRow, True)
			AddHandler dialog.Closed, AddressOf OnEditRowDialogClosed
			dialog.ShowDialog()
		End Sub
		Protected Overridable Function CreateDialogWindow(ByVal content As Object, Optional ByVal isEditingMode As Boolean = False) As DXWindow
			Dim dialog As DXDialog = New DXDialog With {.Tag = content, .Buttons = DialogButtons.OkCancel, .Title = If(isEditingMode, "Edit Row", "Add New Row"), .SizeToContent = SizeToContent.WidthAndHeight}
			Dim c As ContentControl = New ContentControl With {.Content = content}
			If isEditingMode Then
				dialog.Title = "Edit Row"
				c.ContentTemplate = EditRowForm
			Else
				dialog.Title = "Add New Row"
				c.ContentTemplate = NewRowForm
			End If
			dialog.Content = c
			Return dialog
		End Function
		Protected Overridable Sub OnNewRowDialogClosed(ByVal sender As Object, ByVal e As EventArgs)
			RemoveHandler (CType(sender, DXWindow)).Closed, AddressOf OnNewRowDialogClosed
			If CBool((CType(sender, DXWindow)).DialogResult) Then
				AddNewRow((CType(sender, DXWindow)).Tag)
			End If
		End Sub
		Protected Overridable Sub OnEditRowDialogClosed(ByVal sender As Object, ByVal e As EventArgs)
			RemoveHandler (CType(sender, DXWindow)).Closed, AddressOf OnEditRowDialogClosed
			If CBool((CType(sender, DXDialog)).DialogResult) Then
				DataContext.GetTable(RowType).DeleteOnSubmit((CType(sender, DXWindow)).Tag)
				DataContext.GetTable(RowType).InsertOnSubmit((CType(sender, Window)).Tag)
				DataContext.SubmitChanges()
				UpdateDataSource()
			Else
				DataContext.Refresh(RefreshMode.OverwriteCurrentValues, DataContext.GetTable(RowType))
			End If
		End Sub
		Protected Overridable Sub OnViewKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
			If (Not AllowKeyDownActions) Then
				Return
			End If
			If e.Key = Key.Delete Then
				RemoveSelectedRows()
				e.Handled = True
			End If
			If e.Key = Key.Enter Then
				EditRow()
				e.Handled = True
			End If
		End Sub
		Protected Overridable Sub OnViewRowDoubleClick(ByVal sender As Object, ByVal e As RowDoubleClickEventArgs)
			EditRow()
			e.Handled = True
		End Sub
		Protected Overridable Sub OnGridLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			RemoveHandler Grid.Loaded, AddressOf OnGridLoaded
			Initialize()
		End Sub
		Protected Overridable Sub OnViewFocusedRowChanged(ByVal sender As Object, ByVal e As FocusedRowChangedEventArgs)
			RemoveRowCommand.RaiseCanExecuteChangedEvent()
			EditRowCommand.RaiseCanExecuteChangedEvent()
		End Sub
		Protected Overrides Sub OnAttached()
			MyBase.OnAttached()
			If View IsNot Nothing Then
				Initialize()
			Else
				AddHandler Grid.Loaded, AddressOf OnGridLoaded
			End If
		End Sub
		Protected Overrides Sub OnDetaching()
			Uninitialize()
			MyBase.OnDetaching()
		End Sub
		Protected Overridable Sub Initialize()
			AddHandler View.KeyDown, AddressOf OnViewKeyDown
			AddHandler View.RowDoubleClick, AddressOf OnViewRowDoubleClick
			AddHandler View.FocusedRowChanged, AddressOf OnViewFocusedRowChanged
		End Sub
		Protected Overridable Sub Uninitialize()
			RemoveHandler View.KeyDown, AddressOf OnViewKeyDown
			RemoveHandler View.RowDoubleClick, AddressOf OnViewRowDoubleClick
			RemoveHandler View.FocusedRowChanged, AddressOf OnViewFocusedRowChanged
		End Sub
		Protected Overridable Sub UpdateDataSource()
		End Sub
	End Class
	Public Class CustomCommand
		Implements ICommand
		Private _executeMethod As Action
		Private _canExecuteMethod As Func(Of Boolean)
		Public Sub New(ByVal executeMethod As Action, ByVal canExecuteMethod As Func(Of Boolean))
			_executeMethod = executeMethod
			_canExecuteMethod = canExecuteMethod
		End Sub
		Public Function CanExecute(ByVal parameter As Object) As Boolean Implements ICommand.CanExecute
			Return _canExecuteMethod()
		End Function
		Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
		Public Sub Execute(ByVal parameter As Object) Implements ICommand.Execute
			_executeMethod()
		End Sub
		Public Sub RaiseCanExecuteChangedEvent()
			RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
		End Sub
	End Class
End Namespace