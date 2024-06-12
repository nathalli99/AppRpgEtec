using AppRpgEtec.Models;
using AppRpgEtec.Services.Usuarios;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppRpgEtec.ViewModels.Usuarios
{
    public class ImagemUsuarioViewModel : BaseViewModel
    {
        private UsuarioService uService;
        private static string conexaoAzureStorage = "DefaultEndpointsProtocol=https;AccountName=etecstorage;AccountKey=2bsEP2xndZWXnXKQlFOEpFlvYQXKB0V2iRrtDNWqTw780YXubYlzaBDpLkt3aDPOLSfXOg3nUdsD+AStDEbFSA==;EndpointSuffix=core.windows.net";
        private static string container = "arquivos";
        public ImagemUsuarioViewModel() 
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            uService = new UsuarioService(token);

            FotografarCommand = new Command(Fotografar);            
            SalvarImagemCommand = new Command(SalvarImagemAzure);
            AbrirGaleriaCommand = new Command(AbrirGaleria);

            CarregarUsuarioAzure();
        }

        public ICommand FotografarCommand { get; }
        public ICommand SalvarImagemCommand { get; }
        public ICommand AbrirGaleriaCommand { get; }

        private ImageSource fonteImagem;
        public ImageSource FonteImagem
        {
            get => fonteImagem;
            set
            {
                fonteImagem = value;
                OnPropertyChanged(nameof(FonteImagem));//Informa mundaça de estado para a View
            }
        }

        private byte[] foto; //CTRL + R,E        
        public byte[] Foto
        {
            get => foto;
            set
            {
                foto = value;
                OnPropertyChanged(nameof(Foto));//Informa mundaça de estado para a View
            }
        }

        public async void Fotografar()
        {
            try
            {
                //Verificação se o dispositivo suporta mídia como câmera e galeria.
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    //Chamada para a câmera do dispositivo. Fica aguardando usuário tirar foto.
                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                    if (photo != null)
                    {
                        using (Stream sourceStream = await photo.OpenReadAsync())//Leitura dos bytes da foto para Stream
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                await sourceStream.CopyToAsync(ms); //Conversão do Stream para MemoryStream (arquivo em memória)

                                //Carregamento do array de bytes a partir do memória para a propriedade da ViewModel
                                Foto = ms.ToArray();

                                //Carregamento do controle que apresenta a imagem para a ViewModel
                                FonteImagem = ImageSource.FromStream(() => new MemoryStream(ms.ToArray()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops...", ex.Message, "Ok");
            }
        }

        public async void SalvarImagemAzure()
        {
            try
            {
                Usuario u = new Usuario();
                u.Foto = foto;
                u.Id = Preferences.Get("UsuarioId", 0);


                string fileName = $"{u.Id}.jpg";

                //define o BLOB no qual a imagem será armazeda
                var blobClient = new BlobClient(conexaoAzureStorage, container, fileName);

                if (blobClient.Exists())
                    blobClient.Delete();

                using (var stream = new MemoryStream(u.Foto))
                {
                    blobClient.Upload(stream);
                }

                await Application.Current.MainPage.DisplayAlert("Mensagem", "Dados salvos com sucesso!", "Ok");
                await App.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async void AbrirGaleria()
        {
            try
            {
                //Verificação se o dispositivo suporta câmera.
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    //Chamada para a galeria do dispositivo. Fica aguardando usuário escolher a foto da galeria.
                    FileResult photo = await MediaPicker.Default.PickPhotoAsync();

                    if (photo != null)
                    {
                        using (Stream sourceStream = await photo.OpenReadAsync())//Leitura dos bytes da foto para Stream
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                await sourceStream.CopyToAsync(ms); //Conversão do Stream para MemoryStream (arquivo em memória)

                                //Carregamento do array de bytes a partir do memória para a propriedade da ViewModel
                                Foto = ms.ToArray();

                                //Carregamento do controle que apresenta a imagem para a ViewModel
                                FonteImagem = ImageSource.FromStream(() => new MemoryStream(ms.ToArray()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async void CarregarUsuarioAzure()
        {
            try
            {
                int usuarioId = Preferences.Get("UsuarioId", 0);
                string filename = $"{usuarioId}.jpg";

                var blobClient = new BlobClient(conexaoAzureStorage, container, filename);
                Byte[] fileBytes;

                using (MemoryStream ms = new MemoryStream())
                {
                    blobClient.OpenRead().CopyTo(ms);
                    fileBytes = ms.ToArray();
                }

                Foto = fileBytes;

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }


        #region MetodosAntigoSalvamentoAPI
        /*public async void SalvarImagem()
        {
            try
            {
                Usuario u = new Usuario();
                u.Foto = foto;
                u.Id = Preferences.Get("UsuarioId", 0);

                if (await uService.PutFotoUsuarioAsync(u) != 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Mensagem", "Dados salvos com sucesso!", "Ok");
                    await App.Current.MainPage.Navigation.PopAsync();
                }
                else { throw new Exception("Erro ao tentar atualizar imagem"); }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async void CarregarUsuario()
        {
            try
            {
                int usuarioId = Preferences.Get("UsuarioId", 0);
                Usuario u = await uService.GetUsuarioAsync(usuarioId);
                Foto = u.Foto;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }

        }*/
        #endregion










    }

}
