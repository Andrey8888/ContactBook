using System;
using System.Threading.Tasks;

namespace ContactsBook
{
    public class OpenContactsCommand : AsyncRelayCommand
    {
        private readonly IDialogService dialogService;
        private readonly IFileService fileService;

        public OpenContactsCommand(ViewModel viewModel, IDialogService dialogService, IFileService fileService)
            : base(async (o) => await Execute(dialogService, fileService, viewModel))
        {
            this.viewModel = viewModel;
            this.dialogService = dialogService;
            this.fileService = fileService;
        }

        private static async Task Execute(IDialogService dialogService, IFileService fileService, ViewModel viewModel)
        {
            try
            {
                if (dialogService.OpenFileDialog() == true)
                {
                    var contacts = await Task.Run(() => fileService.Open(dialogService.FilePath));

                    await App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        viewModel.Contacts.Clear();
                        foreach (var p in contacts)
                        {
                            viewModel.Contacts.Add(p);
                        }
                    });

                    dialogService.ShowMessage("Файл открыт");

                    using (var dbContext = new ApplicationContext())
                    {
                        dbContext.Contacts.RemoveRange(dbContext.Contacts);
                        dbContext.Contacts.AddRange(contacts);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage($"Ошибка при открытии файла: {ex.Message}");
            }
        }
    }
}
