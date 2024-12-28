
using Firebase.Database;
using Firebase.Database.Query;
using RegistroEmpleados.Modelos.Modelos;
using System.Collections.ObjectModel;

namespace RegistroEmpleados.AppMovil.Vistas;

public partial class EditarEstudiante : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroempleados-433b8-default-rtdb.firebaseio.com/");
    public List<Curso> Cursos { get; set; }
    public ObservableCollection<string> ListaCursos { get; set; } = new ObservableCollection<string>();
    private Estudiante estudianteActualizado = new Estudiante();
    private string estudianteId;

    public EditarEstudiante(string idEstudiante)
    {
        InitializeComponent();
        BindingContext = this;
        estudianteId = idEstudiante;
        CargarListaCursos();
        CargarEstudiante(estudianteId);
    }

    private async void CargarEstudiante(string idEstudiante)
    {
        var estudiante = await client.Child("Estudiantes").Child(idEstudiante).OnceSingleAsync<Estudiante>();

        if (estudiante != null)
        {
            EditPrimerNombreEntry.Text = estudiante.PrimerNombre;
            EditSegundoNombreEntry.Text = estudiante.SegundoNombre;
            EditPrimerApellidoEntry.Text = estudiante.PrimerApellido;
            EditSegundoApellidoEntry.Text = estudiante.SegundoApellido;
            EditCorreoElectronicoEntry.Text = estudiante.CorreoElectronico;
            EditEdadEntry.Text = estudiante.Edad.ToString();
            EditCursoPicker.SelectedItem = estudiante.Curso?.Nombre;

        }
    }

    private async void CargarListaCursos()
    {
        try
        {
            var cursos = await client.Child("Cursos").OnceAsync<Curso>();
            ListaCursos.Clear();
            foreach (var curso in cursos)
            {
                ListaCursos.Add(curso.Object.Nombre);
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error:" + ex.Message, "Ok");
        }

    }

    private async void ActualizarButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(EditPrimerNombreEntry.Text) ||
              string.IsNullOrWhiteSpace(EditPrimerApellidoEntry.Text) ||
              string.IsNullOrWhiteSpace(EditCorreoElectronicoEntry.Text) ||
              string.IsNullOrWhiteSpace(EditEdadEntry.Text) ||
               EditCursoPicker.SelectedItem == null
               )
            {
                await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
                return;
            }

            if (!EditCorreoElectronicoEntry.Text.Contains("@"))
            {
                await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
                return;
            }
            if (!int.TryParse(EditEdadEntry.Text, out var edad))
            {
                await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
                return;
            }

            if (edad <= 0)
            {
                await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
                return;
            }

            estudianteActualizado.Id = estudianteId;
            estudianteActualizado.PrimerNombre = EditPrimerNombreEntry.Text.Trim();
            estudianteActualizado.SegundoNombre = EditSegundoNombreEntry.Text.Trim();
            estudianteActualizado.PrimerApellido = EditPrimerApellidoEntry.Text.Trim();
            estudianteActualizado.SegundoApellido = EditSegundoApellidoEntry.Text.Trim();
            estudianteActualizado.CorreoElectronico = EditCorreoElectronicoEntry.Text.Trim();
            estudianteActualizado.Edad = edad;
            estudianteActualizado.Curso = new Curso { Nombre = EditCursoPicker.SelectedItem.ToString() };
            estudianteActualizado.Estado = EditEstadoSwitch.IsToggled;
            await client.Child("Estudiantes").Child(estudianteActualizado.Id).PutAsync(estudianteActualizado);
            await DisplayAlert("Exito", "El estudiante se ha modificado de manera exitoza", "Ok");
            await Navigation.PopAsync();
        }
        catch (Exception ex)

        {
            Console.WriteLine($"Error al registrar estudiantes: {ex.Message}");
        }
    }
}
