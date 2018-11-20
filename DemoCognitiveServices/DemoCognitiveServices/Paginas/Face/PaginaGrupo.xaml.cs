using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DemoCognitiveServices.Helpers;
using DemoCognitiveServices.Servicios;

namespace DemoCognitiveServices.Paginas.Face
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PaginaGrupo : ContentPage
	{
		public PaginaGrupo ()
		{
			InitializeComponent ();
		}

        private async void btnGroup_Clicked(object sender, EventArgs e)
        {
            var resultado = await ServicioFace.CreateGroup(Constantes.Group);
            await DisplayAlert("Información", resultado ? "Grupo creado correctamente" : "Error al crear el grupo (¿tal vez ya existe)", "OK");
        }
    }
}