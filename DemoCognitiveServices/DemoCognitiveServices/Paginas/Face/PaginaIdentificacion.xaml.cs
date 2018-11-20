using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DemoCognitiveServices.Helpers;
using DemoCognitiveServices.Servicios;
using Plugin.Media.Abstractions;

namespace DemoCognitiveServices.Paginas.Face
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PaginaIdentificacion : ContentPage
	{
        MediaFile foto;

        public PaginaIdentificacion ()
		{
			InitializeComponent ();
		}

        private async void btnFoto_Clicked(object sender, EventArgs e)
        {
            var usarCamara = ((Button)sender).Text == "Tomar foto";
            foto = await ServicioImagen.TakePic(usarCamara);
            imgFoto.Source = ImageSource.FromStream(foto.GetStream);
        }

        private async void btnIdentificar_Clicked(object sender, EventArgs e)
        {
            if (foto != null)
            {
                var resultado = await ServicioFace.IdentifyPerson(foto.GetStream());
                await DisplayAlert("Información", "La foto pertenece a : " + resultado, "OK");
            }
            else
                await DisplayAlert("Información", "No hay imagen", "OK");
        }
    }
}