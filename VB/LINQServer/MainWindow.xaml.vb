Imports DevExpress.Mvvm.Xpf
Imports System.Linq
Imports System.Windows

Namespace LINQServer

    Public Partial Class MainWindow
        Inherits Window

        Public Sub New()
            Me.InitializeComponent()
            Dim context = New DataClassesDataContext()
            Dim source = New DevExpress.Data.Linq.LinqServerModeSource With {.KeyExpression = NameOf(Item.Id), .QueryableSource = context.Items}
            Me.grid.ItemsSource = source
        End Sub

        Private Sub OnCreateEditEntityViewModel(ByVal sender As Object, ByVal e As CreateEditItemViewModelArgs)
            Dim context = New DataClassesDataContext()
            Dim item As Item
            If e.IsNewItem Then
                item = New Item() With {.Id = context.Items.Max(Function(x) x.Id) + 1}
                context.Items.InsertOnSubmit(item)
            Else
                Dim key = CInt(e.Key)
                item = context.Items.[Single](Function(x) x.Id = key)
            End If

            e.ViewModel = New EditItemViewModel(item, context)
        End Sub

        Private Sub OnValidateRow(ByVal sender As Object, ByVal e As EditFormRowValidationArgs)
            Dim context = CType(e.EditOperationContext, DataClassesDataContext)
            context.SubmitChanges()
        End Sub

        Private Sub OnValidateRowDeletion(ByVal sender As Object, ByVal e As EditFormValidateRowDeletionArgs)
            Dim key = CInt(e.Keys.[Single]())
            Dim context = New DataClassesDataContext()
            Dim item = context.Items.[Single](Function(x) x.Id = key)
            context.Items.DeleteOnSubmit(item)
            context.SubmitChanges()
        End Sub
    End Class
End Namespace
