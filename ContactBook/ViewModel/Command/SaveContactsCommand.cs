using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContactsBook
{
    public class SaveContactsCommand : AsyncRelayCommand
    {
        private readonly ViewModel viewModel;
        private readonly IDialogService dialogService;
        private readonly IFileService fileService;

        public SaveContactsCommand(ViewModel viewModel, IDialogService dialogService, IFileService fileService)
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
                if (dialogService.SaveFileDialog() == true)
                {
                    await Task.Run(() => fileService.Save(dialogService.FilePath, viewModel.Contacts.ToList()));
                    dialogService.ShowMessage("Файл сохранен");
                }
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage($"Ошибка при сохранении файла: {ex.Message}");
            }
        }
    }
}