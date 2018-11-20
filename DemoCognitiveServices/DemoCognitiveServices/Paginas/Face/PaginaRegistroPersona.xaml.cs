using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DemoCognitiveServices.Helpers;
using DemoCognitiveServices.Servicios;

namespace DemoCognitiveServices.Paginas.Face
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PaginaRegistroPersona : ContentPage
	{
        string personId = string.Empty;
        MediaFile foto;

		public PaginaRegistroPersona ()
		{
			InitializeComponent ();
		}

        private async void btnAgregar_Clicked(object sender, EventArgs e)
        {
            var resultado = await ServicioFace.CreatePerson(txtNombrePersona.Text);

            if (resultado != "Error")
            {
                await DisplayAlert("Información", "Person ID: " + resultado, "OK");
                personId = resultado;
            }
            else
            {
                await DisplayAlert("Información", "Error al agregar la persona", "OK");
                personId = string.Empty;
            }
        }

        private async void btnFoto_Clicked(object sender, EventArgs e)
        {
            var usarCamara = ((Button)sender).Text == "Tomar foto";
            foto = await ServicioImagen.TakePic(usarCamara);
            imgFoto.Source = ImageSource.FromStream(foto.GetStream);
        }

        private async void btnRegistrar_Clicked(object sender, EventArgs e)
        {
            if (foto!= null)
            {
                var resultado = await ServicioFace.RegisterPerson(personId, foto.GetStream());
                await DisplayAlert("Información", "Persisted Face ID: " + resultado, "OK");
            }
            else
                await DisplayAlert("Información", "No hay imagen", "OK");
        }

        private async void btnObtenerInfo_Clicked(object sender, EventArgs e)
        {
            if (foto != null)
            {
                var face = await ServicioFace.DetectFaces(foto.GetStream());

                if (face != null)
                {
                    string resultado = $"ID: {face.FaceID}\n Edad: {face.FaceAttributes.Age}\n Felicidad: {face.FaceAttributes.Emotion.Happiness * 100} %";
                    await DisplayAlert("Información", resultado, "OK");
                }
                else
                    await DisplayAlert("Información", "Error al detectar la cara", "OK");
            }
            else
                await DisplayAlert("Información", "No hay imagen", "OK");
        }

        private async void btnEntrenar_Clicked(object sender, EventArgs e)
        {
            var resultado = await ServicioFace.TrainGroup();
            await DisplayAlert("Información", "Entrenamiento aceptado", "OK");
        }
    }
}