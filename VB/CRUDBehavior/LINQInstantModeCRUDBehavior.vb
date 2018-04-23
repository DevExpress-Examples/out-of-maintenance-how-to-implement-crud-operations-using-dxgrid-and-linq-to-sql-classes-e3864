Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows
Imports DevExpress.Xpf.Grid
Imports System.Windows.Controls
Imports System.Data.Linq
Imports System.Windows.Input
Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Bars
Imports DevExpress.Xpf.Core.ServerMode
Imports System.Collections
Imports System.Reflection

Namespace LINQInstant
	Public Class LINQInstantModeCRUDBehavior
		Inherits CRUDBehaviorBase.CRUDBehaviorBase
		Public Shared ReadOnly DataSourceProperty As DependencyProperty = DependencyProperty.Register("DataSource", GetType(LinqInstantFeedbackDataSource), GetType(LINQInstantModeCRUDBehavior), New PropertyMetadata(Nothing))
		Public Property DataSource() As LinqInstantFeedbackDataSource
			Get
				Return CType(GetValue(DataSourceProperty), LinqInstantFeedbackDataSource)
			End Get
			Set(ByVal value As LinqInstantFeedbackDataSource)
				SetValue(DataSourceProperty, value)
			End Set
		End Property
		Private Info As PropertyInfo
		Protected Overrides Function CanExecuteRemoveRowCommand() As Boolean
			If DataSource Is Nothing OrElse Grid Is Nothing OrElse View Is Nothing OrElse Grid.CurrentItem Is Nothing Then
				Return False
			End If
			Return True
		End Function
		Protected Overrides Sub UpdateDataSource()
			DataSource.Refresh()
		End Sub
		Protected Overrides Sub OnAttached()
			MyBase.OnAttached()
			Grid.ItemsSource = DataSource.Data
		End Sub
		Public Overrides Sub EditRow()
			If View Is Nothing OrElse Grid.CurrentItem Is Nothing Then
				Return
			End If
			Dim e As IEnumerator = Me.DataContext.GetTable(RowType).GetEnumerator()
			Do While e.MoveNext()
				Dim v As Object = Info.GetValue(e.Current, Nothing)
				Dim v1 As Object = Grid.GetCellValue(View.FocusedRowHandle, DataSource.KeyExpression)
				If v.Equals(v1) Then
					Dim dialog As DXWindow = CreateDialogWindow(e.Current, True)
					AddHandler(dialog.Closed), AddressOf OnEditRowDialogClosed
					dialog.ShowDialog()
					Return
				End If
			Loop
		End Sub
		Public Overrides Sub RemoveRow()
			Dim e As IEnumerator = Me.DataContext.GetTable(RowType).GetEnumerator()
			Do While e.MoveNext()
				Dim v As Object = Info.GetValue(e.Current, Nothing)
				Dim v1 As Object = Grid.GetCellValue(View.FocusedRowHandle, DataSource.KeyExpression)
				If v.Equals(v1) Then
					DataContext.GetTable(RowType).DeleteOnSubmit(e.Current)
					DataContext.SubmitChanges()
					UpdateDataSource()
					Return
				End If
			Loop
		End Sub
		Public Overrides Sub RemoveSelectedRows()
			Dim selectedRowsHandles As List(Of Integer) = Grid.GetSelectedRowHandles().ToList()
			If selectedRowsHandles IsNot Nothing AndAlso selectedRowsHandles.Count <> 0 Then
				Dim e As IEnumerator = Me.DataContext.GetTable(RowType).GetEnumerator()
				Do While e.MoveNext()
					For Each handle As Integer In selectedRowsHandles
						Dim v As Object = Info.GetValue(e.Current, Nothing)
						Dim v1 As Object = Grid.GetCellValue(handle, DataSource.KeyExpression)
						If v.Equals(v1) Then
							DataContext.GetTable(RowType).DeleteOnSubmit(e.Current)
							selectedRowsHandles.Remove(handle)
							Exit For
						End If
					Next handle
				Loop
				DataContext.SubmitChanges()
				UpdateDataSource()
			ElseIf Grid.CurrentItem IsNot Nothing Then
				RemoveRow()
			End If
		End Sub
		Protected Overrides Sub Initialize()
			MyBase.Initialize()
			Dim infos() As PropertyInfo = RowType.GetProperties()
			For Each info As PropertyInfo In infos
				If info.Name = DataSource.KeyExpression Then
					Me.Info = info
					Exit For
				End If
			Next info
		End Sub
	End Class
End Namespace