using System.Collections.ObjectModel;
using Firebase.Database;
using Firebase.Database.Query;
using RegistroEmpleados.Modelos.Modelos;

namespace RegistroEmpleados.AppMovil.Vistas;

public partial class ListarEstudiante : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroempleados-7d347-default-rtdb.firebaseio.com/");
    public ObservableCollection<Estudiante> Lista { get; set; } = new ObservableCollection<Estudiante>();
    public ListarEstudiante()
	{
		InitializeComponent();
        BindingContext = this;
        CargarLista();
	}

    private async void CargarLista()
    {
        var estudiantes = await client.Child("Estudiantes").OnceAsync<Estudiante>();
        Lista.Clear();

        var estudiantesActivos = estudiantes.Where(e => e.Object.Estado == true).ToList();

        foreach (var estudiante in estudiantesActivos)
        {
            Lista.Add(new Estudiante
            {
                Id = estudiante.Key,
                PrimerNombre = estudiante.Object.PrimerNombre,
                SegundoNombre = estudiante.Object.SegundoNombre,
                PrimerApellido = estudiante.Object.PrimerApellido,
                SegundoApellido = estudiante.Object.SegundoApellido,
                CorreoElectronico = estudiante.Object.CorreoElectronico,
                FechaInsripcion = estudiante.Object.FechaInsripcion,
                Edad = estudiante.Object.Edad,
                Estado = estudiante.Object.Estado,
                Curso = estudiante.Object.Curso
            });
        }
    }

    private void filtroSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filtro = filtroSearchBar.Text.ToLower();
        if(filtro.Length > 0 )
        {
            ListaCollection.ItemsSource = Lista.Where(x => x.NombreCompleto.ToLower().Contains(filtro));
        }
        else
        {
            ListaCollection.ItemsSource = Lista;
        }
    }

    private async void NuevoEstudianteBoton_Clicked(object sender, EventArgs e)
    {
        var paginaCrear = new CrearEstudiante();
        await Navigation.PushAsync(paginaCrear);
        paginaCrear.Disappearing += (s, args) => CargarLista();
    }

    private async void editarButton_Clicked(object sender, EventArgs e)
    {
        var boton = sender as ImageButton;
        var estudiante = boton?.CommandParameter as Estudiante;

        if (estudiante != null && !string.IsNullOrEmpty(estudiante.Id))
        {
            var paginaEdicion = new EditarEstudiante(estudiante.Id);
            await Navigation.PushAsync(paginaEdicion);

            paginaEdicion.Disappearing += (s, args) => CargarLista();

        }
        else
        {
            await DisplayAlert("Error", "No se pudo obtener la informacion del estudiante", "Ok");
        }
    }

    private async void deshabilitarButton_Clicked(object sender, EventArgs e)
    {
        var boton = sender as ImageButton;
        var estudiante = boton?.CommandParameter as Estudiante;


        if (estudiante is null)
        {
            await DisplayAlert("Error", "No se pudo obtener la informacion del estudiante", "Ok");
            return;
        }
        else
        {
            bool confirmacion = await DisplayAlert("Confirmacion",
                $"Esta seguro de Deshabilitar al estudiante {estudiante.NombreCompleto}", "Si", "No");
            if (confirmacion)
            {
                try
                {
                    estudiante.Estado = false;
                    await client.Child("Estudiantes").Child(estudiante.Id).PutAsync(estudiante);
                    await DisplayAlert("Exito", $"El estudiante {estudiante.NombreCompleto} Ha sido deshabilitado", "Ok");
                    CargarLista();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al registrar estudiantes: {ex.Message}");

                }
            }
        }
    }
}