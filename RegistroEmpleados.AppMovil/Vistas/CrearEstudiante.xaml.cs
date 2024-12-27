using Firebase.Database;
using Firebase.Database.Query;
using RegistroEmpleados.Modelos.Modelos;

namespace RegistroEmpleados.AppMovil.Vistas;

public partial class CrearEstudiante : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroempleados-7d347-default-rtdb.firebaseio.com/");
    public List<Curso> Cursos { get; set; } = new List<Curso>();
    public CrearEstudiante()
	{
		InitializeComponent();
		ListarCursos();
		BindingContext = this;
	}
	private void ListarCursos()
	{
		var cursos = client.Child("Cursos").OnceAsync<Curso>();
		Cursos = cursos.Result.Select(x => x.Object).ToList();
	}
	private async void guardarButton_Clicked(object sender, EventArgs e)
	{
		Curso curso = cursoPicker.SelectedItem as Curso;

		var estudiante = new Estudiante
		{
			PrimerNombre = primerNombreEntry.Text,
			SegundoNombre = segundoNombreEntry.Text,
			PrimerApellido = primerApellidoEntry.Text,
			SegundoApellido = segundoApellidoEntry.Text,
			CorreoElectronico = correoEntry.Text,
			FechaInsripcion = fechaInscripcionPicker.Date,
			Edad = int.Parse(edadEntry.Text),
			Estado = true,
			Curso = curso
		};
		try
		{
            await client.Child("Estudiantes").PostAsync(estudiante);
            await DisplayAlert("Exito", $"El estudiante {estudiante.PrimerNombre} {estudiante.PrimerApellido} fue guardado correctamente", "OK");
            await Navigation.PopAsync();
        }
		catch(Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}